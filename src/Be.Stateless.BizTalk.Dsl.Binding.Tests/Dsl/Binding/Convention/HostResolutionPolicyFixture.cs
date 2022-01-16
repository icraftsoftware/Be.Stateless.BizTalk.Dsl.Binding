#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class HostResolutionPolicyFixture
	{
		[Fact]
		public void ReceiveLocationTransportHostCanBeAssignedPlainString()
		{
			var receiveLocationMock = new Mock<ReceiveLocation> { CallBase = true };
			receiveLocationMock.Object.Transport.Host = "Plain Receive Host Name";
			receiveLocationMock.Object.Transport.ResolveHost().Should().Be("Plain Receive Host Name");
		}

		[Fact]
		public void ReceiveLocationTransportHostNameResolutionCanBeOverridden()
		{
			var sut = new Mock<HostResolutionPolicy> { CallBase = true };

			var receiveLocationMock = new Mock<ReceiveLocation> { CallBase = true };
			receiveLocationMock.Object.Transport.Host = sut.Object;

			sut.Setup(p => p.ResolveHost(receiveLocationMock.Object.Transport)).Returns("some name").Verifiable();

			receiveLocationMock.Object.Transport.ResolveHost().Should().Be("some name");
			sut.Verify();
		}

		[Fact]
		public void SendPortTransportHostCanBeAssignedPlainString()
		{
			var sendPortMock = new Mock<SendPort> { CallBase = true };
			sendPortMock.Object.Transport.Host = "Plain Send Host Name";
			sendPortMock.Object.Transport.ResolveHost().Should().Be("Plain Send Host Name");
		}

		[Fact]
		public void SendPortTransportHostNameResolutionCanBeOverridden()
		{
			var sut = new Mock<HostResolutionPolicy> { CallBase = true };

			var sendPortMock = new Mock<SendPort> { CallBase = true };
			sendPortMock.Object.Transport.Host = sut.Object;

			sut.Setup(p => p.ResolveHost(sendPortMock.Object.Transport)).Returns("some name").Verifiable();

			sendPortMock.Object.Transport.ResolveHost().Should().Be("some name");
			sut.Verify();
		}
	}
}
