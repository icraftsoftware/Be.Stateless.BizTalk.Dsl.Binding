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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Microsoft.BizTalk.DefaultPipelines;
using Moq;
using Moq.Protected;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	// http://social.msdn.microsoft.com/Forums/en-US/fec0c1c2-a3fd-4b40-8f12-deab25a91c92/sendport-bindingoption
	// A Send Port BindingOption="0" means that no Orchestrations are bound to this Send Port. A Send Port
	// BindingOption="1" indicates that at least one Orchestration is bound to this Send Port. Therefore if
	// Send Port BindingOption="0" make sure Send Port subscribes to messages via a Filter, because if it
	// doesn't then it has no means of subscribing to messages (no Orchestration binding, no Filter expression).

	public class SendPortBaseFixture
	{
		[Fact]
		public void AcceptsVisitor()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) sendPortMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitSendPort(sendPortMock.Object), Times.Once);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void DoesNotForwardApplyEnvironmentOverridesToReceivePipeline()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receivePipelineMock = new Mock<ReceivePipeline<XMLReceive>> { CallBase = true };
			var environmentSensitiveReceivePipelineMock = receivePipelineMock.As<ISupportEnvironmentOverride>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.ReceivePipeline = receivePipelineMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveReceivePipelineMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Never);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void DoesNotForwardApplyEnvironmentOverridesToSendPipeline()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sendPipelineMock = new Mock<SendPipeline<XMLTransmit>> { CallBase = true };
			var environmentSensitiveSendPipelineMock = sendPipelineMock.As<ISupportEnvironmentOverride>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.SendPipeline = sendPipelineMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveSendPipelineMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Never);
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			sendPortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(string.Empty);

			sendPortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ForwardsApplyEnvironmentOverridesToFilter()
		{
			var filterMock = new Mock<Filter>((Expression<Func<bool>>) (() => false));
			var environmentSensitiveFilterMock = filterMock.As<ISupportEnvironmentOverride>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.Filter = filterMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveFilterMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Once);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ForwardsApplyEnvironmentOverridesToTransports()
		{
			var retryPolicyMock = new Mock<RetryPolicy>();
			var environmentSensitiveRetryPolicyMock = retryPolicyMock.As<ISupportEnvironmentOverride>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.Transport.RetryPolicy = retryPolicyMock.Object;
			sendPortMock.Object.BackupTransport.Value.RetryPolicy = retryPolicyMock.Object;

			((ISupportEnvironmentOverride) sendPortMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			// indirectly verifies that SendPortBase forwards ApplyEnvironmentOverrides() call to Transport and BackupTransport, which forward it to their RetryPolicy and ServiceWindow
			environmentSensitiveRetryPolicyMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Exactly(2));
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void NameIsMandatory()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			sendPortMock.Object.Description = "Force Moq to call ctor.";

			Invoking(() => ((ISupportValidation) sendPortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Send Port's Name is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SendPipelineIsMandatory()
		{
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			sendPortMock.Object.Name = "Send Port Name";

			Invoking(() => ((ISupportValidation) sendPortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("[Send Port Name] Send Port's Send Pipeline is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportINamingConvention()
		{
			const string name = "Send Port Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeSendPortName(It.IsAny<ISendPort<object>>())).Returns(name);

			var sendPortMock = new Mock<SendPortBase<object>> { CallBase = true };
			sendPortMock.Object.Name = conventionMock.Object;

			sendPortMock.Object.ResolveName().Should().Be(name);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportStringNamingConvention()
		{
			const string name = "Send Port Name";
			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };

			sendPortMock.Object.Name = name;

			sendPortMock.Object.ResolveName().Should().Be(name);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void TransportIsValidated()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var adapterMock = new Mock<IOutboundAdapter>();
			var validatingAdapterMock = adapterMock.As<ISupportValidation>();

			var backupAdapterMock = new Mock<IOutboundAdapter>();
			var validatingBackupAdapterMock = backupAdapterMock.As<ISupportValidation>();

			var sendPortMock = new Mock<SendPortBase<string>> { CallBase = true };
			sendPortMock.Object.Name = "Send Port Name";
			sendPortMock.Object.SendPipeline = new SendPipeline<XMLTransmit>();
			sendPortMock.Object.Transport.Host = "Host";
			sendPortMock.Object.Transport.Adapter = adapterMock.Object;
			sendPortMock.Object.BackupTransport.Value.Host = "Host";
			sendPortMock.Object.BackupTransport.Value.Adapter = backupAdapterMock.Object;

			((ISupportValidation) sendPortMock.Object).Validate();

			// indirectly verifies that SendPortBase forwards Validate() call to Transport, which forwards it to its adapter
			validatingAdapterMock.Verify(m => m.Validate(), Times.Once);
			// indirectly verifies that SendPortBase forwards Validate() call to BackupTransport, which forwards it to its adapter
			validatingBackupAdapterMock.Verify(m => m.Validate(), Times.Once);
		}
	}
}
