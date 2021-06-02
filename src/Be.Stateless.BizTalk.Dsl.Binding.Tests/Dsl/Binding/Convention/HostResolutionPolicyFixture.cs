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
			((ISupportHostNameResolution) receiveLocationMock.Object.Transport).ResolveHostName().Should().Be("Plain Receive Host Name");
		}

		[Fact]
		public void ReceiveLocationTransportHostNameResolutionCanBeOverridden()
		{
			var policyMock = new Mock<HostResolutionPolicy> { CallBase = true };
			var sut = policyMock.As<IResolveTransportHost>();

			var receiveLocationMock = new Mock<ReceiveLocation> { CallBase = true };
			receiveLocationMock.Object.Transport.Host = policyMock.Object;

			sut.Setup(p => p.ResolveHostName(receiveLocationMock.Object.Transport)).Returns("some name").Verifiable();

			((ISupportHostNameResolution) receiveLocationMock.Object.Transport).ResolveHostName().Should().Be("some name");
			sut.Verify();
		}

		[Fact]
		public void SendPortTransportHostCanBeAssignedPlainString()
		{
			var sendPortMock = new Mock<SendPort> { CallBase = true };
			sendPortMock.Object.Transport.Host = "Plain Send Host Name";
			((ISupportHostNameResolution) sendPortMock.Object.Transport).ResolveHostName().Should().Be("Plain Send Host Name");
		}

		[Fact]
		public void SendPortTransportHostNameResolutionCanBeOverridden()
		{
			var policyMock = new Mock<HostResolutionPolicy> { CallBase = true };
			var sut = policyMock.As<IResolveTransportHost>();

			var sendPortMock = new Mock<SendPort> { CallBase = true };
			sendPortMock.Object.Transport.Host = policyMock.Object;

			sut.Setup(p => p.ResolveHostName(sendPortMock.Object.Transport)).Returns("some name").Verifiable();

			((ISupportHostNameResolution) sendPortMock.Object.Transport).ResolveHostName().Should().Be("some name");
			sut.Verify();
		}
	}
}
