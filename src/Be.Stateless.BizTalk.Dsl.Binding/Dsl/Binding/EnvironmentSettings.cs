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
using System.Linq;
using System.Runtime.CompilerServices;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class EnvironmentSettings : IEnvironmentSettingOverrides
	{
		#region IEnvironmentSettingOverrides Members

		public T[] ValuesForProperty<T>(string propertyName, T[] defaultValues) where T : class
		{
			return defaultValues;
		}

		public T?[] ValuesForProperty<T>(string propertyName, T?[] defaultValues) where T : struct
		{
			return defaultValues;
		}

		#endregion

		protected abstract string SettingsFileName { get; }

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
		protected abstract string[] TargetEnvironments { get; }

		[SuppressMessage("ReSharper", "InvertIf")]
		private IEnvironmentSettingOverrides SettingsOverrides
		{
			get
			{
				if (BindingGenerationContext.EnvironmentSettingRootPath != null && _environmentSettingOverrides == null)
				{
					var filePath = Path.Combine(BindingGenerationContext.EnvironmentSettingRootPath, $"{SettingsFileName}.xml");
					if (File.Exists(filePath)) _environmentSettingOverrides = new EnvironmentSettingOverrides(filePath);
				}
				return _environmentSettingOverrides ??= this;
			}
		}

		private int TargetEnvironmentIndex
		{
			get
			{
				if (TargetEnvironments.Count(e => e == BindingGenerationContext.TargetEnvironment) > 1)
					throw new InvalidOperationException(
						$"'{BindingGenerationContext.TargetEnvironment}' target environment has been declared multiple times in the '{SettingsFileName}' file.");

				_targetEnvironmentsIndex = Array.IndexOf(TargetEnvironments, BindingGenerationContext.TargetEnvironment);
				if (_targetEnvironmentsIndex < 0)
					throw new InvalidOperationException($"'{BindingGenerationContext.TargetEnvironment}' is not a target environment declared in the '{SettingsFileName}' file.");
				return _targetEnvironmentsIndex;
			}
		}

		protected internal T ValueForTargetEnvironment<T>(T?[] values, [CallerMemberName] string propertyName = null) where T : struct
		{
			values = SettingsOverrides.ValuesForProperty(propertyName, values);
			return values[TargetEnvironmentIndex]
				?? values[0]
				?? throw new InvalidOperationException(
					$"'{propertyName}' does not have a defined value neither for '{BindingGenerationContext.TargetEnvironment}' or default target environment.");
		}

		protected internal T ValueForTargetEnvironment<T>(T[] values, [CallerMemberName] string propertyName = null) where T : class
		{
			values = SettingsOverrides.ValuesForProperty(propertyName, values);
			return values[TargetEnvironmentIndex]
				?? values[0]
				?? throw new InvalidOperationException(
					$"'{propertyName}' does not have a defined value neither for '{BindingGenerationContext.TargetEnvironment}' or default target environment.");
		}

		private IEnvironmentSettingOverrides _environmentSettingOverrides;
		private int _targetEnvironmentsIndex = -1;
	}
}
