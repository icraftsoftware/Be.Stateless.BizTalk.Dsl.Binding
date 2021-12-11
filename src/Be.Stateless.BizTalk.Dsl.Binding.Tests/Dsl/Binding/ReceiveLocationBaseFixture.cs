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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
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
	public class ReceiveLocationBaseFixture
	{
		[Fact]
		public void AcceptsVisitor()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) receiveLocationMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitReceiveLocation(receiveLocationMock.Object), Times.Once);
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			receiveLocationMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides(string.Empty);

			receiveLocationMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void ForwardsApplyEnvironmentOverridesToReceivePipeline()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receivePipelineMock = new Mock<ReceivePipeline<XMLReceive>> { CallBase = true };

			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.ReceivePipeline = receivePipelineMock.Object;

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			receivePipelineMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void ForwardsApplyEnvironmentOverridesToSendPipeline()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sendPipelineMock = new Mock<SendPipeline<XMLTransmit>> { CallBase = true };

			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.SendPipeline = sendPipelineMock.Object;

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			sendPipelineMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ForwardsApplyEnvironmentOverridesToTransport()
		{
			var adapterMock = new Mock<IInboundAdapter>();
			var environmentSensitiveAdapterMock = adapterMock.As<ISupportEnvironmentOverride>();

			var receiveLocationMock = new Mock<ReceiveLocationBase<object>> { CallBase = true };
			receiveLocationMock.Object.Name = "Receive Location Name";
			receiveLocationMock.Object.Transport.Host = "Host";
			receiveLocationMock.Object.Transport.Adapter = adapterMock.Object;

			((ISupportEnvironmentOverride) receiveLocationMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			// indirectly verifies that ReceiveLocationBase forwards ApplyEnvironmentOverrides() call to Transport, which forwards it to its adapter
			environmentSensitiveAdapterMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Once);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void NameIsMandatory()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.Description = "Force Moq to call ctor.";

			Invoking(() => ((ISupportValidation) receiveLocationMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Receive Location's Name is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ReceivePipelineIsMandatory()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };
			receiveLocationMock.Object.Name = "Receive Location Name";

			Invoking(() => ((ISupportValidation) receiveLocationMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("[Receive Location Name] Receive Location's Receive Pipeline is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportINamingConvention()
		{
			const string name = "Receive Location Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeReceiveLocationName(It.IsAny<IReceiveLocation<object>>())).Returns(name);

			var receiveLocationMock = new Mock<ReceiveLocationBase<object>> { CallBase = true };
			receiveLocationMock.Object.Name = conventionMock.Object;

			((ISupportNamingConvention) receiveLocationMock.Object).Name.Should().Be(name);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportStringNamingConvention()
		{
			const string name = "Receive Location Name";
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = true };

			receiveLocationMock.Object.Name = name;

			((ISupportNamingConvention) receiveLocationMock.Object).Name.Should().Be(name);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void TransportIsValidated()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var adapterMock = new Mock<IInboundAdapter>();
			var validatingAdapterMock = adapterMock.As<ISupportValidation>();

			var receiveLocationMock = new Mock<ReceiveLocationBase<object>> { CallBase = true };
			receiveLocationMock.Object.Name = "Receive Location Name";
			receiveLocationMock.Object.ReceivePipeline = new ReceivePipeline<XMLReceive>();
			receiveLocationMock.Object.Transport.Host = "Host";
			receiveLocationMock.Object.Transport.Adapter = adapterMock.Object;

			((ISupportValidation) receiveLocationMock.Object).Validate();

			// indirectly verifies that ReceiveLocationBase forwards Validate() call to Transport, which forwards it to its adapter
			validatingAdapterMock.Verify(m => m.Validate(), Times.Once);
		}
	}
}
