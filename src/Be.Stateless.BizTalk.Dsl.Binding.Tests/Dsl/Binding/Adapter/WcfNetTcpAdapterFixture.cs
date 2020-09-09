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
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfNetTcpAdapterFixture
	{
		[Fact]
		public void ProtocolTypeSettingsAreReadFromRegistry()
		{
			var mock = new Mock<WcfNetTcpAdapter<NetTcpRLConfig>> { CallBase = true };
			var nta = mock.Object as IAdapter;
			nta.ProtocolType.Name.Should().Be("WCF-NetTcp");
			nta.ProtocolType.Capabilities.Should().Be(907);
			nta.ProtocolType.ConfigurationClsid.Should().Be("7fd2dfcd-6a7b-44f9-8387-29457fd2eaaf");
		}
	}
}
