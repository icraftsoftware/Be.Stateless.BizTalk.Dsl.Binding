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

using Be.Stateless.BizTalk.Dummies.Bindings;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ApplicationBindingArtifactLookupFactoryFixture
	{
		[Fact]
		public void ApplicationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			ab.Name.Should().Be("MyTestApplication");
		}

		[Fact]
		public void ReceiveLocationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rl = ab.ReceiveLocation<TestReceiveLocation>();
			rl.Name.Should().Be("MyTestReceiveLocation");
		}

		[Fact]
		public void ReceivePortName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rp = ab.ReceivePort<TestReceivePort>();
			rp.Name.Should().Be("MyTestReceivePort");
		}

		[Fact]
		public void ReferencedApplicationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			rab.Name.Should().Be("MyTestReferencedApplication");
		}

		[Fact]
		public void ReferencedApplicationReceiveLocationName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var rl = rab.ReceiveLocation<TestReferencedReceiveLocation>();
			rl.Name.Should().Be("MyTestReferencedReceiveLocation");
		}

		[Fact]
		public void ReferencedApplicationReceivePortName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var rp = rab.ReceivePort<TestReferencedReceivePort>();
			rp.Name.Should().Be("MyTestReferencedReceivePort");
		}

		[Fact]
		public void ReferencedApplicationSendPortName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var sp = rab.SendPort<TestReferencedSendPort>();
			sp.Name.Should().Be("MyTestReferencedSendPort");
		}

		[Fact]
		public void SendPortName()
		{
			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create("DEV");
			var sp = ab.SendPort<TestSendPort>();
			sp.Name.Should().Be("MyTestSendPort");
		}
	}
}
