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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using FluentAssertions;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata
{
	public class VariableMappingFixture
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void PropertyCannotBeNull()
		{
			Action(() => new VariableMapping { new VariablePropertyMapping("id", null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("property");
		}

		[Fact]
		public void SerializeToXmlString()
		{
			var variableMapping = new VariableMapping {
				new VariablePropertyMapping("id", BizTalkFactoryProperties.ReceiverName)
			};

			((string) variableMapping).Should().Be(
				"<BtsVariablePropertyMapping>"
				+ $"<Variable Name=\"id\" PropertyName=\"{BizTalkFactoryProperties.ReceiverName.Name}\" PropertyNamespace=\"{BizTalkFactoryProperties.ReceiverName.Namespace}\" />"
				+ "</BtsVariablePropertyMapping>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void VariableNameCannotBeEmpty()
		{
			Action(() => new VariableMapping { new VariablePropertyMapping(null, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
			Action(() => new VariableMapping { new VariablePropertyMapping("", null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
			Action(() => new VariableMapping { new VariablePropertyMapping("  ", null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
		}
	}
}
