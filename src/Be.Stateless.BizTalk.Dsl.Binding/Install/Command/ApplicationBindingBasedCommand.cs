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
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install.Command
{
	public abstract class ApplicationBindingBasedCommand
	{
		public IVisitable<IApplicationBindingVisitor> ApplicationBinding { get; set; }

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
		public string[] AssemblyProbingPaths { get; set; }

		public string EnvironmentSettingRootPath { get; set; }

		public string TargetEnvironment { get; set; }

		public void Execute(Action<string> logAppender)
		{
			if (ApplicationBinding == null) throw new InvalidOperationException($"{nameof(ApplicationBinding)} is null.");
			if (TargetEnvironment.IsNullOrEmpty()) throw new InvalidOperationException($"{nameof(TargetEnvironment)} has not been set.");
			try
			{
				BizTalkAssemblyResolver.Register(logAppender, AssemblyProbingPaths);
				SetupBindingGenerationContext();
				ExecuteCore(logAppender);
			}
			finally
			{
				BizTalkAssemblyResolver.Unregister();
			}
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Global")]
		protected abstract void ExecuteCore(Action<string> logAppender);

		private void SetupBindingGenerationContext()
		{
			BindingGenerationContext.TargetEnvironment = TargetEnvironment;
			if (!EnvironmentSettingRootPath.IsNullOrEmpty()) BindingGenerationContext.EnvironmentSettingRootPath = EnvironmentSettingRootPath;
		}
	}
}
