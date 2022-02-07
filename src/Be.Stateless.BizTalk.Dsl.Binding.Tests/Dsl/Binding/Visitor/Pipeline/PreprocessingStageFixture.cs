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

using FluentAssertions;
using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor.Pipeline
{
	public class PreprocessingStageFixture
	{
		[Fact]
		public void AdvanceVisitorPipelineStage()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>(MockBehavior.Strict);
			var visitableApplicationBindingMock = applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();
			var visitorPipeline = new VisitorPipeline<string>(applicationBindingMock.Object);

			visitableApplicationBindingMock
				.Setup(m => m.Accept(It.IsAny<IApplicationBindingVisitor>()))
				// ProcessingStage is used for each visitor called in sequence
				.Callback(() => visitorPipeline.Stage.Should().BeOfType<ProcessingStage<string>>())
				.Returns(It.IsAny<IApplicationBindingVisitor>());

			var sut = new PreprocessingStage<string>(visitorPipeline);
			sut.Accept(new Mock<IApplicationBindingVisitor>().Object);

			// PreprocessingStage is restored once all the visitors have done their processing
			visitorPipeline.Stage.Should().BeSameAs(sut);
		}

		[Fact]
		public void CallEnvironmentOverrideApplicatorAndApplicationBindingValidatorBeforeCustomVisitor()
		{
			var visitorMock = new Mock<IApplicationBindingVisitor>();

			var applicationBindingMock = new Mock<IApplicationBinding<string>>(MockBehavior.Strict);
			var visitableApplicationBindingMock = applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var sequence = new MockSequence();
			visitableApplicationBindingMock.InSequence(sequence)
				.Setup(m => m.Accept(It.IsAny<EnvironmentOverrideApplicator>()))
				.Returns(It.IsAny<EnvironmentOverrideApplicator>());
			visitableApplicationBindingMock.InSequence(sequence)
				.Setup(m => m.Accept(It.IsAny<ApplicationBindingValidator>()))
				.Returns(It.IsAny<ApplicationBindingValidator>());
			visitableApplicationBindingMock.InSequence(sequence)
				.Setup(m => m.Accept(visitorMock.Object))
				.Returns(visitorMock.Object);

			var pipelineMock = new Mock<VisitorPipeline<string>>(applicationBindingMock.Object);
			var sut = new PreprocessingStage<string>(pipelineMock.Object);
			sut.Accept(visitorMock.Object);

			visitableApplicationBindingMock.Verify(m => m.Accept(It.IsAny<EnvironmentOverrideApplicator>()), Times.Once);
			visitableApplicationBindingMock.Verify(m => m.Accept(It.IsAny<ApplicationBindingValidator>()), Times.Once);
			visitableApplicationBindingMock.Verify(m => m.Accept(visitorMock.Object), Times.Once);
		}

		[Fact]
		public void DoNotCallApplicationBindingValidatorTwice()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<string>>(MockBehavior.Strict);
			var visitableApplicationBindingMock = applicationBindingMock.As<IVisitable<IApplicationBindingVisitor>>();

			var sequence = new MockSequence();
			visitableApplicationBindingMock.InSequence(sequence)
				.Setup(m => m.Accept(It.IsAny<EnvironmentOverrideApplicator>()))
				.Returns(It.IsAny<EnvironmentOverrideApplicator>());
			visitableApplicationBindingMock.InSequence(sequence)
				.Setup(m => m.Accept(It.IsAny<ApplicationBindingValidator>()))
				.Returns(It.IsAny<ApplicationBindingValidator>());

			var pipelineMock = new Mock<VisitorPipeline<string>>(applicationBindingMock.Object);
			var sut = new PreprocessingStage<string>(pipelineMock.Object);
			sut.Accept(new Mock<ApplicationBindingValidator>().Object);

			visitableApplicationBindingMock.Verify(m => m.Accept(It.IsAny<EnvironmentOverrideApplicator>()), Times.Once);
			visitableApplicationBindingMock.Verify(m => m.Accept(It.IsAny<ApplicationBindingValidator>()), Times.Once);
		}
	}
}
