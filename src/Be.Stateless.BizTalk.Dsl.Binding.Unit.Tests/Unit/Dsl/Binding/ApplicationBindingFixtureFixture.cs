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

using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Unit.Dsl.Binding
{
	public class ApplicationBindingFixtureFixture : ApplicationBindingFixture<ApplicationBindingFixtureFixture.Application>
	{
		[SkippableFact]
		public void GenerateApplicationBindingForTargetEnvironment()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Invoking(() => GenerateApplicationBindingForTargetEnvironment("ANYWHERE"))
				.Should().Throw<BindingException>()
				.WithMessage("[ReceivePort] Receive Port's Receive Locations are not defined.");
		}

		public class Application : ApplicationBinding
		{
			public Application()
			{
				Name = "Dummy";
				ReceivePorts.Add(ReceivePort(rp => { rp.Name = "ReceivePort"; }));
			}
		}
	}
}
