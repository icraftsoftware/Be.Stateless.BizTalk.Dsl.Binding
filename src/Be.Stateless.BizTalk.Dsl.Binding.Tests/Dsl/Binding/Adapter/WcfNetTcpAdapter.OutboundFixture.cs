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
using System.ServiceModel;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfNetTcpAdapterOutboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var nta = new WcfNetTcpAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("net.tcp://localhost/biztalk.factory/service.svc");
					a.SecurityMode = SecurityMode.Message;
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.MessageClientCredentialType = MessageCredentialType.Windows;
				});
			var xml = nta.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<MaxReceivedMessageSize vt=\"3\">65535</MaxReceivedMessageSize>" +
				"<EnableTransaction vt=\"11\">0</EnableTransaction>" +
				"<TransactionProtocol vt=\"8\">OleTransactions</TransactionProtocol>" +
				"<SecurityMode vt=\"8\">Message</SecurityMode>" +
				"<MessageClientCredentialType vt=\"8\">Windows</MessageClientCredentialType>" +
				"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
				"<TransportClientCredentialType vt=\"8\">Windows</TransportClientCredentialType>" +
				"<TransportProtectionLevel vt=\"8\">EncryptAndSign</TransportProtectionLevel>" +
				"<UseSSO vt=\"11\">0</UseSSO>" +
				"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
				"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
				"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
				"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
				"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
				"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
				"<SendTimeout vt=\"8\">00:02:00</SendTimeout>" +
				"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
				"</CustomProps>");
		}

		[Fact(Skip = "TODO")]
		public void Validate()
		{
			// TODO Validate()
		}

		[SkippableFact]
		public void ValidateDoesNotThrow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var nta = new WcfNetTcpAdapter.Outbound(
				a => {
					a.Address = new EndpointAddress("net.tcp://localhost/biztalk.factory/service.svc");
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("service_spn");
					a.SecurityMode = SecurityMode.Message;
					a.MessageClientCredentialType = MessageCredentialType.Windows;
				});

			Invoking(() => ((ISupportValidation) nta).Validate()).Should().NotThrow();
		}
	}
}
