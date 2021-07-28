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
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Orchestrations.Bound;
using ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper;
using ExplorerOM::Microsoft.BizTalk.ExplorerOM;
using FluentAssertions;
using Microsoft.BizTalk.Deployment.Binding;
using Xunit;
using static FluentAssertions.FluentActions;
using ProtocolType = Microsoft.BizTalk.Deployment.Binding.ProtocolType;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class BindingInfoBuilderVisitorFixture
	{
		[SkippableFact]
		public void CreateBindingInfo()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(new TestApplication());

			var binding = visitor.BindingInfo;

			binding.BindingParameters.BindingActions.Should().Be(BindingParameters.BindingActionTypes.Bind);
			binding.BindingParameters.BindingItems.Should().Be(BindingParameters.BindingItemTypes.All);
			binding.BindingParameters.BindingScope.Should().Be(BindingParameters.BindingScopeType.Application);
			binding.BindingParameters.BindingSetState.Should().Be(BindingParameters.BindingSetStateType.UseServiceState);
			binding.BindingParameters.BindingsSourceVersion.ToString().Should().Be(new BindingInfo().Version);
			binding.Description.Should().Be("Some Useless Test Application");
			binding.ModuleRefCollection.Count.Should().Be(1);
			binding.ModuleRefCollection[0].Name.Should().Be($"[Application:{nameof(TestApplication)}]");
		}

		[SkippableFact]
		public void CreateModuleRef()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfo
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(new TestApplication());

			var binding = visitor.CreateOrFindModuleRef(new ProcessOrchestrationBinding());

			binding.FullName.Should().Be(typeof(Process).Assembly.FullName);
		}

		[SkippableFact]
		public void CreateReceiveLocationOneWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var dsl = new OneWayReceiveLocation();
			var visitor = new BindingInfoBuilderVisitor();
			var binding = visitor.CreateReceiveLocation(dsl);

			binding.Name.Should().Be(nameof(OneWayReceiveLocation));
			binding.Enable.Should().BeFalse();
			binding.Address.Should().Be(@"c:\file\drops\*.xml");
			binding.Description.Should().Be("Some Useless One-Way Test Receive Location");

			binding.TransportType.Should().BeEquivalentTo(new ProtocolType { Name = "Test Dummy" });
			binding.TransportTypeData.Should().Be("<CustomProps />");
			binding.ReceiveHandler.TransportType.Name.Should().Be("Test Dummy");
			binding.ReceiveHandler.Name.Should().Be("Receive Host Name");

			binding.ReceivePipeline.Name.Should().Be(typeof(PassThruReceive).FullName);
			binding.ReceivePipeline.FullyQualifiedName.Should().Be(typeof(PassThruReceive).AssemblyQualifiedName);
			binding.ReceivePipeline.TrackingOption.Should().Be(PipelineTrackingTypes.None);
			binding.ReceivePipeline.Type.Should().Be(PipelineRef.ReceivePipelineRef().Type);
			binding.ReceivePipelineData.Should().NotBeNullOrEmpty();

			binding.SendPipeline.Should().BeNull();
			binding.SendPipelineData.Should().BeNull();

			binding.ScheduleTimeZone.Should().Be(TimeZoneInfo.Utc.Id);
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeTrue();
			binding.StartDate.Should().Be(dsl.Transport.Schedule.StartDate);
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(dsl.Transport.Schedule.StopDate);
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.Should().Be(dsl.Transport.Schedule.ServiceWindow.StartTime);
			binding.ToTime.Should().Be(dsl.Transport.Schedule.ServiceWindow.StopTime);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Day);
			binding.ScheduleRecurFrom.Should().Be(new DailyServiceWindow().From);
			binding.ScheduleRecurInterval.Should().Be(1);
			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[SkippableFact]
		public void CreateReceiveLocationTwoWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();
			var binding = visitor.CreateReceiveLocation(new TwoWayReceiveLocation());

			binding.Name.Should().Be(nameof(TwoWayReceiveLocation));
			binding.Enable.Should().BeFalse();
			binding.Address.Should().Be(@"c:\file\drops\*.xml");
			binding.Description.Should().Be("Some Useless Two-Way Test Receive Location");

			binding.TransportType.Should().BeEquivalentTo(new ProtocolType { Name = "Test Dummy" });
			binding.TransportTypeData.Should().Be("<CustomProps />");
			binding.ReceiveHandler.Name.Should().Be("Receive Host Name");
			binding.ReceiveHandler.TransportType.Name.Should().Be("Test Dummy");

			binding.ReceivePipeline.Name.Should().Be(typeof(PassThruReceive).FullName);
			binding.ReceivePipeline.FullyQualifiedName.Should().Be(typeof(PassThruReceive).AssemblyQualifiedName);
			binding.ReceivePipeline.TrackingOption.Should().Be(PipelineTrackingTypes.None);
			binding.ReceivePipeline.Type.Should().Be(PipelineRef.ReceivePipelineRef().Type);
			binding.ReceivePipelineData.Should().BeEmpty();

			binding.SendPipeline.Name.Should().Be(typeof(PassThruTransmit).FullName);
			binding.SendPipeline.FullyQualifiedName.Should().Be(typeof(PassThruTransmit).AssemblyQualifiedName);
			binding.SendPipeline.TrackingOption.Should().Be(PipelineTrackingTypes.None);
			binding.SendPipeline.Type.Should().Be(PipelineRef.TransmitPipelineRef().Type);
			binding.SendPipelineData.Should().NotBeNullOrEmpty();

			binding.ScheduleTimeZone.Should().BeNull();
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(Schedule.None.StartDate);
			binding.StartDateEnabled.Should().BeFalse();
			binding.EndDate.Should().Be(Schedule.None.StopDate);
			binding.EndDateEnabled.Should().BeFalse();

			binding.ServiceWindowEnabled.Should().BeFalse();
			binding.FromTime.Should().Be(ServiceWindow.None.StartTime);
			binding.ToTime.Should().Be(ServiceWindow.None.StopTime);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.None);
			binding.ScheduleRecurFrom.Should().Be(new DailyServiceWindow().From);
			binding.ScheduleRecurInterval.Should().Be(1);
			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[SkippableFact]
		public void CreateReceiveLocationWithCalendricalMonthlyServiceWindow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new() {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new(2020, 1, 30),
						StopDate = new(2020, 3, 13),
						ServiceWindow = new CalendricalMonthlyServiceWindow {
							StartTime = new(19, 19, 19),
							StopTime = new(9, 9, 9),
							Months = Month.January | Month.Feburary | Month.March,
							Days = MonthDay.Day31 | MonthDay.Day14 | MonthDay.Day15,
							OnLastDay = true
						}
					};
					rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				});
			var visitor = new BindingInfoBuilderVisitor();

			var binding = visitor.CreateReceiveLocation(receiveLocation);
			binding.ScheduleTimeZone.Should().Be("Pacific Standard Time");
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(new(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new(2020, 3, 13));
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.TimeOfDay.Should().Be(((DateTime) new Time(19, 19, 19)).TimeOfDay);
			binding.ToTime.TimeOfDay.Should().Be(((DateTime) new Time(9, 9, 9)).TimeOfDay);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Month);
			binding.ScheduleRecurFrom.Should().Be(new DailyServiceWindow().From);
			binding.ScheduleRecurInterval.Should().Be(1);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);

			binding.ScheduleMonths.Should().Be(Month.January | Month.Feburary | Month.March);
			binding.ScheduleMonthDays.Should().Be(MonthDay.Day31 | MonthDay.Day14 | MonthDay.Day15);
			binding.ScheduleLastDayOfMonth.Should().BeTrue();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[SkippableFact]
		public void CreateReceiveLocationWithDailyServiceWindow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new() {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new(2020, 1, 30),
						StopDate = new(2020, 3, 13),
						ServiceWindow = new DailyServiceWindow {
							StartTime = new(19, 19, 19),
							StopTime = new(9, 9, 9),
							From = new(2020, 2, 14),
							Interval = 10
						}
					};
					rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				});
			var visitor = new BindingInfoBuilderVisitor();

			var binding = visitor.CreateReceiveLocation(receiveLocation);
			binding.ScheduleTimeZone.Should().Be("Atlantic Standard Time");
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(new(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new(2020, 3, 13));
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.TimeOfDay.Should().Be(((DateTime) new Time(19, 19, 19)).TimeOfDay);
			binding.ToTime.TimeOfDay.Should().Be(((DateTime) new Time(9, 9, 9)).TimeOfDay);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Day);
			binding.ScheduleRecurFrom.Should().Be(new(2020, 2, 14));
			binding.ScheduleRecurInterval.Should().Be(10);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);

			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[SkippableFact]
		public void CreateReceiveLocationWithOrdinalMonthlyServiceWindow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new() {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new(2020, 1, 30),
						StopDate = new(2020, 3, 13),
						ServiceWindow = new OrdinalMonthlyServiceWindow {
							StartTime = new(19, 19, 19),
							StopTime = new(9, 9, 9),
							Months = Month.January | Month.Feburary | Month.March,
							WeekDays = BtsDayOfWeek.Friday | BtsDayOfWeek.Saturday,
							Ordinality = OrdinalType.Second | OrdinalType.Last
						}
					};
					rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				});
			var visitor = new BindingInfoBuilderVisitor();

			var binding = visitor.CreateReceiveLocation(receiveLocation);
			binding.ScheduleTimeZone.Should().Be("Pacific Standard Time");
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(new(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new(2020, 3, 13));
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.TimeOfDay.Should().Be(((DateTime) new Time(19, 19, 19)).TimeOfDay);
			binding.ToTime.TimeOfDay.Should().Be(((DateTime) new Time(9, 9, 9)).TimeOfDay);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Month);
			binding.ScheduleRecurFrom.Should().Be(new DailyServiceWindow().From);
			binding.ScheduleRecurInterval.Should().Be(1);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);

			binding.ScheduleMonths.Should().Be(Month.January | Month.Feburary | Month.March);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.Friday | BtsDayOfWeek.Saturday);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.Second | OrdinalType.Last);
			binding.ScheduleIsOrdinal.Should().BeTrue();
		}

		[SkippableFact]
		public void CreateReceiveLocationWithoutSchedule()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				});
			var visitor = new BindingInfoBuilderVisitor();

			var binding = visitor.CreateReceiveLocation(receiveLocation);

			binding.ScheduleTimeZone.Should().BeNull();
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(Schedule.None.StartDate);
			binding.StartDateEnabled.Should().BeFalse();
			binding.EndDate.Should().Be(Schedule.None.StopDate);
			binding.EndDateEnabled.Should().BeFalse();

			binding.ServiceWindowEnabled.Should().BeFalse();
			binding.FromTime.Should().Be(ServiceWindow.None.StartTime);
			binding.ToTime.Should().Be(ServiceWindow.None.StopTime);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.None);
			binding.ScheduleRecurFrom.Should().Be(new DailyServiceWindow().From);
			binding.ScheduleRecurInterval.Should().Be(1);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);

			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[SkippableFact]
		public void CreateReceiveLocationWithWeeklyServiceWindow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new() {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new(2020, 1, 30),
						StopDate = new(2020, 3, 13),
						ServiceWindow = new WeeklyServiceWindow {
							StartTime = new(19, 19, 19),
							StopTime = new(9, 9, 9),
							From = new(2020, 2, 14),
							Interval = 10,
							WeekDays = BtsDayOfWeek.Friday | BtsDayOfWeek.Saturday
						}
					};
					rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				});
			var visitor = new BindingInfoBuilderVisitor();

			var binding = visitor.CreateReceiveLocation(receiveLocation);
			binding.ScheduleTimeZone.Should().Be("Atlantic Standard Time");
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(new(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new(2020, 3, 13));
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.TimeOfDay.Should().Be(((DateTime) new Time(19, 19, 19)).TimeOfDay);
			binding.ToTime.TimeOfDay.Should().Be(((DateTime) new Time(9, 9, 9)).TimeOfDay);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Week);
			binding.ScheduleRecurFrom.Should().Be(new(2020, 2, 14));
			binding.ScheduleRecurInterval.Should().Be(10);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.Friday | BtsDayOfWeek.Saturday);

			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[SkippableFact]
		public void CreateReceivePortOneWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateReceivePort(new OneWayReceivePort());

			binding.ApplicationName.Should().Be(nameof(TestApplication));
			binding.Description.Should().Be("Some Useless One-Way Test Receive Port");
			binding.IsTwoWay.Should().BeFalse();
			binding.Name.Should().Be(nameof(OneWayReceivePort));
			binding.ReceiveLocations.Count.Should().Be(0);
		}

		[SkippableFact]
		public void CreateReceivePortTwoWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateReceivePort(new TwoWayReceivePort());

			binding.ApplicationName.Should().Be(nameof(TestApplication));
			binding.Description.Should().Be("Some Useless Two-Way Test Receive Port");
			binding.IsTwoWay.Should().BeTrue();
			binding.Name.Should().Be(nameof(TwoWayReceivePort));
			binding.ReceiveLocations.Count.Should().Be(0);
		}

		[SkippableFact]
		public void CreateSendPortOneWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var dsl = new OneWaySendPort();

			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateSendPort(dsl);

			binding.ApplicationName.Should().Be("TestApplication");
			binding.Description.Should().Be("Some Useless One-Way Test Send Port");
			binding.Filter.Should().NotBeNullOrEmpty();
			binding.IsDynamic.Should().BeFalse();
			binding.IsStatic.Should().BeTrue();
			binding.IsTwoWay.Should().BeFalse();
			binding.Name.Should().Be(nameof(OneWaySendPort));
			binding.OrderedDelivery.Should().BeTrue();
			binding.PrimaryTransport.Address.Should().Be(@"c:\file\drops\*.xml");
			binding.PrimaryTransport.FromTime.Should().Be(dsl.Transport.ServiceWindow.StartTime);
			binding.PrimaryTransport.OrderedDelivery.Should().BeTrue();
			binding.PrimaryTransport.Primary.Should().BeTrue();
			binding.PrimaryTransport.RetryCount.Should().Be(dsl.Transport.RetryPolicy.Count);
			binding.PrimaryTransport.RetryInterval.Should().Be((int) dsl.Transport.RetryPolicy.Interval.TotalMinutes);
			binding.PrimaryTransport.SendHandler.Name.Should().Be("Send Host Name");
			binding.PrimaryTransport.SendHandler.TransportType.Name.Should().Be("Test Dummy");
			binding.PrimaryTransport.ServiceWindowEnabled.Should().BeTrue();
			binding.PrimaryTransport.ToTime.Should().Be(dsl.Transport.ServiceWindow.StopTime);
			binding.Priority.Should().Be(1);
			binding.ReceivePipeline.Should().BeNull();
			binding.ReceivePipelineData.Should().BeNull();
			binding.SecondaryTransport.Should().BeNull();
			binding.SendPipelineData.Should().NotBeNullOrEmpty();
			binding.StopSendingOnFailure.Should().BeTrue();
			binding.TransmitPipeline.Name.Should().Be(typeof(PassThruTransmit).FullName);
			binding.TransmitPipeline.FullyQualifiedName.Should().Be(typeof(PassThruTransmit).AssemblyQualifiedName);
			binding.TransmitPipeline.TrackingOption.Should().Be(PipelineTrackingTypes.None);
			binding.TransmitPipeline.Type.Should().Be(PipelineRef.TransmitPipelineRef().Type);
		}

		[SkippableFact]
		public void CreateSendPortTwoWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateSendPort(new TwoWaySendPort());

			binding.ApplicationName.Should().Be("TestApplication");
			binding.Description.Should().Be("Some Useless Two-Way Test Send Port");
			binding.Filter.Should().BeNull();
			binding.IsDynamic.Should().BeFalse();
			binding.IsStatic.Should().BeTrue();
			binding.IsTwoWay.Should().BeTrue();
			binding.Name.Should().Be(nameof(TwoWaySendPort));
			binding.PrimaryTransport.FromTime.Should().Be(ServiceWindow.None.StartTime);
			binding.PrimaryTransport.Primary.Should().BeTrue();
			binding.PrimaryTransport.RetryCount.Should().Be(RetryPolicy.Default.Count);
			binding.PrimaryTransport.RetryInterval.Should().Be((int) RetryPolicy.Default.Interval.TotalMinutes);
			binding.PrimaryTransport.SendHandler.Name.Should().Be("Send Host Name");
			binding.PrimaryTransport.SendHandler.TransportType.Name.Should().Be("Test Dummy");
			binding.PrimaryTransport.ServiceWindowEnabled.Should().BeFalse();
			binding.PrimaryTransport.ToTime.Should().Be(ServiceWindow.None.StopTime);
			binding.ReceivePipeline.Name.Should().Be(typeof(PassThruReceive).FullName);
			binding.ReceivePipeline.FullyQualifiedName.Should().Be(typeof(PassThruReceive).AssemblyQualifiedName);
			binding.ReceivePipeline.TrackingOption.Should().Be(PipelineTrackingTypes.None);
			binding.ReceivePipeline.Type.Should().Be(PipelineRef.ReceivePipelineRef().Type);
			binding.ReceivePipelineData.Should().NotBeNullOrEmpty();
			binding.SecondaryTransport.Should().BeNull();
			binding.SendPipelineData.Should().BeEmpty();
			binding.TransmitPipeline.Name.Should().Be(typeof(PassThruTransmit).FullName);
			binding.TransmitPipeline.FullyQualifiedName.Should().Be(typeof(PassThruTransmit).AssemblyQualifiedName);
			binding.TransmitPipeline.TrackingOption.Should().Be(PipelineTrackingTypes.None);
			binding.TransmitPipeline.Type.Should().Be(PipelineRef.TransmitPipelineRef().Type);
		}

		[SkippableFact]
		public void CreateServiceRef()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var visitor = new BindingInfoBuilderVisitor();

			var orchestrationBinding = new ProcessOrchestrationBinding(
				ob => {
					ob.Description = "Some Useless Orchestration.";
					ob.Host = "Processing Host Name";
					ob.ReceivePort = new OneWayReceivePort();
					ob.RequestResponsePort = new TwoWayReceivePort();
					ob.SendPort = new OneWaySendPort();
					ob.SolicitResponsePort = new TwoWaySendPort();
				});
			var binding = visitor.CreateServiceRef(orchestrationBinding);

			binding.Description.Should().Be("Some Useless Orchestration.");
			binding.Host.Name.Should().Be("Processing Host Name");
			binding.Host.Trusted.Should().BeFalse();
			binding.Host.Type.Should().Be((int) HostType.Invalid);
			binding.Name.Should().Be(typeof(Process).FullName);
			binding.State.Should().Be(ServiceRef.ServiceRefState.Default);
			binding.TrackingOption.Should().Be(OrchestrationTrackingTypes.None);
			binding.Ports.Count.Should().Be(4);

			binding.Ports[0].Modifier.Should().Be((int) PortModifier.Import);
			binding.Ports[0].Name.Should().Be("SendPort");
			binding.Ports[0].ReceivePortRef.Should().BeNull();
			binding.Ports[0].SendPortRef.Name.Should().Be(((ISupportNamingConvention) new OneWaySendPort()).Name);
			binding.Ports[1].Modifier.Should().Be((int) PortModifier.Export);

			binding.Ports[1].Name.Should().Be("ReceivePort");
			binding.Ports[1].ReceivePortRef.Name.Should().Be(((ISupportNamingConvention) new OneWayReceivePort()).Name);
			binding.Ports[1].SendPortRef.Should().BeNull();

			binding.Ports[2].Modifier.Should().Be((int) PortModifier.Export);
			binding.Ports[2].Name.Should().Be("RequestResponsePort");
			binding.Ports[2].ReceivePortRef.Name.Should().Be(((ISupportNamingConvention) new TwoWayReceivePort()).Name);
			binding.Ports[2].SendPortRef.Should().BeNull();

			binding.Ports[3].Modifier.Should().Be((int) PortModifier.Import);
			binding.Ports[3].Name.Should().Be("SolicitResponsePort");
			binding.Ports[3].ReceivePortRef.Should().BeNull();
			binding.Ports[3].SendPortRef.Name.Should().Be(((ISupportNamingConvention) new TwoWaySendPort()).Name);
		}

		[SkippableFact]
		public void VisitedReceiveLocationNameMustBeUnique()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBinding = new TestApplication();
			var receivePort = applicationBinding.ReceivePorts.Find<OneWayReceivePort>();
			var receiveLocation = receivePort.ReceiveLocations.Find<OneWayReceiveLocation>();

			var visitor = new BindingInfoBuilderVisitor();
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(applicationBinding);
			((IApplicationBindingVisitor) visitor).VisitReceivePort(receivePort);
			((IApplicationBindingVisitor) visitor).VisitReceiveLocation(receiveLocation);

			Invoking(() => ((IApplicationBindingVisitor) visitor).VisitReceiveLocation(receiveLocation))
				.Should().Throw<BindingException>()
				.WithMessage("Duplicate receive location name: 'OneWayReceiveLocation'.");
		}

		[SkippableFact]
		public void VisitedReceivePortNameMustBeUnique()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBinding = new TestApplication();
			var receivePort = applicationBinding.ReceivePorts.Find<OneWayReceivePort>();

			var visitor = new BindingInfoBuilderVisitor();
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(applicationBinding);
			((IApplicationBindingVisitor) visitor).VisitReceivePort(receivePort);

			Invoking(() => ((IApplicationBindingVisitor) visitor).VisitReceivePort(receivePort))
				.Should().Throw<BindingException>()
				.WithMessage("Duplicate receive port name: 'OneWayReceivePort'.");
		}

		[SkippableFact]
		public void VisitedSendPortNameMustBeUnique()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBinding = new TestApplication();
			var sendPort = applicationBinding.SendPorts.Find<OneWaySendPort>();

			var visitor = new BindingInfoBuilderVisitor();
			((IApplicationBindingVisitor) visitor).VisitApplicationBinding(applicationBinding);
			((IApplicationBindingVisitor) visitor).VisitSendPort(sendPort);

			Invoking(() => ((IApplicationBindingVisitor) visitor).VisitSendPort(sendPort))
				.Should().Throw<BindingException>()
				.WithMessage("Duplicate send port name: 'OneWaySendPort'.");
		}
	}
}
