#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	[Collection("DeploymentContext")]
	public class EnvironmentOverrideApplicatorFixture
	{
		[Fact]
		public void VisitApplicationBindingAppliesEnvironmentOverrides()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var applicationBindingMock = new Mock<IApplicationBinding<string>>();
				applicationBindingMock.As<ISupportValidation>();
				var environmentSensitiveApplicationBindingMock = applicationBindingMock.As<ISupportEnvironmentOverride>();

				var sut = new EnvironmentOverrideApplicator();
				sut.VisitApplicationBinding(applicationBindingMock.Object);

				environmentSensitiveApplicationBindingMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
			}
		}

		[Fact]
		public void VisitOrchestrationAppliesEnvironmentOverrides()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
				orchestrationBindingMock.As<ISupportValidation>();
				var environmentSensitiveOrchestrationBindingMock = orchestrationBindingMock.As<ISupportEnvironmentOverride>();

				var sut = new EnvironmentOverrideApplicator();
				sut.VisitOrchestration(orchestrationBindingMock.Object);

				environmentSensitiveOrchestrationBindingMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
			}
		}

		[Fact]
		public void VisitReceiveLocationAppliesEnvironmentOverrides()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var receiveLocationMock = new Mock<IReceiveLocation<string>>();
				receiveLocationMock.As<ISupportValidation>();
				var environmentSensitiveReceiveLocationMock = receiveLocationMock.As<ISupportEnvironmentOverride>();

				var sut = new EnvironmentOverrideApplicator();
				sut.VisitReceiveLocation(receiveLocationMock.Object);

				environmentSensitiveReceiveLocationMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
			}
		}

		[Fact]
		public void VisitReceivePortAppliesEnvironmentOverrides()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var receivePortMock = new Mock<IReceivePort<string>>();
				receivePortMock.As<ISupportValidation>();
				var environmentSensitiveReceivePortMock = receivePortMock.As<ISupportEnvironmentOverride>();

				var sut = new EnvironmentOverrideApplicator();
				sut.VisitReceivePort(receivePortMock.Object);

				environmentSensitiveReceivePortMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
			}
		}

		[Fact]
		public void VisitReferencedApplicationBindingPropagatesVisitor()
		{
			var applicationBindingMock = new Mock<IVisitable<IApplicationBindingVisitor>>();

			var sut = new EnvironmentOverrideApplicator();
			sut.VisitReferencedApplicationBinding(applicationBindingMock.Object);

			applicationBindingMock.Verify(a => a.Accept(sut), Times.Once);
		}

		[Fact]
		public void VisitSendPortAppliesEnvironmentOverrides()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var sendPortMock = new Mock<ISendPort<string>>();
				sendPortMock.As<ISupportValidation>();
				var environmentSensitiveSendPortMock = sendPortMock.As<ISupportEnvironmentOverride>();

				var sut = new EnvironmentOverrideApplicator();
				sut.VisitSendPort(sendPortMock.Object);

				environmentSensitiveSendPortMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
			}
		}
	}
}
