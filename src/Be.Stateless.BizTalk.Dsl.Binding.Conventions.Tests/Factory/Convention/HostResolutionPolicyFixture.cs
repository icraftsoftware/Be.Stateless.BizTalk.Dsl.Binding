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

using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Factory.Convention
{
	public class HostResolutionPolicyFixture
	{
		[Fact]
		public void ResolveIsolatedHostForHttpAdapter()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
			receiveLocationMock.Object.Transport.Adapter = new HttpAdapter.Inbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);
		}

		[Fact]
		public void ResolveIsolatedHostForLogicAppAdapter()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
			receiveLocationMock.Object.Transport.Adapter = new FileAdapter.Inbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);
		}

		[Fact]
		public void ResolveIsolatedHostForWcfBasicHttpAdapter()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
			receiveLocationMock.Object.Transport.Adapter = new WcfBasicHttpAdapter.Inbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);
		}

		[Fact]
		public void ResolveIsolatedHostForWcfCustomIsolatedAdapter()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();

			receiveLocationMock.Object.Transport.Adapter = new WcfCustomIsolatedAdapter.Inbound<NetTcpBindingElement>(_ => { });
			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);

			receiveLocationMock.Object.Transport.Adapter = new WcfCustomIsolatedAdapter.Inbound<WebHttpBindingElement>(_ => { });
			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);
		}

		[Fact]
		public void ResolveIsolatedHostForWcfWebHttpAdapter()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
			receiveLocationMock.Object.Transport.Adapter = new WcfWebHttpAdapter.Inbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);
		}

		[Fact]
		public void ResolveIsolatedHostForWcfWSHttpAdapter()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
			receiveLocationMock.Object.Transport.Adapter = new WcfWSHttpAdapter.Inbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.IsolatedHost);
		}

		[Fact]
		public void ResolveProcessingHost()
		{
			new HostResolutionPolicySpy().ResolveHostName(new Mock<IOrchestrationBinding>().Object).Should().Be(Platform.Settings.ProcessingHost);
		}

		[Fact]
		public void ResolveReceivingHost()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>();
			receiveLocationMock.Object.Transport.Adapter = new FileAdapter.Inbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(receiveLocationMock.Object.Transport).Should().Be(Platform.Settings.ReceivingHost);
		}

		[Fact]
		public void ResolveTransmittingHost()
		{
			var sendPortMock = new Mock<SendPortBase<string>>();
			sendPortMock.Object.Transport.Adapter = new FileAdapter.Outbound(_ => { });

			new HostResolutionPolicySpy().ResolveHostName(sendPortMock.Object.Transport).Should().Be(Platform.Settings.TransmittingHost);
		}

		private class HostResolutionPolicySpy : HostResolutionPolicy
		{
			internal new string ResolveHostName(IOrchestrationBinding orchestration)
			{
				return base.ResolveHostName(orchestration);
			}

			internal new string ResolveHostName<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHostName(transport);
			}

			internal new string ResolveHostName<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
				where TNamingConvention : class
			{
				return base.ResolveHostName(transport);
			}
		}
	}
}
