#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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
using System.Transactions;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Utils;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using FluentAssertions;
using Microsoft.Adapters.OracleDB;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfOracleAdapterOutboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(OdpNet.IsConfigured);

			var ooa = new WcfOracleAdapter.Outbound(
				a => {
					a.Address = new OracleDBConnectionUri { DataSourceName = "TNS" };
					a.IsolationLevel = IsolationLevel.ReadCommitted;
					a.OutboundBodyLocation = OutboundMessageBodySelection.UseBodyElement;
					a.PropagateFaultMessage = true;
					a.SendTimeout = TimeSpan.FromMinutes(2);
					a.StaticAction = new ActionMapping {
						new ActionMappingOperation("CreateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/CREATE_TICKET"),
						new ActionMappingOperation("UpdateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET")
					};
					a.UserName = "Scott";
					a.Password = "Tiger";
				});
			var xml = ooa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<BindingType vt=\"8\">oracleDBBinding</BindingType>" +
				"<BindingConfiguration vt=\"8\">" +
				"&lt;binding name=\"oracleDBBinding\" sendTimeout=\"00:02:00\" enableBizTalkCompatibilityMode=\"true\" /&gt;" +
				"</BindingConfiguration>" +
				"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;</EndpointBehaviorConfiguration>" +
				"<StaticAction vt=\"8\">" +
				"&lt;BtsActionMapping&gt;" +
				"&lt;Operation Name=\"CreateTicket\" Action=\"http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/CREATE_TICKET\" /&gt;" +
				"&lt;Operation Name=\"UpdateTicket\" Action=\"http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET\" /&gt;" +
				"&lt;/BtsActionMapping&gt;" +
				"</StaticAction>" +
				"<UseSSO vt=\"11\">0</UseSSO>" +
				"<UserName vt=\"8\">Scott</UserName>" +
				"<Password vt=\"8\">Tiger</Password>" +
				"<InboundBodyLocation vt=\"8\">UseBodyElement</InboundBodyLocation>" +
				"<InboundNodeEncoding vt=\"8\">Xml</InboundNodeEncoding>" +
				"<OutboundBodyLocation vt=\"8\">UseBodyElement</OutboundBodyLocation>" +
				"<OutboundXmlTemplate vt=\"8\">&lt;bts-msg-body xmlns=\"http://www.microsoft.com/schemas/bts2007\" encoding=\"xml\"/&gt;</OutboundXmlTemplate>" +
				"<PropagateFaultMessage vt=\"11\">-1</PropagateFaultMessage>" +
				"<EnableTransaction vt=\"11\">-1</EnableTransaction>" +
				"<IsolationLevel vt=\"8\">ReadCommitted</IsolationLevel>" +
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
			Skip.IfNot(OdpNet.IsConfigured);

			var ooa = new WcfOracleAdapter.Outbound(
				a => {
					a.Address = new OracleDBConnectionUri { DataSourceName = "TNS" };
					a.IsolationLevel = IsolationLevel.ReadCommitted;
					a.OutboundBodyLocation = OutboundMessageBodySelection.UseBodyElement;
					a.PropagateFaultMessage = true;
					a.StaticAction = new ActionMapping {
						new ActionMappingOperation("CreateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/CREATE_TICKET"),
						new ActionMappingOperation("UpdateTicket", "http://Microsoft.LobServices.OracleDB/2007/03/SCOTT/Procedure/UPDATE_TICKET")
					};
					a.UserName = "Scott";
					a.Password = "Tiger";
				});

			Action(() => ((ISupportValidation) ooa).Validate()).Should().NotThrow();
		}
	}
}
