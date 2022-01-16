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
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class Office365EmailAdapterInboundFixture
	{
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var oea = new Office365EmailAdapter.Inbound(
				a => {
					a.EmailAddress = "john.doe@world.com";
					a.FolderName = "Inbox";
					a.StartTime = DateTime.SpecifyKind(new(2020, 01, 20, 19, 19, 19), DateTimeKind.Local);
					a.UnreadMailsOnly = true;
					a.PostActionTask = Office365EmailAdapter.PostActionTask.MarkAsRead;
					a.DeliverMime = false;
					a.IncludeAttachments = true;
				});
			var xml = oea.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<FolderName vt=\"8\">Inbox</FolderName>" +
				"<UnreadMailsOnly vt=\"11\">-1</UnreadMailsOnly>" +
				"<PostActionTask vt=\"3\">1</PostActionTask>" +
				"<MIMEContent vt=\"11\">0</MIMEContent>" +
				"<IncludeAttachments vt=\"11\">-1</IncludeAttachments>" +
				"<StartTime vt=\"8\">2020-01-20T19:19:19+01:00</StartTime>" +
				"<EmailAddress vt=\"8\">john.doe@world.com</EmailAddress>" +
				"</CustomProps>");
		}

		[SkippableFact]
		public void ValidateDoesNotThrow()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			var oea = new Office365EmailAdapter.Inbound(a => { a.EmailAddress = "john.doe@world.com"; });
			Invoking(() => ((ISupportValidation) oea).Validate()).Should().NotThrow();
		}

		[SkippableFact]
		public void ValidateThrowsWhenEmailAddressIsMissing()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			var oea = new Office365EmailAdapter.Inbound(a => { a.FolderName = "Invoices"; });
			Invoking(() => ((ISupportValidation) oea).Validate())
				.Should().Throw<BindingException>()
				.WithMessage($"{nameof(Office365EmailAdapter.Inbound.EmailAddress)} is not defined.");
		}
	}
}
