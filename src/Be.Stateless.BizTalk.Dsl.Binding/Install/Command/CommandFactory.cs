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
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Extensions;

namespace Be.Stateless.BizTalk.Install.Command
{
	public static class CommandFactory
	{
		public static ICommand<ISupplyApplicationBindingGenerationCommandArguments> CreateApplicationBindingGenerationCommand<T>()
			where T : class, IVisitable<IApplicationBindingVisitor>, new()
		{
			return new ApplicationBindingGenerationCommand<T>();
		}

		public static ICommand<ISupplyApplicationBindingGenerationCommandArguments> CreateApplicationBindingGenerationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationBindingGenerationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationBindingGenerationCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static ICommand<ISupplyApplicationBindingBasedCommandArguments> CreateApplicationBindingValidationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationBindingValidationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationBindingBasedCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static ICommand<ISupplyApplicationFileAdapterFolderSetupCommandArguments> CreateApplicationFileAdapterFolderSetupCommand<T>()
			where T : class, IVisitable<IApplicationBindingVisitor>, new()
		{
			return new ApplicationFileAdapterFolderSetupCommand<T>();
		}

		public static ICommand<ISupplyApplicationFileAdapterFolderSetupCommandArguments> CreateApplicationFileAdapterFolderSetupCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationFileAdapterFolderSetupCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationFileAdapterFolderSetupCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static ICommand<ISupplyApplicationFileAdapterFolderTeardownCommandArguments> CreateApplicationFileAdapterFolderTeardownCommand<T>()
			where T : class, IVisitable<IApplicationBindingVisitor>, new()
		{
			return new ApplicationFileAdapterFolderTeardownCommand<T>();
		}

		public static ICommand<ISupplyApplicationFileAdapterFolderTeardownCommandArguments> CreateApplicationFileAdapterFolderTeardownCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationFileAdapterFolderTeardownCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationFileAdapterFolderTeardownCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static ICommand<ISupplyApplicationBindingBasedCommandArguments> CreateApplicationHostEnumerationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationHostEnumerationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationBindingBasedCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static ICommand<ISupplyApplicationBindingBasedCommandArguments> CreateApplicationStateInitializationCommand<T>()
			where T : class, IVisitable<IApplicationBindingVisitor>, new()
		{
			return new ApplicationStateInitializationCommand<T>();
		}

		public static ICommand<ISupplyApplicationBindingBasedCommandArguments> CreateApplicationStateInitializationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationStateInitializationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationBindingBasedCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static ICommand<ISupplyApplicationBindingBasedCommandArguments> CreateApplicationStateValidationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationStateValidationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (ICommand<ISupplyApplicationBindingBasedCommandArguments>) Activator.CreateInstance(cmdType);
			return cmd;
		}
	}
}
