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
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	public class DailyServiceWindowFixture
	{
		[Fact]
		public void DefaultEqualsBindingDefaultServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();

			var sut = new DailyServiceWindow();

			sut.From.Should().Be(rl.ScheduleRecurFrom);
			sut.Interval.Should().Be(rl.ScheduleRecurInterval);
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void IntervalMustBeGreaterThan0()
		{
			Action(() => new DailyServiceWindow { Interval = 0 }).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void IntervalMustBeLessThan999()
		{
			Action(() => new DailyServiceWindow { Interval = 1000 }).Should().Throw<ArgumentOutOfRangeException>();
		}
	}
}
