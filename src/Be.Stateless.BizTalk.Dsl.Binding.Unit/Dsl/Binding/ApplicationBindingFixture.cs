#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using System.IO;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public DSL API.")]
	public abstract class ApplicationBindingFixture<T> where T : class, IVisitable<IApplicationBindingVisitor>, new()
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		protected string AssemblyPath => Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).AbsolutePath);

		protected virtual string EnvironmentSettingOverridesRootPath => null;

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		protected string GenerateApplicationBindingForTargetEnvironment(string targetEnvironment)
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException(nameof(targetEnvironment));
			BindingGenerationContext.TargetEnvironment = targetEnvironment;
			if (!EnvironmentSettingOverridesRootPath.IsNullOrEmpty()) BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingOverridesRootPath;
			return new T().GetApplicationBindingInfoSerializer().Serialize();
		}
	}
}
