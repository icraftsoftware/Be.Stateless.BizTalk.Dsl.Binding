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

using System.Security;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class FtpAdapterOutboundFixture
	{
		[Fact]
		public void SerializeToXml()
		{
			var ofa = new FtpAdapter.Outbound(
				a => {
					a.Server = "ftp.server.com";
					a.Folder = "/in/from_bts/";
					a.UserName = "ftp-user";
					a.Password = "p@ssw0rd";
					a.AfterPut = "cd /";
					a.BeforePut = "cd /";
					a.AllocateStorage = true;

					a.ConnectionLimit = 10;
				});
			var xml = ofa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<AdapterConfig vt=\"8\">" + SecurityElement.Escape(
					"<Config>" +
					"<uri>ftp://ftp.server.com:21//in/from_bts//%MessageID%.xml</uri>" +
					"<firewallPort>21</firewallPort>" +
					"<firewallType>NoFirewall</firewallType>" +
					"<passiveMode>False</passiveMode>" +
					"<serverAddress>ftp.server.com</serverAddress>" +
					"<serverPort>21</serverPort>" +
					"<targetFolder>/in/from_bts/</targetFolder>" +
					"<targetFileName>%MessageID%.xml</targetFileName>" +
					"<userName>ftp-user</userName>" +
					"<password>p@ssw0rd</password>" +
					"<afterPut>cd /</afterPut>" +
					"<beforePut>cd /</beforePut>" +
					"<allocateStorage>True</allocateStorage>" +
					"<representationType>binary</representationType>" +
					"<ftpsConnMode>Explicit</ftpsConnMode>" +
					"<useDataProtection>True</useDataProtection>" +
					"<connectionLimit>10</connectionLimit>" +
					"</Config>") +
				"</AdapterConfig>" +
				"</CustomProps>");
		}

		[Fact]
		public void Validate()
		{
			var ofa = new FtpAdapter.Outbound();
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("The Server Address is not defined");
		}

		[Fact]
		public void ValidateDoesNotThrow()
		{
			var ofa = new FtpAdapter.Outbound(
				a => {
					a.Server = "ftp.server.com";
					a.Folder = "/in/from_bts/";
					a.UserName = "ftp-user";
					a.Password = "p@ssw0rd";
					a.AfterPut = "cd /";
					a.BeforePut = "cd /";
					a.AllocateStorage = true;

					a.ConnectionLimit = 10;
				});
			Action(() => ((ISupportValidation) ofa).Validate()).Should().NotThrow();
		}
	}
}
