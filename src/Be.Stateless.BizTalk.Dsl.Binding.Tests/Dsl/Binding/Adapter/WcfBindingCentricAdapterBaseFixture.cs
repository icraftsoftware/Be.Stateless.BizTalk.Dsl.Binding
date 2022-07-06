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
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Install;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfBindingCentricAdapterBaseFixture
	{
		[Fact]
		public void ForwardsApplyEnvironmentOverridesToEndpointBehaviors()
		{
			var behaviorMock = new Mock<BehaviorExtensionElement>();
			var environmentSensitiveBehaviorMock = behaviorMock.As<ISupportEnvironmentOverride>();

			var adapterMock = new Mock<WcfBindingCentricAdapterBase<Uri, Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.CustomBindingElement, CustomRLConfig>>(new ProtocolType()) {
				CallBase = true,
				Object = { EndpointBehaviors = new[] { behaviorMock.Object } }
			};

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			environmentSensitiveBehaviorMock.Verify(m => m.ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE), Times.Once);
		}
	}
}
