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

using System;
using System.ServiceModel;
using System.Text;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfWSHttpAdapterOutboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wha = new WcfWSHttpAdapter.Outbound(
				a => {
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("service_spn");
					a.SecurityMode = SecurityMode.Message;
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.TextEncoding = Encoding.Unicode;
				});
			var xml = wha.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<MaxReceivedMessageSize vt=\"3\">65535</MaxReceivedMessageSize>" +
				"<MessageEncoding vt=\"8\">Text</MessageEncoding>" +
				"<TextEncoding vt=\"8\">utf-16</TextEncoding>" +
				"<EnableTransaction vt=\"11\">0</EnableTransaction>" +
				"<SecurityMode vt=\"8\">Message</SecurityMode>" +
				"<MessageClientCredentialType vt=\"8\">Windows</MessageClientCredentialType>" +
				"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
				"<NegotiateServiceCredential vt=\"11\">-1</NegotiateServiceCredential>" +
				"<EstablishSecurityContext vt=\"11\">-1</EstablishSecurityContext>" +
				"<TransportClientCredentialType vt=\"8\">Windows</TransportClientCredentialType>" +
				"<UseSSO vt=\"11\">0</UseSSO>" +
				"<ProxyToUse vt=\"8\">None</ProxyToUse>" +
				"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
				"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
				"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
				"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
				"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
				"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
				"<SendTimeout vt=\"8\">00:02:00</SendTimeout>" +
				"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
				"<Identity vt=\"8\">&lt;identity&gt;\r\n  &lt;servicePrincipalName value=\"service_spn\" /&gt;\r\n&lt;/identity&gt;</Identity>" +
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

			var wha = new WcfWSHttpAdapter.Outbound(
				a => {
					a.Address = new("http://localhost/dummy.svc");
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("service_spn");
					a.SecurityMode = SecurityMode.Message;
					a.TextEncoding = Encoding.Unicode;
				});

			Invoking(() => ((ISupportValidation) wha).Validate()).Should().NotThrow();
		}
	}
}
