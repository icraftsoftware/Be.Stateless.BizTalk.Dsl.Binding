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
using System.Xml;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Office365.Mail;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class Office365EmailAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The Office 365 Outlook Email Adapter allows you to receive mails from your Office 365 Outlook Email from Microsoft
		/// BizTalk Server.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/office365-mail-adapter#receive-email-using-a-receive-port">Receive email using a receive port</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound : Office365EmailAdapter<MailRLConfig>, IInboundAdapter
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
				FolderName = "Inbox";
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return $"O365Mail://{EmailAddress}/{FolderName}";
			}

			protected override void Validate()
			{
				if (EmailAddress.IsNullOrEmpty()) throw new BindingException($"{nameof(EmailAddress)} is not defined.");
			}

			#endregion

			#region Payload Tab

			/// <summary>
			/// <c>true</c> to fetch an email in its raw <c>MIME</c> representation or <c>false</c> to process its content, i.e.
			/// to fetch its <c>MIME</c>-decoded body instead.
			/// </summary>
			/// <remarks>
			/// <para>
			/// It defaults to <c>false</c>.
			/// </para>
			/// <para>
			/// When fetched in its raw <c>MIME</c> representation, the BizTalk message's content naturally includes email body
			/// and all attachments.
			/// </para>
			/// <para>
			/// When its content is processed while being fetched, the BizTalk message's content includes the email body and its
			/// ContentType property is set to content type of the email body.
			/// </para>
			/// </remarks>
			/// <seealso cref="IncludeAttachments"/>
			public bool DeliverMime
			{
				get => AdapterConfig.MIMEContent;
				[SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
				set
				{
					if (AdapterConfig.MIMEContent = value) IncludeAttachments = false;
				}
			}

			/// <summary>
			/// Whether to retrieve email's attachments as parts of the BizTalk message.
			/// </summary>
			/// <remarks>
			/// <para>
			/// It is <c>false</c> by default.
			/// </para>
			/// <para>
			/// Each BizTalk message part has the <see cref="IBaseMessage"/> ContentType property set to the <c>MIME</c> type of
			/// the attachment. Attachments that are Outlook items (emails, calendar events, contacts) are saved in their
			/// <c>MIME</c> representation.
			/// </para>
			/// </remarks>
			public bool IncludeAttachments
			{
				get => AdapterConfig.IncludeAttachments;
				[SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
				set
				{
					if (AdapterConfig.IncludeAttachments = value) DeliverMime = false;
				}
			}

			#endregion

			#region General Tab

			/// <summary>
			/// The email address to fetch emails from.
			/// </summary>
			public string EmailAddress
			{
				get => AdapterConfig.EmailAddress;
				set => AdapterConfig.EmailAddress = value;
			}

			/// <summary>
			/// The folder to fetch emails from.
			/// </summary>
			/// <remarks>
			/// The default folder is <c>Inbox</c>. Note that folders are not recursive.
			/// </remarks>
			public string FolderName
			{
				get => AdapterConfig.FolderName;
				set => AdapterConfig.FolderName = value;
			}

			/// <summary>
			/// The action to be performed after an email is fetched.
			/// </summary>
			/// <remarks>
			/// <para>
			/// <list type="bullet">
			/// <item>
			/// <see cref="Office365EmailAdapter.PostActionTask.None"/> is the default and does nothing after email is received by
			/// Microsoft BizTalk Server.
			/// </item>
			/// <item>
			/// <see cref="Office365EmailAdapter.PostActionTask.MarkAsRead"/> implies, that after an email is received by
			/// Microsoft BizTalk Server, the email in your mailbox is marked as read.
			/// </item>
			/// <item>
			/// <see cref="Office365EmailAdapter.PostActionTask.Delete"/> implies, that after an email is received by Microsoft
			/// BizTalk Server, the email in your mailbox is deleted.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// Post actions are performed on a best-effort basis.
			/// </para>
			/// </remarks>
			public PostActionTask PostActionTask
			{
				get => (PostActionTask) AdapterConfig.PostActionTask;
				set => AdapterConfig.PostActionTask = (int) value;
			}

			/// <summary>
			/// The minimum received TimeStamp of an email in Office 365 Outlook.
			/// </summary>
			/// <remarks>
			/// Only emails more recent than the entered values will be received.
			/// </remarks>
			public DateTimeOffset StartTime
			{
				get => XmlConvert.ToDateTimeOffset(AdapterConfig.StartTime);
				set => AdapterConfig.StartTime = XmlConvert.ToString(value);
			}

			/// <summary>
			/// Whether to fetch only unread email.
			/// </summary>
			public bool UnreadMailsOnly
			{
				get => AdapterConfig.UnreadMailsOnly;
				set => AdapterConfig.UnreadMailsOnly = value;
			}

			#endregion
		}

		#endregion

		#region PostActionTask Enum

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public enum PostActionTask
		{
			None = 0,
			MarkAsRead = 1,
			Delete = 2
		}

		#endregion
	}
}
