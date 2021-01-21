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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings
{
	internal class ExcelEnvironmentSettingOverrides : IExcelEnvironmentSettingOverrides
	{
		internal ExcelEnvironmentSettingOverrides(string filePath)
		{
			if (filePath == null) throw new ArgumentNullException(nameof(filePath));
			using (var xmlReader = XmlReader.Create(new StreamReader(filePath), new XmlReaderSettings { XmlResolver = null, CloseInput = true }))
			{
				var xmlDocument = new XmlDocument();
				xmlDocument.Load(xmlReader);
				_nsm = xmlDocument.GetNamespaceManager();
				_nsm.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
				_data = xmlDocument.SelectSingleNode("/ss:Workbook/ss:Worksheet[@ss:Name='Settings']/ss:Table", _nsm);
			}
		}

		#region IExcelEnvironmentSettingOverrides Members

		T[] IExcelEnvironmentSettingOverrides.ValuesForProperty<T>(string propertyName, T[] defaultValues)
		{
			var values = ValuesForProperty(propertyName)
				.Select(v => (T) Convert.ChangeType(v, typeof(T), CultureInfo.InvariantCulture))
				.ToArray();
			return values.Any() ? values : defaultValues;
		}

		T?[] IExcelEnvironmentSettingOverrides.ValuesForProperty<T>(string propertyName, T?[] defaultValues)
		{
			var values = ValuesForProperty(propertyName)
				.Select(v => (T?) v.IfNotNull(v2 => Convert.ChangeType(v2, typeof(T), CultureInfo.InvariantCulture)))
				.ToArray();
			return values.Any() ? values : defaultValues;
		}

		#endregion

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		private IEnumerable<string> ValuesForProperty(string propertyName)
		{
			var values = _data
				.SelectNodes($"ss:Row[ss:Cell[1]/ss:Data[@ss:Type='String']/text()='{propertyName}']/ss:Cell[position() > 1]", _nsm)
				.Cast<XmlNode>()
				.Select(cell => cell.SelectSingleNode("ss:Data/text()", _nsm)?.Value)
				.ToArray();
			return values;
		}

		private readonly XmlNode _data;
		private readonly XmlNamespaceManager _nsm;
	}
}
