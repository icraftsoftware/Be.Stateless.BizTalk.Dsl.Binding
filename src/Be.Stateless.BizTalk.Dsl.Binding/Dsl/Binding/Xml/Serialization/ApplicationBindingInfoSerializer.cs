#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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
using System.IO;
using System.Text;
using System.Xml;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;
using Be.Stateless.Xml.Serialization;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization
{
	public class ApplicationBindingInfoSerializer : IDslSerializer
	{
		public ApplicationBindingInfoSerializer(IVisitable<IApplicationBindingVisitor> applicationBinding)
		{
			_applicationBinding = applicationBinding ?? throw new ArgumentNullException(nameof(applicationBinding));
		}

		#region IDslSerializer Members

		public string Serialize()
		{
			using (var stringWriter = new StringWriter())
			using (var xmlWriter = XmlWriter.Create(stringWriter, XmlWriterSettings))
			{
				Serialize(xmlWriter);
				return stringWriter.ToString();
			}
		}

		public void Save(string filePath)
		{
			using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				Write(file);
			}
		}

		public void Write(System.IO.Stream stream)
		{
			using (var xmlWriter = XmlWriter.Create(stream, XmlWriterSettings))
			{
				Serialize(xmlWriter);
			}
		}

		#endregion

		private XmlWriterSettings XmlWriterSettings => new() { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };

		private void Serialize(XmlWriter xmlWriter)
		{
			CachingXmlSerializerFactory.Create(typeof(BindingInfo)).Serialize(xmlWriter, _applicationBinding.Accept(new BindingInfoBuilder()).BindingInfo);
		}

		private readonly IVisitable<IApplicationBindingVisitor> _applicationBinding;
	}
}
