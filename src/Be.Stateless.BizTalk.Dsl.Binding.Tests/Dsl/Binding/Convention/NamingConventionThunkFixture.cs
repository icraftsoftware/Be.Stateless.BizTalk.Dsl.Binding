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
using FluentAssertions;
using Moq;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class NamingConventionThunkFixture
	{
		[Fact]
		public void ComputeApplicationNameViaApplicationBinding()
		{
			const string name = "Application Name";

			var applicationBindingMock = new Mock<IApplicationBinding<object>>();
			applicationBindingMock.Setup(m => m.Name).Returns(name);

			NamingConventionThunk.ComputeApplicationName(applicationBindingMock.Object).Should().Be(name);
		}

		[Fact]
		public void ComputeApplicationNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var applicationBindingMock = new Mock<IApplicationBinding<object>>();
			applicationBindingMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeApplicationName(applicationBindingMock.Object);

			conventionMock.Verify(m => m.ComputeApplicationName(applicationBindingMock.Object), Times.Once());
		}

		[Fact]
		public void ComputeReceiveLocationNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var receiveLocationMock = new Mock<IReceiveLocation<object>>();
			receiveLocationMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeReceiveLocationName(receiveLocationMock.Object);

			conventionMock.Verify(m => m.ComputeReceiveLocationName(receiveLocationMock.Object), Times.Once());
		}

		[Fact]
		public void ComputeReceiveLocationNameViaReceiveLocation()
		{
			const string name = "Receive Location Name";

			var receiveLocationMock = new Mock<IReceiveLocation<object>>();
			receiveLocationMock.Setup(m => m.Name).Returns(name);

			NamingConventionThunk.ComputeReceiveLocationName(receiveLocationMock.Object).Should().Be(name);
		}

		[Fact]
		public void ComputeReceivePortNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var receivePortMock = new Mock<IReceivePort<object>>();
			receivePortMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeReceivePortName(receivePortMock.Object);

			conventionMock.Verify(m => m.ComputeReceivePortName(receivePortMock.Object), Times.Once());
		}

		[Fact]
		public void ComputeReceivePortNameViaReceivePort()
		{
			const string name = "Receive Port Name";

			var receivePortMock = new Mock<IReceivePort<object>>();
			receivePortMock.Setup(m => m.Name).Returns(name);

			NamingConventionThunk.ComputeReceivePortName(receivePortMock.Object).Should().Be(name);
		}

		[Fact]
		public void ComputeSendPortNameViaNamingConvention()
		{
			var conventionMock = new Mock<INamingConvention<object>>();

			var sendPortMock = new Mock<ISendPort<object>>();
			sendPortMock.Setup(m => m.Name).Returns(conventionMock.Object);

			NamingConventionThunk.ComputeSendPortName(sendPortMock.Object);

			conventionMock.Verify(m => m.ComputeSendPortName(sendPortMock.Object), Times.Once());
		}

		[Fact]
		public void ComputeSendPortNameViaSendPort()
		{
			const string name = "Send Port Name";

			var sendPortMock = new Mock<ISendPort<object>>();
			sendPortMock.Setup(m => m.Name).Returns(name);

			NamingConventionThunk.ComputeSendPortName(sendPortMock.Object).Should().Be(name);
		}

		[Fact]
		public void WrapExceptionInNamingConventionException()
		{
			var conventionMock = new Mock<INamingConvention<object>>();
			conventionMock.Setup(m => m.ComputeApplicationName(It.IsAny<IApplicationBinding<object>>())).Throws<NotSupportedException>();

			var applicationBindingMock = new Mock<IApplicationBinding<object>>();
			applicationBindingMock.Setup(m => m.Name).Returns(conventionMock.Object);

			Action(() => NamingConventionThunk.ComputeApplicationName(applicationBindingMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithInnerException<NotSupportedException>();
		}
	}
}
