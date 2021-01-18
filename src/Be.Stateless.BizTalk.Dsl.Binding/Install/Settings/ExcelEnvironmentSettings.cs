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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Be.Stateless.BizTalk.Install.Settings
{
	public abstract class ExcelEnvironmentSettings : IExcelEnvironmentSettingOverrides
	{
		#region IExcelEnvironmentSettingOverrides Members

		public T[] ValuesForProperty<T>(string propertyName, T[] defaultValues) where T : class
		{
			return defaultValues;
		}

		public T?[] ValuesForProperty<T>(string propertyName, T?[] defaultValues) where T : struct
		{
			return defaultValues;
		}

		#endregion

		protected abstract string ExcelFileName { get; }

		protected abstract string[] TargetEnvironments { get; }

		[SuppressMessage("ReSharper", "InvertIf")]
		private IExcelEnvironmentSettingOverrides SettingOverrides
		{
			get
			{
				if (DeploymentContext.ExcelSettingOverridesFolderPath != null && _excelEnvironmentSettingOverrides == null)
				{
					var filePath = Path.Combine(DeploymentContext.ExcelSettingOverridesFolderPath, $"{ExcelFileName}.xml");
					if (File.Exists(filePath)) _excelEnvironmentSettingOverrides = new ExcelEnvironmentSettingOverrides(filePath);
				}
				return _excelEnvironmentSettingOverrides ??= this;
			}
		}

		private int TargetEnvironmentIndex
		{
			get
			{
				if (TargetEnvironments.Count(e => e == DeploymentContext.TargetEnvironment) > 1)
					throw new InvalidOperationException(
						$"'{DeploymentContext.TargetEnvironment}' target environment has been declared multiple times in the '{ExcelFileName}' file.");

				_targetEnvironmentsIndex = Array.IndexOf(TargetEnvironments, DeploymentContext.TargetEnvironment);
				if (_targetEnvironmentsIndex < 0)
					throw new InvalidOperationException($"'{DeploymentContext.TargetEnvironment}' is not a target environment declared in the '{ExcelFileName}' file.");
				return _targetEnvironmentsIndex;
			}
		}

		protected T ValueForTargetEnvironment<T>(T?[] values, [CallerMemberName] string propertyName = null) where T : struct
		{
			values = SettingOverrides.ValuesForProperty(propertyName, values);
			return values[TargetEnvironmentIndex]
				?? values[0]
				?? throw new InvalidOperationException(
					$"'{propertyName}' does not have a defined value neither for '{DeploymentContext.TargetEnvironment}' or default target environment.");
		}

		protected T ValueForTargetEnvironment<T>(T[] values, [CallerMemberName] string propertyName = null) where T : class
		{
			values = SettingOverrides.ValuesForProperty(propertyName, values);
			return values[TargetEnvironmentIndex]
				?? values[0]
				?? throw new InvalidOperationException(
					$"'{propertyName}' does not have a defined value neither for '{DeploymentContext.TargetEnvironment}' or default target environment.");
		}

		private IExcelEnvironmentSettingOverrides _excelEnvironmentSettingOverrides;
		private int _targetEnvironmentsIndex = -1;
	}
}
