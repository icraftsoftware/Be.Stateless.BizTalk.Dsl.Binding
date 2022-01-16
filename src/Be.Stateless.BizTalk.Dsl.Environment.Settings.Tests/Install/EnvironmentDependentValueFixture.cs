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
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Install
{
	public class EnvironmentDependentValueFixture
	{
		[Fact]
		public void ReturnAcceptanceValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.ACCEPTANCE))
			{
				DateSetting.Should().Be(DateTime.MinValue);
				Setting1.Should().Be("one.2");
				Setting2.Should().Be(1);
				Setting3.Should().Be("three.2");
				Invoking(() => Setting4).Should().NotThrow().And.Subject().Should().Be("four");
				Invoking(() => Setting5)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting5)}' does not provide a value for target environment '{TargetEnvironment.ACCEPTANCE}'.");
				Setting6.Should().Be(3);
				Setting7.Should().Be(4);
			}
		}

		[Fact]
		public void ReturnBuildValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.BUILD))
			{
				Invoking(() => DateSetting)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(DateSetting)}' does not provide a value for target environment '{TargetEnvironment.BUILD}'.");
				Setting1.Should().Be("one.1");
				Setting2.Should().Be(2);
				Setting3.Should().Be("three.1");
				Invoking(() => Setting4)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting4)}' does not provide a value for target environment '{TargetEnvironment.BUILD}'.");
				Invoking(() => Setting5).Should().NotThrow().And.Subject().Should().Be(0);
				Setting6.Should().Be(2);
				Setting7.Should().Be(2);
			}
		}

		[Fact]
		public void ReturnDevelopmentValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				DateSetting.Should().Be(new(2021, 3, 3));
				Setting1.Should().Be("one.1");
				Setting2.Should().Be(3);
				Setting3.Should().Be("three.1");
				Invoking(() => Setting4)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting4)}' does not provide a value for target environment '{TargetEnvironment.DEVELOPMENT}'.");
				Invoking(() => Setting5).Should().NotThrow().And.Subject().Should().Be(0);
				Setting6.Should().Be(1);
				Setting7.Should().Be(1);
			}
		}

		[Fact]
		public void ReturnIntegrationValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.INTEGRATION))
			{
				Invoking(() => DateSetting)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(DateSetting)}' does not provide a value for target environment '{TargetEnvironment.INTEGRATION}'.");
				Invoking(() => Setting1)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting1)}' does not provide a value for target environment '{TargetEnvironment.INTEGRATION}'.");
				Invoking(() => Setting2)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting2)}' does not provide a value for target environment '{TargetEnvironment.INTEGRATION}'.");
				Setting3.Should().Be("three.2");
				Invoking(() => Setting4)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting4)}' does not provide a value for target environment '{TargetEnvironment.INTEGRATION}'.");
				Invoking(() => Setting5)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting5)}' does not provide a value for target environment '{TargetEnvironment.INTEGRATION}'.");
				Invoking(() => Setting5).Should().Throw<NotSupportedException>();
				Setting7.Should().Be(3);
			}
		}

		[Fact]
		public void ReturnPreProductionValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.PREPRODUCTION))
			{
				DateSetting.Should().Be(DateTime.MinValue);
				Setting1.Should().Be("one.3");
				Setting2.Should().Be(4);
				Setting3.Should().Be("three.2");
				Invoking(() => Setting4).Should().NotThrow().And.Subject().Should().Be("four");
				Invoking(() => Setting5)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting5)}' does not provide a value for target environment '{TargetEnvironment.PREPRODUCTION}'.");
				Setting6.Should().Be(4);
				Setting7.Should().Be(5);
			}
		}

		[Fact]
		public void ReturnProductionValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.PRODUCTION))
			{
				DateSetting.Should().Be(DateTime.MinValue);
				Setting1.Should().Be("one.3");
				Setting2.Should().Be(5);
				Setting3.Should().Be("three.2");
				Invoking(() => Setting4).Should().NotThrow().And.Subject().Should().Be("four");
				Invoking(() => Setting5)
					.Should().Throw<NotSupportedException>()
					.WithMessage($"'{nameof(Setting5)}' does not provide a value for target environment '{TargetEnvironment.PRODUCTION}'.");
				Setting6.Should().Be(4);
				Setting7.Should().Be(5);
			}
		}

		private DateTime DateSetting => EnvironmentDependentValue
			.ForDevelopment(new DateTime(2021, 3, 3))
			.ForAcceptanceUpwards(DateTime.MinValue);

		private string Setting1 => EnvironmentDependentValue
			.ForDevelopmentOrBuild("one.1")
			.ForAcceptance("one.2")
			.ForPreProductionUpwards("one.3");

		private int Setting2 => EnvironmentDependentValue
			.ForAcceptance(1)
			.ForBuild(2)
			.ForDevelopment(3)
			.ForPreProduction(4)
			.ForProduction(5);

		private string Setting3 => EnvironmentDependentValue
			.ForDevelopmentOrBuild("three.1")
			.ForIntegrationUpwards("three.2");

		private string Setting4 => EnvironmentDependentValue
			.ForAcceptanceUpwards("four");

		private int Setting5 => EnvironmentDependentValue
			.ForDevelopmentOrBuild(0);

		private int Setting6 => DeploymentContext.TargetEnvironment switch {
			var e when e.IsDevelopment() => 1,
			var e when e.IsBuild() => 2,
			var e when e.IsAcceptance() => 3,
			var e when e.IsPreProductionUpwards() => 4,
			_ => throw new NotSupportedException()
		};

		private int Setting7 => DeploymentContext.TargetEnvironment switch {
			TargetEnvironment.DEVELOPMENT => 1,
			TargetEnvironment.BUILD => 2,
			TargetEnvironment.INTEGRATION => 3,
			TargetEnvironment.ACCEPTANCE => 4,
			TargetEnvironment.PREPRODUCTION or TargetEnvironment.PRODUCTION => 5,
			_ => throw new NotSupportedException()
		};
	}
}
