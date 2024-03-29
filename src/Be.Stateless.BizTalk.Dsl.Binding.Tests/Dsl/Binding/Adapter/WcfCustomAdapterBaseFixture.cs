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

using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using FluentAssertions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfCustomAdapterBaseFixture
	{
		[Fact]
		public void BasicHttpBindingElementIsSupported()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Invoking(() => adapterMock.Object).Should().NotThrow();
		}

		[Fact]
		public void BasicHttpsBindingElementIsNotSupported()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpsBindingElement, CustomRLConfig>>(new ProtocolType()) { CallBase = true };
			Invoking(() => adapterMock.Object)
				.Should().Throw<TypeInitializationException>()
				.WithInnerException<BindingException>()
				.WithMessage("BasicHttpBindingElement has to be used for https addresses as well.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ValidateCustomBasicHttpBindingWithoutTransportSecurityThrowsWhenSchemeIsHttps()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new("https://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.None;
			Invoking(() => ((ISupportValidation) adapterMock.Object).Validate())
				.Should().Throw<ArgumentException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("Invalid address scheme; expecting \"http\" scheme.");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ValidateCustomBasicHttpBindingWithTransportSecurityDoesNotThrowWhenSchemeIsHttps()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new("https://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
			Invoking(() => ((ISupportValidation) adapterMock.Object).Validate()).Should().NotThrow();
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[Fact]
		public void ValidateCustomBasicHttpBindingWithTransportSecurityThrowsWhenSchemeIsHttp()
		{
			var adapterMock = new Mock<WcfCustomAdapterBase<EndpointAddress, BasicHttpBindingElement, CustomTLConfig>>(new ProtocolType()) { CallBase = true };
			adapterMock.Object.Address = new("http://services.stateless.be/soap/default");
			adapterMock.Object.Binding.Security.Mode = BasicHttpSecurityMode.Transport;
			Invoking(() => ((ISupportValidation) adapterMock.Object).Validate())
				.Should().Throw<ArgumentException>()
				.WithInnerException<ArgumentException>()
				.WithMessage("Invalid address scheme; expecting \"https\" scheme.");
		}
	}
}
