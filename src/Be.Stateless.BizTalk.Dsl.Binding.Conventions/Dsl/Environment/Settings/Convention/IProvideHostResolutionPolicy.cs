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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention
{
	/// <summary>
	/// Elementary Microsoft BizTalk Server's Host Platform Settings that allows <see cref="ApplicationBinding"/> to be
	/// XML-serialized for a given target deployment environment and that provides an overriding point for <see
	/// cref="ApplicationBinding"/> written against these settings.
	/// </summary>
	/// <remarks>
	/// The <see
	/// cref="DeploymentContext.EnvironmentSettingOverridesType">DeploymentContext.EnvironmentSettingOverridesType</see>
	/// overriding <see cref="IProvideHostResolutionPolicy"/> cannot override <see cref="IProvideHostNames"/> at the same time.
	/// </remarks>
	/// <seealso cref="CompositeEnvironmentSettings{T,TI}"/>
	/// <seealso cref="DeploymentContext"/>
	/// <seealso cref="DeploymentContext.EnvironmentSettingOverridesType">DeploymentContext.EnvironmentSettingOverridesType</seealso>
	/// <seealso cref="TargetEnvironment"/>
	/// <seealso cref="IProvideHostNames"/>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public interface IProvideHostResolutionPolicy : IProvideEnvironmentSettings
	{
		/// <summary>
		/// <see cref="Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy"/>-derived policy that overrides the
		/// default <c>BizTalk.Factory</c>'s <see cref="Be.Stateless.BizTalk.Factory.Convention.HostResolutionPolicy"/> policy in
		/// order to resolve the Microsoft BizTalk Server Host names for the target environment for which the <see
		/// cref="ApplicationBinding"/> is being XML-serialized.
		/// </summary>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy HostResolutionPolicy { get; }
	}
}
