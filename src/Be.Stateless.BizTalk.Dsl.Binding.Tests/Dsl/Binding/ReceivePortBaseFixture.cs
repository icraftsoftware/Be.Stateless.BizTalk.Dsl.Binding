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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ReceivePortBaseFixture
	{
		[Fact]
		public void AcceptsAndPropagatesVisitor()
		{
			var receiveLocationMock = new Mock<ReceiveLocationBase<string>> { CallBase = false };
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };
			receivePortMock.Object.ReceiveLocations.Add(receiveLocationMock.Object);
			var visitorMock = new Mock<IApplicationBindingVisitor>();

			((IVisitable<IApplicationBindingVisitor>) receivePortMock.Object).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitReceivePort(receivePortMock.Object), Times.Once);
			visitorMock.Verify(m => m.VisitReceiveLocation(receiveLocationMock.Object), Times.Once);
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receivePortMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			receivePortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) receivePortMock.Object).ApplyEnvironmentOverrides(string.Empty);

			receivePortMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void NameIsMandatory()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			receivePortMock.Object.Description = "Force Moq to call ctor.";

			Invoking(() => ((ISupportValidation) receivePortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Receive Port's Name is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ReceiveLocationIsMandatory()
		{
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			receivePortMock.Object.Name = "Receive Port Name";

			Invoking(() => ((ISupportValidation) receivePortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("[Receive Port Name] Receive Port's Receive Locations are not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
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

			Invoking(() => ((ISupportValidation) receivePortMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("[Receive Port Name] Receive Port cannot define both one-way and two-way Receive Locations.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportINamingConvention()
		{
			const string name = "Receive Port Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeReceivePortName(It.IsAny<IReceivePort<object>>())).Returns(name);

			var receivePortMock = new Mock<ReceivePortBase<object>> { CallBase = true };
			receivePortMock.Object.Name = conventionMock.Object;

			receivePortMock.Object.ResolveName().Should().Be(name);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportStringNamingConvention()
		{
			const string name = "Receive Port Name";
			var receivePortMock = new Mock<ReceivePortBase<string>> { CallBase = true };

			receivePortMock.Object.Name = name;

			receivePortMock.Object.ResolveName().Should().Be(name);
		}
	}
}
