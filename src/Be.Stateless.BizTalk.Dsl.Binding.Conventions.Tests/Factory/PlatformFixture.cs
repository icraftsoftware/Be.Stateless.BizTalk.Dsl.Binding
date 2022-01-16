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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention;
using Be.Stateless.BizTalk.Orchestrations.Direct;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Factory
{
	public static class PlatformFixture
	{
		#region Nested Type: WithoutOverriddenHostResolutionPolicyAndWithoutEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class WithoutOverriddenHostResolutionPolicyAndWithoutEnvironmentSettingOverridesType
		{
			#region Setup/Teardown

			public WithoutOverriddenHostResolutionPolicyAndWithoutEnvironmentSettingOverridesType()
			{
				Platform.LazySingletonInstance = new(Platform.InvokeSingletonFactory);
			}

			#endregion

			[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
			[Fact]
			public void DoesNotOverridePlatformHosts()
			{
				var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
				orchestrationBindingMock.Object.Host = Platform.Settings.HostResolutionPolicy;

				orchestrationBindingMock.Object.ResolveHost().Should().Be("BizTalkServerApplication");
				Invoking(() => ((IProvideHostNames) Platform.Settings).ProcessingHost)
					.Should().Throw<NotSupportedException>()
					.WithMessage("Platform provides a HostResolutionPolicy; its IProvideHostNames trait is only an overriding point and must not be called explicitly.");
			}
		}

		#endregion

		#region Nested Type: WithoutOverriddenHostResolutionPolicyButWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class WithoutOverriddenHostResolutionPolicyButWithEnvironmentSettingOverridesType
		{
			#region Setup/Teardown

			public WithoutOverriddenHostResolutionPolicyButWithEnvironmentSettingOverridesType()
			{
				Platform.LazySingletonInstance = new(Platform.InvokeSingletonFactory);
			}

			#endregion

			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
			[Fact]
			public void DoesOverridePlatformHosts()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(PlatformOverrides)))
				{
					var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
					orchestrationBindingMock.Object.Host = Platform.Settings.HostResolutionPolicy;

					orchestrationBindingMock.Object.ResolveHost().Should().Be("ProcessingHostOverride");
					Invoking(() => ((IProvideHostNames) Platform.Settings).ProcessingHost).Should().Throw<NotSupportedException>();
				}
			}

			private class PlatformOverrides : IProvideHostNames
			{
				#region IProvideHostNames Members

				public string IsolatedHost => $"{nameof(IsolatedHost)}Override";

				public string ProcessingHost => $"{nameof(ProcessingHost)}Override";

				public string ReceivingHost => $"{nameof(ReceivingHost)}Override";

				public string TransmittingHost => $"{nameof(TransmittingHost)}Override";

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithOverriddenHostResolutionPolicyAndOverriddenHostNames

		[Collection("DeploymentContext")]
		public class WithOverriddenHostResolutionPolicyAndOverriddenHostNames
		{
			#region Setup/Teardown

			public WithOverriddenHostResolutionPolicyAndOverriddenHostNames()
			{
				Platform.LazySingletonInstance = new(Platform.InvokeSingletonFactory);
			}

			#endregion

			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
			[Fact]
			public void ThrowsAmbiguous()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(PlatformOverrides)))
				{
					var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
					orchestrationBindingMock.Object.Host = Platform.Settings.HostResolutionPolicy;

					Invoking(() => orchestrationBindingMock.Object.ResolveHost())
						.Should().Throw<InvalidOperationException>()
						.WithMessage(
							"EnvironmentSettingOverrides 'PlatformOverrides' should only implement either 'IProvideHostNames' or 'IProvideHostResolutionPolicy'; but it implements both.");
				}
			}

			private class PlatformOverrides : IProvideHostResolutionPolicy, IProvideHostNames
			{
				#region IProvideHostNames Members

				public string IsolatedHost => throw new NotSupportedException();

				public string ProcessingHost => throw new NotSupportedException();

				public string ReceivingHost => throw new NotSupportedException();

				public string TransmittingHost => throw new NotSupportedException();

				#endregion

				#region IProvideHostResolutionPolicy Members

				public HostResolutionPolicy HostResolutionPolicy => Convention.HostResolutionPolicy.Default;

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithOverriddenHostResolutionPolicyDerivingFromDslBindingConventionHostResolutionPolicy

		[Collection("DeploymentContext")]
		public class WithOverriddenHostResolutionPolicyDerivingFromDslBindingConventionHostResolutionPolicy
		{
			#region Setup/Teardown

			public WithOverriddenHostResolutionPolicyDerivingFromDslBindingConventionHostResolutionPolicy()
			{
				Platform.LazySingletonInstance = new(Platform.InvokeSingletonFactory);
			}

			#endregion

			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
			[Fact]
			public void CanOverrideHostResolutionPolicy()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(PlatformOverrides)))
				{
					Platform.Settings.HostResolutionPolicy.Should().BeOfType<MyPolicy>();

					var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
					orchestrationBindingMock.Object.Host = Platform.Settings.HostResolutionPolicy;

					orchestrationBindingMock.Object.ResolveHost().Should().Be("OrchestrationHost1");
					Invoking(() => ((IProvideHostNames) Platform.Settings).ProcessingHost).Should().Throw<NotSupportedException>();
				}
			}

			private class PlatformOverrides : IProvideHostResolutionPolicy
			{
				#region IProvideHostResolutionPolicy Members

				[SuppressMessage("ReSharper", "RedundantNameQualifier")]
				public Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy HostResolutionPolicy { get; } = new MyPolicy();

				#endregion
			}

			[SuppressMessage("ReSharper", "RedundantNameQualifier")]
			private class MyPolicy : Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy
			{
				#region Base Class Member Overrides

				public override string ResolveHost(IOrchestrationBinding orchestration)
				{
					return "OrchestrationHost1";
				}

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithOverriddenHostResolutionPolicyDerivingFromFactoryConventionHostResolutionPolicy

		[Collection("DeploymentContext")]
		public class WithOverriddenHostResolutionPolicyDerivingFromFactoryConventionHostResolutionPolicy
		{
			#region Setup/Teardown

			public WithOverriddenHostResolutionPolicyDerivingFromFactoryConventionHostResolutionPolicy()
			{
				Platform.LazySingletonInstance = new(Platform.InvokeSingletonFactory);
			}

			#endregion

			[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void CanOverrideHostResolutionPolicy()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(PlatformOverrides)))
				{
					Platform.Settings.HostResolutionPolicy.Should().BeOfType<MyPolicy>();

					var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
					orchestrationBindingMock.Object.Host = Platform.Settings.HostResolutionPolicy;

					orchestrationBindingMock.Object.ResolveHost().Should().Be("OrchestrationHost2");
					Invoking(() => ((IProvideHostNames) Platform.Settings).ProcessingHost).Should().Throw<NotSupportedException>();
				}
			}

			private class PlatformOverrides : IProvideHostResolutionPolicy
			{
				#region IProvideHostResolutionPolicy Members

				[SuppressMessage("ReSharper", "RedundantNameQualifier")]
				public HostResolutionPolicy HostResolutionPolicy { get; } = new MyPolicy();

				#endregion
			}

			[SuppressMessage("ReSharper", "RedundantNameQualifier")]
			private class MyPolicy : Be.Stateless.BizTalk.Factory.Convention.HostResolutionPolicy
			{
				#region Base Class Member Overrides

				public override string ResolveHost(IOrchestrationBinding orchestration)
				{
					return "OrchestrationHost2";
				}

				#endregion
			}
		}

		#endregion
	}
}
