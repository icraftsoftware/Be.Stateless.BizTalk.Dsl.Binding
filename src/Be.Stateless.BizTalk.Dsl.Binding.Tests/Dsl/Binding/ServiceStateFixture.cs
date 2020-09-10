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

extern alias ExplorerOM;
using ExplorerOM::Microsoft.BizTalk.ExplorerOM;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ServiceStateFixture
	{
		[Fact]
		public void CastToOrchestrationStatus()
		{
			((OrchestrationStatus) ServiceState.Unenlisted).Should().Be(OrchestrationStatus.Unenlisted);
			((OrchestrationStatus) ServiceState.Enlisted).Should().Be(OrchestrationStatus.Enlisted);
			((OrchestrationStatus) ServiceState.Stopped).Should().Be(OrchestrationStatus.Enlisted);
			((OrchestrationStatus) ServiceState.Started).Should().Be(OrchestrationStatus.Started);
		}

		[Fact]
		public void CastToPortStatus()
		{
			((PortStatus) ServiceState.Unenlisted).Should().Be(PortStatus.Bound);
			((PortStatus) ServiceState.Enlisted).Should().Be(PortStatus.Stopped);
			((PortStatus) ServiceState.Stopped).Should().Be(PortStatus.Stopped);
			((PortStatus) ServiceState.Started).Should().Be(PortStatus.Started);
		}

		[Fact]
		public void EnlistedIsStopped()
		{
			ServiceState.Enlisted.Should().Be(ServiceState.Stopped);
		}
	}
}
