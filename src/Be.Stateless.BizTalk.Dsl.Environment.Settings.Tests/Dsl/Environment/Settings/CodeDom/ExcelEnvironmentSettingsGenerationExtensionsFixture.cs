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

using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Be.Stateless.Resources;
using FluentAssertions;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.CodeDom
{
	public class ExcelEnvironmentSettingsGenerationExtensionsFixture
	{
		[Fact]
		public void CompileToDynamicAssembly()
		{
			var xmlDocument = ResourceManager.Load(
				Assembly.GetExecutingAssembly(),
				"Be.Stateless.BizTalk.Resources.Settings.BizTalk.Factory.Settings.xml",
				stream => {
					var document = new XmlDocument();
					document.Load(stream);
					return document;
				});

			// Roslyn compiler is required because the code generated features C# expression-bodied property members which is not supported by native CSharpCodeProvider
			var providerOptions = new ProviderOptions(@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\bin\Roslyn\csc.exe", 0);
			using (var provider = new CSharpCodeProvider(providerOptions))
			{
				var parameters = new CompilerParameters {
					GenerateInMemory = true,
					IncludeDebugInformation = true
				};
				// System.dll
				parameters.ReferencedAssemblies.Add(typeof(GeneratedCodeAttribute).Assembly.Location);
				// Be.Stateless.BizTalk.Dsl.Environment.Settings.dll
				parameters.ReferencedAssemblies.Add(typeof(ExcelEnvironmentSettings).Assembly.Location);

				const string @namespace = "Be.Stateless.BizTalk.Factory";
				const string typeName = "BizTalkFactorySettings";

				var compileUnit = xmlDocument.ConvertToEnvironmentSettingsCodeCompileUnit(@namespace, typeName, "BizTalk.Factory.Settings.xml");
				var results = provider.CompileAssemblyFromDom(parameters, compileUnit);
				if (results.Errors.Count > 0) throw new(results.Errors.Cast<CompilerError>().Aggregate(string.Empty, (k, e) => $"{k}\r\n{e}"));

				var assembly = results.CompiledAssembly;
				var settings = assembly.CreateInstance($"{@namespace}.{typeName}");
				settings.Should().NotBeNull().And.BeAssignableTo<ExcelEnvironmentSettings>();
			}
		}

		[Fact]
		public void ConvertToEnvironmentSettingsCodeCompileUnit()
		{
			var xmlDocument = ResourceManager.Load(
				Assembly.GetExecutingAssembly(),
				"Be.Stateless.BizTalk.Resources.Settings.BizTalk.Factory.Settings.xml",
				stream => {
					var document = new XmlDocument();
					document.Load(stream);
					return document;
				});

			var builder = new StringBuilder();
			using (var provider = new CSharpCodeProvider())
			using (var writer = new StringWriter(builder))
			{
				provider.GenerateCodeFromCompileUnit(
					xmlDocument.ConvertToEnvironmentSettingsCodeCompileUnit("Be.Stateless.BizTalk.Factory", "BizTalkFactorySettings", "BizTalk.Factory.Settings.xml"),
					writer,
					new() { BracingStyle = "C", IndentString = "\t", VerbatimOrder = true });
			}

			builder.ToString()
				.Should().Be(
					ResourceManager.Load(
						Assembly.GetExecutingAssembly(),
						"Be.Stateless.BizTalk.Resources.Settings.BizTalk.Factory.Settings.Designer.cs",
						s => new StreamReader(s).ReadToEnd()));
		}
	}
}
