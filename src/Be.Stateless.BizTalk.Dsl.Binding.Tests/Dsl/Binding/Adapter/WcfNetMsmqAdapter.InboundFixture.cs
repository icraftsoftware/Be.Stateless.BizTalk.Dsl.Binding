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

using System.ServiceModel;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfNetMsmqAdapterInboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var nma = new WcfNetMsmqAdapter.Inbound(
				a => {
					a.Address = new EndpointAddress("net.msmq://localhost/private/service_queue");
					a.SecurityMode = NetMsmqSecurityMode.Message;
					a.EnableTransaction = true;
					a.OrderedProcessing = true;
				});
			var xml = nma.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<MaxReceivedMessageSize vt=\"3\">65535</MaxReceivedMessageSize>" +
				"<EnableTransaction vt=\"11\">-1</EnableTransaction>" +
				"<OrderedProcessing vt=\"11\">-1</OrderedProcessing>" +
				"<SecurityMode vt=\"8\">Message</SecurityMode>" +
				"<MessageClientCredentialType vt=\"8\">Windows</MessageClientCredentialType>" +
				"<AlgorithmSuite vt=\"8\">Basic256</AlgorithmSuite>" +
				"<MsmqAuthenticationMode vt=\"8\">WindowsDomain</MsmqAuthenticationMode>" +
				"<MsmqProtectionLevel vt=\"8\">Sign</MsmqProtectionLevel>" +
				"<MsmqSecureHashAlgorithm vt=\"8\">Sha1</MsmqSecureHashAlgorithm>" +
				"<MsmqEncryptionAlgorithm vt=\"8\">RC4Stream</MsmqEncryptionAlgorithm>" +
				"<MaxConcurrentCalls vt=\"3\">200</MaxConcurrentCalls>" +
				"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
				"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
				"<DisableLocationOnFailure vt=\"11\">0</DisableLocationOnFailure>" +
				"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
				"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
				"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
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

			var nma = new WcfNetMsmqAdapter.Inbound(
				a => {
					a.Address = new EndpointAddress("net.msmq://localhost/private/service_queue");
					a.SecurityMode = NetMsmqSecurityMode.Message;
					a.EnableTransaction = true;
					a.OrderedProcessing = true;
				});

			Invoking(() => ((ISupportValidation) nma).Validate()).Should().NotThrow();
		}
	}
}
