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

using System.Net.Mime;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class HttpAdapterInboundFixture
	{
		[Fact]
		public void SerializeToXml()
		{
			var iha = new HttpAdapter.Inbound(
				a => {
					a.LoopBack = false;
					a.ResponseContentType = MediaTypeNames.Application.Pdf;
					a.ReturnCorrelationHandle = true;
				});
			var xml = iha.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<LoopBack vt=\"11\">0</LoopBack>" +
				"<ResponseContentType vt=\"8\">application/pdf</ResponseContentType>" +
				"<ReturnCorrelationHandle vt=\"11\">-1</ReturnCorrelationHandle>" +
				"<SuspendFailedRequests vt=\"11\">-1</SuspendFailedRequests>" +
				"<UseSSO vt=\"11\">0</UseSSO>" +
				"</CustomProps>");
		}

		[Fact(Skip = "TODO")]
		public void Validate()
		{
			// TODO Validate()
		}

		[Fact]
		public void ValidateDoesNotThrow()
		{
			var iha = new HttpAdapter.Inbound(
				a => {
					a.LoopBack = false;
					a.ResponseContentType = MediaTypeNames.Application.Pdf;
					a.ReturnCorrelationHandle = true;
				});
			Action(() => ((ISupportValidation) iha).Validate()).Should().NotThrow();
		}
	}
}
