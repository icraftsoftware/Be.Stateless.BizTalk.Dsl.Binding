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
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Install.Command;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install
{
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public DSL API.")]
	public abstract class ApplicationBindingInstaller<T> : Installer
		where T : class, IVisitable<IApplicationBindingVisitor>, new()
	{
		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "InvertIf")]
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			if (Context.Parameters.ContainsKey(nameof(IApplicationBindingGenerationCommand.OutputFilePath)))
			{
				var cmd = new ApplicationBindingGenerationCommand<T> {
					OutputFilePath = Context.Parameters[nameof(IApplicationBindingGenerationCommand.OutputFilePath)]
				};
				SetCommandCommonArguments(cmd);
				cmd.Execute(msg => Context.LogMessage(msg));
			}

			if (Context.Parameters.ContainsKey("SetupFileAdapterFolders"))
			{
				var cmd = new ApplicationFileAdapterFolderSetupCommand<T> {
					Users = Context.Parameters[nameof(IApplicationFileAdapterFolderSetupCommand.Users)].IfNotNullOrEmpty(u => u.Split(';', ','))
				};
				SetCommandCommonArguments(cmd);
				cmd.Execute(msg => Context.LogMessage(msg));
			}

			if (Context.Parameters.ContainsKey("InitializeServices"))
			{
				var cmd = new ApplicationInitializationCommand<T>();
				SetCommandCommonArguments(cmd);
				cmd.Execute(msg => Context.LogMessage(msg));
			}
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);

			if (Context.Parameters.ContainsKey("TeardownFileAdapterFolders"))
			{
				var cmd = new ApplicationFileAdapterFolderTeardownCommand<T> {
					Recurse = Context.Parameters.ContainsKey(nameof(ApplicationFileAdapterFolderTeardownCommand<T>.Recurse))
				};
				SetCommandCommonArguments(cmd);
				cmd.Execute(msg => Context.LogMessage(msg));
			}
		}

		#endregion

		private void SetCommandCommonArguments(IApplicationBindingCommand cmd)
		{
			cmd.EnvironmentSettingOverridesType = Context.Parameters[nameof(IApplicationBindingCommand.EnvironmentSettingOverridesType)]
				.IfNotNullOrEmpty(t => Type.GetType(t, true));
			cmd.AssemblyProbingFolderPaths = Context.Parameters[nameof(IApplicationBindingCommand.AssemblyProbingFolderPaths)]
				.IfNotNullOrEmpty(p => p.Split(';'));
			cmd.ExcelSettingOverridesFolderPath = Context.Parameters[nameof(IApplicationBindingCommand.ExcelSettingOverridesFolderPath)];
			cmd.TargetEnvironment = Context.Parameters[nameof(IApplicationBindingCommand.TargetEnvironment)];
		}
	}
}
