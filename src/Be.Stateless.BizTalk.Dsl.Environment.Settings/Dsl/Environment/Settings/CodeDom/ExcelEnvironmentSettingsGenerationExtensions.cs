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

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Be.Stateless.Resources;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.CodeDom
{
	public static class ExcelEnvironmentSettingsGenerationExtensions
	{
		#region Nested Type: Stringifier

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "XSLT extension object.")]
		private class Stringifier
		{
			public string Escape(string value)
			{
				// http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
				using (var writer = new StringWriter())
				using (var provider = CodeDomProvider.CreateProvider("CSharp"))
				{
					provider.GenerateCodeFromExpression(new CodePrimitiveExpression(value), writer, null);
					return writer.ToString();
				}
			}
		}

		#endregion

		#region Nested Type: Typifier

		[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "XSLT extension object.")]
		[SuppressMessage("ReSharper", "IdentifierTypo")]
		private class Typifier
		{
			public bool IsInteger(string value)
			{
				return int.TryParse(value, out _);
			}
		}

		#endregion

		static ExcelEnvironmentSettingsGenerationExtensions()
		{
			var type = typeof(ExcelEnvironmentSettingsGenerationExtensions);
			_xslt = ResourceManager.Load(
				type.Assembly,
				type.FullName + ".xslt",
				stream => {
					using (var xmlReader = XmlReader.Create(stream))
					{
						var xslt = new XslCompiledTransform(true);
						xslt.Load(xmlReader, XsltSettings.TrustedXslt, new XmlUrlResolver());
						return xslt;
					}
				});
		}

		public static CodeCompileUnit ConvertToEnvironmentSettingsCodeCompileUnit(
			this XmlDocument xmlDocument,
			string @namespace,
			string typeName,
			string excelEnvironmentSettingsFileName)
		{
			using (var writer = new StringWriter())
			{
				var arguments = new XsltArgumentList();
				arguments.AddExtensionObject("urn:extensions.stateless.be:biztalk:environment-setting-class-generation:stringifier:2021:01", new Stringifier());
				arguments.AddExtensionObject("urn:extensions.stateless.be:biztalk:environment-setting-class-generation:typifier:2021:01", new Typifier());
				arguments.AddParam("clr-namespace-name", string.Empty, @namespace);
				arguments.AddParam("clr-class-name", string.Empty, typeName);
				arguments.AddParam("settings-file-name", string.Empty, excelEnvironmentSettingsFileName);
				_xslt.Transform(xmlDocument, arguments, writer);
				writer.Flush();
				return new CodeSnippetCompileUnit(writer.ToString());
			}
		}

		private static readonly XslCompiledTransform _xslt;
	}
}
