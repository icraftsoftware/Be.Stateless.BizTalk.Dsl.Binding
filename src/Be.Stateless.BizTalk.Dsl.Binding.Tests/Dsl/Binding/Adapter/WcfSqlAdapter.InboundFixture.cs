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
using System.Transactions;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfSqlAdapterInboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var isa = new WcfSqlAdapter.Inbound(
				a => {
					a.Address = new SqlAdapterConnectionUri { InboundId = "AvailableBatches", Server = "localhost", InitialCatalog = "BizTalkFactoryTransientStateDb" };
					a.InboundOperationType = InboundOperation.XmlPolling;
					a.PolledDataAvailableStatement = "SELECT COUNT(1) FROM vw_claim_AvailableTokens";
					a.PollingStatement = "EXEC usp_claim_CheckOut";
					a.PollingInterval = TimeSpan.FromHours(2);
					a.InboundBodyLocation = InboundMessageBodySelection.UseBodyPath;
					a.InboundBodyPathExpression = "/BodyWrapper/*";
					a.InboundNodeEncoding = MessageBodyFormat.Xml;
					a.XmlStoredProcedureRootNodeName = "BodyWrapper";
					a.ServiceBehaviors = new[] {
						new SqlAdapterInboundTransactionBehavior {
							TransactionIsolationLevel = IsolationLevel.Serializable
						}
					};
				});
			var xml = isa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<BindingType vt=\"8\">sqlBinding</BindingType>" +
				"<BindingConfiguration vt=\"8\">" +
				"&lt;binding name=\"sqlBinding\" " +
				"polledDataAvailableStatement=\"SELECT COUNT(1) FROM vw_claim_AvailableTokens\" " +
				"pollingStatement=\"EXEC usp_claim_CheckOut\" " +
				"pollingIntervalInSeconds=\"7200\" " +
				"inboundOperationType=\"XmlPolling\" " +
				"xmlStoredProcedureRootNodeName=\"BodyWrapper\" /&gt;" +
				"</BindingConfiguration>" +
				"<ServiceBehaviorConfiguration vt=\"8\">" +
				"&lt;behavior name=\"ServiceBehavior\"&gt;" +
				"&lt;sqlAdapterInboundTransactionBehavior transactionIsolationLevel=\"Serializable\" /&gt;" +
				"&lt;/behavior&gt;" +
				"</ServiceBehaviorConfiguration>" +
				"<EndpointBehaviorConfiguration vt=\"8\">&lt;behavior name=\"EndpointBehavior\" /&gt;</EndpointBehaviorConfiguration>" +
				"<InboundBodyLocation vt=\"8\">UseBodyPath</InboundBodyLocation>" +
				"<InboundBodyPathExpression vt=\"8\">/BodyWrapper/*</InboundBodyPathExpression>" +
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

			var isa = new WcfSqlAdapter.Inbound(
				a => {
					a.Address = new SqlAdapterConnectionUri { InboundId = "AvailableBatches", Server = "localhost", InitialCatalog = "BizTalkFactoryTransientStateDb" };
					a.InboundOperationType = InboundOperation.XmlPolling;
					a.PolledDataAvailableStatement = "SELECT COUNT(1) FROM vw_claim_AvailableTokens";
					a.PollingStatement = "EXEC usp_claim_CheckOut";
					a.PollingInterval = TimeSpan.FromHours(2);
					a.InboundBodyLocation = InboundMessageBodySelection.UseBodyPath;
					a.InboundBodyPathExpression = "/BodyWrapper/*";
					a.InboundNodeEncoding = MessageBodyFormat.Xml;
					a.XmlStoredProcedureRootNodeName = "BodyWrapper";
					a.ServiceBehaviors = new[] {
						new SqlAdapterInboundTransactionBehavior {
							TransactionIsolationLevel = IsolationLevel.Serializable
						}
					};
				});

			Invoking(() => ((ISupportValidation) isa).Validate()).Should().NotThrow();
		}
	}
}
