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
using Microsoft.BizTalk.Adapter.Sftp;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class SftpAdapterInboundFixture
	{
		[SkippableFact]
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var isa = new SftpAdapter.Inbound(
				a => {
					a.ServerAddress = "sftp.server.com";
					a.Port = 23;
					a.FolderPath = "/out/to_bts/";
					a.FileMask = "*.xml";
					a.ConnectionLimit = 6;

					a.ClientAuthenticationMode = ClientAuthenticationMode.MultiFactorAuthentication;
					a.PrivateKeyFile = @"c:\file\key.ppk";
					a.PrivateKeyPassword = "p@ssw0rd";
					a.UserName = "user";
					a.Password = "p@ssw0rd";
					a.SshServerHostKeyFingerPrint = "fingerprint";
					a.EncryptionCipher = EncryptionCipher.AES;

					a.ProxyAddress = "proxy.server.com";
					a.ProxyPort = 1082;
					a.ProxyType = ProxyType.SOCKS4;
					a.ProxyUserName = "proxy-user";
					a.ProxyPassword = "p@ssw0rd";
				});
			var xml = isa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" + (
					"<ServerAddress vt=\"8\">sftp.server.com</ServerAddress>" +
					"<Port vt=\"3\">23</Port>" +
					"<FolderPath vt=\"8\">/out/to_bts/</FolderPath>" +
					"<FileMask vt=\"8\">*.xml</FileMask>" +
					"<AccessAnySSHServerHostKey vt=\"11\">0</AccessAnySSHServerHostKey>" +
					"<SSHServerHostKey vt=\"8\">fingerprint</SSHServerHostKey>" +
					"<ClientAuthenticationMode vt=\"8\">MultiFactorAuthentication</ClientAuthenticationMode>" +
					"<PrivateKey vt=\"8\">c:\\file\\key.ppk</PrivateKey>" +
					"<PrivateKeyPassword vt=\"8\">p@ssw0rd</PrivateKeyPassword>" +
					"<UserName vt=\"8\">user</UserName>" +
					"<Password vt=\"8\">p@ssw0rd</Password>" +
					"<PollingInterval vt=\"3\">5</PollingInterval>" +
					"<PollingIntervalUnit vt=\"8\">Seconds</PollingIntervalUnit>" +
					"<RetainAfterDownload vt=\"11\">0</RetainAfterDownload>" +
					"<EnableTimeComparison vt=\"11\">0</EnableTimeComparison>" +
					"<RedownloadInterval vt=\"3\">0</RedownloadInterval>" +
					"<ConnectionLimit vt=\"3\">6</ConnectionLimit>" +
					"<MaxConnectionReuseTimeInSeconds vt=\"3\">0</MaxConnectionReuseTimeInSeconds>" +
					"<ProxyPassword vt=\"8\">p@ssw0rd</ProxyPassword>" +
					"<ProxyUserName vt=\"8\">proxy-user</ProxyUserName>" +
					"<ProxyType vt=\"8\">SOCKS4</ProxyType>" +
					"<ProxyAddress vt=\"8\">proxy.server.com</ProxyAddress>" +
					"<ProxyPort vt=\"3\">1082</ProxyPort>" +
					"<EncryptionCipher vt=\"8\">AES</EncryptionCipher>") +
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

			var isa = new SftpAdapter.Inbound(
				a => {
					a.ServerAddress = "sftp.server.com";
					a.Port = 23;
					a.FolderPath = "out/to_bts";
					a.FileMask = "*.xml";
					a.PollingInterval = TimeSpan.FromMinutes(10);
					a.ConnectionLimit = 6;

					a.ClientAuthenticationMode = ClientAuthenticationMode.MultiFactorAuthentication;
					a.PrivateKeyFile = @"c:\file\key.ppk";
					a.PrivateKeyPassword = "p@ssw0rd";
					a.UserName = "user";
					a.Password = "p@ssw0rd";
					a.SshServerHostKeyFingerPrint = "fingerprint";
					a.EncryptionCipher = EncryptionCipher.AES;

					a.ProxyAddress = "proxy.server.com";
					a.ProxyPort = 1082;
					a.ProxyType = ProxyType.SOCKS4;
					a.ProxyUserName = "proxy-user";
					a.ProxyPassword = "p@ssw0rd";
				});
			Action(() => ((ISupportValidation) isa).Validate()).Should().NotThrow();
		}
	}
}
