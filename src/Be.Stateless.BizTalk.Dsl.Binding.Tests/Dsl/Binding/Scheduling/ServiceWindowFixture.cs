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

using System;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	public class ServiceWindowFixture
	{
		[Fact]
		public void DateOfCustomStartAndSopTimeIsEqualsToDateOfDefaultStartAndStopTime()
		{
			var sw = new ServiceWindow { StartTime = new(8, 0), StopTime = new(20, 0) };

			((DateTime) sw.StartTime).Date.Should().Be(((DateTime) ServiceWindow.None.StartTime).Date);
			((DateTime) sw.StartTime).TimeOfDay.Should().NotBe(((DateTime) ServiceWindow.None.StartTime).TimeOfDay);
			((DateTime) sw.StopTime).Date.Should().Be(((DateTime) ServiceWindow.None.StopTime).Date);
			((DateTime) sw.StopTime).TimeOfDay.Should().NotBe(((DateTime) ServiceWindow.None.StopTime).TimeOfDay);
		}

		[Fact]
		public void EnabledOnStartAndStopTime()
		{
			var sw = new ServiceWindow { StartTime = new(8, 0), StopTime = new(20, 0) };

			sw.Enabled.Should().BeTrue();
		}

		[Fact]
		public void EnabledOnStartTime()
		{
			var sw = new ServiceWindow { StartTime = new(8, 0) };

			sw.Enabled.Should().BeTrue();
		}

		[Fact]
		public void EnabledOnStopTime()
		{
			var sw = new ServiceWindow { StopTime = new(20, 0) };

			sw.Enabled.Should().BeTrue();
		}

		[Fact]
		public void NoneEqualsBindingDefaultServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			((DateTime) ServiceWindow.None.StartTime).Should().Be(rl.FromTime);
			((DateTime) ServiceWindow.None.StopTime).Should().Be(rl.ToTime);
			ServiceWindow.None.Enabled.Should().BeFalse();
		}
	}
}
