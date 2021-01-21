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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	public abstract class EnvironmentSettings<T>
		where T : IEnvironmentSettings, new()
	{
		private static T CreateSingletonInstance()
		{
			var resolvedEnvironmentSettingType = DeploymentContext.EnvironmentSettingOverridesType ?? typeof(T);
			if (!typeof(T).IsAssignableFrom(resolvedEnvironmentSettingType))
				throw new InvalidCastException($"Unable to cast object of type '{resolvedEnvironmentSettingType.Name}' to type '{typeof(T).Name}'.");
			return (T) Activator.CreateInstance(resolvedEnvironmentSettingType);
		}

		public static T Settings => _lazy.Value;

		protected EnvironmentSettings()
		{
			if (_lazy.IsValueCreated)
				throw new InvalidOperationException(
					$"EnvironmentSettings<T>-derived {GetType().Name} class cannot be instantiated explicitly and is accessible only through its static {nameof(Settings)} property.");
		}

		public Dictionary<string, string> SsoSettings => _runtimeSsoSettings ??= GetType().GetTypeInfo()
			.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
			.Where(pi => pi.GetCustomAttribute(typeof(SsoSettingAttribute), true) != null)
			.ToDictionary(pi => pi.Name, pi => pi.GetValue(this).ToString());

		private static readonly Lazy<T> _lazy = new Lazy<T>(CreateSingletonInstance);

		private Dictionary<string, string> _runtimeSsoSettings;
	}
}
