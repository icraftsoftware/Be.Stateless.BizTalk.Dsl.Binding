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

using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Microsoft.BizTalk.Adapter.Office365.Mail;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class Office365EmailAdapterFixture
	{
		[SkippableFact]
		public void ProtocolTypeSettingsAreReadFromWmiConfigurationClassId()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var mock = new Mock<Office365EmailAdapter<MailRLConfig>> { CallBase = true };
			var fa = mock.Object as IAdapter;
			fa.ProtocolType.Name.Should().Be("Office365 Outlook Email");
			fa.ProtocolType.Capabilities.Should().Be(11);
			fa.ProtocolType.ConfigurationClsid.Should().Be("48b96e09-bd96-4f46-95ef-57accc55f23d");
		}
	}
}
