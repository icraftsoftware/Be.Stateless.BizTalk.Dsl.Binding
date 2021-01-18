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

using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class FileAdapterInboundFixture
	{
		[SkippableFact]
		public void CredentialsAreCompatibleWithNetworkFolder()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ifa = new FileAdapter.Inbound(
				a => {
					a.ReceiveFolder = @"\\server\folder";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Action(() => ((ISupportValidation) ifa).Validate())
				.Should().NotThrow();
		}

		[SkippableFact]
		public void CredentialsAreNotCompatibleWithLocalFolder()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ifa = new FileAdapter.Inbound(
				a => {
					a.ReceiveFolder = @"c:\file\drops";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Action(() => ((ISupportValidation) ifa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
		}

		[SkippableFact]
		public void FileNameIsRequired()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ifa = new FileAdapter.Inbound(
				a => {
					a.ReceiveFolder = @"\\server";
					a.FileMask = string.Empty;
				});
			Action(() => ((ISupportValidation) ifa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Inbound file adapter has no source file mask.");
		}

		[SkippableFact]
		public void ReceiveFolderIsRequired()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ifa = new FileAdapter.Inbound(a => { });
			Action(() => ((ISupportValidation) ifa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Inbound file adapter has no source folder.");
		}

		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ifa = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
			var xml = ifa.GetAdapterBindingInfoSerializer().Serialize();

			xml.Should().Be(
				"<CustomProps>" +
				"<BatchSize vt=\"19\">20</BatchSize>" +
				"<BatchSizeInBytes vt=\"19\">102400</BatchSizeInBytes>" +
				"<FileMask vt=\"8\">*.xml</FileMask>" +
				"<FileNetFailRetryCount vt=\"19\">5</FileNetFailRetryCount>" +
				"<FileNetFailRetryInt vt=\"19\">5</FileNetFailRetryInt>" +
				"<PollingInterval vt=\"19\">60000</PollingInterval>" +
				"<RemoveReceivedFileDelay vt=\"19\">10</RemoveReceivedFileDelay>" +
				"<RemoveReceivedFileMaxInterval vt=\"19\">300000</RemoveReceivedFileMaxInterval>" +
				"<RemoveReceivedFileRetryCount vt=\"19\">5</RemoveReceivedFileRetryCount>" +
				"<RenameReceivedFiles vt=\"11\">-1</RenameReceivedFiles>" +
				"</CustomProps>");
		}
	}
}
