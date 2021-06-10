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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class SendPortTransportFixture
	{
		[Fact]
		public void DefaultUnknownOutboundAdapterFailsValidate()
		{
			var spt = new SendPortTransport<string>(new Mock<ISendPort<string>>().Object) { Host = "Host" };
			Invoking(() => ((ISupportValidation) spt).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Transport's Adapter is not defined.");
		}

		[Fact]
		public void DelegatesHostNameResolutionToHostResolutionPolicy()
		{
			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.As<ISupportNamingConvention>();

			var hostResolutionPolicyMock = new Mock<HostResolutionPolicy> { CallBase = true };
			var sut = hostResolutionPolicyMock.As<IResolveHost>();

			var spt = new SendPortTransport<string>(sendPortMock.Object) { Host = hostResolutionPolicyMock.Object };
			sut.Setup(p => p.ResolveHostName(spt)).Returns("name");
			((ISupportHostNameResolution) spt).ResolveHostName();

			sut.Verify(p => p.ResolveHostName(spt));
		}

		[Fact]
		public void ForwardsApplyEnvironmentOverridesToRetryPolicy()
		{
			var retryPolicyMock = new Mock<RetryPolicy>();
			var environmentSensitiveRetryPolicyMock = retryPolicyMock.As<ISupportEnvironmentOverride>();

			var spt = new SendPortTransport<string>(new Mock<ISendPort<string>>().Object) { RetryPolicy = retryPolicyMock.Object };
			((ISupportEnvironmentOverride) spt).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveRetryPolicyMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Once);
		}

		[Fact]
		public void ForwardsApplyEnvironmentOverridesToServiceWindow()
		{
			var serviceWindowMock = new Mock<ServiceWindow>();
			var environmentSensitiveServiceWindowMock = serviceWindowMock.As<ISupportEnvironmentOverride>();

			var spt = new SendPortTransport<string>(new Mock<ISendPort<string>>().Object) { ServiceWindow = serviceWindowMock.Object };
			((ISupportEnvironmentOverride) spt).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveServiceWindowMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Once);
		}

		[Fact]
		public void SendPortIsReferencedBack()
		{
			var sendPortMock = new Mock<SendPort>();
			sendPortMock.Object.Transport.SendPort.Should().BeSameAs(sendPortMock.Object);
		}

		[Fact]
		public void ThrowsWhenBackupTransportHostNameCannotBeResolved()
		{
			var sendPortMock = new Mock<SendPort> { CallBase = true };
			var conventionalSendPortMock = sendPortMock.As<ISupportNamingConvention>();
			conventionalSendPortMock.Setup(m => m.Name).Returns("Dummy Send Port Name");

			var spt = sendPortMock.Object.BackupTransport.Value;

			Invoking(() => ((ISupportHostNameResolution) spt).ResolveHostName())
				.Should().Throw<BindingException>()
				.WithMessage("Backup Transport's Host could not be resolved for SendPort 'Dummy Send Port Name'.");
		}

		[Fact]
		public void ThrowsWhenPrimaryTransportHostNameCannotBeResolved()
		{
			var sendPortMock = new Mock<SendPort> { CallBase = true };
			var conventionalSendPortMock = sendPortMock.As<ISupportNamingConvention>();
			conventionalSendPortMock.Setup(m => m.Name).Returns("Dummy Send Port Name");

			var spt = sendPortMock.Object.Transport;

			Invoking(() => ((ISupportHostNameResolution) spt).ResolveHostName())
				.Should().Throw<BindingException>()
				.WithMessage("Primary Transport's Host could not be resolved for SendPort 'Dummy Send Port Name'.");
		}
	}
}
