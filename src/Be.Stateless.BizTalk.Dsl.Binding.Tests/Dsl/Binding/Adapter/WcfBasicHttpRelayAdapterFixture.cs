﻿#region Copyright & License

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
using Microsoft.BizTalk.Adapter.ServiceBus;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfBasicHttpRelayAdapterFixture
	{
		[Fact]
		public void ProtocolTypeSettingsAreReadFromWmiConfigurationClassId()
		{
			var mock = new Mock<WcfBasicHttpRelayAdapter<BasicHttpRelayRLConfig>> { CallBase = true };
			var nta = mock.Object as IAdapter;
			nta.ProtocolType.Name.Should().Be("WCF-BasicHttpRelay");
			nta.ProtocolType.Capabilities.Should().Be(907);
			nta.ProtocolType.ConfigurationClsid.Should().Be("f15097a3-283a-40b2-aca7-6b7bae0a0955");
		}
	}
}
