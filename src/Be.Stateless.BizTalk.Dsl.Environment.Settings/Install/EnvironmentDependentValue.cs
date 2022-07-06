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
using System.Runtime.CompilerServices;

namespace Be.Stateless.BizTalk.Install
{
	// TODO move to Dsl.Binding.Conventions assembly
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static class EnvironmentDependentValue
	{
		public static EnvironmentDependentValue<T> ForDevelopment<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForDevelopment(value);
		}

		public static EnvironmentDependentValue<T> ForBuild<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForBuild(value);
		}

		public static EnvironmentDependentValue<T> ForDevelopmentOrBuild<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForDevelopmentOrBuild(value);
		}

		public static EnvironmentDependentValue<T> ForIntegration<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForIntegration(value);
		}

		public static EnvironmentDependentValue<T> ForIntegrationUpwards<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForIntegrationUpwards(value);
		}

		public static EnvironmentDependentValue<T> ForAcceptance<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForAcceptance(value);
		}

		public static EnvironmentDependentValue<T> ForAcceptanceUpwards<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForAcceptanceUpwards(value);
		}

		public static EnvironmentDependentValue<T> ForPreProduction<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForPreProduction(value);
		}

		public static EnvironmentDependentValue<T> ForPreProductionUpwards<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForPreProductionUpwards(value);
		}

		public static EnvironmentDependentValue<T> ForProduction<T>(T value, [CallerMemberName] string settingName = null!)
		{
			return new EnvironmentDependentValue<T>(settingName).ForProduction(value);
		}
	}

	public class EnvironmentDependentValue<T>
	{
		#region Operators

		public static implicit operator T(EnvironmentDependentValue<T> environmentDependentValue)
		{
			if (!environmentDependentValue._hasValue)
				throw new NotSupportedException($"'{environmentDependentValue._name}' does not provide a value for target environment '{DeploymentContext.TargetEnvironment}'.");
			return environmentDependentValue.Value;
		}

		#endregion

		internal EnvironmentDependentValue(string name)
		{
			_name = name ?? throw new ArgumentNullException(nameof(name));
		}

		private T Value
		{
			get => _value;
			set
			{
				_hasValue = true;
				_value = value;
			}
		}

		public EnvironmentDependentValue<T> ForDevelopment(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopment()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForBuild(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsBuild()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForDevelopmentOrBuild(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsDevelopmentOrBuild()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForIntegration(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsIntegration()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForIntegrationUpwards(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsIntegrationUpwards()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForAcceptance(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsAcceptance()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForAcceptanceUpwards(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsAcceptanceUpwards()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForPreProduction(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsPreProduction()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForPreProductionUpwards(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsPreProductionUpwards()) Value = value;
			return this;
		}

		public EnvironmentDependentValue<T> ForProduction(T value)
		{
			if (DeploymentContext.TargetEnvironment.IsProduction()) Value = value;
			return this;
		}

		private readonly string _name;
		private bool _hasValue;
		private T _value;
	}
}
