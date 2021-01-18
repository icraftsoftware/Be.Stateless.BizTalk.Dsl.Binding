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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Dummies.Transforms;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using BTS;
using FluentAssertions;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Utilities;
using Microsoft.XLANGs.BaseTypes;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization
{
	public class PipelineBindingInfoSerializerFixture
	{
		[SkippableFact]
		public void ReceivePipelineDslGrammarVariant1()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new ReceivePipeline<XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.Validate.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL first variant
			var pipeline1 = new ReceivePipeline<XmlReceive>(
				pl => {
					pl.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.Stages.Disassemble.Component<XmlDasmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						});
					pl.Stages.Validate.Component<MicroPipelineComponent>(mp => { mp.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding1 = pipeline1.GetPipelineBindingInfoSerializer().Serialize();

			binding1.Should().Be(binding);
		}

		[SkippableFact]
		public void ReceivePipelineDslGrammarVariant2()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new ReceivePipeline<XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.Validate.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL second variant
			var pipeline2 = ReceivePipeline<XmlReceive>.Configure(
				pl => pl
					.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.Disassembler<XmlDasmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						})
					.Validator<MicroPipelineComponent>(mp => { mp.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; }));
			var binding2 = pipeline2.GetPipelineBindingInfoSerializer().Serialize();

			binding2.Should().Be(binding);
		}

		[SkippableFact]
		public void ReceivePipelineDslGrammarVariant3()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new ReceivePipeline<XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.Validate.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL third variant
			var pipeline3 = ReceivePipeline<XmlReceive>.Configure(
				pl => pl
					.FirstDecoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.FirstDisassembler<XmlDasmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						})
					.FirstValidator<MicroPipelineComponent>(mp => { mp.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; }));
			var binding3 = pipeline3.GetPipelineBindingInfoSerializer().Serialize();

			binding3.Should().Be(binding);
		}

		[SkippableFact]
		public void ReceivePipelineDslGrammarVariant4()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new ReceivePipeline<XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Decode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.Validate.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL fourth variant
			var pipeline4 = ReceivePipeline<XmlReceive>.Configure(
				pl => {
					pl.Stages.Decode.Components
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<MicroPipelineComponent>(1)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
					pl.Stages.Disassemble.Components.ComponentAt<XmlDasmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						});
					pl.Stages.Validate.Components.ComponentAt<MicroPipelineComponent>(0).Configure(
						mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding4 = pipeline4.GetPipelineBindingInfoSerializer().Serialize();

			binding4.Should().Be(binding);
		}

		[SkippableFact]
		public void ReceivePipelineDslGrammarVariant5()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new ReceivePipeline<XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Decode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.Validate.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL fifth variant
			var pipeline5 = ReceivePipeline<XmlReceive>.Configure(
				pl => {
					pl.Stages.Decode
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<MicroPipelineComponent>(1)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
					pl.Stages.Disassemble.ComponentAt<XmlDasmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						});
					pl.Stages.Validate.ComponentAt<MicroPipelineComponent>(0)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding5 = pipeline5.GetPipelineBindingInfoSerializer().Serialize();

			binding5.Should().Be(binding);
		}

		[SkippableFact]
		public void ReceivePipelineDslGrammarVariant6()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new ReceivePipeline<XmlReceive>();
			pipeline.Stages.Decode.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Decode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Disassemble.Component<XmlDasmComp>().RecoverableInterchangeProcessing = true;
			pipeline.Stages.Validate.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL sixth variant
			var pipeline6 = ReceivePipeline<XmlReceive>.Configure(
				pl => pl
					.DecoderAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
					.DecoderAt<MicroPipelineComponent>(1).Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; })
					.DisassemblerAt<XmlDasmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.RecoverableInterchangeProcessing = true;
						})
					.ValidatorAt<MicroPipelineComponent>(0)
					.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; }));
			var binding6 = pipeline6.GetPipelineBindingInfoSerializer().Serialize();

			binding6.Should().Be(binding);
		}

		[SkippableFact]
		public void ReceivePipelineSerializationIsEmptyWhenDefaultPipelineConfigIsNotOverridden()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var pipeline = new ReceivePipeline<Microsoft.BizTalk.DefaultPipelines.XMLReceive>();
			var pipelineBindingSerializer = pipeline.GetPipelineBindingInfoSerializer();

			var binding = pipelineBindingSerializer.Serialize();

			binding.Should().BeEmpty();
		}

		[SkippableFact]
		public void ReceivePipelineSerializationKeepsOnlyStagesWhoseComponentsDefaultConfigHasBeenOverridden()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var pipeline = new ReceivePipeline<Microsoft.BizTalk.DefaultPipelines.XMLReceive>(
				pl => pl.Disassembler<XmlDasmComp>(c => { c.RecoverableInterchangeProcessing = true; }));
			var pipelineBindingSerializer = pipeline.GetPipelineBindingInfoSerializer();

			var binding = pipelineBindingSerializer.Serialize();

			binding.Should().Be(
				"<Root><Stages>" +
				"<Stage CategoryId=\"9d0e4105-4cce-4536-83fa-4a5040674ad6\">" +
				"<Components>" +
				$"<Component Name=\"{typeof(XmlDasmComp).FullName}\">" +
				"<Properties><RecoverableInterchangeProcessing vt=\"11\">-1</RecoverableInterchangeProcessing></Properties>" +
				"</Component>" +
				"</Components>" +
				"</Stage>" +
				"</Stages></Root>");
		}

		[SkippableFact]
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		public void ReceivePipelineSerializationKeepsOnlyStagesWhoseComponentsDefaultConfigHasBeenOverridden2()

		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var pipeline = ReceivePipeline<XmlReceive>.Configure(
				pl => pl
					.FirstDecoder<FailedMessageRoutingEnablerComponent>(
						c => {
							c.Enabled = false;
							c.SuppressRoutingFailureReport = false;
						})
					.FirstDecoder<MicroPipelineComponent>(
						c => {
							c.Enabled = true;
							c.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
						}));

			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			binding.Should().Be(
				"<Root><Stages>" +
				"<Stage CategoryId=\"9d0e4103-4cce-4536-83fa-4a5040674ad6\">" +
				"<Components>" +
				$"<Component Name=\"{typeof(FailedMessageRoutingEnablerComponent).FullName}\">" +
				"<Properties>" +
				"<Enabled vt=\"11\">0</Enabled>" +
				"<SuppressRoutingFailureReport vt=\"11\">0</SuppressRoutingFailureReport>" +
				"</Properties>" +
				"</Component>" +
				$"<Component Name=\"{typeof(MicroPipelineComponent).FullName}\">" +
				"<Properties>" +
				"<Components vt=\"8\">" + (
					"&lt;mComponents&gt;" +
					$"&lt;mComponent name='{typeof(XsltRunner).AssemblyQualifiedName}'&gt;" + (
						"&lt;Encoding&gt;utf-8&lt;/Encoding&gt;" +
						$"&lt;Map&gt;{typeof(IdentityTransform).AssemblyQualifiedName}&lt;/Map&gt;") +
					"&lt;/mComponent&gt;" +
					"&lt;/mComponents&gt;") +
				"</Components>" +
				"</Properties>" +
				"</Component>" +
				"</Components>" +
				"</Stage>" +
				"</Stages></Root>"
			);
		}

		[SkippableFact]
		public void SendPipelineDslGrammarVariant1()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new SendPipeline<XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL first variant
			var pipeline1 = new SendPipeline<XmlTransmit>(
				pl => {
					pl.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.Stages.Assemble.Component<XmlAsmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Stages.Encode.Component<MicroPipelineComponent>(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding1 = pipeline1.GetPipelineBindingInfoSerializer().Serialize();

			binding1.Should().Be(binding);
		}

		[SkippableFact]
		public void SendPipelineDslGrammarVariant2()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new SendPipeline<XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL second variant
			var pipeline2 = new SendPipeline<XmlTransmit>(
				pl => {
					pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.Assembler<XmlAsmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Encoder<MicroPipelineComponent>(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding2 = pipeline2.GetPipelineBindingInfoSerializer().Serialize();

			binding2.Should().Be(binding);
		}

		[SkippableFact]
		public void SendPipelineDslGrammarVariant3()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new SendPipeline<XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL third variant
			var pipeline3 = new SendPipeline<XmlTransmit>(
				pl => {
					pl.FirstPreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; });
					pl.FirstAssembler<XmlAsmComp>(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.FirstEncoder<MicroPipelineComponent>(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding3 = pipeline3.GetPipelineBindingInfoSerializer().Serialize();

			binding3.Should().Be(binding);
		}

		[SkippableFact]
		public void SendPipelineDslGrammarVariant4()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new SendPipeline<XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.PreAssemble.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL fourth variant
			var pipeline4 = new SendPipeline<XmlTransmit>(
				pl => {
					pl.Stages.PreAssemble.Components
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<MicroPipelineComponent>(1)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
					pl.Stages.Assemble.Components.ComponentAt<XmlAsmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Stages.Encode.Components.ComponentAt<MicroPipelineComponent>(0)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding4 = pipeline4.GetPipelineBindingInfoSerializer().Serialize();

			binding4.Should().Be(binding);
		}

		[SkippableFact]
		public void SendPipelineDslGrammarVariant5()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new SendPipeline<XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.PreAssemble.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL fifth variant
			var pipeline5 = new SendPipeline<XmlTransmit>(
				pl => {
					pl.Stages.PreAssemble
						.ComponentAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
						.ComponentAt<MicroPipelineComponent>(1)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
					pl.Stages.Assemble.ComponentAt<XmlAsmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						});
					pl.Stages.Encode.ComponentAt<MicroPipelineComponent>(0)
						.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; });
				});
			var binding5 = pipeline5.GetPipelineBindingInfoSerializer().Serialize();

			binding5.Should().Be(binding);
		}

		[SkippableFact]
		public void SendPipelineDslGrammarVariant6()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			// not fluent-DSL
			var pipeline = new SendPipeline<XmlTransmit>();
			pipeline.Stages.PreAssemble.Component<FailedMessageRoutingEnablerComponent>().Enabled = false;
			pipeline.Stages.PreAssemble.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			pipeline.Stages.Assemble.Component<XmlAsmComp>().DocumentSpecNames = new SchemaList {
				new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
				new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
			};
			pipeline.Stages.Assemble.Component<XmlAsmComp>().AddXMLDeclaration = true;
			pipeline.Stages.Encode.Component<MicroPipelineComponent>().Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			// fluent-DSL sixth variant
			var pipeline6 = new SendPipeline<XmlTransmit>(
				pl => pl
					.PreAssemblerAt<FailedMessageRoutingEnablerComponent>(0).Configure(c => { c.Enabled = false; })
					.PreAssemblerAt<MicroPipelineComponent>(1)
					.Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; })
					.AssemblerAt<XmlAsmComp>(0).Configure(
						c => {
							c.DocumentSpecNames = new SchemaList {
								new SchemaWithNone(Schema<Any>.AssemblyQualifiedName),
								new SchemaWithNone(Schema<soap_envelope_1__2.Envelope>.AssemblyQualifiedName)
							};
							c.AddXMLDeclaration = true;
						})
					.EncoderAt<MicroPipelineComponent>(0).Configure(mpc => { mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } }; }));
			var binding6 = pipeline6.GetPipelineBindingInfoSerializer().Serialize();

			binding6.Should().Be(binding);
		}

		[SkippableFact]
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		public void SendPipelineSerializationKeepsOnlyStagesWhoseComponentsDefaultConfigHasBeenOverridden2()

		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var pipeline = SendPipeline<XmlTransmit>.Configure(
				pl => pl
					.FirstPreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; })
					.FirstPreAssembler<MicroPipelineComponent>(
						mpc => {
							mpc.Enabled = true;
							mpc.Components = new IMicroComponent[] { new XsltRunner { MapType = typeof(IdentityTransform) } };
						}));

			var binding = pipeline.GetPipelineBindingInfoSerializer().Serialize();

			binding.Should().Be(
				"<Root><Stages>" +
				"<Stage CategoryId=\"9d0e4101-4cce-4536-83fa-4a5040674ad6\">" +
				"<Components>" +
				$"<Component Name=\"{typeof(FailedMessageRoutingEnablerComponent).FullName}\">" +
				"<Properties>" +
				"<Enabled vt=\"11\">0</Enabled>" +
				"</Properties>" +
				"</Component>" +
				$"<Component Name=\"{typeof(MicroPipelineComponent).FullName}\">" +
				"<Properties>" +
				"<Components vt=\"8\">" + (
					"&lt;mComponents&gt;" +
					$"&lt;mComponent name='{typeof(XsltRunner).AssemblyQualifiedName}'&gt;" + (
						"&lt;Encoding&gt;utf-8&lt;/Encoding&gt;" +
						$"&lt;Map&gt;{typeof(IdentityTransform).AssemblyQualifiedName}&lt;/Map&gt;") +
					"&lt;/mComponent&gt;" +
					"&lt;/mComponents&gt;") +
				"</Components>" +
				"</Properties>" +
				"</Component>" +
				"</Components>" +
				"</Stage>" +
				"</Stages></Root>");
		}
	}
}
