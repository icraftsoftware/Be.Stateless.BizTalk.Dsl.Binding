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
using System.ServiceModel;
using System.ServiceModel.Configuration;
using FluentAssertions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;
using Moq;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class WcfAdapterBaseFixture
	{
		[Fact]
		public void ValidateThrowsIfAddressIsNull()
		{
			var adapterMock = new Mock<WcfAdapterBase<EndpointAddress, NetMsmqBindingElement, NetMsmqRLConfig>>(new ProtocolType()) { CallBase = true };
			var validatingMock = adapterMock.As<ISupportValidation>();
			Action(() => validatingMock.Object.Validate())
				.Should().Throw<ArgumentException>()
				.WithMessage("Required property Address (URI) not specified.");
		}
	}
}
