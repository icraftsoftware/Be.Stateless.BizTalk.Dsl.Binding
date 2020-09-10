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
using ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	public class ScheduleFixture
	{
		[Fact]
		public void DefaultEqualsBindingDefaultSchedule()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			var sut = new Schedule();

			sut.TimeZone.Should().Be(TimeZoneInfo.Utc);
			sut.AutomaticallyAdjustForDaylightSavingTime.Should().BeTrue();
			sut.StartDate.Should().Be(rl.StartDate);
			sut.StartDateEnabled.Should().Be(rl.StartDateEnabled);
			sut.StopDate.Should().Be(rl.EndDate);
			sut.StopDateEnabled.Should().Be(rl.EndDateEnabled);
			sut.RecurrenceType.Should().Be(rl.ScheduleRecurrenceType);
		}

		[Fact]
		public void NoneEqualsBindingDefaultSchedule()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			Schedule.None.TimeZone.Should().Be(TimeZoneInfo.Utc);
			Schedule.None.AutomaticallyAdjustForDaylightSavingTime.Should().BeTrue();
			Schedule.None.StartDate.Should().Be(rl.StartDate);
			Schedule.None.StartDateEnabled.Should().BeFalse();
			Schedule.None.StopDate.Should().Be(rl.EndDate);
			Schedule.None.StopDateEnabled.Should().BeFalse();
			Schedule.None.RecurrenceType.Should().Be(rl.ScheduleRecurrenceType);
		}

		[Fact]
		public void NoneEqualsServiceWindowNone()
		{
			Schedule.None.ServiceWindow.Enabled.Should().Be(ServiceWindow.None.Enabled);
			Schedule.None.ServiceWindow.StartTime.Should().Be(ServiceWindow.None.StartTime);
			Schedule.None.ServiceWindow.StopTime.Should().Be(ServiceWindow.None.StopTime);
		}

		[Fact]
		public void RecurrenceTypeIsComputedAfterServiceWindow()
		{
			new Schedule().RecurrenceType.Should().Be(RecurrenceType.None);
			new Schedule { ServiceWindow = new DailyServiceWindow() }.RecurrenceType.Should().Be(RecurrenceType.Day);
			new Schedule { ServiceWindow = new WeeklyServiceWindow() }.RecurrenceType.Should().Be(RecurrenceType.Week);
			new Schedule { ServiceWindow = new CalendricalMonthlyServiceWindow() }.RecurrenceType.Should().Be(RecurrenceType.Month);
			new Schedule { ServiceWindow = new OrdinalMonthlyServiceWindow() }.RecurrenceType.Should().Be(RecurrenceType.Month);
		}

		[Fact]
		public void StartAndStopDatesAreEnabled()
		{
			var s = new Schedule { StartDate = new DateTime(2015, 2, 13), StopDate = new DateTime(2015, 2, 20) };

			s.StartDateEnabled.Should().BeTrue();
			s.StopDateEnabled.Should().BeTrue();

			s.ServiceWindow.Enabled.Should().Be(ServiceWindow.None.Enabled);
			s.ServiceWindow.StartTime.Should().Be(ServiceWindow.None.StartTime);
			s.ServiceWindow.StopTime.Should().Be(ServiceWindow.None.StopTime);
		}

		[Fact]
		public void StartDateIsEnabled()
		{
			var s = new Schedule { StartDate = new DateTime(2015, 2, 13) };

			s.StartDateEnabled.Should().BeTrue();
			s.StopDate.Should().Be(Schedule.None.StopDate);
			s.StopDateEnabled.Should().BeFalse();

			s.ServiceWindow.Enabled.Should().Be(ServiceWindow.None.Enabled);
			s.ServiceWindow.StartTime.Should().Be(ServiceWindow.None.StartTime);
			s.ServiceWindow.StopTime.Should().Be(ServiceWindow.None.StopTime);
		}

		[Fact]
		public void StopDateIsEnabled()
		{
			var s = new Schedule { StopDate = new DateTime(2015, 2, 20) };

			s.StartDate.Should().Be(Schedule.None.StartDate);
			s.StartDateEnabled.Should().BeFalse();
			s.StopDateEnabled.Should().BeTrue();

			s.ServiceWindow.Enabled.Should().Be(ServiceWindow.None.Enabled);
			s.ServiceWindow.StartTime.Should().Be(ServiceWindow.None.StartTime);
			s.ServiceWindow.StopTime.Should().Be(ServiceWindow.None.StopTime);
		}
	}
}
