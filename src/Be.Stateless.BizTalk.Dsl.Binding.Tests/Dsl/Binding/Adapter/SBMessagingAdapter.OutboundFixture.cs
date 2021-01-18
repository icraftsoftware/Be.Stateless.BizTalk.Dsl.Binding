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
using System.Globalization;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class SBMessagingAdapterOutboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var defaultScheduledEnqueueTimeUtc = DateTime.UtcNow.Date.AddMonths(1);
			var adapter = new SBMessagingAdapter.Outbound(
				a => {
					a.Address = new Uri("sb://biztalkfactory.servicebus.windows.net/");

					a.UseAcsAuthentication = true;
					a.StsUri = new Uri("https://biztalk.factory-sb.accesscontrol.windows.net/");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "issuer_secret";

					a.DefaultScheduledEnqueueTimeUtc = defaultScheduledEnqueueTimeUtc;
					a.DefaultCorrelationId = "correlation_id";
					a.DefaultPartitionKey = "partition";

					a.CustomBrokeredPropertyNamespace = "urn:schemas.stateless.be:biztalk:service-bus:queue";
				});
			var xml = adapter.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<IssuerName vt=\"8\">issuer_name</IssuerName>" +
				"<IssuerSecret vt=\"8\">issuer_secret</IssuerSecret>" +
				"<StsUri vt=\"8\">https://biztalk.factory-sb.accesscontrol.windows.net/</StsUri>" +
				"<BatchFlushInterval vt=\"8\">00:00:00.0200000</BatchFlushInterval>" +
				"<SessionIdleTimeout vt=\"8\">00:01:00</SessionIdleTimeout>" +
				"<MaxReceivedMessageSize vt=\"3\">262144</MaxReceivedMessageSize>" +
				"<CustomBrokeredPropertyNamespace vt=\"8\">urn:schemas.stateless.be:biztalk:service-bus:queue</CustomBrokeredPropertyNamespace>" +
				"<DefaultCorrelationId vt=\"8\">correlation_id</DefaultCorrelationId>" +
				$"<DefaultScheduledEnqueueTimeUtc vt=\"7\">{defaultScheduledEnqueueTimeUtc.ToString("G", DateTimeFormatInfo.InvariantInfo)}</DefaultScheduledEnqueueTimeUtc>" +
				$"<DefaultTimeToLive vt=\"8\">{TimeSpan.MaxValue}</DefaultTimeToLive>" +
				"<DefaultPartitionKey vt=\"8\">partition</DefaultPartitionKey>" +
				"<UseAcsAuthentication vt=\"11\">-1</UseAcsAuthentication>" +
				"<UseSasAuthentication vt=\"11\">0</UseSasAuthentication>" +
				"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
				"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
				"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>" +
				"</CustomProps>");
		}

		[SkippableFact]
		public void Validate()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var adapter = new SBMessagingAdapter.Outbound();
			Action(() => ((ISupportValidation) adapter).Validate())
				.Should().Throw<ArgumentException>()
				.WithMessage(@"Required property Address (URI) not specified.");
		}

		[SkippableFact]
		public void ValidateAddress()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var adapter = new SBMessagingAdapter.Outbound(a => { a.Address = new Uri("file://biztalf.factory.servicebus.windows.net/batching/"); });
			Action(() => ((ISupportValidation) adapter).Validate())
				.Should().Throw<ArgumentException>()
				.WithMessage(@"The specified address is invalid.");
		}

		[SkippableFact]
		public void ValidateDoesNotThrow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var adapter = new SBMessagingAdapter.Outbound(
				a => {
					a.Address = new Uri("sb://biztalkfactory.servicebus.windows.net/");

					a.UseAcsAuthentication = true;
					a.StsUri = new Uri("https://biztalk.factory-sb.accesscontrol.windows.net/");
					a.IssuerName = "issuer_name";
					a.IssuerSecret = "issuer_secret";

					a.DefaultScheduledEnqueueTimeUtc = DateTime.UtcNow.Date.AddMonths(1);
					a.DefaultCorrelationId = "correlation_id";

					a.CustomBrokeredPropertyNamespace = "urn:schemas.stateless.be:biztalk:service-bus:queue";
				});
			Action(() => ((ISupportValidation) adapter).Validate()).Should().NotThrow();
		}
	}
}
