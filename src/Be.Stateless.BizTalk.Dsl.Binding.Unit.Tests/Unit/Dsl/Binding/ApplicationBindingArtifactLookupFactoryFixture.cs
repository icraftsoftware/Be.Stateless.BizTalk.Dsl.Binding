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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Unit.Dsl.Binding
{
	public class ApplicationBindingArtifactLookupFactoryFixture
	{
		[SkippableFact]
		public void ApplicationBindingInstanceIsCached()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT)
				.Should().BeSameAs(ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT));
		}

		[SkippableFact]
		public void ApplicationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			ab.Name.Should().Be("TestApplication");
		}

		[SkippableFact]
		public void ReceiveLocationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var rl = ab.ReceiveLocation<OneWayReceiveLocation>();
			rl.Name.Should().Be("OneWayReceiveLocation");
		}

		[SkippableFact]
		public void ReceivePortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var rp = ab.ReceivePort<OneWayReceivePort>();
			rp.Name.Should().Be("OneWayReceivePort");
		}

		[SkippableFact]
		public void ReferencedApplicationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			rab.Name.Should().Be("MyTestReferencedApplication");
		}

		[SkippableFact]
		public void ReferencedApplicationReceiveLocationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var rl = rab.ReceiveLocation<TestReferencedReceiveLocation>();
			rl.Name.Should().Be("MyTestReferencedReceiveLocation");
		}

		[SkippableFact]
		public void ReferencedApplicationReceivePortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var rp = rab.ReceivePort<TestReferencedReceivePort>();
			rp.Name.Should().Be("MyTestReferencedReceivePort");
		}

		[SkippableFact]
		public void ReferencedApplicationSendPortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var rab = ab.ReferencedApplication<TestReferencedApplication>();
			var sp = rab.SendPort<TestReferencedSendPort>();
			sp.Name.Should().Be("MyTestReferencedSendPort");
		}

		[SkippableFact]
		public void SendPortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var ab = ApplicationBindingArtifactLookupFactory<TestApplication>.Create(TargetEnvironment.DEVELOPMENT);
			var sp = ab.SendPort<OneWaySendPort>();
			sp.Name.Should().Be("OneWaySendPort");
		}
	}
}
