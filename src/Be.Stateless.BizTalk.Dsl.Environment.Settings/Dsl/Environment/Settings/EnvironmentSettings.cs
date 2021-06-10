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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	/// <summary>
	/// Provides and enforces singleton instantiation and access to the <see cref="IEnvironmentSettings"/>-derived classes
	/// throughout an inheritance chain.
	/// </summary>
	/// <typeparam name="T">
	/// The <see cref="IEnvironmentSettings"/>-derived class.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// <see cref="EnvironmentSettings{T}"/> allows a class, <typeparamref name="T"/>, that directly derives from it to expose
	/// singleton semantics &#8212;i.e. regarding instantiation and access&#8212; while supporting environment setting overrides
	/// through inheritance.
	/// </para>
	/// <para>
	/// Even though instantiation and access to environment setting overrides is offered by this class, the actual settings
	/// class, <typeparamref name="T"/>, must allow for overrides via standard C# mechanisms, i.e. <c>abstract</c> or
	/// <c>virtual</c> properties. <see cref="EnvironmentSettings{T}"/> therefore is the base class for the first inheritance
	/// level of an <see cref="IEnvironmentSettings"/> inheritance chain. Further inheriting classes, i.e. environment setting
	/// overrides, down the inheritance chain must consequently directly derive from the class they provide overrides for
	/// &#8212;as one would typically do in C#&#8212; and not directly from this class.
	/// </para>
	/// </remarks>
	public abstract class EnvironmentSettings<T> : IProvideSsoSettings
		where T : EnvironmentSettings<T>, IEnvironmentSettings, new()
	{
		static EnvironmentSettings()
		{
			SingletonFactory = CreateEnvironmentSettingsSingletonInstance;
		}

		/// <summary>
		/// Supports runtime discovery and composition of <see cref="EnvironmentSettings{T}"/> overrides via <see
		/// cref="DeploymentContext.EnvironmentSettingOverridesType">DeploymentContext.EnvironmentSettingOverridesType</see> and
		/// their instantiation.
		/// </summary>
		/// <returns>
		/// The possibly <typeparamref name="T"/>-derived environment settings instance.
		/// </returns>
		private static T CreateEnvironmentSettingsSingletonInstance()
		{
			var resolvedEnvironmentSettingType = DeploymentContext.EnvironmentSettingOverridesType ?? typeof(T);
			if (!typeof(T).IsAssignableFrom(resolvedEnvironmentSettingType))
				throw new InvalidOperationException(
					$"'{resolvedEnvironmentSettingType.Name}' does not derive from '{typeof(T).Name}' and cannot be used as its {nameof(DeploymentContext.EnvironmentSettingOverridesType)}.");
			return (T) Activator.CreateInstance(resolvedEnvironmentSettingType);
		}

		protected internal static Lazy<T> LazySingletonInstance { get; internal set; } = new(InvokeSingletonFactory);

		/// <summary>
		/// Singleton instance.
		/// </summary>
		public static T Settings => LazySingletonInstance.Value;

		protected static Func<T> SingletonFactory { get; set; }

		/// <summary>
		/// Together with this class protected default ctor, enforces that <see cref="EnvironmentSettings{T}"/> instances, or
		/// derived instances thereof, can only be created through <seealso cref="LazySingletonInstance"/> lazy factory.
		/// </summary>
		internal static T InvokeSingletonFactory()
		{
			try
			{
				_isCreatingSingletonInstance = true;
				return SingletonFactory.Invoke();
			}
			finally
			{
				_isCreatingSingletonInstance = false;
			}
		}

		/// <summary>
		/// Ensures singleton instantiation.
		/// </summary>
		protected EnvironmentSettings()
		{
			if (!_isCreatingSingletonInstance || LazySingletonInstance.IsValueCreated)
				throw new InvalidOperationException(
					$"EnvironmentSettings<T>-derived {GetType().Name} class cannot be instantiated explicitly and is accessible only through its static {nameof(Settings)} property.");
		}

		#region IProvideSsoSettings Members

		/// <summary>
		/// Dictionary of environment setting properties, together with their values, that must be deployed in an SSO affiliate
		/// application store in order to be available at runtime to the BizTalk application.
		/// </summary>
		/// <remarks>
		/// Notice that the set of properties that need to be deployed in SSO is determined by the actual concrete type that
		/// inherits from <see cref="EnvironmentSettings{T}"/>, that is to say <typeparamref name="T"/>, and cannot be customized
		/// by any override type provided through <see
		/// cref="DeploymentContext.EnvironmentSettingOverridesType">DeploymentContext.EnvironmentSettingOverridesType</see>.
		/// </remarks>
		public Dictionary<string, string> SsoSettings => _runtimeSsoSettings ??= typeof(T).GetInterfaces().Append(typeof(T))
			.Select(t => t.GetTypeInfo().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public))
			.SelectMany(p => p.Where(pi => pi.GetCustomAttribute(typeof(SsoSettingAttribute), true) != null))
			.ToDictionary(pi => pi.Name, pi => pi.GetValue(this).ToString());

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static bool _isCreatingSingletonInstance;

		private Dictionary<string, string> _runtimeSsoSettings;
	}
}
