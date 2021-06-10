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
using System.Linq;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dummies.Conventions;
using Be.Stateless.BizTalk.Dummies.MicroComponent;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.Finance.Income;
using Be.Stateless.Finance.Invoice;
using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class DummyHostResolutionPolicyFixture
	{
		[SkippableFact]
		public void HowToResolveHostNameAccordingToAdapter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocationMock = new Mock<ReceiveLocationBase<string>>(
				(Action<IReceiveLocation<string>>) (rl => {
					rl.Transport.Adapter = new WcfBasicHttpAdapter.Inbound();
					rl.Transport.Host = DummyHostResolutionPolicy.Default;
				})) {
				CallBase = true
			};
			receiveLocationMock.As<IReceiveLocation<string>>().Setup(rl => rl.ReceivePort).Returns(new Mock<ReceivePortBase<string>>().Object);
			// assume validity
			receiveLocationMock.As<ISupportValidation>().Setup(rl => rl.Validate());

			((ISupportHostNameResolution) receiveLocationMock.Object.Transport).ResolveHostName().Should().Be("BizTalkServerIsolatedHost");
		}

		[Fact]
		public void HowToResolveHostNameAccordingToOrchestration()
		{
			var orchestrationBinding = new Orchestrations.Bound.ProcessOrchestrationBinding {
				Host = DummyHostResolutionPolicy.Default
			};

			((ISupportHostNameResolution) orchestrationBinding).ResolveHostName().Should().Be("DummyHost");
		}

		[SkippableFact]
		public void HowToResolveHostNameAccordingToPipelineComponent()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sendPortMock = new Mock<SendPortBase<string>>(
				(Action<ISendPort<string>>) (sp => {
					sp.SendPipeline = new SendPipeline<XmlTransmit>(
						pl => pl.Encoder<MicroPipelineComponent>(c => { c.Components = new IMicroComponent[] { new MimeDecoder() }; })
					);
					sp.Transport.Host = DummyHostResolutionPolicy.Default;
				})) {
				CallBase = true
			};
			// assume validity
			sendPortMock.As<ISupportValidation>().Setup(rl => rl.Validate());

			((ISupportHostNameResolution) sendPortMock.Object.Transport).ResolveHostName().Should().Be("BizTalkServerApplication_x86");
		}

		[SkippableFact]
		public void HowToResolveHostNameAccordingToPortParty()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receiveLocation = new TaxAgencyReceivePort().ReceiveLocations.Single();

			((ISupportHostNameResolution) receiveLocation.Transport).ResolveHostName().Should().Be("TaxAgency_Host");
		}

		[SkippableFact]
		public void HowToResolveHostNameAccordingToPortSubject()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sendPort = new BankSendPort();

			((ISupportHostNameResolution) sendPort.Transport).ResolveHostName().Should().Be("Anything_Host");
		}
	}
}
