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
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Moq.Protected;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class AdapterBaseFixture
	{
		[Fact]
		public void DelegatesValidateCall()
		{
			var adapterMock = new Mock<AdapterBase>(new ProtocolType()) { CallBase = true };

			((ISupportValidation) adapterMock.Object).Validate();

			adapterMock.Protected().Verify(nameof(ISupportValidation.Validate), Times.Once());
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var adapterMock = new Mock<AdapterBase>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			adapterMock.Protected().Verify(nameof(ISupportEnvironmentOverride.ApplyEnvironmentOverrides), Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var adapterMock = new Mock<AdapterBase>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides(string.Empty);

			adapterMock.Protected().Verify(nameof(ISupportEnvironmentOverride.ApplyEnvironmentOverrides), Times.Never(), ItExpr.IsAny<string>());
		}
	}
}
