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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Explorer;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Schema;
using FluentAssertions;
using Microsoft.BizTalk.B2B.PartnerManagement;
using Microsoft.XLANGs.BaseTypes;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	public class FilterFixture
	{
		[Fact]
		public void BooleanContextPropertyBasedFilter()
		{
			// filter's predicate cannot be made of only a boolean property, assumed to be true, as in:
			// var filter = new Filter(() => BtsProperties.AckRequired);

			// embedded C# DSL requires comparison operator and value to be explicitly written as follows:
			var filter = new Filter(() => BtsProperties.AckRequired == true);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.AckRequired.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"True\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void CombiningCompoundFiltersByConjunction()
		{
			var expectedFilter = new Filter(
				() => (BtsProperties.AckRequired == true || BtsProperties.ActualRetryCount > 3)
					&& (BtsProperties.MessageType == "type" || BtsProperties.MessageDestination == "MD"));

			var actualFilter = new Filter(() => BtsProperties.AckRequired == true || BtsProperties.ActualRetryCount > 3)
				&& new Filter(() => BtsProperties.MessageType == "type" || BtsProperties.MessageDestination == "MD");

			actualFilter.ToString().Should().Be(expectedFilter.ToString());
		}

		[Fact]
		public void CombiningCompoundFiltersByDisjunction()
		{
			var expectedFilter = new Filter(
				() => BtsProperties.AckRequired == true && BtsProperties.ActualRetryCount > 3
					|| BtsProperties.MessageType == "type" && BtsProperties.MessageDestination == "MD");

			var actualFilter = new Filter(() => BtsProperties.AckRequired == true && BtsProperties.ActualRetryCount > 3)
				|| new Filter(() => BtsProperties.MessageType == "type" && BtsProperties.MessageDestination == "MD");

			actualFilter.ToString().Should().Be(expectedFilter.ToString());
		}

		[Fact]
		public void CombiningFiltersByConjunction()
		{
			var expectedFilter = new Filter(() => BtsProperties.AckRequired == true && BtsProperties.MessageType == "type");

			var actualFilter = new Filter(() => BtsProperties.AckRequired == true) && new Filter(() => BtsProperties.MessageType == "type");

			actualFilter.ToString().Should().Be(expectedFilter.ToString());
		}

		[Fact]
		public void CombiningFiltersByDisjunction()
		{
			var expectedFilter = new Filter(() => BtsProperties.AckRequired == true || BtsProperties.MessageType == "type");

			var actualFilter = new Filter(() => BtsProperties.AckRequired == true) || new Filter(() => BtsProperties.MessageType == "type");

			actualFilter.ToString().Should().Be(expectedFilter.ToString());
		}

		[Theory]
		[MemberData(nameof(ConjunctionFilters))]
		public void ConjunctionIsDistributed(Filter actualFilter, Filter expectedFilter)
		{
			actualFilter.ToString().Should().Be(expectedFilter.ToString());
		}

		[Fact]
		public void ConjunctionIsDistributedOverDisjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(
				() => (
						BizTalkFactoryProperties.MapTypeName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken)
					&& BtsProperties.MessageType == SchemaMetadata.For<Any>().MessageType
			);

			filter.ToString().Should().Be(
				string.Format(
					"<Filter><Group>" + (
						$"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />" +
						"<Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" />") +
					"</Group><Group>" + (
						$"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" />" +
						"<Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" />") +
					"</Group></Filter>",
					BtsProperties.MessageType.Type.FullName,
					(int) FilterOperator.Equals,
					SchemaMetadata.For<Any>().MessageType));
		}

		[Fact]
		public void ConjunctionIsDistributedOverDisjunctionOfFiltersAndDistributionIsCommutative()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(
				() => BtsProperties.MessageType == SchemaMetadata.For<Any>().MessageType
					&& (BizTalkFactoryProperties.MapTypeName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken)
			);

			filter.ToString().Should().Be(
				string.Format(
					"<Filter><Group>" + (
						"<Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" />" +
						$"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />") +
					"</Group><Group>" + (
						"<Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" />" +
						$"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" />") +
					"</Group></Filter>",
					BtsProperties.MessageType.Type.FullName,
					(int) FilterOperator.Equals,
					SchemaMetadata.For<Any>().MessageType));
		}

		[Fact]
		public void ConjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName == senderNameToken && BtsProperties.ActualRetryCount > retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void ConstantFilterIsNotSupported()
		{
			var filter = new Filter(() => false);

			Invoking(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage("Cannot translate FilterPredicate \"False\" because Constant node is not supported.");
		}

		[Fact]
		public void DisjunctionOfConjunctionsOfFilters()
		{
			const string token1 = "BizTalkFactory.Batching";
			const int token2 = 3;

			var filter = new Filter(
				() => BizTalkFactoryProperties.MapTypeName == token1 || BtsProperties.ActualRetryCount > token2
					&& BtsProperties.MessageType == SchemaMetadata.For<Any>().MessageType
			);

			filter.ToString().Should().Be(
				"<Filter><Group>" + (
					$"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{token1}\" />") +
				"</Group><Group>" + (
					$"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{token2}\" />" +
					$"<Statement Property=\"{BtsProperties.MessageType.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{SchemaMetadata.For<Any>().MessageType}\" />") +
				"</Group></Filter>");
		}

		[Fact]
		public void DisjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>" + (
					$"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />") +
				"</Group><Group>" + (
					$"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" />") +
				"</Group></Filter>");
		}

		[Fact]
		public void EqualsBasedFilter()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName == senderNameToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void EqualsInstanceFieldFilter()
		{
			var value = new InstanceTokens().Field;
			var filter = new Filter(() => SBMessagingProperties.LockToken == value);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{SBMessagingProperties.LockToken.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{value}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		public void EqualsInstancePropertyFilter()
		{
			var value = new InstanceTokens().Property;
			var filter = new Filter(() => SBMessagingProperties.LockToken == value);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{SBMessagingProperties.LockToken.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{value}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void EqualsNullBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName == null);

			Invoking(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage("Cannot translate FilterPredicate \"() => (BizTalkFactoryProperties.MapTypeName == null)\" because filter value can be null only if the operator is exists.")
				.WithInnerException<TpmException>();
		}

		[Fact]
		public void EqualsStaticConstantFilter()
		{
			var filter = new Filter(() => SBMessagingProperties.LockToken == StaticTokens.Constant);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{SBMessagingProperties.LockToken.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{StaticTokens.Constant}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		public void EqualsStaticFieldFilter()
		{
			var filter = new Filter(() => SBMessagingProperties.LockToken == StaticTokens.Field);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{SBMessagingProperties.LockToken.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{StaticTokens.Field}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		public void EqualsStaticMethodFilter()
		{
			var filter = new Filter(() => SBMessagingProperties.LockToken == StaticTokens.Method());

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{SBMessagingProperties.LockToken.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{StaticTokens.Method()}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		public void EqualsStaticPropertyFilter()
		{
			var filter = new Filter(() => SBMessagingProperties.LockToken == StaticTokens.Property);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{SBMessagingProperties.LockToken.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{StaticTokens.Property}\" /></Group>"
				+ "</Filter>");
		}

		[SkippableFact]
		public void EqualsToReceivePortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var receivePort = new TestApplication().ReceivePorts.Find<OneWayReceivePort>();
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ReceivePortName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{receivePort.ResolveName()}\" />"
				+ "</Group></Filter>");
		}

		[SkippableFact]
		public void EqualsToSendPortName()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sendPort = new TestApplication().SendPorts.Find<TwoWaySendPort>();
			var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.SendPortName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{sendPort.ResolveName()}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void EqualToStringMember()
		{
			var environmentTag = new ImplicitlyStringUnderlainEnvironmentTag(TargetEnvironment.ACCEPTANCE);

			var filter = new Filter(() => BizTalkFactoryProperties.OutboundTransportLocation == environmentTag.Value);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.OutboundTransportLocation.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{environmentTag.Value}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void FilterOnEnumLabel()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName == Priority.Highest);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{Priority.Highest}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void FilterOnEnumValue()
		{
			var filter = new Filter(() => BtsProperties.ActualRetryCount == (int) Priority.BelowNormal);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{(int) Priority.BelowNormal}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void FilterOnTypeWithImplicitStringCastOperator()
		{
			var environmentTag = new ImplicitlyStringUnderlainEnvironmentTag("TAG");
			var filter = new Filter(() => BizTalkFactoryProperties.OutboundTransportLocation == environmentTag);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.OutboundTransportLocation.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{(string) environmentTag}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void GreaterThanBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount > retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void GreaterThanNullBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName > null);

			Invoking(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage("Cannot translate FilterPredicate \"() => (BizTalkFactoryProperties.MapTypeName > null)\" because filter value can be null only if the operator is exists.")
				.WithInnerException<TpmException>();
		}

		[Fact]
		public void GreaterThanOrEqualsBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount >= retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThanOrEquals}\" Value=\"{retryCountToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void LessThanBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount < retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.LessThan}\" Value=\"{retryCountToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void LessThanOrEqualsBasedFilter()
		{
			const int retryCountToken = 3;
			var filter = new Filter(() => BtsProperties.ActualRetryCount <= retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.LessThanOrEquals}\" Value=\"{retryCountToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void MessageTypeBasedFilter()
		{
			var filter = new Filter(() => BtsProperties.MessageType == SchemaMetadata.For<Any>().MessageType);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.MessageType.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{SchemaMetadata.For<Any>().MessageType}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void NAryConjunction()
		{
			var filter = new Filter(
				() => BtsProperties.ActualRetryCount > 3
					&& BtsProperties.MessageType == SchemaMetadata.For<Any>().MessageType
					&& BtsProperties.SendPortName == "Dummy port name"
					&& BtsProperties.IsRequestResponse != true
			);
			Invoking(() => filter.ToString()).Should().NotThrow();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void NAryDisjunction()
		{
			var filter = new Filter(
				() => BizTalkFactoryProperties.MapTypeName == "BizTalkFactory.Batching"
					|| BtsProperties.ActualRetryCount > 3
					|| BtsProperties.AckRequired != true
					|| BtsProperties.InboundTransportLocation == "inbound-transport-location");
			Invoking(() => filter.ToString()).Should().NotThrow();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void NonMessageContextPropertyBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => GetType().Name == "any value");

			Invoking(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage(
					"Cannot translate property Expression \"value(Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterFixture).GetType().Name\" because only MessageContextProperty<T, TR>-derived type's member access expressions are supported.");
		}

		[Fact]
		public void NotEqualsBasedFilter()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName != senderNameToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.NotEqual}\" Value=\"{senderNameToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void NotEqualsNullBasedFilterIsRewrittenAsExistsOperator()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.MapTypeName != null);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.MapTypeName.Type.FullName}\" Operator=\"{(int) FilterOperator.Exists}\" />"
				+ "</Group></Filter>");
		}

		private class InstanceTokens
		{
			[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
			public string Property => "InstanceTokens.Property";

			[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
			[SuppressMessage("ReSharper", "InconsistentNaming")]
			public readonly string Field = "InstanceTokens.Field";
		}

		private static class StaticTokens
		{
			public static string Property => "StaticTokens.Property";

			public static string Method()
			{
				return "StaticTokens.Method";
			}

			[SuppressMessage("ReSharper", "InconsistentNaming")]
			public const string Constant = "StaticTokens.Constant";

			[SuppressMessage("ReSharper", "ConvertToConstant.Local")]
			public static readonly string Field = "StaticTokens.Field";
		}

		public static IEnumerable<object> ConjunctionFilters
		{
			get
			{
				var messageType = SchemaMetadata.For<Any>().MessageType;
				const string operation = "Operation";
				const string senderName = "BizTalkFactory.Accumulator";

				// Scalar
				yield return new[] {
					new Filter(() => BizTalkFactoryProperties.MapTypeName == senderName),
					new Filter(() => BizTalkFactoryProperties.MapTypeName == senderName)
				};

				// Conjunction
				yield return new[] {
					new Filter(() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType),
					new Filter(() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType)
				};

				// Disjunction
				yield return new[] {
					new Filter(() => BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3),
					new Filter(() => BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
				};

				// ConjunctionAndBinaryDisjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
							&& BtsProperties.MessageType == messageType),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType)
				};

				// ConjunctionAndTernaryDisjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3 || BtsProperties.Operation > operation)
							&& BtsProperties.MessageType == messageType),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.Operation > operation && BtsProperties.MessageType == messageType)
				};

				// ConjunctionOfBinaryDisjunctions
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (BtsProperties.MessageType == messageType || BtsProperties.Operation == operation)),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.Operation == operation
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.Operation == operation)
				};

				// ConjunctionOfBinaryDisjunctionsWhoseOneWithNestedConjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "op2")),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "op2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "op2")
				};

				// ConjunctionOfBinaryDisjunctionsWhoseOneWithNestedConjunctionAndDisjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (
								BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1"
								&& (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3"))),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.MapTypeName == senderName && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3")
				};

				// ConjunctionOfBinaryDisjunctionsEachWithNestedConjunctionAndDisjunction
				yield return new[] {
					new Filter(
						() => (
							BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3
							&& (BtsProperties.AckRequired == true || BtsProperties.SendPortName == "SP")
						) && (
							BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1"
							&& (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3"))),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.MapTypeName == senderName && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3")
				};

				// ConjunctionMixingLeft
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (
								BtsProperties.AckRequired == true
								|| BtsProperties.SendPortName == "SP" && (BtsProperties.MessageDestination == "M.D" || BtsProperties.Operation == operation))),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.AckRequired == true
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D"
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation)
				};

				// ConjunctionMixingRight
				yield return new[] {
					new Filter(
						() => BtsProperties.MessageType == messageType
							|| SBMessagingProperties.Label == "v1" && (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3")),
					new Filter(
						() => BtsProperties.MessageType == messageType
							|| SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3")
				};

				// @formatter:off
				// ConjunctionMixingLeftAndRight
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.MapTypeName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (BtsProperties.AckRequired == true || BtsProperties.SendPortName == "SP" && (BtsProperties.MessageDestination == "M.D" || BtsProperties.Operation == operation))
							&& (BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1" && (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3"))
					),
					new Filter(
						() => BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.AckRequired == true && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.MapTypeName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
					)
				};
				// @formatter:on
			}
		}

		private class ImplicitlyStringUnderlainEnvironmentTag
		{
			#region Operators

			public static implicit operator string(ImplicitlyStringUnderlainEnvironmentTag environmentTag)
			{
				return environmentTag.Value;
			}

			#endregion

			public ImplicitlyStringUnderlainEnvironmentTag(string tag)
			{
				Value = tag;
			}

			public string Value { get; }
		}
	}
}
