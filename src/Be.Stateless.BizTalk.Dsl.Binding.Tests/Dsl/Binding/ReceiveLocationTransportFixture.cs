﻿#region Copyright & License

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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ReceiveLocationTransportFixture
	{
		[Fact]
		public void DefaultUnknownInboundAdapterFailsValidate()
		{
			var rlt = new ReceiveLocationTransport<string>(new Mock<IReceiveLocation<string>>().Object) { Host = "Host" };
			Invoking(() => ((ISupportValidation) rlt).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Transport's Adapter is not defined.");
		}

		[Fact]
		public void DelegatesHostNameResolutionToHostResolutionPolicy()
		{
			var receiveLocationMock = new Mock<IReceiveLocation<string>>();

			var sut = new Mock<HostResolutionPolicy> { CallBase = true };

			var rlt = new ReceiveLocationTransport<string>(receiveLocationMock.Object) { Host = sut.Object };
			sut.Setup(p => p.ResolveHost(rlt)).Returns("name");
			rlt.ResolveHost();

			sut.Verify(p => p.ResolveHost(rlt));
		}

		[Fact]
		public void ForwardsApplyEnvironmentOverridesToSchedule()
		{
			var scheduleMock = new Mock<Schedule>();
			var environmentSensitiveScheduleMock = scheduleMock.As<ISupportEnvironmentOverride>();

			var rlt = new ReceiveLocationTransport<string>(new Mock<IReceiveLocation<string>>().Object) { Schedule = scheduleMock.Object };
			((ISupportEnvironmentOverride) rlt).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveScheduleMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Once);
		}

		[Fact]
		public void ReceiveLocationIsReferencedBack()
		{
			var receiveLocationMock = new Mock<ReceiveLocation>();
			receiveLocationMock.Object.Transport.ReceiveLocation.Should().BeSameAs(receiveLocationMock.Object);
		}

		[Fact]
		public void ThrowsWhenTransportHostNameCannotBeResolved()
		{
			var receiveLocationMock = new Mock<ReceiveLocation> { CallBase = true };
			receiveLocationMock.As<ISupportNameResolution>().Setup(m => m.ResolveName()).Returns("Dummy Receive Location Name");

			var rlt = receiveLocationMock.Object.Transport;

			Invoking(() => rlt.ResolveHost())
				.Should().Throw<BindingException>()
				.WithMessage("Transport's Host could not be resolved for ReceiveLocation 'Dummy Receive Location Name'.");
		}
	}
}
