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

using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	public class OrdinalMonthlyServiceWindowFixture
	{
		[Fact]
		public void DefaultEqualsBindingDefaultServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			var sut = new OrdinalMonthlyServiceWindow();

			sut.IsOrdinal.Should().BeTrue();
			sut.Months.Should().Be(rl.ScheduleMonths);
			sut.Ordinality.Should().Be(rl.ScheduleOrdinalType);
			sut.WeekDays.Should().Be(rl.ScheduleOrdinalDayOfWeek);
		}
	}
}
