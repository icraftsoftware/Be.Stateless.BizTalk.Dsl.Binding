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

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	/// <summary>
	/// Attribute identifying individual <see cref="EnvironmentSettings{T}"/> properties that need to be deployed in an SSO
	/// affiliate application store in order to be available at runtime to the BizTalk application.
	/// </summary>
	/// <remarks>
	/// This attribute is meant to be used to qualify individual application setting properties that one developer typically
	/// writes in order to generate application bindings.
	/// </remarks>
	/// <seealso cref="EnvironmentSettings{T}"/>
	// TODO move to Dsl.Binding assembly
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SsoSettingAttribute : Attribute { }
}
