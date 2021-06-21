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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class LogicAppAdapterInboundFixture
	{
		[SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
		[SkippableFact]
		public void SerializeToXml()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var isa = new LogicAppAdapter.Inbound(
				a => {
					//a.ServerAddress = "sftp.server.com";
					//a.Port = 23;
					//a.FolderPath = "/out/to_bts/";
					//a.FileMask = "*.xml";
					//a.ConnectionLimit = 6;

					//a.ClientAuthenticationMode = ClientAuthenticationMode.MultiFactorAuthentication;
					//a.PrivateKeyFile = @"c:\file\key.ppk";
					//a.PrivateKeyPassword = "p@ssw0rd";
					//a.UserName = "user";
					//a.Password = "p@ssw0rd";
					//a.SshServerHostKeyFingerPrint = "fingerprint";
					//a.EncryptionCipher = EncryptionCipher.AES;

					//a.ProxyAddress = "proxy.server.com";
					//a.ProxyPort = 1082;
					//a.ProxyType = ProxyType.SOCKS4;
					//a.ProxyUserName = "proxy-user";
					//a.ProxyPassword = "p@ssw0rd";
				});
			var xml = isa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" + (
					"<MaxReceivedMessageSize vt=\"3\">65536</MaxReceivedMessageSize>" +
					"<SecurityMode vt=\"8\">TransportCredentialOnly</SecurityMode>" +
					"<TransportClientCredentialType vt=\"8\">Ntlm</TransportClientCredentialType>" +
					"<UseSSO vt=\"11\">0</UseSSO>" +
					"<MaxConcurrentCalls vt=\"3\">200</MaxConcurrentCalls>" +
					"<IncludeExceptionDetailInFaults vt=\"11\">0</IncludeExceptionDetailInFaults>" +
					"<DisableLocationOnFailure vt=\"11\">0</DisableLocationOnFailure>" +
					"<OpenTimeout vt=\"8\">00:01:00</OpenTimeout>" +
					"<SendTimeout vt=\"8\">00:01:00</SendTimeout>" +
					"<CloseTimeout vt=\"8\">00:01:00</CloseTimeout>") +
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

			var isa = new LogicAppAdapter.Inbound(
				a => {
					//a.ServerAddress = "sftp.server.com";
					//a.Port = 23;
					//a.FolderPath = "out/to_bts";
					//a.FileMask = "*.xml";
					//a.PollingInterval = TimeSpan.FromMinutes(10);
					//a.ConnectionLimit = 6;

					//a.ClientAuthenticationMode = ClientAuthenticationMode.MultiFactorAuthentication;
					//a.PrivateKeyFile = @"c:\file\key.ppk";
					//a.PrivateKeyPassword = "p@ssw0rd";
					//a.UserName = "user";
					//a.Password = "p@ssw0rd";
					//a.SshServerHostKeyFingerPrint = "fingerprint";
					//a.EncryptionCipher = EncryptionCipher.AES;

					//a.ProxyAddress = "proxy.server.com";
					//a.ProxyPort = 1082;
					//a.ProxyType = ProxyType.SOCKS4;
					//a.ProxyUserName = "proxy-user";
					//a.ProxyPassword = "p@ssw0rd";
				});
			Invoking(() => ((ISupportValidation) isa).Validate()).Should().NotThrow();
		}
	}
}
