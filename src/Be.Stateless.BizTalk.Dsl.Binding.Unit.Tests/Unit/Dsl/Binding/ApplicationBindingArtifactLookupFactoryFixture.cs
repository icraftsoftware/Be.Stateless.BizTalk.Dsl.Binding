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

using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Install;
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

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create().Should().BeSameAs(ApplicationBindingArtifactLookupFactory<TestApplication>.Create());
			}
		}

		[SkippableFact]
		public void ApplicationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.Name.Should().Be("TestApplication");
			}
		}

		[SkippableFact]
		public void ReceiveLocationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.ReceiveLocation<OneWayReceiveLocation>()
					.Name.Should().Be("OneWayReceiveLocation");
			}
		}

		[SkippableFact]
		public void ReceivePortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.ReceivePort<OneWayReceivePort>()
					.Name.Should().Be("OneWayReceivePort");
			}
		}

		[SkippableFact]
		public void ReferencedApplicationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.ReferencedApplication<TestReferencedApplication>()
					.Name.Should().Be("MyTestReferencedApplication");
			}
		}

		[SkippableFact]
		public void ReferencedApplicationNameAddedAtCreationTime()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestReferencedApplication>.Create(new TestApplication())
					.ReferencedApplication<TestApplication>()
					.Name.Should().Be("TestApplication");
			}
		}

		[SkippableFact]
		public void ReferencedApplicationReceiveLocationName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.ReferencedApplication<TestReferencedApplication>()
					.ReceiveLocation<TestReferencedReceiveLocation>()
					.Name.Should().Be("MyTestReferencedReceiveLocation");
			}
		}

		[SkippableFact]
		public void ReferencedApplicationReceivePortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.ReferencedApplication<TestReferencedApplication>()
					.ReceivePort<TestReferencedReceivePort>()
					.Name.Should().Be("MyTestReferencedReceivePort");
			}
		}

		[SkippableFact]
		public void ReferencedApplicationSendPortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.ReferencedApplication<TestReferencedApplication>()
					.SendPort<TestReferencedSendPort>()
					.Name.Should().Be("MyTestReferencedSendPort");
			}
		}

		[SkippableFact]
		public void SendPortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: TargetEnvironment.DEVELOPMENT))
			{
				ApplicationBindingArtifactLookupFactory<TestApplication>.Create()
					.SendPort<OneWaySendPort>()
					.Name.Should().Be("OneWaySendPort");
			}
		}
	}
}
