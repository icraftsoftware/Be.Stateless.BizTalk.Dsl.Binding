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
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Be.Stateless.Reflection;
using FluentAssertions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Moq.Protected;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfCustomAdapterBaseFixture
	{
		[Fact]
		public void BasicHttpBindingElementIsSupported()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Function(() => adapterMock.Object).Should().NotThrow();
		}

		[Fact]
		public void BasicHttpsBindingElementIsNotSupported()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpsBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Function(() => adapterMock.Object)
				.Should().Throw<TypeInitializationException>()
				.WithInnerException<BindingException>()
				.WithMessage("BasicHttpBindingElement has to be used for https addresses as well.");
		}

		[Fact]
		public void EnvironmentOverridesAreAppliedForGivenEnvironment()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides("ACC");

			adapterMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Once(), ItExpr.Is<string>(v => v == "ACC"));
		}

		[Fact]
		public void EnvironmentOverridesAreSkippedWhenNoGivenEnvironment()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides(string.Empty);

			adapterMock.Protected().Verify("ApplyEnvironmentOverrides", Times.Never(), ItExpr.IsAny<string>());
		}

		[Fact]
		public void ForwardsApplyEnvironmentOverridesToBindingElement()
		{
			var bindingMock = new Mock<NetMsmqBindingElement> { CallBase = true };
			var environmentSensitiveBindingMock = bindingMock.As<ISupportEnvironmentOverride>();

			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Reflector.SetProperty((WcfAdapterBase<EndpointAddress, NetMsmqBindingElement, CustomRLConfig>) adapterMock.Object, "BindingElement", bindingMock.Object);

			((ISupportEnvironmentOverride) adapterMock.Object).ApplyEnvironmentOverrides("ACC");

			environmentSensitiveBindingMock.Verify(b => b.ApplyEnvironmentOverrides("ACC"), Times.Once());
		}

		[Fact]
		public void ValidateCustomBasicHttpBindingWithoutTransportSecurityThrowsWhenSchemeIsHttps()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new EndpointAddress("https://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.None;
			Action(() => ((ISupportValidation) adapterMock.Object).Validate())
				.Should().Throw<ArgumentException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("Invalid address scheme; expecting \"http\" scheme.");
		}

		[Fact]
		public void ValidateCustomBasicHttpBindingWithTransportSecurityDoesNotThrowWhenSchemeIsHttps()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new EndpointAddress("https://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
			Action(() => ((ISupportValidation) adapterMock.Object).Validate()).Should().NotThrow();
		}

		[Fact]
		public void ValidateCustomBasicHttpBindingWithTransportSecurityThrowsWhenSchemeIsHttp()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new EndpointAddress("http://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
			Action(() => ((ISupportValidation) adapterMock.Object).Validate())
				.Should().Throw<ArgumentException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("Invalid address scheme; expecting \"https\" scheme.");
		}
	}
}
