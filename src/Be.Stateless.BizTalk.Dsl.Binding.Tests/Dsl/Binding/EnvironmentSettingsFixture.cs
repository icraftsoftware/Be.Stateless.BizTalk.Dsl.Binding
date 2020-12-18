#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using System.IO;
using System.Runtime.CompilerServices;
using Be.Stateless.BizTalk.Dummies.EnvironmentSettings;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class EnvironmentSettingsFixture
	{
		[Fact]
		public void FailsWhenTargetEnvironmentValuesAreDefinedMultipleTimes()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new DummyEnvironmentSettings();
			sut._targetEnvironments[0] = "DEV";
			Function(() => sut.BamArchiveWindowTimeLength)
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'DEV' target environment has been declared multiple times in the 'BizTalk.Factory.SettingsFileGenerator' file.");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			Function(() => sut.UninitializedReferenceTypeSetting)
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'UninitializedReferenceTypeSetting' does not have a defined value neither for 'DEV' or default target environment.");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			sut.UnoverriddenReferenceTypeSetting.Should().Be("unoverridden");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithNotFoundSettingOverrideFileFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = Path.Combine(EnvironmentSettingRootPath, "dummy");

			var sut = new DummyEnvironmentSettings();
			sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new DummyEnvironmentSettings();
			sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn");
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			Function(() => sut.UninitializedValueTypeSetting)
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'UninitializedValueTypeSetting' does not have a defined value neither for 'DEV' or default target environment.");
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			sut.UnoverriddenValueTypeSetting.Should().Be(-1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithNotFoundSettingOverrideFileFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = Path.Combine(EnvironmentSettingRootPath, "dummy");

			var sut = new DummyEnvironmentSettings();
			sut.BamArchiveWindowTimeLength.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new DummyEnvironmentSettings();
			sut.BamArchiveWindowTimeLength.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			sut.BamArchiveWindowTimeLength.Should().Be(30);
		}

		private static string EnvironmentSettingRootPath => ComputeEnvironmentSettingRootPath();

		private static string ComputeEnvironmentSettingRootPath([CallerFilePath] string filepath = "")
		{
			return Path.Combine(Path.GetDirectoryName(filepath)!, @"..\..\Resources");
		}
	}
}
