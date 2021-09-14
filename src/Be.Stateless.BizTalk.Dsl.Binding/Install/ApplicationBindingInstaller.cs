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
using Be.Stateless.BizTalk.Reflection;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install
{
	[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public DSL API.")]
	public abstract class ApplicationBindingInstaller<T> : Installer,
		ISupplyApplicationBindingBasedCommandArguments,
		ISupplyApplicationBindingGenerationCommandArguments,
		ISupplyApplicationFileAdapterFolderSetupCommandArguments,
		ISupplyApplicationFileAdapterFolderTeardownCommandArguments
		where T : class, IVisitable<IApplicationBindingVisitor>, new()
	{
		#region ISupplyApplicationBindingBasedCommandArguments Members

		public Type EnvironmentSettingOverridesType => Context.Parameters[$"{nameof(EnvironmentSettingOverridesType)}Name"].IfNotNullOrEmpty(t => Type.GetType(t, true));

		public string ExcelSettingOverridesFolderPath => Context.Parameters[nameof(ExcelSettingOverridesFolderPath)];

		public string TargetEnvironment => Context.Parameters[nameof(TargetEnvironment)];

		#endregion

		#region ISupplyApplicationBindingGenerationCommandArguments Members

		public string OutputFilePath => Context.Parameters[nameof(OutputFilePath)];

		#endregion

		#region ISupplyApplicationFileAdapterFolderSetupCommandArguments Members

		public string[] Users => Context.Parameters[nameof(ApplicationFileAdapterFolderSetupCommand<T>.Users)]?.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

		#endregion

		#region ISupplyApplicationFileAdapterFolderTeardownCommandArguments Members

		public bool Recurse => Context.Parameters.ContainsKey(nameof(Recurse));

		#endregion

		#region Base Class Member Overrides

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			using (new BizTalkAssemblyResolver(Context.LogMessage, true, AssemblyProbingFolderPaths))
			{
				ICommand cmd = Context.Parameters switch {
					var p when p.ContainsKey("CreateFileAdapterFolders") => CommandFactory.CreateApplicationFileAdapterFolderSetupCommand<T>().InitializeParameters(this),
					var p when p.ContainsKey("GenerateBindings") => CommandFactory.CreateApplicationBindingGenerationCommand<T>().InitializeParameters(this),
					var p when p.ContainsKey("InitializeApplication") => CommandFactory.CreateApplicationStateInitializationCommand<T>().InitializeParameters(this),
					_ => throw new InvalidOperationException("No operation has been provided.")
				};
				cmd.Execute(Context.LogMessage);
			}
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);

			using (new BizTalkAssemblyResolver(Context.LogMessage, true, AssemblyProbingFolderPaths))
			{
				ICommand cmd = Context.Parameters switch {
					var p when p.ContainsKey("DeleteFileAdapterFolders") => CommandFactory.CreateApplicationFileAdapterFolderTeardownCommand<T>().InitializeParameters(this),
					_ => throw new InvalidOperationException("No operation has been provided.")
				};
				cmd.Execute(Context.LogMessage);
			}
		}

		#endregion

		public string[] AssemblyProbingFolderPaths => Context.Parameters[nameof(AssemblyProbingFolderPaths)]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
	}
}
