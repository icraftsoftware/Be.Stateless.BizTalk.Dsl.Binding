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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class NetMsmqRetryPolicyFixture
	{
		[Fact]
		public void IsEnvironmentSensitive()
		{
			var wca = NetMsmqRetryPolicy.LongRunning;

			((ISupportEnvironmentOverride) wca).ApplyEnvironmentOverrides("BLD");
			wca.MaxRetryCycles.Should().Be(0);
			wca.ReceiveRetryCount.Should().Be(2);
			wca.RetryCycleDelay.Should().Be(TimeSpan.Zero);
			wca.TimeToLive.Should().Be(TimeSpan.FromMinutes(1));

			((ISupportEnvironmentOverride) wca).ApplyEnvironmentOverrides("ACC");
			wca.MaxRetryCycles.Should().Be(3);
			wca.ReceiveRetryCount.Should().Be(3);
			wca.RetryCycleDelay.Should().Be(TimeSpan.FromMinutes(9));
			wca.TimeToLive.Should().Be(TimeSpan.FromMinutes(30));

			((ISupportEnvironmentOverride) wca).ApplyEnvironmentOverrides("PRD");
			wca.MaxRetryCycles.Should().Be(71);
			wca.ReceiveRetryCount.Should().Be(1);
			wca.RetryCycleDelay.Should().Be(TimeSpan.FromHours(1));
			wca.TimeToLive.Should().Be(TimeSpan.FromDays(3));
		}

		[Fact]
		public void SerializeToXml()
		{
			var binding = new NetMsmqBindingElement { RetryPolicy = NetMsmqRetryPolicy.LongRunning };
			binding.ApplyEnvironmentOverrides("ACC");

			binding.GetBindingElementXml("netMsmqBinding")
				.Should().Be("<binding name=\"netMsmqBinding\" maxRetryCycles=\"3\" receiveRetryCount=\"3\" retryCycleDelay=\"00:09:00\" timeToLive=\"00:30:00\" />");
		}
	}
}
