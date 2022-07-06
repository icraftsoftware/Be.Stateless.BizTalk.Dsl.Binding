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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	// TODO move to Dsl.Binding assembly
	public interface IProvideSsoSettings
	{
		/// <summary>
		/// Dictionary of environment setting properties, together with their values, that must be deployed in an SSO affiliate
		/// application store in order to be available at runtime to the BizTalk application.
		/// </summary>
		/// <remarks>
		/// Notice that the set of properties that need to be deployed in SSO is determined by the actual concrete type that
		/// inherits from <see cref="EnvironmentSettings{T}"/>, that is to say <c>T</c>, and cannot be customized by any override
		/// type provided through <see
		/// cref="DeploymentContext.EnvironmentSettingOverridesType">DeploymentContext.EnvironmentSettingOverridesType</see>.
		/// </remarks>
		[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API.")]
		Dictionary<string, string> SsoSettings { get; }
	}
}
