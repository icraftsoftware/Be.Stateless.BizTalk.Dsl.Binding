#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install
{
	public static class DeploymentContext
	{
		public static Type EnvironmentSettingOverridesType { get; internal set; }

		public static string TargetEnvironment
		{
			get => _targetEnvironment;
			internal set => _targetEnvironment = value.IsNullOrEmpty() ? default : value;
		}

		private static string _targetEnvironment;
	}
}
