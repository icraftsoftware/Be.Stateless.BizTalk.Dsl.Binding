﻿#region Copyright & License

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

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitApplicationBinding(applicationBindingMock.Object);

				environmentSensitiveApplicationBindingMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
				visitorMock.Verify(m => m.VisitApplicationBinding(It.IsAny<IApplicationBinding<string>>()), Times.Once);
			}
		}

		[Fact]
		public void VisitApplicationBindingValidatesApplicationBinding()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var applicationBindingMock = new Mock<IApplicationBinding<string>>();
				var validatingApplicationBindingMock = applicationBindingMock.As<ISupportValidation>();
				applicationBindingMock.As<ISupportEnvironmentOverride>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitApplicationBinding(applicationBindingMock.Object);

				validatingApplicationBindingMock.Verify(m => m.Validate(), Times.Once);
				visitorMock.Verify(m => m.VisitApplicationBinding(It.IsAny<IApplicationBinding<string>>()), Times.Once);
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

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitOrchestration(orchestrationBindingMock.Object);

				environmentSensitiveOrchestrationBindingMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
				visitorMock.Verify(m => m.VisitOrchestration(It.IsAny<IOrchestrationBinding>()), Times.Once);
			}
		}

		[Fact]
		public void VisitOrchestrationValidatesOrchestrationBinding()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
				var validatingOrchestrationBindingMock = orchestrationBindingMock.As<ISupportValidation>();
				orchestrationBindingMock.As<ISupportEnvironmentOverride>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitOrchestration(orchestrationBindingMock.Object);

				validatingOrchestrationBindingMock.Verify(m => m.Validate(), Times.Once);
				visitorMock.Verify(m => m.VisitOrchestration(It.IsAny<IOrchestrationBinding>()), Times.Once);
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

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitReceiveLocation(receiveLocationMock.Object);

				environmentSensitiveReceiveLocationMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
				visitorMock.Verify(m => m.VisitReceiveLocation(It.IsAny<IReceiveLocation<string>>()), Times.Once);
			}
		}

		[Fact]
		public void VisitReceiveLocationValidatesReceiveLocationBinding()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var receiveLocationMock = new Mock<IReceiveLocation<string>>();
				var validatingReceiveLocationMock = receiveLocationMock.As<ISupportValidation>();
				receiveLocationMock.As<ISupportEnvironmentOverride>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitReceiveLocation(receiveLocationMock.Object);

				validatingReceiveLocationMock.Verify(m => m.Validate(), Times.Once);
				visitorMock.Verify(m => m.VisitReceiveLocation(It.IsAny<IReceiveLocation<string>>()), Times.Once);
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

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitReceivePort(receivePortMock.Object);

				environmentSensitiveReceivePortMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
				visitorMock.Verify(m => m.VisitReceivePort(It.IsAny<IReceivePort<string>>()), Times.Once);
			}
		}

		[Fact]
		public void VisitReceivePortValidatesReceivePortBinding()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var receivePortMock = new Mock<IReceivePort<string>>();
				var validatingReceivePortMock = receivePortMock.As<ISupportValidation>();
				receivePortMock.As<ISupportEnvironmentOverride>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitReceivePort(receivePortMock.Object);

				validatingReceivePortMock.Verify(m => m.Validate(), Times.Once);
				visitorMock.Verify(m => m.VisitReceivePort(It.IsAny<IReceivePort<string>>()), Times.Once);
			}
		}

		[Fact]
		public void VisitReferencedApplicationBindingPropagatesVisitor()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var applicationBindingMock = new Mock<IVisitable<IApplicationBindingVisitor>>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitReferencedApplicationBinding(applicationBindingMock.Object);

				applicationBindingMock.Verify(a => a.Accept(visitorMock.Object), Times.Once);
			}
		}

		[Fact]
		public void VisitSendPortAppliesEnvironmentOverrides()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var sendPortMock = new Mock<ISendPort<string>>();
				sendPortMock.As<ISupportValidation>();
				var environmentSensitiveSendPortMock = sendPortMock.As<ISupportEnvironmentOverride>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitSendPort(sendPortMock.Object);

				environmentSensitiveSendPortMock.Verify(m => m.ApplyEnvironmentOverrides("ANYWHERE"), Times.Once);
				visitorMock.Verify(m => m.VisitSendPort(It.IsAny<ISendPort<string>>()), Times.Once);
			}
		}

		[Fact]
		public void VisitSendPortValidatesSendPortBinding()
		{
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var sendPortMock = new Mock<ISendPort<string>>();
				var validatingSendPortMock = sendPortMock.As<ISupportValidation>();
				sendPortMock.As<ISupportEnvironmentOverride>();

				var visitorMock = new Mock<EnvironmentOverrideApplicator> { CallBase = true }
					.As<IApplicationBindingVisitor>();
				visitorMock.Object.VisitSendPort(sendPortMock.Object);

				validatingSendPortMock.Verify(m => m.Validate(), Times.Once);
				visitorMock.Verify(m => m.VisitSendPort(It.IsAny<ISendPort<string>>()), Times.Once);
			}
		}
	}
}
