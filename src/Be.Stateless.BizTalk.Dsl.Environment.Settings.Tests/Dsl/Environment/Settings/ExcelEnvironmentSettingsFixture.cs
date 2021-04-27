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
using System.IO;
using System.Runtime.CompilerServices;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dummies.Environment.Settings;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	[Collection("DeploymentContext")]
	public class ExcelEnvironmentSettingsFixture
	{
		[Fact]
		public void FailsWhenTargetEnvironmentValuesAreDefinedMultipleTimes()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV"))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut._targetEnvironments[0] = "DEV";
				Invoking(() => sut.BamArchiveWindowTimeLength)
					.Should().Throw<InvalidOperationException>()
					.WithMessage("'DEV' target environment has been declared multiple times in the 'BizTalk.Factory.Settings' file.");
			}
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: ExcelSettingOverridesFolderPath))
			{
				var sut = new DummyExcelEnvironmentSettings();
				Invoking(() => sut.UninitializedReferenceTypeSetting)
					.Should().Throw<InvalidOperationException>()
					.WithMessage("'UninitializedReferenceTypeSetting' does not have a defined value neither for 'DEV' or default target environment.");
			}
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: ExcelSettingOverridesFolderPath))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.UnoverriddenReferenceTypeSetting.Should().Be("unoverridden");
			}
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithNotFoundSettingOverridesFileFallsBackToEmbeddedValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: Path.Combine(ExcelSettingOverridesFolderPath, "dummy")))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\CheckIn");
			}
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV"))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\CheckIn");
			}
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: ExcelSettingOverridesFolderPath))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn");
			}
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: ExcelSettingOverridesFolderPath))
			{
				var sut = new DummyExcelEnvironmentSettings();
				Invoking(() => sut.UninitializedValueTypeSetting)
					.Should().Throw<InvalidOperationException>()
					.WithMessage("'UninitializedValueTypeSetting' does not have a defined value neither for 'DEV' or default target environment.");
			}
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: ExcelSettingOverridesFolderPath))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.UnoverriddenValueTypeSetting.Should().Be(-1);
			}
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithNotFoundSettingOverridesFileFallsBackToEmbeddedValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: Path.Combine(ExcelSettingOverridesFolderPath, "dummy")))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.BamArchiveWindowTimeLength.Should().Be(1);
			}
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV"))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.BamArchiveWindowTimeLength.Should().Be(1);
			}
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "DEV", excelSettingOverridesFolderPath: ExcelSettingOverridesFolderPath))
			{
				var sut = new DummyExcelEnvironmentSettings();
				sut.BamArchiveWindowTimeLength.Should().Be(30);
			}
		}

		private static string ExcelSettingOverridesFolderPath => ComputeExcelSettingOverridesFolderPath();

		private static string ComputeExcelSettingOverridesFolderPath([CallerFilePath] string filepath = "")
		{
			return Path.Combine(Path.GetDirectoryName(filepath)!, @"..\..\..\Resources\Settings");
		}
	}
}
