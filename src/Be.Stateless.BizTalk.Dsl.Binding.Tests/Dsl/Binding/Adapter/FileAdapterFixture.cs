﻿#region Copyright & License

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

using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class FileAdapterFixture
	{
		[Fact]
		public void ProtocolTypeSettingsAreReadFromWmiConfigurationClassId()
		{
			var mock = new Mock<FileAdapter> { CallBase = true };
			var fa = mock.Object as IAdapter;
			fa.ProtocolType.Name.Should().Be("FILE");
			fa.ProtocolType.Capabilities.Should().Be(11);
			fa.ProtocolType.ConfigurationClsid.Should().Be("5e49e3a6-b4fc-4077-b44c-22f34a242fdb");
		}
	}
}
