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
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ScheduleFixture
	{
		[Fact]
		public void NoneEqualsBindingDefaultServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			Schedule.None.StartDate.Should().Be(rl.StartDate);
			Schedule.None.StartDateEnabled.Should().BeFalse();
			Schedule.None.StopDate.Should().Be(rl.EndDate);
			Schedule.None.StopDateEnabled.Should().BeFalse();

			Schedule.None.ServiceWindow.Enabled.Should().Be(ServiceWindow.None.Enabled);
			Schedule.None.ServiceWindow.StartTime.Should().Be(ServiceWindow.None.StartTime);
			Schedule.None.ServiceWindow.StopTime.Should().Be(ServiceWindow.None.StopTime);
		}

		[Fact]
		public void StartAndStopDateEnabled()
		{
			var s = new Schedule { StartDate = new DateTime(2015, 2, 13), StopDate = new DateTime(2015, 2, 20) };

			s.StartDateEnabled.Should().BeTrue();
			s.StopDateEnabled.Should().BeTrue();

			s.ServiceWindow.Enabled.Should().Be(ServiceWindow.None.Enabled);
			s.ServiceWindow.StartTime.Should().Be(ServiceWindow.None.StartTime);
			s.ServiceWindow.StopTime.Should().Be(ServiceWindow.None.StopTime);
		}

		[Fact]
		public void StartDateEnabled()
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
		public void StopDateEnabled()
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
