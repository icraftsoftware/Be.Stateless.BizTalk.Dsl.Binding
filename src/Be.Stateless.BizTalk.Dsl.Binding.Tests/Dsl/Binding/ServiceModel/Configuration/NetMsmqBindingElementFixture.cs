﻿#region Copyright & License

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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.BizTalk.Explorer;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	public class NetMsmqBindingElementFixture
	{
		[SkippableFact]
		[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
		public void NetMsmqBindingElementDecoratorCanBeUsedAsWcfCustomAdapterTypeParameter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			Invoking(() => new WcfCustomAdapter.Inbound<NetMsmqBindingElement>(a => { a.Binding.ExactlyOnce = true; })).Should().NotThrow();
		}

		[Fact]
		public void SerializeToXml()
		{
			var binding = new NetMsmqBindingElement { ExactlyOnce = false };
			binding.GetBindingElementXml("netMsmqBinding").Should().Be("<binding name=\"netMsmqBinding\" exactlyOnce=\"false\" retryCycleDelay=\"00:10:00\" />");
		}
	}
}
