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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Microsoft.Adapters.SAP;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfSapAdapterOutboundFixture
	{
		[SkippableFact]
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var osa = new WcfSapAdapter.Outbound(
				a => {
					a.Address = new SAPConnectionUri {
						ApplicationServerHost = "appHost",
						ConnectionType = OutboundConnectionType.A,
						MsServ = "msServer",
						Group = "dialog",
						Client = "100",
						Language = "FR"
					};
					a.AutoConfirmSentIdocs = true;
					a.MaxConnectionsPerSystem = 30;
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.Password = "p@ssw0rd";
					a.UserName = "BTS_USER";
					a.MaxConnectionsPerSystem = 30;
				});
			var xml = osa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<BindingType vt=\"8\">sapBinding</BindingType>" +
				"<BindingConfiguration vt=\"8\">" + (
					"&lt;binding name=\"sapBinding\" sendTimeout=\"00:02:00\" enableBizTalkCompatibilityMode=\"true\" maxConnectionsPerSystem=\"30\" autoConfirmSentIdocs=\"true\" /&gt;"
				) +
				"</BindingConfiguration>" +
				"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;</EndpointBehaviorConfiguration>" +
				"<UseSSO vt=\"11\">0</UseSSO>" +
				"<UserName vt=\"8\">BTS_USER</UserName>" +
				"<Password vt=\"8\">p@ssw0rd</Password>" +
				"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
				"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
				"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
				"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
				"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
				"<EnableTransaction vt=\"11\">0</EnableTransaction>" +
				"<IsolationLevel vt=\"8\">Serializable</IsolationLevel>" +
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

			var osa = new WcfSapAdapter.Outbound(
				a => {
					a.Address = new SAPConnectionUri {
						ApplicationServerHost = "appHost",
						ConnectionType = OutboundConnectionType.A,
						MsServ = "msServer",
						Group = "dialog",
						Client = "100",
						Language = "FR"
					};
					a.AutoConfirmSentIdocs = true;
					a.MaxConnectionsPerSystem = 30;
					a.Password = "p@ssw0rd";
					a.UserName = "BTS_USER";
					a.MaxConnectionsPerSystem = 30;
				});

			Invoking(() => ((ISupportValidation) osa).Validate()).Should().NotThrow();
		}
	}
}
