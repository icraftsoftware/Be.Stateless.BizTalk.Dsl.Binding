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
		#region Nested Type: AccessingAndConstructingSingletonViaCompositeEnvironmentSettingsBaseClass

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaCompositeEnvironmentSettingsBaseClass
		{
			[Fact]
			public void SingletonCanBeConstructedAndCanBeAccessed()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE", environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					// construction
					Invoking(() => CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>.Settings).Should().NotThrow();

					FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
					// access
					CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>.Settings.Should().BeSameAs(FooApp.Settings);
					CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>.Settings.ApplicationName.Should().Be(nameof(FooApp));
					CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>.Settings.ReceivingHost.Should().Be("ReceivingHostOverride");
				}
			}

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class FooAppOverrides : IPlatformEnvironmentSettings
			{
				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => nameof(IsolatedHost) + "Override";

				public string ManagementDatabaseInstance => "Override";

				public string ManagementDatabaseServer => "localhost" + "Override";

				public string MonitoringDatabaseInstance => "Override";

				public string MonitoringDatabaseServer => "localhost" + "Override";

				public string ProcessingDatabaseInstance => string.Empty;

				public string ProcessingDatabaseServer => "localhost" + "Override";

				public string ProcessingHost => nameof(ProcessingHost) + "Override";

				public string ReceivingHost => nameof(ReceivingHost) + "Override";

				public string TransmittingHost => nameof(TransmittingHost) + "Override";

				#endregion
			}

			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			private class FooApp : CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("myIsolatedHost");

				public string ManagementDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ManagementDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string MonitoringDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string MonitoringDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ProcessingDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion
			}
		}

		#endregion

		#region Nested Type: AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClass

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClass
		{
			[Fact]
			public void SingletonCannotBeConstructedButCanBeAccessed()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE", environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					// construction
					Invoking(() => EnvironmentSettings<FooApp>.Settings).Should().Throw<InvalidCastException>();

					FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
					// access
					EnvironmentSettings<FooApp>.Settings.Should().BeSameAs(FooApp.Settings);
					EnvironmentSettings<FooApp>.Settings.ApplicationName.Should().Be(nameof(FooApp));
					EnvironmentSettings<FooApp>.Settings.ReceivingHost.Should().Be("ReceivingHostOverride");
				}
			}

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class FooAppOverrides : IPlatformEnvironmentSettings
			{
				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => nameof(IsolatedHost) + "Override";

				public string ManagementDatabaseInstance => "Override";

				public string ManagementDatabaseServer => "localhost" + "Override";

				public string MonitoringDatabaseInstance => "Override";

				public string MonitoringDatabaseServer => "localhost" + "Override";

				public string ProcessingDatabaseInstance => string.Empty;

				public string ProcessingDatabaseServer => "localhost" + "Override";

				public string ProcessingHost => nameof(ProcessingHost) + "Override";

				public string ReceivingHost => nameof(ReceivingHost) + "Override";

				public string TransmittingHost => nameof(TransmittingHost) + "Override";

				#endregion
			}

			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			private class FooApp : CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("myIsolatedHost");

				public string ManagementDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ManagementDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string MonitoringDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string MonitoringDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ProcessingDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

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
					FooApp.Settings.Should().BeAssignableTo<IPlatformEnvironmentSettings>();
					FooApp.Settings.Should().BeOfType<FooApp>();
					FooApp.Settings.ReceivingHost.Should().Be("ReceivingHost");
				}
			}

			[Fact]
			public void SsoSettingsReturnDefaultValues()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
					FooApp.Settings.SsoSettings.Should().BeEquivalentTo(
						new Dictionary<string, string> {
							{ nameof(FooApp.TargetEnvironment), DeploymentContext.TargetEnvironment }
						});
			}

			private class FooApp : CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("IsolatedHost");

				public string ManagementDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ManagementDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string MonitoringDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string MonitoringDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ProcessingDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion

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
					FooApp.Settings.Should().BeAssignableTo<IPlatformEnvironmentSettings>();
					FooApp.Settings.Should().BeOfType<FooApp>();
					FooApp.Settings.Should().NotBeOfType<FooAppOverrides>();
					FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
					FooApp.Settings.ReceivingHost.Should().Be("ReceivingHostOverride");
					// overriden even though not part of IPlatformEnvironmentSettings interface
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

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class FooAppOverrides : IPlatformEnvironmentSettings
			{
				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => nameof(IsolatedHost) + "Override";

				public string ManagementDatabaseInstance => "Override";

				public string ManagementDatabaseServer => "localhost" + "Override";

				public string MonitoringDatabaseInstance => "Override";

				public string MonitoringDatabaseServer => "localhost" + "Override";

				public string ProcessingDatabaseInstance => string.Empty;

				public string ProcessingDatabaseServer => "localhost" + "Override";

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

			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			private class FooApp : CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("myIsolatedHost");

				public string ManagementDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ManagementDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string MonitoringDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string MonitoringDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ProcessingDatabaseServer => GetOverriddenOrDefaultValue("localhost");

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

		#region Nested Type: WithUnrelatedEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithUnrelatedEnvironmentSettingOverrides
		{
			[Fact]
			public void SettingsThrows()
			{
				using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE", environmentSettingOverridesType: typeof(FooAppOverrides)))
				{
					Invoking(() => FooApp.Settings.ApplicationName).Should().Throw<InvalidCastException>();
				}
			}

			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			private interface IForeignPlatformEnvironmentSettings
			{
				string ForeignHost { get; }
			}

			[SuppressMessage("ReSharper", "UnusedMember.Local")]
			private class FooAppOverrides : IForeignPlatformEnvironmentSettings
			{
				#region IForeignPlatformEnvironmentSettings Members

				public string ForeignHost => nameof(ForeignHost);

				#endregion
			}

			[SuppressMessage("ReSharper", "UnusedMember.Global")]
			private class FooApp : CompositeEnvironmentSettings<FooApp, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(FooApp);

				#endregion

				#region IPlatformEnvironmentSettings Members

				public string IsolatedHost => GetOverriddenOrDefaultValue("myIsolatedHost");

				public string ManagementDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ManagementDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string MonitoringDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string MonitoringDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

				public string ProcessingDatabaseServer => GetOverriddenOrDefaultValue("localhost");

				public string ProcessingHost => GetOverriddenOrDefaultValue(nameof(ProcessingHost));

				public string ReceivingHost => GetOverriddenOrDefaultValue(nameof(ReceivingHost));

				public string TransmittingHost => GetOverriddenOrDefaultValue(nameof(TransmittingHost));

				#endregion
			}
		}

		#endregion
	}
}
