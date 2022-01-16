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

using Be.Stateless.BizTalk.Install;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	[Collection("DeploymentContext")]
	public class RetryPolicyFixture
	{
		[Theory]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		[InlineData("ANYWHERE")]
		public void LongRunningIsLongRunningForPreProductionOrProductionOrAnyOtherTargetEnvironments(string targetEnvironment)
		{
			var sut = RetryPolicy.LongRunning;
			((ISupportEnvironmentOverride) sut).ApplyEnvironmentOverrides(targetEnvironment);
			sut.Should().BeEquivalentTo(RetryPolicy.ActualLongRunning);
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		public void LongRunningIsRealTimeForDevelopmentOrBuildTargetEnvironments(string targetEnvironment)
		{
			var sut = RetryPolicy.LongRunning;
			((ISupportEnvironmentOverride) sut).ApplyEnvironmentOverrides(targetEnvironment);
			sut.Should().BeEquivalentTo(RetryPolicy.RealTime);
		}

		[Theory]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		public void LongRunningIsShortRunningForAcceptanceTargetEnvironment(string targetEnvironment)
		{
			var sut = RetryPolicy.LongRunning;
			((ISupportEnvironmentOverride) sut).ApplyEnvironmentOverrides(targetEnvironment);
			sut.Should().BeEquivalentTo(RetryPolicy.ActualShortRunning);
		}

		[Fact]
		public void RealTimeIsTargetEnvironmentInsensitive()
		{
			(RetryPolicy.RealTime as ISupportEnvironmentOverride).Should().BeNull();
		}

		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		public void ShortRunningIsRealTimeForDevelopmentOrBuildOrAcceptanceTargetEnvironments(string targetEnvironment)
		{
			var sut = RetryPolicy.ShortRunning;
			((ISupportEnvironmentOverride) sut).ApplyEnvironmentOverrides(targetEnvironment);
			sut.Should().BeEquivalentTo(RetryPolicy.RealTime);
		}

		[Theory]
		[InlineData(TargetEnvironment.PREPRODUCTION)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		[InlineData("ANYWHERE")]
		public void ShortRunningIsShortRunningForPreProductionOrProductionOrAnyOtherTargetEnvironments(string targetEnvironment)
		{
			var sut = RetryPolicy.ShortRunning;
			((ISupportEnvironmentOverride) sut).ApplyEnvironmentOverrides(targetEnvironment);
			sut.Should().BeEquivalentTo(RetryPolicy.ActualShortRunning);
		}
	}
}
