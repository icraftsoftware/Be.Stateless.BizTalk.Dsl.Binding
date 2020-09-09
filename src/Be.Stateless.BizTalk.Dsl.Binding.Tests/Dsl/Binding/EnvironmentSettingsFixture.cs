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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
	public class EnvironmentSettingsFixture
	{
		[Fact]
		public void FailsWhenTargetEnvironmentValuesAreDefinedMultipleTimes()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			try
			{
				_targetEnvironments[0] = "DEV";
				var sut = new DummyEnvironmentSettings();
				Action(() => sut.ValueForTargetEnvironment(new int?[] { null, 30, 30, 30, 30 }, "BamArchiveWindowTimeLength"))
					.Should().Throw<InvalidOperationException>()
					.WithMessage("'DEV' target environment has been declared multiple times in the 'BizTalk.Factory.SettingsFileGenerator' file.");
			}
			finally
			{
				_targetEnvironments[0] = null;
			}
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			Action(() => sut.ValueForTargetEnvironment(new string[] { null, null, null, null, null }, "ClaimStoreCheckInDirectory2"))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'ClaimStoreCheckInDirectory2' does not have a defined value neither for 'DEV' or default target environment.");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory2");
			value.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithNotFoundSettingOverrideFileFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = Path.Combine(EnvironmentSettingRootPath, "dummy");

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			value.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			value.Should().Be("C:\\Files\\CheckIn");
		}

		[Fact]
		public void ReferenceTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null }, "ClaimStoreCheckInDirectory");
			value.Should().Be("C:\\Files\\Drops\\BizTalk.Factory\\CheckIn");
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentThrowsWhenValueIsNull()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			Action(() => sut.ValueForTargetEnvironment(new int?[] { null, null, null, null, null }, "BamArchiveWindowTimeLength2"))
				.Should().Throw<InvalidOperationException>()
				.WithMessage("'BamArchiveWindowTimeLength2' does not have a defined value neither for 'DEV' or default target environment.");
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWhoseOverrideIsNotFoundFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength2");
			value.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithNotFoundSettingOverrideFileFallsBackToEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = Path.Combine(EnvironmentSettingRootPath, "dummy");

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength");
			value.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithoutOverrideReturnsEmbeddedValue()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = null;

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength");
			value.Should().Be(1);
		}

		[Fact]
		public void ValueTypeValueForTargetEnvironmentWithOverrideReturnsOverride()
		{
			BindingGenerationContext.TargetEnvironment = "DEV";
			BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;

			var sut = new DummyEnvironmentSettings();
			var value = sut.ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 }, "BamArchiveWindowTimeLength");
			value.Should().Be(30);
		}

		private class DummyEnvironmentSettings : EnvironmentSettings
		{
			#region Base Class Member Overrides

			protected override string SettingsFileName => "BizTalk.Factory.SettingsFileGenerator";

			protected override string[] TargetEnvironments => _targetEnvironments;

			#endregion
		}

		private string EnvironmentSettingRootPath => _rootPath.Value;

		private static string ComputeEnvironmentSettingRootPath([CallerFilePath] string filepath = "")
		{
			return Path.Combine(Path.GetDirectoryName(filepath)!, @"..\..\Resources");
		}

		private static readonly Lazy<string> _rootPath = new Lazy<string>(() => ComputeEnvironmentSettingRootPath());
		private static readonly string[] _targetEnvironments = { null, "DEV", "BLD", "ACC", "PRD" };
	}
}
