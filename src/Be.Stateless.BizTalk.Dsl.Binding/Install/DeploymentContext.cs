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

namespace Be.Stateless.BizTalk.Install
{
	public static class DeploymentContext
	{
		#region Nested Type: DeploymentContextMemento

		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		private class DeploymentContextMemento : IDeploymentContext
		{
			#region IDeploymentContext Members

			public Type EnvironmentSettingOverridesType { get; internal set; }

			public string ExcelSettingOverridesFolderPath { get; internal set; }

			public string TargetEnvironment { get; internal set; }

			#endregion
		}

		#endregion

		public static Type EnvironmentSettingOverridesType
		{
			get => _instance.EnvironmentSettingOverridesType;
			internal set => _instance.EnvironmentSettingOverridesType = value;
		}

		public static string ExcelSettingOverridesFolderPath
		{
			get => _instance.ExcelSettingOverridesFolderPath;
			internal set => _instance.ExcelSettingOverridesFolderPath = value;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public static IDeploymentContext Instance => _instance;

		public static string TargetEnvironment
		{
			get => _instance.TargetEnvironment;
			internal set => _instance.TargetEnvironment = value;
		}

		private static readonly DeploymentContextMemento _instance = new DeploymentContextMemento();
	}
}
