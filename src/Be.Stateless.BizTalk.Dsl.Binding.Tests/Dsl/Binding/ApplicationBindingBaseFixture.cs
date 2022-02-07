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

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor.Pipeline;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Reflection;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ApplicationBindingBaseFixture
	{
		[Fact]
		public void AcceptsAndPropagatesVisitorToVisitorPipeline()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			var visitorPipelineMock = new Mock<VisitorPipeline<string>>(applicationBindingMock.Object);
			Reflector.SetField(applicationBindingMock.Object, "_visitorPipeline", visitorPipelineMock.Object);

			var visitorMock = new Mock<IApplicationBindingVisitor>();
			((IVisitable<IApplicationBindingVisitor>) applicationBindingMock.Object).Accept(visitorMock.Object);

			visitorPipelineMock.Verify(m => m.Accept(visitorMock.Object), Times.Once);
		}

		[SkippableFact]
		public void AutomaticallyValidatesOnConfiguratorInvocation()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<ApplicationBindingBase<string>>((Action<IApplicationBinding<string>>) (ab => { ab.Name = "name"; })) { CallBase = true };
			var validatingApplicationBindingMock = applicationBindingMock.As<ISupportValidation>();

			applicationBindingMock.Object.Description = "Force Moq to call ctor.";

			validatingApplicationBindingMock.Verify(m => m.Validate(), Times.Once);
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) applicationBindingMock.Object).ApplyEnvironmentOverrides(TargetEnvironment.ACCEPTANCE);

			applicationBindingMock.Protected().Verify(
				nameof(ISupportEnvironmentOverride.ApplyEnvironmentOverrides),
				Times.Once(),
				ItExpr.Is<string>(v => v == TargetEnvironment.ACCEPTANCE));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			((ISupportEnvironmentOverride) applicationBindingMock.Object).ApplyEnvironmentOverrides(string.Empty);

			applicationBindingMock.Protected().Verify(nameof(ISupportEnvironmentOverride.ApplyEnvironmentOverrides), Times.Never(), ItExpr.IsAny<string>());
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void NameIsMandatory()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };
			applicationBindingMock.Object.Description = "Force Moq to call ctor.";

			Invoking(() => ((ISupportValidation) applicationBindingMock.Object).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Application's Name is not defined.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportINamingConvention()
		{
			const string name = "Application Name";

			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeApplicationName(It.IsAny<IApplicationBinding<object>>())).Returns(name);

			var applicationBindingMock = new Mock<ApplicationBindingBase<object>> { CallBase = true };
			applicationBindingMock.Object.Name = conventionMock.Object;

			applicationBindingMock.Object.ResolveName().Should().Be(name);
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void SupportStringNamingConvention()
		{
			const string name = "ApplicationName";

			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };
			applicationBindingMock.Object.Name = name;

			applicationBindingMock.Object.ResolveName().Should().Be(name);
		}
	}
}
