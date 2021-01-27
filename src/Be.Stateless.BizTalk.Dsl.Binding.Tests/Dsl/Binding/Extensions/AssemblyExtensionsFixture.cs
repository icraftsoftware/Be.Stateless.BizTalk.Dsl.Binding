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
using Be.Stateless.BizTalk.Dummies.Bindings;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Extensions
{
	public class AssemblyExtensionsFixture
	{
		[Fact]
		public void GetApplicationBindingType()
		{
			typeof(UnnamedApplication).Assembly.GetApplicationBindingType(true).Should().BeSameAs(typeof(UnnamedApplication));
		}

		[Fact]
		public void GetApplicationBindingTypeDoesNotThrowWhenNotFound()
		{
			Function(() => typeof(int).Assembly.GetApplicationBindingType()).Should().NotThrow();
		}

		[Fact]
		public void GetApplicationBindingTypeThrowsWhenNotFound()
		{
			var assembly = typeof(int).Assembly;
			Function(() => assembly.GetApplicationBindingType(true))
				.Should().Throw<InvalidOperationException>()
				.WithMessage($"No {nameof(IApplicationBinding)}-derived type was found in assembly '{assembly.FullName}' located at '{assembly.Location}'.");
		}
	}
}
