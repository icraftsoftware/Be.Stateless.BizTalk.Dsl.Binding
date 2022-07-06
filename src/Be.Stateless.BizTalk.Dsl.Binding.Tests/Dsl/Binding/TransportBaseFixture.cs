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
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class TransportBaseFixture
	{
		[Fact]
		public void ForwardsApplyEnvironmentOverridesToAdapter()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var environmentSensitiveTransportMock = transportMock.As<ISupportEnvironmentOverride>();
			var adapterMock = new Mock<IAdapter>();
			var environmentSensitiveAdapterMock = adapterMock.As<ISupportEnvironmentOverride>();

			transportMock.Object.Adapter = adapterMock.Object;
			environmentSensitiveTransportMock.Object.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveAdapterMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Never);
		}

		[Fact]
		public void ForwardsValidateToAdapter()
		{
			var adapterMock = new Mock<IAdapter>();
			var validatingAdapterMock = adapterMock.As<ISupportValidation>();

			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var validatingTransportMock = transportMock.As<ISupportValidation>();
			transportMock.Object.Host = "Host";
			transportMock.Object.Adapter = adapterMock.Object;

			validatingTransportMock.Object.Validate();

			validatingAdapterMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void IgnoresEnvironmentOverrides()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var environmentSensitiveTransportMock = transportMock.As<ISupportEnvironmentOverride>();

			environmentSensitiveTransportMock.Object.ApplyEnvironmentOverrides(string.Empty);

			transportMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Fact]
		public void SupportsEnvironmentOverrides()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			var environmentSensitiveTransportMock = transportMock.As<ISupportEnvironmentOverride>();

			environmentSensitiveTransportMock.Object.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			transportMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void TransportAdapterIsMandatory()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			transportMock.Object.Host = "Host";

			Invoking(() => ((ISupportValidation) transportMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Transport's Adapter is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void TransportHostIsMandatory()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			transportMock.Object.Adapter = new FileAdapter.Outbound(_ => { });

			Invoking(() => ((ISupportValidation) transportMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Transport's Host is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void TransportUnknownAdapterIsInvalid()
		{
			var transportMock = new Mock<TransportBase<IAdapter>> { CallBase = true };
			transportMock.Object.Host = "Host";

			var adapterMock = new Mock<TransportBase<IAdapter>.UnknownAdapter>();
			transportMock.Object.Adapter = adapterMock.Object;

			Invoking(() => ((ISupportValidation) transportMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Transport's Adapter is not defined.");
		}
	}
}
