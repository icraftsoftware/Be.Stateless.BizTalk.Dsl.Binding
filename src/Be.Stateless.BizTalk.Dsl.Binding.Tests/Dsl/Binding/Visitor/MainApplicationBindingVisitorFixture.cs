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
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class MainApplicationBindingVisitorFixture
	{
		[Fact]
		public void VisitApplicationBindingAppliesEnvironmentOverridesAndCallsOverridableVisitApplicationBinding()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);

			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>().Verify(a => a.Accept(It.IsAny<EnvironmentOverrideApplicationVisitor>()), Times.Once);
			visitor.Verify(m => m.VisitApplicationBinding(applicationBindingMock.Object), Times.Once);
		}

		[Fact]
		public void VisitOrchestrationCallsOverridableVisitOrchestrationForOrchestrationsBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			orchestrationBindingMock.Setup(o => o.ApplicationBinding).Returns(applicationBindingMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitOrchestration(orchestrationBindingMock.Object);

			visitor.Verify(m => m.VisitOrchestration(orchestrationBindingMock.Object), Times.Once);
		}

		[Fact]
		public void VisitOrchestrationDoesNotCallOverridableVisitOrchestrationForOrchestrationsNotBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding>();

			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			orchestrationBindingMock.Setup(o => o.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitOrchestration(orchestrationBindingMock.Object);

			visitor.Verify(m => m.VisitOrchestration(orchestrationBindingMock.Object), Times.Never);
		}

		[Fact]
		public void VisitReceiveLocationCallsOverridableVisitReceiveLocationForReceiveLocationsBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(p => p.ApplicationBinding).Returns(applicationBindingMock.Object);

			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			receiveLocationMock.Setup(l => l.ReceivePort).Returns(receivePortMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitReceiveLocation(receiveLocationMock.Object);

			visitor.Verify(m => m.VisitReceiveLocation(receiveLocationMock.Object), Times.Once);
		}

		[Fact]
		public void VisitReceiveLocationDoesNotCallOverridableVisitReceiveLocationForReceiveLocationsNotBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding<string>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(p => p.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			receiveLocationMock.Setup(l => l.ReceivePort).Returns(receivePortMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitReceiveLocation(receiveLocationMock.Object);

			visitor.Verify(m => m.VisitReceiveLocation(receiveLocationMock.Object), Times.Never);
		}

		[Fact]
		public void VisitReceivePortCallsOverridableVisitReceivePortForReceivePortsBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(p => p.ApplicationBinding).Returns(applicationBindingMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitReceivePort(receivePortMock.Object);

			visitor.Verify(m => m.VisitReceivePort(receivePortMock.Object), Times.Once);
		}

		[Fact]
		public void VisitReceivePortDoesNotCallOverridableVisitReceivePortForReceivePortsNotBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding<string>>();

			var receivePortMock = new Mock<IReceivePort<string>>();
			receivePortMock.Setup(p => p.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitReceivePort(receivePortMock.Object);

			visitor.Verify(m => m.VisitReceivePort(receivePortMock.Object), Times.Never);
		}

		[Fact]
		public void VisitSendPortCallsOverridableVisitSendPortForSendPortsBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.Setup(o => o.ApplicationBinding).Returns(applicationBindingMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitSendPort(sendPortMock.Object);

			visitor.Verify(m => m.VisitSendPort(sendPortMock.Object), Times.Once);
		}

		[Fact]
		public void VisitSendPortDoesNotCallOverridableVisitSendPortForSendPortsNotBelongingToMainApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			applicationBindingMock.As<ISupportNamingConvention>();
			applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var referencedApplicationBindingMock = new Mock<IApplicationBinding<string>>();

			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.Setup(o => o.ApplicationBinding).Returns(referencedApplicationBindingMock.Object);

			var visitor = new Mock<MainApplicationBindingVisitor> { CallBase = true };
			visitor.As<IApplicationBindingVisitor>().Object.VisitApplicationBinding(applicationBindingMock.Object);
			visitor.As<IApplicationBindingVisitor>().Object.VisitSendPort(sendPortMock.Object);

			visitor.Verify(m => m.VisitSendPort(sendPortMock.Object), Times.Never);
		}
	}
}
