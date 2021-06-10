#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

extern alias ExplorerOM;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Extensions;
using ExplorerOM::Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Deployment.Binding;
using BtsReceiveLocation = Microsoft.BizTalk.Deployment.Binding.ReceiveLocation;
using BtsReceivePort = Microsoft.BizTalk.Deployment.Binding.ReceivePort;
using BtsSendPort = Microsoft.BizTalk.Deployment.Binding.SendPort;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> implementation that generates BizTalk Server bindings file.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see
	/// href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.biztalk.deployment.binding">Microsoft.BizTalk.Deployment.Binding
	/// Namespace</see>
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Public DSL API.")]
	public class BindingInfoBuilderVisitor : MainApplicationBindingVisitor
	{
		#region Base Class Member Overrides

		protected internal override void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			ApplicationName = ((ISupportNamingConvention) applicationBinding).Name;
			BindingInfo = CreateBindingInfo(applicationBinding);
		}

		protected internal override void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			var moduleRef = CreateOrFindModuleRef(orchestrationBinding);
			// a ModuleRef just created has no ServiceRef in its Services collection yet
			if (moduleRef.Services.Count == 0) BindingInfo.ModuleRefCollection.Add(moduleRef);
			var serviceRef = CreateServiceRef(orchestrationBinding);
			moduleRef.Services.Add(serviceRef);
		}

		protected internal override void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			var visitedReceiveLocation = CreateReceiveLocation(receiveLocation);
			if (_lastVisitedReceivePort.ReceiveLocations.Cast<BtsReceiveLocation>().Any(rl => rl.Name == visitedReceiveLocation.Name))
				throw new BindingException($"Duplicate receive location name: '{visitedReceiveLocation.Name}'.");
			_lastVisitedReceivePort.ReceiveLocations.Add(visitedReceiveLocation);
		}

		protected internal override void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			_lastVisitedReceivePort = CreateReceivePort(receivePort);
			if (BindingInfo.ReceivePortCollection.Find(_lastVisitedReceivePort.Name) != null)
				throw new BindingException($"Duplicate receive port name: '{_lastVisitedReceivePort.Name}'.");
			BindingInfo.ReceivePortCollection.Add(_lastVisitedReceivePort);
		}

		protected internal override void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			var visitedSendPort = CreateSendPort(sendPort);
			if (BindingInfo.SendPortCollection.Find(visitedSendPort.Name) != null) throw new BindingException($"Duplicate send port name: '{visitedSendPort.Name}'.");
			BindingInfo.SendPortCollection.Add(visitedSendPort);
		}

		#endregion

		public BindingInfo BindingInfo { get; private set; }

		private string ApplicationName { get; set; }

		protected virtual BindingInfo CreateBindingInfo<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			var bi = new BindingInfo();
			bi.BindingParameters = new BindingParameters(new Version(bi.Version)) {
				BindingActions = BindingParameters.BindingActionTypes.Bind,
				BindingItems = BindingParameters.BindingItemTypes.All,
				BindingScope = BindingParameters.BindingScopeType.Application,
				BindingSetState = BindingParameters.BindingSetStateType.UseServiceState
			};
			bi.Description = applicationBinding.Description;
			bi.ModuleRefCollection = new ModuleRefCollection {
				new ModuleRef($"[Application:{ApplicationName}]", string.Empty, string.Empty, string.Empty)
			};
			bi.Timestamp = applicationBinding.Timestamp;
			return bi;
		}

		protected internal virtual ModuleRef CreateOrFindModuleRef(IOrchestrationBinding orchestrationBinding)
		{
			var serviceAssemblyName = orchestrationBinding.Type.Assembly.GetName();
			var name = serviceAssemblyName.Name;
			var version = serviceAssemblyName.Version.ToString();
			// see BizTalkFactory.Management.Automation.BtsCatalog.ExportBinding, BizTalkFactory.Management.Automation
			var culture = serviceAssemblyName.CultureInfo == null || serviceAssemblyName.CultureInfo.Name.IsNullOrEmpty()
				? "neutral"
				: serviceAssemblyName.CultureInfo.Name;
			var publicKeyTokenBytes = serviceAssemblyName.GetPublicKeyToken();
			// see BizTalkFactory.Management.Automation.BtsCatalog.ExportBinding, BizTalkFactory.Management.Automation
			var publicKeyToken = publicKeyTokenBytes == null || publicKeyTokenBytes.Length == 0
				? null
				: publicKeyTokenBytes.Aggregate(string.Empty, (k, token) => k + token.ToString("x2", CultureInfo.InvariantCulture));
			var module = BindingInfo.ModuleRefCollection.Find(name, version, culture, publicKeyToken);
			return module ?? new ModuleRef(name, version, culture, publicKeyToken);
		}

		protected internal virtual ServiceRef CreateServiceRef(IOrchestrationBinding orchestrationBinding)
		{
			// see https://docs.microsoft.com/en-us/dotnet/api/microsoft.biztalk.deployment.binding.serviceref
			var serviceRef = new ServiceRef {
				Description = orchestrationBinding.Description,
				Host = new HostRef {
					Name = ((ISupportHostNameResolution) orchestrationBinding).ResolveHostName()
				},
				Name = orchestrationBinding.Type.FullName,
				// Un/Enlisting/Starting/Stopping orchestrations is the responsibility of BizTalkServiceStateInitializerVisitor
				State = orchestrationBinding.State switch {
					ServiceState.Unenlisted => ServiceRef.ServiceRefState.Unenlisted,
					ServiceState.Enlisted => ServiceRef.ServiceRefState.Enlisted,
					ServiceState.Started => ServiceRef.ServiceRefState.Started,
					_ => ServiceRef.ServiceRefState.Default
				},
				TrackingOption = OrchestrationTrackingTypes.None
			};
			// ensure service ref port collection is initialized even if there are only direct ports
			var serviceRefPorts = serviceRef.Ports;
			foreach (var portBinding in orchestrationBinding.PortBindings)
			{
				serviceRefPorts.Add(CreateServicePortRef(portBinding));
			}
			return serviceRef;
		}

		protected virtual ServicePortRef CreateServicePortRef(IOrchestrationPortBinding portBinding)
		{
			// see https://docs.microsoft.com/en-us/dotnet/api/microsoft.biztalk.deployment.binding.serviceref
			if (portBinding.IsInbound)
				return new ServicePortRef {
					// see Microsoft.BizTalk.Deployment.Assembly.BtsOrchestrationPort.GetBindingOption(IBizTalkPort port)
					// see Microsoft.BizTalk.OrchestrationDesigner.PortBinding, Microsoft.BizTalk.OrchestrationDesigner
					// where 0=Physical, 1=Logical, 2=Direct, 3=Dynamic
					// however it never seems to be set to an other value than 1 in binding exports
					BindingOption = 1,
					// see Microsoft.BizTalk.ExplorerOM.PortModifier (Import Indicates an Outbound port of an orchestration,
					// Export Indicates an Inbound port of an orchestration)
					Modifier = (int) PortModifier.Export,
					Name = portBinding.LogicalPortName,
					ReceivePortRef = new ReceivePortRef { Name = portBinding.ActualPortName }
				};

			return new ServicePortRef {
				Modifier = (int) PortModifier.Import,
				Name = portBinding.LogicalPortName,
				SendPortRef = new SendPortRef { Name = portBinding.ActualPortName }
			};
		}

		protected internal virtual BtsReceivePort CreateReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			var port = new BtsReceivePort {
				ApplicationName = ApplicationName,
				Description = receivePort.Description,
				IsTwoWay = receivePort.IsTwoWay,
				Name = ((ISupportNamingConvention) receivePort).Name,
				ReceiveLocations = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocationCollection()
			};
			return port;
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		protected internal virtual BtsReceiveLocation CreateReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			var location = new BtsReceiveLocation {
				// General
				Name = ((ISupportNamingConvention) receiveLocation).Name,
				Enable = false, // Enabling Receive Locations is actually the responsibility of BizTalkServiceStateInitializerVisitor
				Address = receiveLocation.Transport.Adapter.Address,
				PublicAddress = receiveLocation.Transport.Adapter.PublicAddress,
				Description = receiveLocation.Description,

				TransportType = receiveLocation.Transport.Adapter.ProtocolType,
				TransportTypeData = receiveLocation.Transport.Adapter.GetAdapterBindingInfoSerializer().Serialize(),
				ReceiveHandler = new ReceiveHandlerRef {
					Name = ((ISupportHostNameResolution) receiveLocation.Transport).ResolveHostName(),
					TransportType = receiveLocation.Transport.Adapter.ProtocolType
				},
				ReceivePipeline = CreateReceivePipelineRef(receiveLocation.ReceivePipeline),
				ReceivePipelineData = receiveLocation.ReceivePipeline.GetPipelineBindingInfoSerializer().Serialize()
			};
			if (receiveLocation.Transport.Schedule != Schedule.None)
			{
				var transportSchedule = receiveLocation.Transport.Schedule;
				// Schedule
				location.ScheduleTimeZone = transportSchedule.TimeZone.Id;
				location.ScheduleAutoAdjustToDaylightSaving = transportSchedule.AutomaticallyAdjustForDaylightSavingTime;
				location.StartDate = transportSchedule.StartDate;
				location.StartDateEnabled = transportSchedule.StartDateEnabled;
				location.EndDate = transportSchedule.StopDate;
				location.EndDateEnabled = transportSchedule.StopDateEnabled;
				// Schedule Service Window
				location.ServiceWindowEnabled = transportSchedule.ServiceWindow.Enabled;
				location.FromTime = transportSchedule.ServiceWindow.StartTime;
				location.ToTime = transportSchedule.ServiceWindow.StopTime;
				// Schedule Recurrence
				location.ScheduleRecurrenceType = transportSchedule.RecurrenceType;
				switch (transportSchedule.ServiceWindow)
				{
					case DailyServiceWindow dailyServiceWindow:
						location.ScheduleRecurFrom = dailyServiceWindow.From;
						location.ScheduleRecurInterval = dailyServiceWindow.Interval;
						break;
					case WeeklyServiceWindow weeklyServiceWindow:
						location.ScheduleRecurFrom = weeklyServiceWindow.From;
						location.ScheduleRecurInterval = weeklyServiceWindow.Interval;
						location.ScheduleDaysOfWeek = weeklyServiceWindow.WeekDays;
						break;
					case CalendricalMonthlyServiceWindow calendricalMonthlyServiceWindow:
						location.ScheduleMonths = calendricalMonthlyServiceWindow.Months;
						location.ScheduleIsOrdinal = false;
						location.ScheduleMonthDays = calendricalMonthlyServiceWindow.Days;
						location.ScheduleLastDayOfMonth = calendricalMonthlyServiceWindow.OnLastDay;
						break;
					case OrdinalMonthlyServiceWindow ordinalMonthlyServiceWindow:
						location.ScheduleMonths = ordinalMonthlyServiceWindow.Months;
						location.ScheduleIsOrdinal = true;
						location.ScheduleOrdinalDayOfWeek = ordinalMonthlyServiceWindow.WeekDays;
						location.ScheduleOrdinalType = ordinalMonthlyServiceWindow.Ordinality;
						break;
				}
			}
			if (receiveLocation.SendPipeline != null)
			{
				location.SendPipeline = CreateSendPipelineRef(receiveLocation.SendPipeline);
				location.SendPipelineData = receiveLocation.SendPipeline.GetPipelineBindingInfoSerializer().Serialize();
			}
			return location;
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		protected internal virtual BtsSendPort CreateSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			var port = new BtsSendPort {
				ApplicationName = ApplicationName,
				Description = sendPort.Description,
				Filter = sendPort.Filter?.ToString(),
				IsDynamic = false,
				Name = ((ISupportNamingConvention) sendPort).Name,
				OrderedDelivery = sendPort.OrderedDelivery,
				PrimaryTransport = CreateTransportInfo(sendPort.Transport),
				Priority = (int) sendPort.Priority,
				SendPipelineData = sendPort.SendPipeline.GetPipelineBindingInfoSerializer().Serialize(),
				// sendPort.Status is the responsibility of BizTalkServiceStateInitializerVisitor
				StopSendingOnFailure = sendPort.StopSendingOnOrderedDeliveryFailure,
				TransmitPipeline = CreateSendPipelineRef(sendPort.SendPipeline)
			};
			if (sendPort.BackupTransport.IsValueCreated)
			{
				port.SecondaryTransport = CreateTransportInfo(sendPort.BackupTransport.Value);
			}
			if (sendPort.IsTwoWay)
			{
				port.IsTwoWay = true;
				port.ReceivePipeline = CreateReceivePipelineRef(sendPort.ReceivePipeline);
				port.ReceivePipelineData = sendPort.ReceivePipeline.GetPipelineBindingInfoSerializer().Serialize();
			}
			return port;
		}

		protected virtual Microsoft.BizTalk.Deployment.Binding.TransportInfo CreateTransportInfo<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
			where TNamingConvention : class
		{
			var transportInfo = new Microsoft.BizTalk.Deployment.Binding.TransportInfo {
				Address = transport.Adapter.Address,
				FromTime = transport.ServiceWindow.StartTime,
				// ordered delivery is meaningful only for a SendPort's primary transport 
				OrderedDelivery = ReferenceEquals(transport.SendPort.Transport, transport) && transport.SendPort.OrderedDelivery,
				// is it the SendPort's primary transport
				Primary = ReferenceEquals(transport.SendPort.Transport, transport),
				RetryCount = transport.RetryPolicy.Count,
				RetryInterval = (int) transport.RetryPolicy.Interval.TotalMinutes,
				SendHandler = new SendHandlerRef {
					Name = ((ISupportHostNameResolution) transport).ResolveHostName(),
					TransportType = transport.Adapter.ProtocolType
				},
				ServiceWindowEnabled = transport.ServiceWindow.Enabled,
				ToTime = transport.ServiceWindow.StopTime,
				TransportType = transport.Adapter.ProtocolType,
				TransportTypeData = transport.Adapter.GetAdapterBindingInfoSerializer().Serialize()
			};
			return transportInfo;
		}

		protected virtual PipelineRef CreateReceivePipelineRef(ReceivePipeline receivePipeline)
		{
			var rp = PipelineRef.ReceivePipelineRef();
			receivePipeline.Description.IfNotNullOrEmpty(d => rp.Description = d);
			rp.Name = ((ITypeDescriptor) receivePipeline).FullName;
			rp.FullyQualifiedName = ((ITypeDescriptor) receivePipeline).AssemblyQualifiedName;
			rp.TrackingOption = PipelineTrackingTypes.None;
			return rp;
		}

		protected virtual PipelineRef CreateSendPipelineRef(SendPipeline sendPipeline)
		{
			var tp = PipelineRef.TransmitPipelineRef();
			sendPipeline.Description.IfNotNullOrEmpty(d => tp.Description = d);
			tp.Name = ((ITypeDescriptor) sendPipeline).FullName;
			tp.FullyQualifiedName = ((ITypeDescriptor) sendPipeline).AssemblyQualifiedName;
			tp.TrackingOption = PipelineTrackingTypes.None;
			return tp;
		}

		private BtsReceivePort _lastVisitedReceivePort;
	}
}
