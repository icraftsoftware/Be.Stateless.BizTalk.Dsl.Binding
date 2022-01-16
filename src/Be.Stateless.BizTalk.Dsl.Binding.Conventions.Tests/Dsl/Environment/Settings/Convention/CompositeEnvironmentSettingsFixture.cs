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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention
{
	public static class CompositeEnvironmentSettingsFixture
	{
		#region Nested Type: AccessingAndConstructingSingletonViaCompositeEnvironmentSettingsBaseClassWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaCompositeEnvironmentSettingsBaseClassWithEnvironmentSettingOverridesType
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void SingletonCanBeConstructedAndCanBeAccessed()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					// construction
					Invoking(() => CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings).Should().NotThrow();
					FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
					// access
					CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings.Should().BeSameAs(FooApp.Settings);
					CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings.ApplicationName.Should().Be(nameof(FooApp));
					CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings.ReceivingHost.Should().Be("ReceivingHostOverride");
				}
			}

			private class FooAppOverrides : IProvideHostNames
			{
				#region IProvideHostNames Members

				public string IsolatedHost => nameof(IsolatedHost) + "Override";

				public string ProcessingHost => nameof(ProcessingHost) + "Override";

				public string ReceivingHost => nameof(ReceivingHost) + "Override";

				public string TransmittingHost => nameof(TransmittingHost) + "Override";

				#endregion
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideHostNames, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IProvideHostNames Members

				public string IsolatedHost => GetOverriddenOrDefaultValue(nameof(IsolatedHost));

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion
			}
		}

		#endregion

		#region Nested Type: AccessingAndConstructingSingletonViaCompositeEnvironmentSettingsBaseClassWithoutEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaCompositeEnvironmentSettingsBaseClassWithoutEnvironmentSettingOverridesType
		{
			[Fact]
			public void SingletonCanBeConstructedAndCanBeAccessed()
			{
				// construction
				Invoking(() => CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings).Should().NotThrow();
				FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
				// access
				CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings.Should().BeSameAs(FooApp.Settings);
				CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings.ApplicationName.Should().Be(nameof(FooApp));
				CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>.Settings.ReceivingHost.Should().Be("ReceivingHost");
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideHostNames, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IProvideHostNames Members

				public string IsolatedHost => GetOverriddenOrDefaultValue(nameof(IsolatedHost));

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion
			}
		}

		#endregion

		#region Nested Type: AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClassWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClassWithEnvironmentSettingOverridesType
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void SingletonCannotBeConstructedAndCannotBeAccessed()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					// construction
					Invoking(() => EnvironmentSettings<FooApp>.Settings)
						.Should().Throw<InvalidOperationException>()
						.WithMessage(
							$"'{nameof(FooAppOverrides)}' does not derive from '{nameof(FooApp)}' and cannot be used as its {nameof(DeploymentContext.EnvironmentSettingOverridesType)}.");

					// Lazy<T> caches the previous result and makes FooApp.Settings unusable
					Invoking(() => FooApp.Settings)
						.Should().Throw<InvalidOperationException>()
						.WithMessage(
							$"'{nameof(FooAppOverrides)}' does not derive from '{nameof(FooApp)}' and cannot be used as its {nameof(DeploymentContext.EnvironmentSettingOverridesType)}.");
				}
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion
			}

			private class FooAppOverrides : IProvideEnvironmentSettings { }
		}

		#endregion

		#region Nested Type: AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClassWithoutEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClassWithoutEnvironmentSettingOverridesType
		{
			[Fact]
			public void SingletonCanBeConstructedAndCanBeAccessed()
			{
				// construction
				Invoking(() => EnvironmentSettings<FooApp>.Settings).Should().NotThrow();
				FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
				// access
				EnvironmentSettings<FooApp>.Settings.Should().BeSameAs(FooApp.Settings);
				EnvironmentSettings<FooApp>.Settings.ApplicationName.Should().Be(nameof(FooApp));
				EnvironmentSettings<FooApp>.Settings.ReceivingHost.Should().Be("ReceivingHost");
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideHostNames, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IProvideHostNames Members

				public string IsolatedHost => GetOverriddenOrDefaultValue(nameof(IsolatedHost));

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion
			}
		}

		#endregion

		#region Nested Type: InstantiatingCompositeEnvironmentSettingsExplicitlyWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class InstantiatingCompositeEnvironmentSettingsExplicitlyWithEnvironmentSettingOverridesType
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void CompositeEnvironmentSettingsThrowWhenInstantiatedExplicitly()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					Invoking(() => new FooApp()).Should().Throw<InvalidOperationException>();
					Invoking(() => new FooAppOverrides()).Should().NotThrow();
				}
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideHostNames, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IProvideHostNames Members

				public string IsolatedHost => GetOverriddenOrDefaultValue(nameof(IsolatedHost));

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion
			}

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class FooAppOverrides : IProvideHostNames
			{
				#region IProvideHostNames Members

				public string IsolatedHost => nameof(IsolatedHost) + "Override";

				public string ProcessingHost => nameof(ProcessingHost) + "Override";

				public string ReceivingHost => nameof(ReceivingHost) + "Override";

				public string TransmittingHost => nameof(TransmittingHost) + "Override";

				#endregion
			}
		}

		#endregion

		#region Nested Type: InstantiatingCompositeEnvironmentSettingsExplicitlyWithoutEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class InstantiatingCompositeEnvironmentSettingsExplicitlyWithoutEnvironmentSettingOverridesType
		{
			[Fact]
			public void CompositeEnvironmentSettingsThrowWhenInstantiatedExplicitly()
			{
				Invoking(() => new FooApp()).Should().Throw<InvalidOperationException>();
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithoutEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithoutEnvironmentSettingOverrides
		{
			[Fact]
			public void SettingsReturnDefaultValues()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
				{
					FooApp.Settings.Should().BeAssignableTo<IEnvironmentSettings>();
					FooApp.Settings.Should().BeAssignableTo<IProvideEnvironmentSettings>();
					FooApp.Settings.Should().BeOfType<FooApp>();
					FooApp.Settings.ReceivingHost.Should().Be("ReceivingHost");
				}
			}

			[Fact]
			public void SsoSettingsReturnDefaultValues()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
				{
					FooApp.Settings.SsoSettings
						.Should().BeEquivalentTo(
							new Dictionary<string, string> {
								{ nameof(FooApp.CheckInFolder), @"c:\claim\store\in\default" },
								{ nameof(FooApp.CheckOutFolder), @"c:\claim\store\out\default" },
								{ nameof(FooApp.SsoProperty), "SsoProperty" },
								{ nameof(FooApp.TargetEnvironment), DeploymentContext.TargetEnvironment }
							});
				}
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideHostNames, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IProvideHostNames Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("myIsolatedHost");

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion

				[SsoSetting]
				public string CheckInFolder => GetOverriddenOrDefaultValue(@"c:\claim\store\in\default");

				[SsoSetting]
				public string CheckOutFolder => GetOverriddenOrDefaultValue(@"c:\claim\store\out\default");

				[SsoSetting]
				public string SsoProperty => GetOverriddenOrDefaultValue(nameof(SsoProperty));

				[SsoSetting]
				public string TargetEnvironment => DeploymentContext.TargetEnvironment;
			}
		}

		#endregion

		#region Nested Type: WithRelatedEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithRelatedEnvironmentSettingOverrides
		{
			[Fact]
			public void SettingsReturnOverriddenValues()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE", environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					FooApp.Settings.Should().BeAssignableTo<IEnvironmentSettings>();
					FooApp.Settings.Should().BeAssignableTo<IProvideEnvironmentSettings>();
					FooApp.Settings.Should().BeOfType<FooApp>();
					FooApp.Settings.Should().NotBeOfType<FooAppOverrides>();
					FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
					FooApp.Settings.ReceivingHost.Should().Be("ReceivingHostOverride");
					// overriden even though not part of IProvideEnvironmentSettings interface
					FooApp.Settings.CheckInFolder.Should().Be(@"c:\claim\store\in\overridden");
					FooApp.Settings.SsoProperty.Should().Be("SsoPropertyOverride");
				}
			}

			[Fact]
			public void SsoSettingsReturnOverriddenValues()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE", environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					FooApp.Settings.SsoSettings
						.Should().BeEquivalentTo(
							new Dictionary<string, string> {
								{ nameof(FooApp.CheckInFolder), @"c:\claim\store\in\overridden" },
								{ nameof(FooApp.CheckOutFolder), @"c:\claim\store\out\overridden" },
								{ nameof(FooApp.SsoProperty), "SsoPropertyOverride" },
								{ nameof(FooApp.TargetEnvironment), DeploymentContext.TargetEnvironment }
							});
				}
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideHostNames, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IProvideHostNames Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("myIsolatedHost");

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion

				[SsoSetting]
				public string CheckInFolder => GetOverriddenOrDefaultValue(@"c:\claim\store\in\default");

				[SsoSetting]
				public string CheckOutFolder => GetOverriddenOrDefaultValue(@"c:\claim\store\out\default");

				[SsoSetting]
				public string SsoProperty => GetOverriddenOrDefaultValue(nameof(SsoProperty));

				[SsoSetting]
				public string TargetEnvironment => DeploymentContext.TargetEnvironment;
			}

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class FooAppOverrides : IProvideHostNames
			{
				#region IProvideHostNames Members

				public string IsolatedHost => nameof(IsolatedHost) + "Override";

				public string ProcessingHost => nameof(ProcessingHost) + "Override";

				public string ReceivingHost => nameof(ReceivingHost) + "Override";

				public string TransmittingHost => nameof(TransmittingHost) + "Override";

				#endregion

				/// <summary>
				///  This override will be ignored because FooApp does not allow it to be overridden.
				/// </summary>
				public string ApplicationName => nameof(FooAppOverrides);

				public string CheckInFolder => @"c:\claim\store\in\overridden";

				public string CheckOutFolder => @"c:\claim\store\out\overridden";

				/// <summary>
				/// This property will not be part of the properties being deployed to SSO.
				/// </summary>
				[SsoSetting]
				public string IgnoredSsoProperty => nameof(IgnoredSsoProperty);

				public string SsoProperty => nameof(SsoProperty) + "Override";
			}
		}

		#endregion

		#region Nested Type: WithUnrelatedEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithUnrelatedEnvironmentSettingOverrides
		{
			[Fact]
			public void SettingsThrows()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE", environmentSettingOverridesType: typeof(ForeignAppOverrides)))
				{
					Invoking(() => FooApp.Settings.ApplicationName).Should().Throw<InvalidCastException>();
				}
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IProvideEnvironmentSettings>, IProvideEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion
			}

			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			private interface IForeignPlatformEnvironmentSettings
			{
				string ForeignHost { get; }
			}

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class ForeignAppOverrides : IForeignPlatformEnvironmentSettings
			{
				#region IForeignPlatformEnvironmentSettings Members

				public string ForeignHost => nameof(ForeignHost);

				#endregion
			}
		}

		#endregion
	}
}
