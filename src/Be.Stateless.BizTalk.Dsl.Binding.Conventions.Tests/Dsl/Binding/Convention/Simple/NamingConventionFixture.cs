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

using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Dummies.Bindings.Simple;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.Finance;
using Be.Stateless.Resources;
using FluentAssertions;
using Microsoft.BizTalk.B2B.PartnerManagement;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple
{
	[Collection("DeploymentContext")]
	public class NamingConventionFixture
	{
		[SkippableFact]
		public void ConventionalApplicationBindingSupportsBindingGeneration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var applicationBindingSerializer = new SampleApplication().GetApplicationBindingInfoSerializer();

				var binding = applicationBindingSerializer.Serialize();

				XDocument.Parse(binding).Should().BeEquivalentTo(
					ResourceManager.Load(
						Assembly.GetExecutingAssembly(),
						"Be.Stateless.BizTalk.Resources.Simple.Application.Bindings.xml",
						XDocument.Load));
			}
		}

		[SkippableFact]
		public void ConventionalApplicationBindingWithAggregateSupportsBindingGeneration()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var applicationBindingSerializer = new FinanceSampleApplication().GetApplicationBindingInfoSerializer();

				var binding = applicationBindingSerializer.Serialize();

				XDocument.Parse(binding).Should().BeEquivalentTo(
					ResourceManager.Load(
						Assembly.GetExecutingAssembly(),
						"Be.Stateless.BizTalk.Resources.Simple.Finance.Application.Bindings.xml",
						XDocument.Load));
			}
		}

		[SkippableFact]
		public void ConventionalReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var receivePort = new SampleApplication().BatchReceivePort;
				var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

				filter.ToString().Should().Be(
					string.Format(
						CultureInfo.InvariantCulture,
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name));
			}
		}

		[SkippableFact]
		public void ConventionalSendPortNameCanBeReferencedInSubscriptionFilter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var sendPort = new SampleApplication().UnitTestSendPort;
				var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

				filter.ToString().Should().Be(
					string.Format(
						CultureInfo.InvariantCulture,
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.SendPortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) sendPort).Name));
			}
		}

		[SkippableFact]
		public void ConventionalStandaloneReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);
			using (new DeploymentContextInjectionScope(targetEnvironment: "ANYWHERE"))
			{
				var receivePort = new SampleApplication().ReceivePorts.Find<StandaloneReceivePort>();
				var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

				filter.ToString().Should().Be(
					string.Format(
						CultureInfo.InvariantCulture,
						"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
						BtsProperties.ReceivePortName.Type.FullName,
						(int) FilterOperator.Equals,
						((ISupportNamingConvention) receivePort).Name));
			}
		}
	}
}
