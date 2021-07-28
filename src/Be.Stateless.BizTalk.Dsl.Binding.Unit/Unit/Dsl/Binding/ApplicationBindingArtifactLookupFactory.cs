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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Unit.Dsl.Binding
{
	public static class ApplicationBindingArtifactLookupFactory<T>
		where T : IApplicationBinding, IApplicationBindingArtifactLookup, IVisitable<IApplicationBindingVisitor>, new()
	{
		[SuppressMessage("ReSharper", "InvertIf")]
		public static IApplicationBindingArtifactLookup Create(params IApplicationBinding[] referencedApplications)
		{
			// rely on ApplicationBinding's assembly to compute an ExcelSettingOverridesFolderPath by convention
			DeploymentContext.ExcelSettingOverridesFolderPath ??= Path.Combine(typeof(T).Assembly.Location!, "EnvironmentSettings");
			// rely on BizTalk.Factory SSO store, which has to be deployed to run process tests anyway, to determine the target
			// environment to generate the binding artifacts' names for
			DeploymentContext.TargetEnvironment ??= BizTalkFactorySettings.TargetEnvironment;

			if (!_cache.ContainsKey(DeploymentContext.TargetEnvironment))
			{
				var applicationBinding = new T();
				applicationBinding.ReferencedApplications.Add(referencedApplications);
				// ensure application bindings are settled for target environment
				applicationBinding.Accept(new EnvironmentOverrideApplicationVisitor());
				_cache[DeploymentContext.TargetEnvironment] = applicationBinding;
			}
			return _cache[DeploymentContext.TargetEnvironment];
		}

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly Dictionary<string, IApplicationBindingArtifactLookup> _cache = new();
	}
}
