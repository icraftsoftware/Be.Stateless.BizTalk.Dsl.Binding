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
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	public static class EnvironmentSettingsFixture
	{
		#region Nested Type: WithoutEnvironmentSettingOverrides

		[Collection("Sequential")]
		public class WithoutEnvironmentSettingOverrides
		{
			[Fact]
			public void BaseStaticSettingsPropertyGivesAccessToAllEnvironmentSettingProperties()
			{
				BarApp.Settings.ApplicationName.Should().Be(nameof(BarApp));
				FooApp.Settings.ApplicationName.Should().Be(nameof(FooApp));
				FooApp.Settings.ReceiveHost.Should().Be("Default Receive Host");
			}

			[Fact]
			public void BaseStaticSettingsPropertyIsSingleton()
			{
				BarApp.Settings.Should().BeSameAs(BarApp.Settings);
				FooAppOverride.Settings.Should().BeSameAs(FooApp.Settings);
				FooApp.Settings.Should().BeSameAs(FooApp.Settings);
				BarApp.Settings.Should().NotBeSameAs(FooApp.Settings);
			}

			[Fact]
			public void EnvironmentSettingOverridesAreUselessWhenNotProvidedViaDeploymentContext()
			{
				FooApp.Settings.Should().BeOfType<FooApp>();
				FooApp.Settings.Should().NotBeOfType<FooAppOverride>();
				FooAppOverride.Settings.Should().BeSameAs(FooApp.Settings);
				FooApp.Settings.ReceiveHost.Should().Be("Default Receive Host");
				FooAppOverride.Settings.ReceiveHost.Should().Be("Default Receive Host");
			}

			[Fact]
			public void EnvironmentSettingsThrowWhenInstantiatedExplicitly()
			{
				Function(() => new BarApp()).Should().Throw<InvalidOperationException>();
				Function(() => new BarAppOverride()).Should().Throw<InvalidOperationException>();
			}

			[Fact]
			public void SsoSettings()
			{
				FooApp.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\default" } });
				FooAppOverride.Settings.SsoSettings
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

				public virtual string ReceiveHost => "Default Receive Host";
			}

			[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
			private class FooAppOverride : FooApp
			{
				#region Base Class Member Overrides

				[SsoSetting]
				public override string CheckInFolder => @"c:\claim\store\in\overridden";

				public override string ReceiveHost => "Overridden Receive Host";

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithRelatedEnvironmentSettingOverrides

		[Collection("Sequential")]
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

				Function(() => BarApp.Settings).Should().NotThrow();
				BarApp.Settings.ApplicationName.Should().Be(nameof(BarApp));
			}

			[Fact]
			public void SettingsReturnsDerivedAppSettingInstanceAndValuesAreOverridden()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(FooAppOverride);

				FooApp.Settings.Should().BeOfType<FooAppOverride>();
				FooApp.Settings.Should().NotBeOfType<FooApp>();
				FooAppOverride.Settings.Should().BeSameAs(FooApp.Settings);
				FooApp.Settings.ReceiveHost.Should().Be("Overridden Receive Host");
				FooAppOverride.Settings.ReceiveHost.Should().Be("Overridden Receive Host");
			}

			[Fact]
			public void SsoSettings()
			{
				DeploymentContext.EnvironmentSettingOverridesType = typeof(FooAppOverride);

				FooApp.Settings.SsoSettings
					.Should().BeEquivalentTo(new Dictionary<string, string> { { "CheckInFolder", @"c:\claim\store\in\overridden" } });
				FooAppOverride.Settings.SsoSettings
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

				public virtual string ReceiveHost => "Default Receive Host";
			}

			private class FooAppOverride : FooApp
			{
				#region Base Class Member Overrides

				public override string CheckInFolder => @"c:\claim\store\in\overridden";

				public override string ReceiveHost => "Overridden Receive Host";

				#endregion
			}
		}

		#endregion

		#region Nested Type: WithUnrelatedEnvironmentSettingOverrides

		[Collection("Sequential")]
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

				Function(() => FooApp.Settings).Should().Throw<InvalidCastException>();
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
