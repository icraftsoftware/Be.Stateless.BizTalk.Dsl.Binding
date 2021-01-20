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
using System.IO;
using System.Linq;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install.Command
{
	public abstract class ApplicationBindingBasedCommand<T>
		where T : class, IVisitable<IApplicationBindingVisitor>, new()
	{
		protected ApplicationBindingBasedCommand()
		{
			_lazyApplicationBindingFactory = new Lazy<IVisitable<IApplicationBindingVisitor>>(() => new T());
		}

		public IVisitable<IApplicationBindingVisitor> ApplicationBinding => _lazyApplicationBindingFactory.Value ?? throw new ArgumentNullException(nameof(ApplicationBinding));

		public string[] AssemblyProbingFolderPaths
		{
			get => _assemblyProbingFolderPaths ?? new[] { BindingAssemblyFolderPath };
			set => _assemblyProbingFolderPaths = value.IfNotNull(v => v.Append(BindingAssemblyFolderPath).ToArray()) ?? new[] { BindingAssemblyFolderPath };
		}

		public Type EnvironmentSettingOverridesType { get; set; }

		public string ExcelSettingOverridesFolderPath { get; set; }

		public string TargetEnvironment { get; set; }

		private string BindingAssemblyFolderPath => Path.GetDirectoryName(typeof(T).Assembly.Location);

		public void Execute(Action<string> logAppender)
		{
			if (TargetEnvironment.IsNullOrEmpty()) throw new InvalidOperationException($"{nameof(TargetEnvironment)} has not been set.");
			try
			{
				BizTalkAssemblyResolver.Register(logAppender, AssemblyProbingFolderPaths);
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
			DeploymentContext.TargetEnvironment = TargetEnvironment;
			if (EnvironmentSettingOverridesType != null) DeploymentContext.EnvironmentSettingOverridesType = EnvironmentSettingOverridesType;
			if (!ExcelSettingOverridesFolderPath.IsNullOrEmpty()) DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;
		}

		private readonly Lazy<IVisitable<IApplicationBindingVisitor>> _lazyApplicationBindingFactory;
		private string[] _assemblyProbingFolderPaths;
	}
}
