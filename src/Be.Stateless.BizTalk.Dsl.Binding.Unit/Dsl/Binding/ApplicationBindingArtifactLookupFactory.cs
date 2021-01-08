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
using System.IO;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public static class ApplicationBindingArtifactLookupFactory<T>
		where T : IApplicationBinding, IApplicationBindingArtifactLookup, IVisitable<IApplicationBindingVisitor>, new()
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public static IApplicationBindingArtifactLookup Create()
		{
			// rely on BizTalk.Factory SSO store, which has to be deployed to run process tests anyway, to determine the target
			// environment to use to generate the binding artifacts' names
			var targetEnvironment = BizTalkFactorySettings.TargetEnvironment;
			var assemblyPath = Path.GetDirectoryName(new Uri(typeof(T).Assembly.CodeBase).AbsolutePath);
			// rely on ApplicationBinding's assembly to compute an EnvironmentSettingOverridesRootPath by convention
			var environmentSettingOverridesRootPath = Path.Combine(assemblyPath!, "EnvironmentSettings");
			return Create(targetEnvironment, environmentSettingOverridesRootPath);
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public static IApplicationBindingArtifactLookup Create(string targetEnvironment)
		{
			if (targetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException(nameof(targetEnvironment));
			BindingGenerationContext.TargetEnvironment = targetEnvironment;
			if (!_cache.ContainsKey(targetEnvironment))
			{
				var applicationBinding = new T();
				SettleApplicationBindingForBindingGenerationContext(applicationBinding);
				_cache[targetEnvironment] = applicationBinding;
			}
			return _cache[targetEnvironment];
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
		public static IApplicationBindingArtifactLookup Create(string targetEnvironment, string environmentSettingRootPath)
		{
			if (environmentSettingRootPath.IsNullOrEmpty()) throw new ArgumentNullException(nameof(environmentSettingRootPath));
			BindingGenerationContext.EnvironmentSettingRootPath = environmentSettingRootPath;
			return Create(targetEnvironment);
		}

		private static void SettleApplicationBindingForBindingGenerationContext(T applicationBinding)
		{
			// ensure application bindings are settled for target environment before visit
			applicationBinding.Accept(new EnvironmentOverrideApplicationVisitor());
		}

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly Dictionary<string, IApplicationBindingArtifactLookup> _cache = new Dictionary<string, IApplicationBindingArtifactLookup>();
	}
}
