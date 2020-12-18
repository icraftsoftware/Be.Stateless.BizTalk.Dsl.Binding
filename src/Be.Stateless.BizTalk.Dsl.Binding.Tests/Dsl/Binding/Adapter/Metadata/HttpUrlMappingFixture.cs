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
using System.Net.Http;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Metadata
{
	public class HttpUrlMappingFixture
	{
		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void MethodCannotBeEmpty()
		{
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("AddCustomer", null, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("method");
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("AddCustomer", "", null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("method");
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("AddCustomer", "  ", null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("method");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void OperationNameCannotBeEmpty()
		{
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation(null, null, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("", null, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("  ", null, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("name");
		}

		[Fact]
		public void SerializeToXmlString()
		{
			var httpUrlMapping = new HttpUrlMapping {
				new HttpUrlMappingOperation("AddCustomer", HttpMethod.Post.Method, "/Customer/{id}"),
				new HttpUrlMappingOperation("DeleteCustomer", HttpMethod.Delete.Method, "/Customer/{id}")
			};

			((string) httpUrlMapping).Should().Be(
				"<BtsHttpUrlMapping>"
				+ "<Operation Name=\"AddCustomer\" Method=\"POST\" Url=\"/Customer/{id}\" />"
				+ "<Operation Name=\"DeleteCustomer\" Method=\"DELETE\" Url=\"/Customer/{id}\" />"
				+ "</BtsHttpUrlMapping>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void UrlCannotBeEmpty()
		{
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("AddCustomer", HttpMethod.Delete.Method, null) })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("url");
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("AddCustomer", HttpMethod.Delete.Method, "") })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("url");
			Action(() => new HttpUrlMapping { new HttpUrlMappingOperation("AddCustomer", HttpMethod.Delete.Method, "  ") })
				.Should().Throw<ArgumentNullException>()
				.Which.ParamName.Should().Be("url");
		}
	}
}
