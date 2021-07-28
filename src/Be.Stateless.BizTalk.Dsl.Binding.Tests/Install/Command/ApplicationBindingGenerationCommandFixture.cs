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
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Install.Command
{
	public class ApplicationBindingGenerationCommandFixture : ISupplyApplicationBindingGenerationCommandArguments, IDisposable
	{
		#region Setup/Teardown

		public ApplicationBindingGenerationCommandFixture(ITestOutputHelper outputHelper)
		{
			_outputHelper = outputHelper;
		}

		public void Dispose()
		{
			File.Delete(OutputFilePath);
		}

		#endregion

		[SkippableFact]
		public void Execute()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Invoking(
					() => CommandFactory.CreateApplicationBindingGenerationCommand(typeof(TestApplication))
						.InitializeParameters(this)
						.Execute(_outputHelper.WriteLine)
				)
				.Should().NotThrow();
		}

		[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
		public Type EnvironmentSettingOverridesType { get; }

		[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
		public string ExcelSettingOverridesFolderPath { get; }

		public string TargetEnvironment => Be.Stateless.BizTalk.Install.TargetEnvironment.DEVELOPMENT;

		public string OutputFilePath => Path.GetTempFileName();

		private readonly ITestOutputHelper _outputHelper;
	}
}
