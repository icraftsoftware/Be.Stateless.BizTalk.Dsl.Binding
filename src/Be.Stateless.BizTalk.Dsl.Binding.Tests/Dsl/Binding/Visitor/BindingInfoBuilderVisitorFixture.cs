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

extern alias ExplorerOM;
using System;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Orchestrations.Bound;
using ExplorerOM::Microsoft.BizTalk.ExplorerOM;
using FluentAssertions;
using Microsoft.BizTalk.BtsScheduleHelper;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;
using BtsDayOfWeek = ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper.BtsDayOfWeek;
using Month = ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper.Month;
using MonthDay = ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper.MonthDay;
using OrdinalType = ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper.OrdinalType;
using ProtocolType = Microsoft.BizTalk.Deployment.Binding.ProtocolType;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class BindingInfoBuilderVisitorFixture
	{
		[Fact]
		public void CreateBindingInfo()
		{
			var visitor = new BindingInfoBuilderVisitor();
			visitor.VisitApplicationBinding(new TestApplication());

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

		[Fact]
		public void CreateModuleRef()
		{
			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfo
			visitor.VisitApplicationBinding(new TestApplication());

			var binding = visitor.CreateOrFindModuleRef(new ProcessOrchestrationBinding());

			binding.FullName.Should().Be(typeof(Process).Assembly.FullName);
		}

		[Fact]
		public void CreateReceiveLocationOneWay()
		{
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

		[Fact]
		public void CreateReceiveLocationTwoWay()
		{
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

		[Fact]
		public void CreateReceiveLocationWithCalendricalMonthlyServiceWindow()
		{
			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new Schedule {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new DateTime(2020, 1, 30),
						StopDate = new DateTime(2020, 3, 13),
						ServiceWindow = new CalendricalMonthlyServiceWindow {
							StartTime = new Time(19, 19, 19),
							StopTime = new Time(9, 9, 9),
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
			binding.StartDate.Should().Be(new DateTime(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new DateTime(2020, 3, 13));
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

		[Fact]
		public void CreateReceiveLocationWithDailyServiceWindow()
		{
			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new Schedule {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new DateTime(2020, 1, 30),
						StopDate = new DateTime(2020, 3, 13),
						ServiceWindow = new DailyServiceWindow {
							StartTime = new Time(19, 19, 19),
							StopTime = new Time(9, 9, 9),
							From = new DateTime(2020, 2, 14),
							Interval = 10
						}
					};
					rl.ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				});
			var visitor = new BindingInfoBuilderVisitor();

			var binding = visitor.CreateReceiveLocation(receiveLocation);
			binding.ScheduleTimeZone.Should().Be("Atlantic Standard Time");
			binding.ScheduleAutoAdjustToDaylightSaving.Should().BeFalse();
			binding.StartDate.Should().Be(new DateTime(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new DateTime(2020, 3, 13));
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.TimeOfDay.Should().Be(((DateTime) new Time(19, 19, 19)).TimeOfDay);
			binding.ToTime.TimeOfDay.Should().Be(((DateTime) new Time(9, 9, 9)).TimeOfDay);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Day);
			binding.ScheduleRecurFrom.Should().Be(new DateTime(2020, 2, 14));
			binding.ScheduleRecurInterval.Should().Be(10);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.None);

			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[Fact]
		public void CreateReceiveLocationWithOrdinalMonthlyServiceWindow()
		{
			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new Schedule {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new DateTime(2020, 1, 30),
						StopDate = new DateTime(2020, 3, 13),
						ServiceWindow = new OrdinalMonthlyServiceWindow {
							StartTime = new Time(19, 19, 19),
							StopTime = new Time(9, 9, 9),
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
			binding.StartDate.Should().Be(new DateTime(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new DateTime(2020, 3, 13));
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

		[Fact]
		public void CreateReceiveLocationWithoutSchedule()
		{
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

		[Fact]
		public void CreateReceiveLocationWithWeeklyServiceWindow()
		{
			var receiveLocation = new ReceiveLocation(
				rl => {
					rl.Name = "Dummy Receive Location";
					rl.Transport.Adapter = new DummyAdapter();
					rl.Transport.Host = "Receive Host Name";
					rl.Transport.Schedule = new Schedule {
						TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time"),
						AutomaticallyAdjustForDaylightSavingTime = false,
						StartDate = new DateTime(2020, 1, 30),
						StopDate = new DateTime(2020, 3, 13),
						ServiceWindow = new WeeklyServiceWindow {
							StartTime = new Time(19, 19, 19),
							StopTime = new Time(9, 9, 9),
							From = new DateTime(2020, 2, 14),
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
			binding.StartDate.Should().Be(new DateTime(2020, 1, 30));
			binding.StartDateEnabled.Should().BeTrue();
			binding.EndDate.Should().Be(new DateTime(2020, 3, 13));
			binding.EndDateEnabled.Should().BeTrue();

			binding.ServiceWindowEnabled.Should().BeTrue();
			binding.FromTime.TimeOfDay.Should().Be(((DateTime) new Time(19, 19, 19)).TimeOfDay);
			binding.ToTime.TimeOfDay.Should().Be(((DateTime) new Time(9, 9, 9)).TimeOfDay);

			binding.ScheduleRecurrenceType.Should().Be(RecurrenceType.Week);
			binding.ScheduleRecurFrom.Should().Be(new DateTime(2020, 2, 14));
			binding.ScheduleRecurInterval.Should().Be(10);

			binding.ScheduleDaysOfWeek.Should().Be(BtsDayOfWeek.Friday | BtsDayOfWeek.Saturday);

			binding.ScheduleMonths.Should().Be(Month.None);
			binding.ScheduleMonthDays.Should().Be(MonthDay.None);
			binding.ScheduleLastDayOfMonth.Should().BeFalse();
			binding.ScheduleOrdinalDayOfWeek.Should().Be(BtsDayOfWeek.None);
			binding.ScheduleOrdinalType.Should().Be(OrdinalType.None);
			binding.ScheduleIsOrdinal.Should().BeFalse();
		}

		[Fact]
		public void CreateReceivePortOneWay()
		{
			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateReceivePort(new OneWayReceivePort());

			binding.ApplicationName.Should().Be(nameof(TestApplication));
			binding.Description.Should().Be("Some Useless One-Way Test Receive Port");
			binding.IsTwoWay.Should().BeFalse();
			binding.Name.Should().Be(nameof(OneWayReceivePort));
			binding.ReceiveLocations.Count.Should().Be(0);
		}

		[Fact]
		public void CreateReceivePortTwoWay()
		{
			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
			var binding = visitor.CreateReceivePort(new TwoWayReceivePort());

			binding.ApplicationName.Should().Be(nameof(TestApplication));
			binding.Description.Should().Be("Some Useless Two-Way Test Receive Port");
			binding.IsTwoWay.Should().BeTrue();
			binding.Name.Should().Be(nameof(TwoWayReceivePort));
			binding.ReceiveLocations.Count.Should().Be(0);
		}

		[Fact]
		public void CreateSendPortOneWay()
		{
			var dsl = new OneWaySendPort();

			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
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

		[Fact]
		public void CreateSendPortTwoWay()
		{
			var visitor = new BindingInfoBuilderVisitor();
			// initialize BindingInfoBuilderVisitor.ApplicationName
			visitor.VisitApplicationBinding(new TestApplication());
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

		[Fact]
		public void CreateServiceRef()
		{
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

		[Fact]
		public void VisitApplicationBindingSettlesTargetEnvironmentOverrides()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			var visitableApplicationBindingMock = applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var visitor = new BindingInfoBuilderVisitor();
			visitor.VisitApplicationBinding(applicationBindingMock.Object);

			visitableApplicationBindingMock.Verify(a => a.Accept(It.IsAny<ApplicationBindingEnvironmentSettlerVisitor>()), Times.Once);
		}

		[Fact]
		public void VisitedReceiveLocationNameMustBeUnique()
		{
			var visitor = new BindingInfoBuilderVisitor();
			visitor.VisitApplicationBinding(new TestApplication());
			visitor.VisitReceivePort(new OneWayReceivePort());
			visitor.VisitReceiveLocation(new OneWayReceiveLocation());

			Action(() => visitor.VisitReceiveLocation(new OneWayReceiveLocation()))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Duplicate receive location name: 'OneWayReceiveLocation'.");
		}

		[Fact]
		public void VisitedReceivePortNameMustBeUnique()
		{
			var visitor = new BindingInfoBuilderVisitor();
			visitor.VisitApplicationBinding(new TestApplication());
			visitor.VisitReceivePort(new OneWayReceivePort());

			Action(() => visitor.VisitReceivePort(new OneWayReceivePort()))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Duplicate receive port name: 'OneWayReceivePort'.");
		}

		[Fact]
		public void VisitedSendPortNameMustBeUnique()
		{
			var visitor = new BindingInfoBuilderVisitor();
			visitor.VisitApplicationBinding(new TestApplication());
			visitor.VisitSendPort(new OneWaySendPort());

			Action(() => visitor.VisitSendPort(new OneWaySendPort()))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("Duplicate send port name: 'OneWaySendPort'.");
		}
	}
}
