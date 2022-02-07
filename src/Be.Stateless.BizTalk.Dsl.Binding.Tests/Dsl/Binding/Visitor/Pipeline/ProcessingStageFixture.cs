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

using Be.Stateless.BizTalk.Orchestrations.Direct;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor.Pipeline
{
	public class ProcessingStageFixture
	{
		[Fact]
		public void AcceptsAndPropagatesVisitor()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>>();

			var referencedApplicationBindingMock = new Mock<ApplicationBindingBase<string>>();
			applicationBindingMock.Object.ReferencedApplications.Add(referencedApplicationBindingMock.Object);

			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>>();
			applicationBindingMock.Object.Orchestrations.Add(orchestrationBindingMock.Object);

			var receivePortMock = new Mock<ReceivePortBase<string>>();
			applicationBindingMock.Object.ReceivePorts.Add(receivePortMock.Object);

			var sendPortMock = new Mock<SendPortBase<string>>();
			applicationBindingMock.Object.SendPorts.Add(sendPortMock.Object);

			var pipeline = new VisitorPipeline<string>(applicationBindingMock.Object);

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			var sut = new ProcessingStage<string>(pipeline);
			sut.Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitReferencedApplicationBinding(referencedApplicationBindingMock.Object), Times.Once);
			visitorMock.Verify(m => m.VisitApplicationBinding(applicationBindingMock.Object), Times.Once);
			visitorMock.Verify(m => m.VisitOrchestration(orchestrationBindingMock.Object), Times.Once);
			visitorMock.Verify(m => m.VisitReceivePort(receivePortMock.Object), Times.Once);
			visitorMock.Verify(m => m.VisitSendPort(sendPortMock.Object), Times.Once);
		}
	}
}
