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
using Be.Stateless.Extensions;
using Be.Stateless.Reflection;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.Extensions
{
	public static class TypeExtensions
	{
		public static IEnvironmentSettings CreateEnvironmentSettingsSingleton(this Type type)
		{
			if (!type.IsEnvironmentSettingsType()) throw new ArgumentException($"{type.Name} does not derive from {typeof(EnvironmentSettings<>).Name}.", nameof(type));
			return (IEnvironmentSettings) Reflector.GetProperty(type, "Settings");
		}

		public static bool IsEnvironmentSettingsType(this Type type)
		{
			return type != null && typeof(IEnvironmentSettings).IsAssignableFrom(type) && type.IsSubclassOfGenericType(typeof(EnvironmentSettings<>));
		}
	}
}
