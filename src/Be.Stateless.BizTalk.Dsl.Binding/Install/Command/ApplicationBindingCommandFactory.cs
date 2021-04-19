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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Extensions;

namespace Be.Stateless.BizTalk.Install.Command
{
	public static class ApplicationBindingCommandFactory
	{
		public static IApplicationBindingGenerationCommand CreateApplicationBindingGenerationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationBindingGenerationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationBindingGenerationCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static IApplicationBindingCommand CreateApplicationBindingValidationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationBindingValidationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationBindingCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static IApplicationFileAdapterFolderSetupCommand CreateApplicationFileAdapterFolderSetupCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationFileAdapterFolderSetupCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationFileAdapterFolderSetupCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static IApplicationBindingCommand CreateApplicationFileAdapterFolderTeardownCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationFileAdapterFolderTeardownCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationBindingCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static IApplicationHostEnumerationCommand CreateApplicationHostEnumerationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationHostEnumerationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationHostEnumerationCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static IApplicationBindingCommand CreateApplicationStateInitializationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationStateInitializationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationBindingCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		public static IApplicationBindingCommand CreateApplicationStateValidationCommand(Type applicationBindingType)
		{
			applicationBindingType.ValidateApplicationBindingType();
			var cmdType = typeof(ApplicationStateValidationCommand<>).MakeGenericType(applicationBindingType);
			var cmd = (IApplicationBindingCommand) Activator.CreateInstance(cmdType);
			return cmd;
		}

		private static void ValidateApplicationBindingType(this Type applicationBindingType)
		{
			if (applicationBindingType == null) throw new ArgumentNullException(nameof(applicationBindingType));
			if (!applicationBindingType.IsApplicationBindingType())
				throw new InvalidOperationException($"Type '{applicationBindingType.Name}' does not derive from {nameof(IApplicationBinding)}.'.");
		}
	}
}
