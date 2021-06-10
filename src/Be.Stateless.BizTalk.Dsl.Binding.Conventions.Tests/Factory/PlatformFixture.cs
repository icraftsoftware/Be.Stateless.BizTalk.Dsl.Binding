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

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention;
using Be.Stateless.BizTalk.Orchestrations.Direct;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Factory
{
	// TODO When NOT Supporting IPlatformEnvironmentSettings

	public static class PlatformFixture
	{
		#region Nested Type: WithoutOverriddenHostResolutionPolicyButWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class WithoutOverriddenHostResolutionPolicyButWithEnvironmentSettingOverridesType
		{
			#region Setup/Teardown

			public WithoutOverriddenHostResolutionPolicyButWithEnvironmentSettingOverridesType()
			{
				Platform.LazySingletonInstance = new Lazy<Platform>(Platform.InvokeSingletonFactory);
			}

			#endregion

			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void DoesOverridePlatformHosts()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(PlatformOverrides)))
				{
					var settingsHostResolutionPolicy = Platform.Settings.HostResolutionPolicy;

					var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
					orchestrationBindingMock.Object.Host = settingsHostResolutionPolicy;

					((ISupportHostNameResolution) orchestrationBindingMock.Object).ResolveHostName().Should().Be("ProcessingHostOverride");
					Platform.Settings.ProcessingHost.Should().Be("ProcessingHostOverride");
				}
			}

			private class PlatformOverrides : IPlatformEnvironmentSettings
			{
				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => $"{nameof(IsolatedHost)}Override";

				public string ManagementDatabaseInstance => $"{nameof(ManagementDatabaseInstance)}Override";

				public string ManagementDatabaseServer => $"{nameof(ManagementDatabaseServer)}Override";

				public string MonitoringDatabaseInstance => $"{nameof(MonitoringDatabaseInstance)}Override";

				public string MonitoringDatabaseServer => $"{nameof(MonitoringDatabaseServer)}Override";

				public string ProcessingDatabaseInstance => $"{nameof(ProcessingDatabaseInstance)}Override";

				public string ProcessingDatabaseServer => $"{nameof(ProcessingDatabaseServer)}Override";

				public string ProcessingHost => $"{nameof(ProcessingHost)}Override";

				public string ReceivingHost => $"{nameof(ReceivingHost)}Override";

				public string TransmittingHost => $"{nameof(TransmittingHost)}Override";

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithoutOverriddenHostResolutionPolicyButWithoutEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class WithoutOverriddenHostResolutionPolicyButWithoutEnvironmentSettingOverridesType
		{
			#region Setup/Teardown

			public WithoutOverriddenHostResolutionPolicyButWithoutEnvironmentSettingOverridesType()
			{
				Platform.LazySingletonInstance = new Lazy<Platform>(Platform.InvokeSingletonFactory);
			}

			#endregion

			[Fact]
			public void DoesNotOverridePlatformHosts()
			{
				var settingsHostResolutionPolicy = Platform.Settings.HostResolutionPolicy;

				var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
				orchestrationBindingMock.Object.Host = settingsHostResolutionPolicy;

				((ISupportHostNameResolution) orchestrationBindingMock.Object).ResolveHostName().Should().Be("BizTalkServerApplication");
				Platform.Settings.ProcessingHost.Should().Be("BizTalkServerApplication");
			}
		}

		#endregion

		#region Nested Type: WithOverriddenHostResolutionPolicyDerivingFromDslBindingConventionHostResolutionPolicy

		[Collection("DeploymentContext")]
		public class WithOverriddenHostResolutionPolicyDerivingFromDslBindingConventionHostResolutionPolicy
		{
			[Fact]
			public void CanOverrideHostResolutionPolicy()
			{
				var settingsHostResolutionPolicy = new MyPolicy();

				var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
				orchestrationBindingMock.Object.Host = settingsHostResolutionPolicy;

				((ISupportHostNameResolution) orchestrationBindingMock.Object).ResolveHostName().Should().Be("OrchestrationHost");
				Platform.Settings.ProcessingHost.Should().Be("BizTalkServerApplication");
			}

			[SuppressMessage("ReSharper", "RedundantNameQualifier")]
			private class MyPolicy : Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy
			{
				#region Base Class Member Overrides

				protected override string ResolveHostName(IOrchestrationBinding orchestration)
				{
					return "OrchestrationHost";
				}

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithOverriddenHostResolutionPolicyDerivingFromFactoryConventionHostResolutionPolicy

		[Collection("DeploymentContext")]
		public class WithOverriddenHostResolutionPolicyDerivingFromFactoryConventionHostResolutionPolicy
		{
			[Fact]
			public void CanOverrideHostResolutionPolicy()
			{
				var settingsHostResolutionPolicy = new MyPolicy();

				var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
				orchestrationBindingMock.Object.Host = settingsHostResolutionPolicy;

				((ISupportHostNameResolution) orchestrationBindingMock.Object).ResolveHostName().Should().Be("MyPolicyOrchestrationHost");
				Platform.Settings.ProcessingHost.Should().Be("BizTalkServerApplication");
			}

			[SuppressMessage("ReSharper", "RedundantNameQualifier")]
			private class MyPolicy : Be.Stateless.BizTalk.Factory.Convention.HostResolutionPolicy
			{
				#region Base Class Member Overrides

				protected override string ResolveHostName(IOrchestrationBinding orchestration)
				{
					return "MyPolicyOrchestrationHost";
				}

				#endregion
			}
		}

		#endregion
	}
}
