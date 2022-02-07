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

using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class ApplicationBindingValidatorFixture
	{
		[Fact]
		public void VisitApplicationBindingValidatesApplicationBinding()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>();
			var validatingApplicationBindingMock = applicationBindingMock.As<ISupportValidation>();
			applicationBindingMock.As<ISupportEnvironmentOverride>();

			var sut = new ApplicationBindingValidator();
			sut.VisitApplicationBinding(applicationBindingMock.Object);

			validatingApplicationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void VisitOrchestrationValidatesOrchestrationBinding()
		{
			var orchestrationBindingMock = new Mock<IOrchestrationBinding>();
			var validatingOrchestrationBindingMock = orchestrationBindingMock.As<ISupportValidation>();
			orchestrationBindingMock.As<ISupportEnvironmentOverride>();

			var sut = new ApplicationBindingValidator();
			sut.VisitOrchestration(orchestrationBindingMock.Object);

			validatingOrchestrationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void VisitReceiveLocationValidatesReceiveLocationBinding()
		{
			var receiveLocationMock = new Mock<IReceiveLocation<string>>();
			var validatingReceiveLocationMock = receiveLocationMock.As<ISupportValidation>();
			receiveLocationMock.As<ISupportEnvironmentOverride>();

			var sut = new ApplicationBindingValidator();
			sut.VisitReceiveLocation(receiveLocationMock.Object);

			validatingReceiveLocationMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void VisitReceivePortValidatesReceivePortBinding()
		{
			var receivePortMock = new Mock<IReceivePort<string>>();
			var validatingReceivePortMock = receivePortMock.As<ISupportValidation>();
			receivePortMock.As<ISupportEnvironmentOverride>();

			var sut = new ApplicationBindingValidator();
			sut.VisitReceivePort(receivePortMock.Object);

			validatingReceivePortMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void VisitReferencedApplicationBindingPropagatesVisitor()
		{
			var applicationBindingMock = new Mock<IVisitable<IApplicationBindingVisitor>>();

			var sut = new ApplicationBindingValidator();
			sut.VisitReferencedApplicationBinding(applicationBindingMock.Object);

			applicationBindingMock.Verify(a => a.Accept(sut), Times.Once);
		}

		[Fact]
		public void VisitSendPortValidatesSendPortBinding()
		{
			var sendPortMock = new Mock<ISendPort<string>>();
			var validatingSendPortMock = sendPortMock.As<ISupportValidation>();
			sendPortMock.As<ISupportEnvironmentOverride>();

			var sut = new ApplicationBindingValidator();
			sut.VisitSendPort(sendPortMock.Object);

			validatingSendPortMock.Verify(m => m.Validate(), Times.Once);
		}
	}
}
