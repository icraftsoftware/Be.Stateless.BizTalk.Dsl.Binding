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

using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Moq;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor.Pipeline
{
	public class VisitorPipelineFixture
	{
		[Fact]
		public void AcceptsAndPropagatesVisitorToCurrentStage()
		{
			var visitorMock = new Mock<IApplicationBindingVisitor>();

			var sut = new VisitorPipeline<string>(new Mock<IApplicationBinding<string>>().Object);
			var stageMock = new Mock<ProcessingStage<string>>(sut);
			sut.Stage = stageMock.Object;

			sut.Accept(visitorMock.Object);

			stageMock.Verify(m => m.Accept(visitorMock.Object));
		}

		[SkippableFact]
		public void ValidationHappensAfterEnvironmentOverrideApplication()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				IVisitable<IApplicationBindingVisitor> app = new ApplicationBinding(
					ab => {
						ab.Name = "Application";
						ab.SendPorts.Add(new OneWaySendPort());
					});
				var visitor = new ApplicationBindingValidator();
				Invoking(() => app.Accept(visitor)).Should().NotThrow();
			}
		}

		private class OneWaySendPort : SendPort
		{
			public OneWaySendPort()
			{
				Name = "SendPort";
				SendPipeline = new SendPipeline<XmlTransmit>();
				Transport.Adapter = new WcfWebHttpAdapter.Outbound(a => { a.Address = new("https://some.domain.com/service/api"); });
				Transport.Host = "Send Host";
			}

			#region Base Class Member Overrides

			protected override void ApplyEnvironmentOverrides(string environment)
			{
				((WcfWebHttpAdapter.Outbound) Transport.Adapter).SecurityMode = WebHttpSecurityMode.Transport;
			}

			#endregion
		}
	}
}
