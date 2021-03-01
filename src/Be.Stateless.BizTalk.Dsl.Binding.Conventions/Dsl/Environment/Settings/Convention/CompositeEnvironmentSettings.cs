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
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Reflection;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention
{
	/// <summary>
	/// Provides and enforces singleton instantiation and access to an <see cref="IEnvironmentSettings"/>-derived class and
	/// allows for settings to be overridden by <typeparamref name="TI"/> through composition and delegation instead of
	/// inheritance as it is the case when directly deriving from <see cref="EnvironmentSettings{T}"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the <see cref="CompositeEnvironmentSettings{T,TI}"/>-derived class that provides default settings value.
	/// </typeparam>
	/// <typeparam name="TI">
	/// The <typeparamref name="TI"/>-derived class that provides setting override values. As <typeparamref name="T"/> must
	/// support it too, <typeparamref name="TI"/> must be an interface which acts as an extension point for setting overrides.
	/// </typeparam>
	/// <seealso cref="EnvironmentSettings{T}"/>
	public class CompositeEnvironmentSettings<T, TI> : EnvironmentSettings<T>
		where T : CompositeEnvironmentSettings<T, TI>, TI, IEnvironmentSettings, new()
	{
		static CompositeEnvironmentSettings()
		{
			// overriding base class' lazy singleton factory is possible because this class hides base class' Settings property
			_lazySingletonInstance = new Lazy<T>(CreateSingletonInstance);
		}

		/// <summary>
		/// Supports runtime discovery and composition of environment setting overrides.
		/// </summary>
		/// <returns>
		/// The <typeparamref name="T"/> environment settings instance possibly composing a <typeparamref name="TI"/>-derived
		/// setting overrides instance.
		/// </returns>
		private static T CreateSingletonInstance()
		{
			var instance = new T();
			if (DeploymentContext.EnvironmentSettingOverridesType != null)
			{
				instance.EnvironmentSettingOverrides = (TI) Activator.CreateInstance(DeploymentContext.EnvironmentSettingOverridesType);
			}
			return instance;
		}

		/// <summary>
		/// Singleton instance.
		/// </summary>
		// hack: hiding base class' Settings property and declaring it again ensures this class' static ctor is called
		// see https://stackoverflow.com/a/4653075/1789441
		public new static T Settings => _lazySingletonInstance.Value;

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		protected TI EnvironmentSettingOverrides { get; private set; }

		/// <summary>
		/// Returns either the default or overridden value for a given settings property named <paramref name="propertyName"/>.
		/// </summary>
		/// <typeparam name="TV">
		/// The <see cref="Type"/> of the property <param name="propertyName"></param>.
		/// </typeparam>
		/// <param name="value">
		/// The default value of the property <param name="propertyName"></param>.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property.
		/// </param>
		/// <returns>
		/// The value of the property to be used.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method returns either the value provided by the <typeparamref name="TI"/>-derived setting overrides instance if
		/// this instance was declared via <see
		/// cref="DeploymentContext.EnvironmentSettingOverridesType">DeploymentContext.EnvironmentSettingOverridesType</see> or
		/// the default <paramref name="value"/> if it was not, i.e. the value provided by <typeparamref name="T"/>.
		/// </para>
		/// <para>
		/// The <see cref="CompositeEnvironmentSettings{T,TI}"/>-derived <typeparamref name="T"/> class must call this method
		/// should it be willing to allow for a setting property to be overridden. Failure to do so will prevent the property to
		/// be overridden at deployment time.
		/// </para>
		/// </remarks>
		protected TV GetOverriddenOrDefaultValue<TV>(TV value, [CallerMemberName] string propertyName = null)
		{
			return EnvironmentSettingOverrides != null && Reflector.TryGetProperty(EnvironmentSettingOverrides, propertyName, out var overriddenValue)
				? (TV) overriddenValue
				: value;
		}
	}
}
