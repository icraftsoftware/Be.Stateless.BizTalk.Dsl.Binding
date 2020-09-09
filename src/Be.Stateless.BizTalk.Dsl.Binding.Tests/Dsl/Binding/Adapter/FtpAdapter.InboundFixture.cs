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
using System.Security;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using FluentAssertions;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class FtpAdapterInboundFixture
	{
		[Fact]
		public void SerializeToXml()
		{
			var ifa = new FtpAdapter.Inbound(
				a => {
					a.MaximumNumberOfFiles = 1;
					a.MaximumBatchSize = 2;

					a.FirewallAddress = "firewall.com";
					a.FirewallType = FtpAdapter.FirewallType.Socks4;
					a.FirewallUserName = "firewall-user";
					a.FirewallPassword = "firewall-p@ssw0rd";

					a.Server = "ftp.server.com";
					a.Folder = "/out/to_bts/";
					a.FileMask = "*.*.csv";
					a.UserName = "ftp-user";
					a.Password = "p@ssw0rd";
					a.AfterGet = a.BeforeGet = "cd /";
					a.ErrorThreshold = 11;
					a.Log = "c:\\windows\\temp\\ftp.log";
					a.MaximumFileSize = 100;
					a.UseNameList = false;

					a.DeleteAfterDownload = true;
					a.EnableTimestampComparison = true;
					a.PollingInterval = TimeSpan.FromSeconds(120);

					a.ClientCertificate = "hash";
					a.FtpsConnectionMode = FtpAdapter.FtpsConnectionMode.Implicit;
					a.UseSsl = true;

					a.ReceiveTimeout = TimeSpan.FromMinutes(1);
					a.TemporaryFolder = "c:\\windows\\temp";
				});
			var xml = ifa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<AdapterConfig vt=\"8\">" + SecurityElement.Escape(
					"<Config>" +
					"<uri>ftp://ftp.server.com:21//out/to_bts//*.*.csv</uri>" +
					"<maximumNumberOfFiles>1</maximumNumberOfFiles>" +
					"<maximumBatchSize>2</maximumBatchSize>" +
					"<firewallAddress>firewall.com</firewallAddress>" +
					"<firewallPort>21</firewallPort>" +
					"<firewallUserName>firewall-user</firewallUserName>" +
					"<firewallPassword>firewall-p@ssw0rd</firewallPassword>" +
					"<firewallType>Socks4</firewallType>" +
					"<passiveMode>False</passiveMode>" +
					"<serverAddress>ftp.server.com</serverAddress>" +
					"<serverPort>21</serverPort>" +
					"<targetFolder>/out/to_bts/</targetFolder>" +
					"<fileMask>*.*.csv</fileMask>" +
					"<userName>ftp-user</userName>" +
					"<password>p@ssw0rd</password>" +
					"<afterGet>cd /</afterGet>" +
					"<beforeGet>cd /</beforeGet>" +
					"<errorThreshold>11</errorThreshold>" +
					"<commandLogFilename>c:\\windows\\temp\\ftp.log</commandLogFilename>" +
					"<maxFileSize>100</maxFileSize>" +
					"<representationType>binary</representationType>" +
					"<useNLST>False</useNLST>" +
					"<deleteAfterDownload>True</deleteAfterDownload>" +
					"<enableTimeComparison>True</enableTimeComparison>" +
					"<pollingInterval>2</pollingInterval>" +
					"<pollingUnitOfMeasure>Minutes</pollingUnitOfMeasure>" +
					"<redownloadInterval>-1</redownloadInterval>" +
					"<clientCertificateHash>hash</clientCertificateHash>" +
					"<ftpsConnMode>Implicit</ftpsConnMode>" +
					"<useDataProtection>True</useDataProtection>" +
					"<useSsl>True</useSsl>" +
					"<receiveDataTimeout>60000</receiveDataTimeout>" +
					"<spoolingFolder>c:\\windows\\temp</spoolingFolder>" +
					"</Config>") +
				"</AdapterConfig>" +
				"</CustomProps>");
		}

		[Fact]
		public void Validate()
		{
			var ifa = new FtpAdapter.Inbound();
			Action(() => ((ISupportValidation) ifa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage(@"The Server Address is not defined");
		}

		[Fact]
		public void ValidateDoesNotThrow()
		{
			var ifa = new FtpAdapter.Inbound(
				a => {
					a.MaximumNumberOfFiles = 1;
					a.MaximumBatchSize = 2;

					a.FirewallAddress = "firewall.com";
					a.FirewallType = FtpAdapter.FirewallType.Socks4;
					a.FirewallUserName = "firewall-user";
					a.FirewallPassword = "firewall-p@ssw0rd";

					a.Server = "ftp.server.com";
					a.Folder = "/out/to_bts/";
					a.FileMask = "*.*.csv";
					a.UserName = "ftp-user";
					a.Password = "p@ssw0rd";
					a.AfterGet = a.BeforeGet = "cd /";
					a.ErrorThreshold = 11;
					a.Log = "c:\\windows\\temp\\ftp.log";
					a.MaximumFileSize = 100;
					a.UseNameList = false;

					a.DeleteAfterDownload = true;
					a.EnableTimestampComparison = true;
					a.PollingInterval = TimeSpan.FromSeconds(120);

					a.ClientCertificate = "hash";
					a.FtpsConnectionMode = FtpAdapter.FtpsConnectionMode.Implicit;
					a.UseSsl = true;

					a.ReceiveTimeout = TimeSpan.FromMinutes(1);
					a.TemporaryFolder = "c:\\windows\\temp";
				});
			Action(() => ((ISupportValidation) ifa).Validate()).Should().NotThrow();
		}
	}
}
