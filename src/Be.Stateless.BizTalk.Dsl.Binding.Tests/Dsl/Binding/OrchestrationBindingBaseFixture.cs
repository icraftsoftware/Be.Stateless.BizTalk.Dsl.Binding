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

using System;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.CodeDom;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Orchestrations.Bound;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class OrchestrationBindingBaseFixture
	{
		[Fact]
		public void AcceptsVisitor()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) orchestrationBindingMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitOrchestration(orchestrationBindingMock.Object), Times.Once);
		}

		[Fact]
		public void AutomaticallyValidatesOnConfiguratorAction()
		{
			var orchestrationBindingMock = new Mock<ProcessOrchestrationBinding>((Action<IProcessOrchestrationBinding>) (o => { })) { CallBase = true };
			var validatingOrchestrationBindingMock = orchestrationBindingMock.As<ISupportValidation>();
			validatingOrchestrationBindingMock.Setup(o => o.Validate()).Verifiable();

			orchestrationBindingMock.Object.Host = "Force Moq to call ctor.";

			validatingOrchestrationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void DirectPortsHaveNoMatchingPropertyInGeneratedOrchestrationBinding()
		{
			var assembly = typeof(Process).CompileToDynamicAssembly();
			var orchestrationBinding = assembly.CreateInstance($"{typeof(Process).FullName}OrchestrationBinding");

			orchestrationBinding!.GetType().GetProperties().Any(p => p.Name.StartsWith("Direct")).Should().BeFalse();
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };

			((ISupportEnvironmentOverride) orchestrationBindingMock.Object).ApplyEnvironmentOverrides("ACC");

			orchestrationBindingMock.Protected().Verify(nameof(ISupportEnvironmentOverride.ApplyEnvironmentOverrides), Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };

			((ISupportEnvironmentOverride) orchestrationBindingMock.Object).ApplyEnvironmentOverrides(string.Empty);

			orchestrationBindingMock.Protected().Verify(nameof(ISupportEnvironmentOverride.ApplyEnvironmentOverrides), Times.Never(), ItExpr.IsAny<string>());
		}

		[Fact]
		public void HostIsMandatory()
		{
			var orchestrationBindingMock = new Mock<OrchestrationBindingBase<Process>> { CallBase = true };
			orchestrationBindingMock.Object.Description = "Force Moq to call ctor.";

			Action(() => ((ISupportValidation) orchestrationBindingMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Orchestration's Host is not defined.");
		}

		[Fact]
		public void LogicalOneWayReceivePortMustBeBoundToOneWayReceivePort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding { Host = "Host" };
			orchestrationBinding.ReceivePort = new TwoWayReceivePort();
			orchestrationBinding.SendPort = new OneWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TwoWaySendPort();
			orchestrationBinding.RequestResponsePort = new TwoWayReceivePort();

			Action(() => ((ISupportValidation) orchestrationBinding).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Orchestration's one-way logical port 'ReceivePort' is bound to two-way port 'TwoWayReceivePort'.");
		}

		[Fact]
		public void LogicalOneWaySendPortMustBeBoundToOneWaySendPort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding { Host = "Host" };
			orchestrationBinding.ReceivePort = new OneWayReceivePort();
			orchestrationBinding.SendPort = new TwoWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TwoWaySendPort();
			orchestrationBinding.RequestResponsePort = new TwoWayReceivePort();

			Action(() => ((ISupportValidation) orchestrationBinding).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Orchestration's one-way logical port 'SendPort' is bound to two-way port 'TwoWaySendPort'.");
		}

		[Fact]
		public void LogicalPortsMustAllBeBound()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding { Host = "Host" };
			orchestrationBinding.ReceivePort = new Mock<ReceivePort>().Object;
			orchestrationBinding.SendPort = new Mock<SendPort>().Object;

			Action(() => ((ISupportValidation) orchestrationBinding).Validate())
				.Should().Throw<BindingException>()
				.WithMessage($"The '{typeof(Process).FullName}' orchestration has unbound logical ports: 'RequestResponsePort', 'SolicitResponsePort'.");
		}

		[Fact]
		public void LogicalRequestResponsePortMustBeBoundToTwoWayReceivePort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding { Host = "Host" };
			orchestrationBinding.ReceivePort = new OneWayReceivePort();
			orchestrationBinding.SendPort = new OneWaySendPort();
			orchestrationBinding.SolicitResponsePort = new TwoWaySendPort();
			orchestrationBinding.RequestResponsePort = new OneWayReceivePort();

			Action(() => ((ISupportValidation) orchestrationBinding).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Orchestration's two-way logical port 'RequestResponsePort' is bound to one-way port 'OneWayReceivePort'.");
		}

		[Fact]
		public void LogicalSolicitResponsePortMustBeBoundToTwoWaySendPort()
		{
			IProcessOrchestrationBinding orchestrationBinding = new ProcessOrchestrationBinding { Host = "Host" };
			orchestrationBinding.ReceivePort = new OneWayReceivePort();
			orchestrationBinding.SendPort = new OneWaySendPort();
			orchestrationBinding.SolicitResponsePort = new OneWaySendPort();
			orchestrationBinding.RequestResponsePort = new TwoWayReceivePort();

			Action(() => ((ISupportValidation) orchestrationBinding).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Orchestration's two-way logical port 'SolicitResponsePort' is bound to one-way port 'OneWaySendPort'.");
		}

		[Fact]
		public void OperationName()
		{
			ProcessOrchestrationBinding.SolicitResponsePort.Operations.SolicitResponseOperation.Name.Should().NotBeEmpty();
		}
	}
}
