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
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class HttpAdapterOutboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var oha = new HttpAdapter.Outbound(
				a => {
					a.RequestTimeout = TimeSpan.FromMinutes(2);
					a.AuthenticationScheme = HttpAdapter.AuthenticationScheme.Digest;
					a.UseSSO = true;
					a.AffiliateApplicationName = "BizTalk.Factory";
				});
			var xml = oha.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<EnableChunkedEncoding vt=\"11\">-1</EnableChunkedEncoding>" +
				"<RequestTimeout vt=\"3\">120</RequestTimeout>" +
				"<MaxRedirects vt=\"3\">5</MaxRedirects>" +
				"<ContentType vt=\"8\">text/xml</ContentType>" +
				"<UseHandlerProxySettings vt=\"11\">-1</UseHandlerProxySettings>" +
				"<AuthenticationScheme vt=\"8\">Digest</AuthenticationScheme>" +
				"<UseSSO vt=\"11\">-1</UseSSO>" +
				"<AffiliateApplicationName vt=\"8\">BizTalk.Factory</AffiliateApplicationName>" +
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

			var oha = new HttpAdapter.Outbound(
				a => {
					a.RequestTimeout = TimeSpan.FromMinutes(2);
					a.AuthenticationScheme = HttpAdapter.AuthenticationScheme.Anonymous;
					a.UseSSO = true;
					a.AffiliateApplicationName = "BizTalk.Factory";
				});
			Action(() => ((ISupportValidation) oha).Validate()).Should().NotThrow();
		}
	}
}
