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

using System.IO;
using Be.Stateless.BizTalk.Dummies.Bindings;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Install.Command
{
	public class ApplicationBindingBasedCommandFixture
	{
		[Fact]
		public void DefaultAssemblyProbingFolderPaths()
		{
			var commandMock = new Mock<ApplicationBindingCommand<TestApplication>> { CallBase = true };

			commandMock.Object.AssemblyProbingFolderPaths.Should().BeEquivalentTo(TestApplicationBindingAssemblyFolderPath);
		}

		[Fact]
		public void DefinedAssemblyProbingFolderPaths()
		{
			var commandMock = new Mock<ApplicationBindingCommand<TestApplication>> { CallBase = true };

			commandMock.Object.AssemblyProbingFolderPaths = new[] { "folder1", "folder2" };
			commandMock.Object.AssemblyProbingFolderPaths.Should().BeEquivalentTo("folder1", "folder2", TestApplicationBindingAssemblyFolderPath);

			commandMock.Object.AssemblyProbingFolderPaths = null;
			commandMock.Object.AssemblyProbingFolderPaths.Should().BeEquivalentTo(TestApplicationBindingAssemblyFolderPath);
		}

		private string TestApplicationBindingAssemblyFolderPath => Path.GetDirectoryName(typeof(TestApplication).Assembly.Location);
	}
}
