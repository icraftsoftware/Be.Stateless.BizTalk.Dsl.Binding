﻿#region Copyright & License

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

using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class HostEnumerationVisitorFixture
	{
		[SkippableFact]
		public void EnumerateApplicationHosts()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			IVisitable<IApplicationBindingVisitor> application = new TestReferencedApplication();

			var sut = new HostEnumerationVisitor();
			application.Accept(sut);

			sut.Should().BeEquivalentTo("host");
		}

		[SkippableFact]
		public void EnumerateMainApplicationOnlyHosts()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var application = new TestApplication();
			application.ReferencedApplications.Find<TestReferencedApplication>().Should().NotBeNull();

			var sut = new HostEnumerationVisitor();
			((IVisitable<IApplicationBindingVisitor>) application).Accept(sut);

			sut.Should().BeEquivalentTo("Receive Host Name", "Send Host Name");
		}
	}
}
