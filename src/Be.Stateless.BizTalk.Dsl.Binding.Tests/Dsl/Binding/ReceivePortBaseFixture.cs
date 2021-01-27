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

using System;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Reflection;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ReceivePortBaseFixture
	{
		[Fact]
		public void AcceptsAndPropagatesVisitor()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			var receiveLocationCollectionMock = new Mock<ReceiveLocationCollection<string>>(receivePortMock.Object) { CallBase = false };
			var visitableReceiveLocationCollectionMock = receiveLocationCollectionMock.As<IVisitable<IApplicationBindingVisitor>>();
			Reflector.SetField(receivePortMock.Object, "_receiveLocations", receiveLocationCollectionMock.Object);

			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) receivePortMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitReceivePort(receivePortMock.Object), Times.Once);
			visitableReceiveLocationCollectionMock.Verify(m => m.Accept(visitorMock.Object), Times.Once);
		}

		[Fact]
		public void AutomaticallyValidatesOnConfiguratorAction()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>>(
				(Action<IReceivePort<string>>) (rp => {
					rp.Name = "Receive Port Name";
					rp.ReceiveLocations.Add(new Mock<ReceiveLocationBase<string>>().Object);
				})) { CallBase = true };
			var validatingReceivePortMock = receivePortMock.As<ISupportValidation>();

			receivePortMock.Object.Description = "Force Moq to call ctor.";

			validatingReceivePortMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receivePortMock.Object).ApplyEnvironmentOverrides("ACC");

			receivePortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receivePortMock.Object).ApplyEnvironmentOverrides(string.Empty);

			receivePortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Fact]
		public void NameIsMandatory()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			receivePortMock.Object.Description = "Force Moq to call ctor.";

			Action(() => ((ISupportValidation) receivePortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Receive Port's Name is not defined.");
		}

		[Fact]
		public void ReceiveLocationIsMandatory()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			receivePortMock.Object.Name = "Receive Port Name";

			Action(() => ((ISupportValidation) receivePortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Receive Port has no Receive Locations.");
		}

		[Fact]
		public void ReceivePortCannotMixOneWayAndTwoWayReceiveLocations()
		{
			var rl1 = new Mock<ReceiveLocationBase<string>>().As<IReceiveLocation<string>>();
			rl1.As<ISupportValidation>().Setup(l => l.Validate());
			rl1.Setup(l => l.SendPipeline).Returns(new Mock<SendPipeline>().Object);

			var rl2 = new Mock<ReceiveLocationBase<string>>().As<IReceiveLocation<string>>();
			rl2.As<ISupportValidation>().Setup(l => l.Validate());

			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };
			receivePortMock.Object.Name = "Receive Port Name";
			receivePortMock.Object.ReceiveLocations.Add(rl1.Object, rl2.Object);

			Action(() => ((ISupportValidation) receivePortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Receive Port has a mix of one-way and two-way Receive Locations.");
		}

		[Fact]
		public void SupportINamingConvention()
		{
			const string name = "Receive Port Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeReceivePortName(It.IsAny<IReceivePort<object>>())).Returns(name);

			var receivePortMock = new Mock<ReceivePortBase<object>> { CallBase = true };
			receivePortMock.Object.Name = conventionMock.Object;

			((ISupportNamingConvention) receivePortMock.Object).Name.Should().Be(name);
		}

		[Fact]
		public void SupportStringNamingConvention()
		{
			const string name = "Receive Port Name";
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			receivePortMock.Object.Name = name;

			((ISupportNamingConvention) receivePortMock.Object).Name.Should().Be(name);
		}
	}
}
