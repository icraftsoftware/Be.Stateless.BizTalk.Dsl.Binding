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
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfCustomAdapterInboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var wca = new WcfCustomAdapter.Inbound<NetTcpBindingElement>(
				a => {
					const int tenMegaBytes = 1024 * 1024 * 10;
					a.Address = new("net.tcp://localhost/biztalk.factory/service.svc");
					a.Binding.MaxReceivedMessageSize = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxArrayLength = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxStringContentLength = tenMegaBytes;
					a.Binding.Security.Mode = SecurityMode.Transport;
					a.Binding.Security.Transport.ProtectionLevel = ProtectionLevel.Sign;
					a.Binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
					a.OpenTimeout = TimeSpan.FromMinutes(3);
					// TODO LeaseTimeout
				});
			var xml = wca.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<BindingType vt=\"8\">netTcpBinding</BindingType>" +
				"<BindingConfiguration vt=\"8\">" +
				"&lt;binding name=\"netTcpBinding\" openTimeout=\"00:03:00\" maxReceivedMessageSize=\"10485760\"&gt;" +
				"&lt;readerQuotas maxStringContentLength=\"10485760\" maxArrayLength=\"10485760\" /&gt;" +
				"&lt;security&gt;&lt;transport protectionLevel=\"Sign\" /&gt;&lt;/security&gt;" +
				"&lt;/binding&gt;" +
				"</BindingConfiguration>" +
				"<ServiceBehaviorConfiguration vt=\"8\">&lt;behavior name=\"ServiceBehavior\" /&gt;</ServiceBehaviorConfiguration>" +
				"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;</EndpointBehaviorConfiguration>" +
				"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
				"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
				"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
				"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
				"<DisableLocationOnFailure vt=\"11\">0</DisableLocationOnFailure>" +
				"<SuspendMessageOnFailure vt=\"11\">-1</SuspendMessageOnFailure>" +
				"<IncludeExceptionDetailInFaults vt=\"11\">-1</IncludeExceptionDetailInFaults>" +
				"<CredentialType vt=\"8\">None</CredentialType>" +
				"<OrderedProcessing vt=\"11\">0</OrderedProcessing>" +
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

			var wca = new WcfCustomAdapter.Inbound<NetTcpBindingElement>(
				a => {
					const int tenMegaBytes = 1024 * 1024 * 10;
					a.Address = new("net.tcp://localhost/biztalk.factory/service.svc");
					a.Binding.MaxReceivedMessageSize = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxArrayLength = tenMegaBytes;
					a.Binding.ReaderQuotas.MaxStringContentLength = tenMegaBytes;
					a.Binding.Security.Mode = SecurityMode.Transport;
					a.Binding.Security.Transport.ProtectionLevel = ProtectionLevel.Sign;
					a.Binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
					a.OpenTimeout = TimeSpan.FromMinutes(3);
				});

			Invoking(() => ((ISupportValidation) wca).Validate()).Should().NotThrow();
		}
	}
}
