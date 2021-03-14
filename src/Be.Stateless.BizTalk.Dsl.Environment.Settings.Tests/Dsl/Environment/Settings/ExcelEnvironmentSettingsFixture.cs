﻿#region Copyright & License

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
using Be.Stateless.BizTalk.Dummies.Environment.Settings;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	[Collection("DeploymentContext")]
	public class ExcelEnvironmentSettingsFixture : IDisposable
	{
		#region Setup/Teardown

		public ExcelEnvironmentSettingsFixture()
		{
			DeploymentContext.TargetEnvironment = "DEV";
		}

		public void Dispose()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = null;
			DeploymentContext.TargetEnvironment = null;
		}

		#endregion

		[Fact]
		public void FailsWhenTargetEnvironmentValuesAreDefinedMultipleTimes()
		{
			var sut = new DummyExcelEnvironmentSettings();
			sut._targetEnvironments[0] = "DEV";
			Invoking(() => sut.BamArchiveWindowTimeLength)
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'DEV' target environment has been declared multiple times in the 'BizTalk.Factory.Settings' file.");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;

			var sut = new DummyExcelEnvironmentSettings();
			Invoking(() => sut.UninitializedReferenceTypeSetting)
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'UninitializedReferenceTypeSetting' does not have a defined value neither for 'DEV' or default target environment.");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;

			var sut = new DummyExcelEnvironmentSettings();
			sut.UnoverriddenReferenceTypeSetting.Should().Be("unoverridden");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithNotFoundSettingOverridesFileFallsBackToEmbeddedValue()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = Path.Combine(ExcelSettingOverridesFolderPath, "dummy");

			var sut = new DummyExcelEnvironmentSettings();
			sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			var sut = new DummyExcelEnvironmentSettings();
			sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;

			var sut = new DummyExcelEnvironmentSettings();
			sut.ClaimStoreCheckInDirectory.Should().Be("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn");
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;

			var sut = new DummyExcelEnvironmentSettings();
			Invoking(() => sut.UninitializedValueTypeSetting)
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'UninitializedValueTypeSetting' does not have a defined value neither for 'DEV' or default target environment.");
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;

			var sut = new DummyExcelEnvironmentSettings();
			sut.UnoverriddenValueTypeSetting.Should().Be(-1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithNotFoundSettingOverridesFileFallsBackToEmbeddedValue()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = Path.Combine(ExcelSettingOverridesFolderPath, "dummy");

			var sut = new DummyExcelEnvironmentSettings();
			sut.BamArchiveWindowTimeLength.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			var sut = new DummyExcelEnvironmentSettings();
			sut.BamArchiveWindowTimeLength.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;

			var sut = new DummyExcelEnvironmentSettings();
			sut.BamArchiveWindowTimeLength.Should().Be(30);
		}

		private static string ExcelSettingOverridesFolderPath => ComputeExcelSettingOverridesFolderPath();

		private static string ComputeExcelSettingOverridesFolderPath([CallerFilePath] string filepath = "")
		{
			return Path.Combine(Path.GetDirectoryName(filepath)!, @"..\..\..\Resources\Settings");
		}
	}
}
