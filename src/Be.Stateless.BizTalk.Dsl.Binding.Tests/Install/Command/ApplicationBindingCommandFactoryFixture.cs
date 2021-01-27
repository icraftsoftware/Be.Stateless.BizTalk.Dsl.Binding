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

using Be.Stateless.BizTalk.Dummies.Bindings;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Install.Command
{
	public class ApplicationBindingCommandFactoryFixture
	{
		[Fact]
		public void CreateApplicationBindingGenerationCommand()
		{
			ApplicationBindingCommandFactory.CreateApplicationBindingGenerationCommand(typeof(UnnamedApplication))
				.Should().NotBeNull()
				.And.BeAssignableTo<ApplicationBindingGenerationCommand<UnnamedApplication>>();
		}

		[Fact]
		public void CreateApplicationBindingValidationCommand()
		{
			ApplicationBindingCommandFactory.CreateApplicationBindingValidationCommand(typeof(UnnamedApplication))
				.Should().NotBeNull()
				.And.BeAssignableTo<ApplicationBindingValidationCommand<UnnamedApplication>>();
		}

		[Fact]
		public void CreateBizTalkApplicationInitializationCommand()
		{
			ApplicationBindingCommandFactory.CreateBizTalkApplicationInitializationCommand(typeof(UnnamedApplication))
				.Should().NotBeNull()
				.And.BeAssignableTo<ApplicationBindingCommand<UnnamedApplication>>();
		}

		[Fact]
		public void CreateFileAdapterFolderSetupCommand()
		{
			ApplicationBindingCommandFactory.CreateFileAdapterFolderSetupCommand(typeof(UnnamedApplication))
				.Should().NotBeNull()
				.And.BeAssignableTo<ApplicationBindingCommand<UnnamedApplication>>();
		}

		[Fact]
		public void CreateFileAdapterFolderTeardownCommand()
		{
			ApplicationBindingCommandFactory.CreateFileAdapterFolderTeardownCommand(typeof(UnnamedApplication))
				.Should().NotBeNull()
				.And.BeAssignableTo<ApplicationBindingCommand<UnnamedApplication>>();
		}
	}
}
