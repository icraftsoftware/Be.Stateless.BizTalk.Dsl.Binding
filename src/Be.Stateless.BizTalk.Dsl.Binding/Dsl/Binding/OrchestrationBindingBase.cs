#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Services.Description;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.Extensions;
using Be.Stateless.Reflection;
using Microsoft.BizTalk.XLANGs.BTXEngine;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class OrchestrationBindingBase<T>
		: IOrchestrationBinding,
			ISupportEnvironmentOverride,
			ISupportNamingConvention,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where T : BTXService
	{
		#region Nested Type: PortBindingInfo

		private class PortBindingInfo : IOrchestrationPortBinding
		{
			internal PortBindingInfo(OrchestrationBindingBase<T> orchestrationBinding, PortInfo logicalPort)
			{
				_orchestrationBinding = orchestrationBinding ?? throw new ArgumentNullException(nameof(orchestrationBinding));
				_logicalPort = logicalPort ?? throw new ArgumentNullException(nameof(logicalPort));
			}

			#region IOrchestrationPortBinding Members

			// unbound ports, (i.e. GetActualPort() returning null) have been caught by OrchestrationBindingBase<T>'s ISupportValidation.Validate()
			public string ActualPortName => ((ISupportNamingConvention) _orchestrationBinding.GetActualPort(_logicalPort.Name)).Name;

			public bool IsInbound => _logicalPort.Polarity == Polarity.implements;

			public string LogicalPortName => _logicalPort.Name;

			#endregion

			private readonly PortInfo _logicalPort;
			private readonly OrchestrationBindingBase<T> _orchestrationBinding;
		}

		#endregion

		#region IOrchestrationBinding Members

		public IApplicationBinding ApplicationBinding { get; set; }

		public string Description { get; set; }

		public string Host { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public Type Type => typeof(T);

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IOrchestrationPortBinding[] PortBindings => LogicalPorts.Select(lp => new PortBindingInfo(this, lp)).Cast<IOrchestrationPortBinding>().ToArray();

		public ServiceState State { get; set; }

		#endregion

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty())
			{
				ApplyEnvironmentOverrides(environment);
			}
		}

		#endregion

		#region ISupportNamingConvention Members

		[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types")]
		string ISupportNamingConvention.Name => typeof(T).FullName;

		#endregion

		#region ISupportValidation Members

		[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types")]
		[SuppressMessage("ReSharper", "CommentTypo")]
		void ISupportValidation.Validate()
		{
			if (Host.IsNullOrEmpty()) throw new BindingException("Orchestration's Host is not defined.");

			// validate that all logical ports are bound
			var unboundPorts = LogicalPorts
				.Where(p => GetActualPort(p.Name) == null)
				.ToArray();
			if (unboundPorts.Any())
				throw new BindingException(
					$"The '{typeof(T).FullName}' orchestration has unbound logical ports: '{unboundPorts.Aggregate(string.Empty, (k, p) => $"{k}', '{p.Name}", s => s.Substring(4))}'.");

			// Microsoft.XLANGs.BaseTypes.Polarity (i.e. implements/receive or uses/send) validation is statically enforced
			// by IReceivePort and ISendPort interfaces

			// validate that the operation flow (i.e. one or two way) of each individual logical port matches the one of
			// its associated actual port
			foreach (var logicalPort in LogicalPorts)
			{
				var operationFlow = logicalPort.Operations.First().OperationFlow;
				if (operationFlow != OperationFlow.OneWay && operationFlow != OperationFlow.RequestResponse)
					throw new NotSupportedException("Unexpected OperationFlow enumeration value.");
				var isLogicalPortTwoWay = operationFlow != OperationFlow.OneWay;

				var actualPort = GetActualPort(logicalPort.Name);
				var isActualPortTwoWay = logicalPort.Polarity == Polarity.implements
					? ((IReceivePort) actualPort).IsTwoWay
					: ((ISendPort) actualPort).IsTwoWay;

				if (isLogicalPortTwoWay && !isActualPortTwoWay)
					throw new BindingException(
						$"Orchestration's two-way logical port '{logicalPort.Name}' is bound to one-way port '{((ISupportNamingConvention) actualPort).Name}'.");
				if (!isLogicalPortTwoWay && isActualPortTwoWay)
					throw new BindingException(
						$"Orchestration's one-way logical port '{logicalPort.Name}' is bound to two-way port '{((ISupportNamingConvention) actualPort).Name}'.");
			}
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types")]
		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			visitor.VisitOrchestration(this);
		}

		#endregion

		private IEnumerable<PortInfo> LogicalPorts
		{
			get
			{
				// see also https://docs.microsoft.com/en-us/dotnet/api/system.web.services.description.operationflow
				return ((PortInfo[]) Reflector.GetField(typeof(T), "_portInfo"))
					// filter out direct ports
					.Where(p => p.FindAttribute(typeof(DirectBindingAttribute)) == null);
			}
		}

		private object GetActualPort(string name)
		{
			var portBindingType = GetType().GetInterfaces().SingleOrDefault(i => i.Namespace == GetType().Namespace) ?? GetType();
			var portProperty = portBindingType.GetProperty(name);
			var port = portProperty!.GetValue(this);
			return port;
		}

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		protected virtual void ApplyEnvironmentOverrides(string environment) { }
	}
}
