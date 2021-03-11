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
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	public static class EnvironmentSettingsFixture
	{
		#region Nested Type: AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClass

		[Collection("DeploymentContext")]
		public class AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClass : IDisposable
		{
			#region Setup/Teardown

			public AccessingAndConstructingSingletonViaEnvironmentSettingsBaseClass()
			{
				DeploymentContext.TargetEnvironment = "ANYWHERE";
			}

			public void Dispose()
			{
				DeploymentContext.EnvironmentSettingOverridesType = null;
				DeploymentContext.TargetEnvironment = null;
			}

			#endregion

			[Fact]
			public void SingletonCanBeConstructedAndCanBeAccessed()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(FooAppOverrides);

				// construction
				Invoking(() => EnvironmentSettings<FooApp>.Settings).Should().NotThrow();

				FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
				// access
				EnvironmentSettings<FooApp>.Settings.Should().BeSameAs(FooApp.Settings);
				EnvironmentSettings<FooApp>.Settings.ApplicationName.Should().Be(nameof(FooApp));
				EnvironmentSettings<FooApp>.Settings.ReceivingHost.Should().Be("Overridden Receive Host");
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
			public void EnvironmentSettingsThrowWhenInstantiatedExplicitly()
			{
				Invoking(() => new BarApp()).Should().Throw<InvalidOperationException>();
				Invoking(() => new BarAppOverride()).Should().Throw<InvalidOperationException>();
			}

			[Fact]
			public void SsoSettings()
			{
				FooApp.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\default" } });
				FooAppOverrides.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\default" } });
			}

			public class BarApp : EnvironmentSettings<BarApp>, IEnvironmentSettings
			{
				#region IEnvironmentSettings Members

				public string ApplicationName => nameof(BarApp);

				#endregion
			}

			private class BarAppOverride : BarApp { }

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
		public class WithRelatedEnvironmentSettingOverrides : IDisposable
		{
			#region Setup/Teardown

			public void Dispose()
			{
				DeploymentContext.EnvironmentSettingOverridesType = null;
			}

			#endregion

			[Fact]
			public void EnvironmentSettingOverridesCanBeItself()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(BarApp);

				Invoking(() => BarApp.Settings).Should().NotThrow();
				BarApp.Settings.ApplicationName.Should().Be(nameof(BarApp));
			}

			[Fact]
			public void SettingsReturnsDerivedAppSettingInstanceAndValuesAreOverridden()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(FooAppOverrides);

				FooApp.Settings.Should().BeOfType<FooAppOverrides>();
				FooApp.Settings.Should().NotBeOfType<FooApp>();
				FooAppOverrides.Settings.Should().BeSameAs(FooApp.Settings);
				FooApp.Settings.ReceivingHost.Should().Be("Overridden Receive Host");
				FooAppOverrides.Settings.ReceivingHost.Should().Be("Overridden Receive Host");
			}

			[Fact]
			public void SsoSettings()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(FooAppOverrides);

				FooApp.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\overridden" } });
				FooAppOverrides.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\overridden" } });
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
				[SsoSetting]
				[SuppressMessage("ReSharper", "UnusedMember.Local")]
				public string MySsoProperty => nameof(MySsoProperty);
			}
		}

		#endregion

		#region Nested Type: WithUnrelatedEnvironmentSettingOverrides

		[Collection("DeploymentContext")]
		public class WithUnrelatedEnvironmentSettingOverrides : IDisposable
		{
			#region Setup/Teardown

			public void Dispose()
			{
				DeploymentContext.EnvironmentSettingOverridesType = null;
			}

			#endregion

			[Fact]
			public void EnvironmentSettingOverridesThrow()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(BarApp);

				Invoking(() => FooApp.Settings).Should().Throw<InvalidCastException>();
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
