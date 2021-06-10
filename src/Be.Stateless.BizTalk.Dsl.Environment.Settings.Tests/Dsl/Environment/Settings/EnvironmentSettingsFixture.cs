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

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	public static class EnvironmentSettingsFixture
	{
		#region Nested Type: AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClassWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClassWithEnvironmentSettingOverridesType
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void SingletonCanBeConstructedAndCanBeAccessed()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					// construction
					Invoking(() => EnvironmentSettings<FooApp>.Settings).Should().NotThrow();
					FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
					// access
					EnvironmentSettings<FooApp>.Settings.Should().BeSameAs(FooApp.Settings);
					EnvironmentSettings<FooApp>.Settings.ApplicationName.Should().Be(nameof(FooApp));
					EnvironmentSettings<FooApp>.Settings.ReceivingHost.Should().Be("Overridden Receive Host");
				}
			}

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				public virtual string ReceivingHost => "Default Receive Host";
			}

			private class FooAppOverrides : FooApp
			{
				#region Base Class Member Overrides

				public override string ReceivingHost => "Overridden Receive Host";

				#endregion
			}
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
				EnvironmentSettings<FooApp>.Settings.ReceivingHost.Should().Be("Default Receive Host");
			}

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				public string ReceivingHost => "Default Receive Host";
			}
		}

		#endregion

		#region Nested Type: InstantiatingEnvironmentSettingsExplicitlyWithEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class InstantiatingEnvironmentSettingsExplicitlyWithEnvironmentSettingOverridesType
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void EnvironmentSettingsThrowWhenInstantiatedExplicitly()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					Invoking(() => new BarApp()).Should().Throw<InvalidOperationException>();
					Invoking(() => new BarAppOverrides()).Should().Throw<InvalidOperationException>();
					Invoking(() => new FooApp()).Should().Throw<InvalidOperationException>();
					Invoking(() => new FooAppOverrides()).Should().Throw<InvalidOperationException>();
				}
			}

			private class BarApp : EnvironmentSettings<BarApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(BarApp);

				#endregion
			}

			private class BarAppOverrides : BarApp { }

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion
			}

			private class FooAppOverrides : FooApp { }
		}

		#endregion

		#region Nested Type: InstantiatingEnvironmentSettingsExplicitlyWithoutEnvironmentSettingOverridesType

		[Collection("DeploymentContext")]
		public class InstantiatingEnvironmentSettingsExplicitlyWithoutEnvironmentSettingOverridesType
		{
			[Fact]
			public void EnvironmentSettingsThrowWhenInstantiatedExplicitly()
			{
				Invoking(() => new BarApp()).Should().Throw<InvalidOperationException>();
				Invoking(() => new BarAppOverrides()).Should().Throw<InvalidOperationException>();
				Invoking(() => new FooApp()).Should().Throw<InvalidOperationException>();
				Invoking(() => new FooAppOverrides()).Should().Throw<InvalidOperationException>();
			}

			private class BarApp : EnvironmentSettings<BarApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(BarApp);

				#endregion
			}

			private class BarAppOverrides : BarApp { }

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion
			}

			private class FooAppOverrides : FooApp { }
		}

		#endregion

		#region Nested Type: WithoutEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithoutEnvironmentSettingOverrides
		{
			[Fact]
			public void BaseStaticSettingsPropertyGivesAccessToAllEnvironmentSettingProperties()
			{
				BarApp.Settings.ApplicationName.Should().Be(nameof(BarApp));
				FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
				FooApp.Settings.ReceivingHost.Should().Be("Default Receive Host");
			}

			[Fact]
			public void BaseStaticSettingsPropertyIsSingleton()
			{
				BarApp.Settings.Should().BeSameAs(BarApp.Settings);
				FooAppOverrides.Settings.Should().BeSameAs(FooApp.Settings);
				FooApp.Settings.Should().BeSameAs(FooApp.Settings);
				BarApp.Settings.Should().NotBeSameAs(FooApp.Settings);
			}

			[Fact]
			public void EnvironmentSettingOverridesAreUselessWhenNotProvidedViaDeploymentContext()
			{
				FooApp.Settings.Should().BeOfType<FooApp>();
				FooApp.Settings.Should().NotBeOfType<FooAppOverrides>();
				FooAppOverrides.Settings.Should().BeSameAs(FooApp.Settings);
				FooApp.Settings.ReceivingHost.Should().Be("Default Receive Host");
				FooAppOverrides.Settings.ReceivingHost.Should().Be("Default Receive Host");
			}

			[Fact]
			public void SsoSettingsReturnDefaultValues()
			{
				FooApp.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\default" } });
				FooAppOverrides.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\default" } });
			}

			private class BarApp : EnvironmentSettings<BarApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(BarApp);

				#endregion
			}

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				[SsoSetting]
				[SuppressMessage("ReSharper", "UnusedMember.Global")]
				public virtual string CheckInFolder => @"c:\claim\store\in\default";

				public virtual string ReceivingHost => "Default Receive Host";
			}

			[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
			private class FooAppOverrides : FooApp
			{
				#region Base Class Member Overrides

				public override string CheckInFolder => @"c:\claim\store\in\overridden";

				/// <summary>
				/// This property will not be part of the properties being deployed to SSO.
				/// </summary>
				[SsoSetting]
				public override string ReceivingHost => "Overridden Receive Host";

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithRelatedEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithRelatedEnvironmentSettingOverrides
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void EnvironmentSettingOverridesCanBeItself()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(BarApp)))
				{
					Invoking(() => BarApp.Settings).Should().NotThrow();
					BarApp.Settings.ApplicationName.Should().Be(nameof(BarApp));
				}
			}

			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void SettingsReturnsDerivedAppSettingInstanceAndValuesAreOverridden()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					FooApp.Settings.Should().BeOfType<FooAppOverrides>();
					FooApp.Settings.Should().NotBeOfType<FooApp>();
					FooAppOverrides.Settings.Should().BeSameAs(FooApp.Settings);
					FooApp.Settings.ReceivingHost.Should().Be("Overridden Receive Host");
					FooAppOverrides.Settings.ReceivingHost.Should().Be("Overridden Receive Host");
				}
			}

			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void SsoSettingsReturnOverriddenValues()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					FooApp.Settings.SsoSettings
						.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\overridden" } });
					FooAppOverrides.Settings.SsoSettings
						.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\overridden" } });
				}
			}

			private class BarApp : EnvironmentSettings<BarApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(BarApp);

				#endregion
			}

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				[SuppressMessage("ReSharper", "UnusedMember.Global")]
				[SsoSetting]
				public virtual string CheckInFolder => @"c:\claim\store\in\default";

				public virtual string ReceivingHost => "Default Receive Host";
			}

			private class FooAppOverrides : FooApp
			{
				#region Base Class Member Overrides

				public override string CheckInFolder => @"c:\claim\store\in\overridden";

				/// <summary>
				/// This property will not be part of the properties being deployed to SSO.
				/// </summary>
				[SsoSetting]
				public override string ReceivingHost => "Overridden Receive Host";

				#endregion

				/// <summary>
				/// This property will not be part of the properties being deployed to SSO.
				/// </summary>
				[SuppressMessage("ReSharper", "UnusedMember.Local")]
				[SsoSetting]
				public string MySsoProperty => nameof(MySsoProperty);
			}
		}

		#endregion

		#region Nested Type: WithUnrelatedEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithUnrelatedEnvironmentSettingOverrides
		{
			[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
			[Fact]
			public void SettingThrows()
			{
				using (new DeploymentContextInjectionScope(environmentSettingOverridesType: typeof(BarApp)))
				{
					Invoking(() => FooApp.Settings)
						.Should().Throw<InvalidOperationException>()
						.WithMessage(
							$"'{nameof(BarApp)}' does not derive from '{nameof(FooApp)}' and cannot be used as its {nameof(DeploymentContext.EnvironmentSettingOverridesType)}.");
				}
			}

			private class BarApp : EnvironmentSettings<BarApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(BarApp);

				#endregion
			}

			private class FooApp : EnvironmentSettings<FooApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion
			}
		}

		#endregion
	}
}
