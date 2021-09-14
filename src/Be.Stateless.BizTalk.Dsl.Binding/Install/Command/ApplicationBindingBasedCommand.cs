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
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Install.Command
{
	public abstract class ApplicationBindingBasedCommand<TB> : ApplicationBindingBasedCommand<TB, ISupplyApplicationBindingBasedCommandArguments>
		where TB : class, IVisitable<IApplicationBindingVisitor>, new()
	{
		#region Base Class Member Overrides

		protected sealed override void InitializeParameters(ISupplyApplicationBindingBasedCommandArguments arguments) { }

		protected sealed override void ValidateParameters() { }

		#endregion
	}

	public abstract class ApplicationBindingBasedCommand<TB, TA> : ICommand<TA>
		where TB : class, IVisitable<IApplicationBindingVisitor>, new()
		where TA : class, ISupplyApplicationBindingBasedCommandArguments
	{
		protected ApplicationBindingBasedCommand()
		{
			_lazyApplicationBindingFactory = new(() => new TB());
		}

		#region ICommand<TA> Members

		ICommand<TA> ICommand<TA>.InitializeParameters(TA arguments)
		{
			EnvironmentSettingOverridesType = arguments.EnvironmentSettingOverridesType;
			ExcelSettingOverridesFolderPath = arguments.ExcelSettingOverridesFolderPath;
			TargetEnvironment = arguments.TargetEnvironment;

			InitializeParameters(arguments);

			return this;
		}

		[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
		ICommand ICommand.Execute(Action<string> logAppender)
		{
			if (logAppender == null) throw new ArgumentNullException(nameof(logAppender));

			if (TargetEnvironment.IsNullOrEmpty()) throw new ArgumentNullException(nameof(TargetEnvironment));

			ValidateParameters();

			DeploymentContext.EnvironmentSettingOverridesType = EnvironmentSettingOverridesType;
			DeploymentContext.ExcelSettingOverridesFolderPath = ExcelSettingOverridesFolderPath;
			DeploymentContext.TargetEnvironment = TargetEnvironment;

			Execute(logAppender);

			return this;
		}

		#endregion

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public Type EnvironmentSettingOverridesType { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
		public string ExcelSettingOverridesFolderPath { get; set; }

		public string TargetEnvironment { get; set; }

		protected IVisitable<IApplicationBindingVisitor> ApplicationBinding => _lazyApplicationBindingFactory.Value;

		protected abstract void Execute(Action<string> logAppender);

		protected abstract void InitializeParameters(TA arguments);

		protected abstract void ValidateParameters();

		private readonly Lazy<IVisitable<IApplicationBindingVisitor>> _lazyApplicationBindingFactory;
	}
}
