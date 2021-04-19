﻿#region Copyright & License

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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Linq.Extensions;
using Be.Stateless.Xml.Serialization;
using Be.Stateless.Xml.Serialization.Extensions;
using Microsoft.BizTalk.PipelineEditor.PipelineFile;
using Stage = Microsoft.BizTalk.PipelineEditor.PipelineFile.Stage;

namespace Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization
{
	/// <summary>
	/// Provides a binding-compliant serialization of a <see cref="Pipeline"/>-derived pipeline.
	/// </summary>
	internal class PipelineBindingInfoSerializer : IDslSerializer
	{
		internal PipelineBindingInfoSerializer(IVisitable<IPipelineVisitor> pipeline)
		{
			_pipeline = pipeline;
		}

		#region IDslSerializer Members

		public string Serialize()
		{
			using (var stringWriter = new StringWriter())
			using (var writer = XmlWriter.Create(stringWriter, XmlWriterSettings))
			{
				Serialize(writer);
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
			using (var writer = XmlWriter.Create(stream, XmlWriterSettings))
			{
				Serialize(writer);
			}
		}

		#endregion

		private XmlWriterSettings XmlWriterSettings => new() { Indent = false, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };

		[SuppressMessage("ReSharper", "InvertIf")]
		private void Serialize(XmlWriter writer)
		{
			var document = CreatePipelineDocument();
			if (document != null)
			{
				var serializer = CreateXmlSerializer();
				var absorbXsdAndXsiXmlns = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
				serializer.Serialize(writer, document, absorbXsdAndXsiXmlns);
			}
		}

		private Document CreatePipelineDocument()
		{
			var visitor = new PipelineBindingDocumentBuilderVisitor();
			_pipeline.Accept(visitor);
			var document = visitor.Document;

			// compact binding, i.e. get rid of not-configured stages, i.e. stages without component or stages for which none
			// of its components' default configuration has been overridden
			document.Stages.Cast<Stage>()
				.Where(s => s.Components.Count == 0 || s.Components.Cast<ComponentBinding>().All(c => c.Properties.Count == 0))
				.ToArray()
				.ForEach(s => document.Stages.Remove(s));

			return document.Stages.Count == 0 ? null : document;
		}

		private XmlSerializer CreateXmlSerializer()
		{
			var overrides = new XmlAttributeOverrides();

			overrides.Add<Document>(new XmlAttributes { XmlType = new XmlTypeAttribute("Root") });
			overrides.Ignore<Document>(d => d.Description);
			overrides.Ignore<Document>(d => d.MajorVersion);
			overrides.Ignore<Document>(d => d.MinorVersion);
			overrides.Ignore<Document>(d => d.PolicyFilePath);

			overrides.Add<Stage>(s => s.Components, new XmlAttributes { XmlArrayItems = { new XmlArrayItemAttribute("Component", typeof(ComponentBinding)) } });

			overrides.Add<ComponentInfo>(ci => ci.QualifiedNameOrClassId, new XmlAttributes { XmlAttribute = new XmlAttributeAttribute("Name") });
			overrides.Ignore<ComponentInfo>(ci => ci.CachedDisplayName);
			overrides.Ignore<ComponentInfo>(ci => ci.CachedIsManaged);
			overrides.Ignore<ComponentInfo>(ci => ci.ComponentName);
			overrides.Ignore<ComponentInfo>(ci => ci.ComponentProperties);
			overrides.Ignore<ComponentInfo>(ci => ci.Description);
			overrides.Ignore<ComponentInfo>(ci => ci.Version);
			return CachingXmlSerializerFactory.Create(typeof(Document), overrides);
		}

		private readonly IVisitable<IPipelineVisitor> _pipeline;
	}
}
