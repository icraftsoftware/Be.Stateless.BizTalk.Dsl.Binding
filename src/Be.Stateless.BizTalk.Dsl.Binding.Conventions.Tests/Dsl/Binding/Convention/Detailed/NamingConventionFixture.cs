﻿#region Copyright & License

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
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Be.Stateless.Banking;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.Resources;
using FluentAssertions;
using Microsoft.BizTalk.B2B.PartnerManagement;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed
{
	public class NamingConventionFixture : IDisposable
	{
		[Fact]
		public void ConventionalApplicationBindingSupportsBindingGeneration()
		{
			var applicationBinding = new SampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = applicationBinding.GetApplicationBindingInfoSerializer();

			var binding = applicationBindingSerializer.Serialize();

			XDocument.Parse(binding).Should().BeEquivalentTo(
				ResourceManager.Load(
					Assembly.GetExecutingAssembly(),
					"Be.Stateless.BizTalk.Resources.Detailed.Application.Bindings.xml",
					XDocument.Load));
		}

		[Fact]
		public void ConventionalApplicationBindingWithAggregateSupportsBindingGeneration()
		{
			var applicationBinding = new BankingSampleApplication {
				Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local)
			};
			var applicationBindingSerializer = applicationBinding.GetApplicationBindingInfoSerializer();

			var binding = applicationBindingSerializer.Serialize();

			XDocument.Parse(binding).Should().BeEquivalentTo(
				ResourceManager.Load(
					Assembly.GetExecutingAssembly(),
					"Be.Stateless.BizTalk.Resources.Detailed.Banking.Application.Bindings.xml",
					XDocument.Load));
		}

		[Fact]
		public void ConventionalReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			var receivePort = new SampleApplication().CustomerOneWayReceivePort;
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			filter.ToString().Should().Be(
				string.Format(
					CultureInfo.InvariantCulture,
					"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
					BtsProperties.ReceivePortName.Type.FullName,
					(int) FilterOperator.Equals,
					((ISupportNamingConvention) receivePort).Name));
		}

		[Fact]
		public void ConventionalSendPortNameCanBeReferencedInSubscriptionFilter()
		{
			var sendPort = new SampleApplication().CustomerTwoWaySendPort;
			var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

			filter.ToString().Should().Be(
				string.Format(
					CultureInfo.InvariantCulture,
					"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
					BtsProperties.SendPortName.Type.FullName,
					(int) FilterOperator.Equals,
					((ISupportNamingConvention) sendPort).Name));
		}

		[Fact]
		public void ConventionalStandaloneReceivePortNameCanBeReferencedInSubscriptionFilter()
		{
			var receivePort = new SampleApplication().TaxAgencyOneWayReceivePort;
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			filter.ToString().Should().Be(
				string.Format(
					CultureInfo.InvariantCulture,
					"<Filter><Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /></Group></Filter>",
					BtsProperties.ReceivePortName.Type.FullName,
					(int) FilterOperator.Equals,
					((ISupportNamingConvention) receivePort).Name));
		}

		public NamingConventionFixture()
		{
			BindingGenerationContext.TargetEnvironment = "ANYTHING";
		}

		public void Dispose()
		{
			BindingGenerationContext.TargetEnvironment = null;
		}
	}
}
