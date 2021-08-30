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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfNetTcpRelayAdapterInboundFixture
	{
		[SkippableFact]
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wnt = new WcfNetTcpRelayAdapter.Inbound(
				a => {
					a.Address = new("sb://biztalk.factory.servicebus.windows.net/batch-queue");
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.MaxConcurrentCalls = 201;
					a.MaxReceivedMessageSize = 64512;

					a.SuspendRequestMessageOnFailure = true;
					a.IncludeExceptionDetailInFaults = true;

					a.StsUri = new("https://biztalk.factory-sb.accesscontrol.windows.net/");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "issuer_secret";

					a.EnableServiceDiscovery = true;
					a.ServiceDisplayName = "display_name";
				});
			var xml = wnt.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
					"<CustomProps>" +
					"<MaxReceivedMessageSize vt=\"3\">64512</MaxReceivedMessageSize>" +
					"<SecurityMode vt=\"8\">Transport</SecurityMode>" +
					"<MessageClientCredentialType vt=\"8\">Windows</MessageClientCredentialType>" +
					"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
					"<TransportProtectionLevel vt=\"8\">EncryptAndSign</TransportProtectionLevel>" +
					"<RelayClientAuthenticationType vt=\"8\">RelayAccessToken</RelayClientAuthenticationType>" +
					"<LeaseTimeout vt=\"8\">00:00:00</LeaseTimeout>" +
					"<UseSSO vt=\"11\">0</UseSSO>" +
					"<MaxConcurrentCalls vt=\"3\">201</MaxConcurrentCalls>" +
					"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
					"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
					"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
					"<OutboundXmlTemplate vt=\"8\">" + (
						"&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;") +
					"</OutboundXmlTemplate>" +
					"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
					"<IncludeExceptionDetailInFaults vt=\"11\">-1</IncludeExceptionDetailInFaults>" +
					"<StsUri vt=\"8\">https://biztalk.factory-sb.accesscontrol.windows.net/</StsUri>" +
					"<IssuerName vt=\"8\">issuer_name</IssuerName>" +
					"<IssuerSecret vt=\"8\">issuer_secret</IssuerSecret>" +
					"<UseAcsAuthentication vt=\"11\">0</UseAcsAuthentication>" +
					"<UseSasAuthentication vt=\"11\">0</UseSasAuthentication>" +
					"<EnableServiceDiscovery vt=\"11\">-1</EnableServiceDiscovery>" +
					"<DiscoveryMode vt=\"8\">Public</DiscoveryMode>" +
					"<ServiceDisplayName vt=\"8\">display_name</ServiceDisplayName>" +
					"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
					"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
					"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
					"<Identity vt=\"8\">" + (
						"&lt;identity&gt;\r\n  &lt;servicePrincipalName value=\"spn_name\" /&gt;\r\n&lt;/identity&gt;") +
					"</Identity>" +
					"</CustomProps>")
				;
		}

		[SkippableFact]
		public void Validate()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wnt = new WcfNetTcpRelayAdapter.Inbound(
				a => { a.Address = new("https://biztalk.factory.servicebus.windows.net/batch-queue"); });
			Invoking(() => ((ISupportValidation) wnt).Validate())
				.Should().Throw<ArgumentException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("Invalid address scheme; expecting \"sb\" scheme.*");
		}

		[SkippableFact]
		public void ValidateDoesNotThrow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wnt = new WcfNetTcpRelayAdapter.Inbound(
				a => {
					a.Address = new("sb://biztalk.factory.servicebus.windows.net/batch-queue");
					a.Identity = EndpointIdentityFactory.CreateSpnIdentity("spn_name");
					a.MaxConcurrentCalls = 201;
					a.MaxReceivedMessageSize = 64512;

					a.SuspendRequestMessageOnFailure = true;
					a.IncludeExceptionDetailInFaults = true;

					a.StsUri = new("https://biztalk.factory-sb.accesscontrol.windows.net/");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "issuer_secret";

					a.EnableServiceDiscovery = true;
					a.ServiceDisplayName = "display_name";
				});

			Invoking(() => ((ISupportValidation) wnt).Validate()).Should().NotThrow();
		}
	}
}
