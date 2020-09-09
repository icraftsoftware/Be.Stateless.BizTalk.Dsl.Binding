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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dummies.Bindings;
using FluentAssertions;
using Microsoft.BizTalk.B2B.PartnerManagement;
using Microsoft.XLANGs.BaseTypes;
using Xunit;
using static Be.Stateless.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	public class FilterFixture
	{
		[Fact]
		public void BooleanContextPropertyBasedFilter()
		{
			// filter's predicate cannot be made of only a boolean property, as in:
			// var filter = new Filter(() => BtsProperties.AckRequired);
			// embedded C# DSL requires comparison operator and value to be explicit written as follows:
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
				() => (BizTalkFactoryProperties.SenderName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken)
					&& BtsProperties.MessageType == Schema<Any>.MessageType);

			filter.ToString().Should().Be(
				string.Format(
					"<Filter>"
					+ "<Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /><Statement Property=\"{3}\" Operator=\"{4}\" Value=\"{5}\" /></Group>"
					+ "<Group><Statement Property=\"{6}\" Operator=\"{7}\" Value=\"{8}\" /><Statement Property=\"{3}\" Operator=\"{4}\" Value=\"{5}\" /></Group>"
					+ "</Filter>",
					BizTalkFactoryProperties.SenderName.Type.FullName,
					(int) FilterOperator.Equals,
					senderNameToken,
					BtsProperties.MessageType.Type.FullName,
					(int) FilterOperator.Equals,
					Schema<Any>.MessageType,
					BtsProperties.ActualRetryCount.Type.FullName,
					(int) FilterOperator.GreaterThan,
					retryCountToken));
		}

		[Fact]
		public void ConjunctionIsDistributedOverDisjunctionOfFiltersAndDistributionIsCommutative()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(
				() => BtsProperties.MessageType == Schema<Any>.MessageType
					&& (BizTalkFactoryProperties.SenderName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken));

			filter.ToString().Should().Be(
				string.Format(
					"<Filter>"
					+ "<Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /><Statement Property=\"{3}\" Operator=\"{4}\" Value=\"{5}\" /></Group>"
					+ "<Group><Statement Property=\"{0}\" Operator=\"{1}\" Value=\"{2}\" /><Statement Property=\"{6}\" Operator=\"{7}\" Value=\"{8}\" /></Group>"
					+ "</Filter>",
					BtsProperties.MessageType.Type.FullName,
					(int) FilterOperator.Equals,
					Schema<Any>.MessageType,
					BizTalkFactoryProperties.SenderName.Type.FullName,
					(int) FilterOperator.Equals,
					senderNameToken,
					BtsProperties.ActualRetryCount.Type.FullName,
					(int) FilterOperator.GreaterThan,
					retryCountToken));
		}

		[Fact]
		public void ConjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == senderNameToken && BtsProperties.ActualRetryCount > retryCountToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />"
				+ $"<Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void ConstantFilterIsNotSupported()
		{
			var filter = new Filter(() => false);

			Action(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage("Cannot translate FilterPredicate \"False\" because Constant node is not supported.");
		}

		[Fact]
		public void DisjunctionOfConjunctionsOfFilters()
		{
			const string token1 = "BizTalkFactory.Batching";
			const int token2 = 3;

			var filter = new Filter(
				() => BizTalkFactoryProperties.SenderName == token1 || BtsProperties.ActualRetryCount > token2 && BtsProperties.MessageType == Schema<Any>.MessageType);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{token1}\" /></Group>"
				+ $"<Group><Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{token2}\" />"
				+ $"<Statement Property=\"{BtsProperties.MessageType.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{Schema<Any>.MessageType}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		public void DisjunctionOfFilters()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			const int retryCountToken = 3;
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == senderNameToken || BtsProperties.ActualRetryCount > retryCountToken);

			filter.ToString().Should().Be(
				"<Filter>"
				+ $"<Group><Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" /></Group>"
				+ $"<Group><Statement Property=\"{BtsProperties.ActualRetryCount.Type.FullName}\" Operator=\"{(int) FilterOperator.GreaterThan}\" Value=\"{retryCountToken}\" /></Group>"
				+ "</Filter>");
		}

		[Fact]
		public void EqualsBasedFilter()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == senderNameToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{senderNameToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void EqualsNullBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == null);

			Action(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage(
					"Cannot translate FilterPredicate \"() => (BizTalkFactoryProperties.SenderName == null)\" because filter value can be null only if the operator is exists.")
				.WithInnerException<TpmException>();
		}

		[Fact]
		public void EqualsToReceivePortName()
		{
			var receivePort = new TestApplication().ReceivePorts.Find<OneWayReceivePort>();
			var filter = new Filter(() => BtsProperties.ReceivePortName == receivePort.Name);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.ReceivePortName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{((ISupportNamingConvention) receivePort).Name}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void EqualsToSendPortName()
		{
			var sendPort = new TestApplication().SendPorts.Find<TwoWaySendPort>();
			var filter = new Filter(() => BtsProperties.SendPortName == sendPort.Name);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.SendPortName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{((ISupportNamingConvention) sendPort).Name}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void EqualToStringMember()
		{
			var environmentTag = new ImplicitlyStringUnderlainEnvironmentTag("ACC");

			var filter = new Filter(() => BizTalkFactoryProperties.EnvironmentTag == environmentTag.Value);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.EnvironmentTag.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{environmentTag.Value}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void FilterOnEnumLabel()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName == Priority.Highest);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{Priority.Highest}\" />"
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
			var filter = new Filter(() => BizTalkFactoryProperties.EnvironmentTag == environmentTag);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.EnvironmentTag.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{(string) environmentTag}\" />"
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
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName > null);

			Action(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage(
					"Cannot translate FilterPredicate \"() => (BizTalkFactoryProperties.SenderName > null)\" because filter value can be null only if the operator is exists.")
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
			var filter = new Filter(() => BtsProperties.MessageType == Schema<Any>.MessageType);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BtsProperties.MessageType.Type.FullName}\" Operator=\"{(int) FilterOperator.Equals}\" Value=\"{Schema<Any>.MessageType}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void NAryConjunction()
		{
			var filter = new Filter(
				() => BtsProperties.ActualRetryCount > 3
					&& BtsProperties.MessageType == Schema<Any>.MessageType
					&& BtsProperties.SendPortName == "Dummy port name"
					&& BtsProperties.IsRequestResponse != true);
			Action(() => filter.ToString()).Should().NotThrow();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void NAryDisjunction()
		{
			var filter = new Filter(
				() => BizTalkFactoryProperties.SenderName == "BizTalkFactory.Batching"
					|| BtsProperties.ActualRetryCount > 3
					|| BtsProperties.AckRequired != true
					|| BtsProperties.InboundTransportLocation == "inbound-transport-location");
			Action(() => filter.ToString()).Should().NotThrow();
		}

		[Fact]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void NonMessageContextPropertyBasedFilterIsNotSupported()
		{
			var filter = new Filter(() => GetType().Name == "any value");

			Action(() => filter.ToString())
				.Should().Throw<NotSupportedException>()
				.WithMessage(
					"Cannot translate property Expression \"value(Be.Stateless.BizTalk.Dsl.Binding.Subscription.FilterFixture).GetType().Name\" because only MessageContextProperty<T, TR>-derived type's member access expressions are supported.");
		}

		[Fact]
		public void NotEqualsBasedFilter()
		{
			const string senderNameToken = "BizTalkFactory.Batching";
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName != senderNameToken);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.NotEqual}\" Value=\"{senderNameToken}\" />"
				+ "</Group></Filter>");
		}

		[Fact]
		public void NotEqualsNullBasedFilterIsRewrittenAsExistsOperator()
		{
			var filter = new Filter(() => BizTalkFactoryProperties.SenderName != null);

			filter.ToString().Should().Be(
				"<Filter><Group>"
				+ $"<Statement Property=\"{BizTalkFactoryProperties.SenderName.Type.FullName}\" Operator=\"{(int) FilterOperator.Exists}\" />"
				+ "</Group></Filter>");
		}

		public static IEnumerable<object> ConjunctionFilters
		{
			get
			{
				var messageType = Schema<Any>.MessageType;
				const string operation = "Operation";
				const string senderName = "BizTalkFactory.Accumulator";

				// Scalar
				yield return new[] {
					new Filter(() => BizTalkFactoryProperties.SenderName == senderName),
					new Filter(() => BizTalkFactoryProperties.SenderName == senderName)
				};

				// Conjunction
				yield return new[] {
					new Filter(() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType),
					new Filter(() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType)
				};

				// Disjunction
				yield return new[] {
					new Filter(() => BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3),
					new Filter(() => BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
				};

				// ConjunctionAndBinaryDisjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
							&& BtsProperties.MessageType == messageType),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType)
				};

				// ConjunctionAndTernaryDisjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3 || BtsProperties.Operation > operation)
							&& BtsProperties.MessageType == messageType),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.Operation > operation && BtsProperties.MessageType == messageType)
				};

				// ConjunctionOfBinaryDisjunctions
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (BtsProperties.MessageType == messageType || BtsProperties.Operation == operation)),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.Operation == operation
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.Operation == operation)
				};

				// ConjunctionOfBinaryDisjunctionsWhoseOneWithNestedConjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "op2")),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "op2"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "op2")
				};

				// ConjunctionOfBinaryDisjunctionsWhoseOneWithNestedConjunctionAndDisjunction
				yield return new[] {
					new Filter(
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (
								BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1"
								&& (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3"))),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.SenderName == senderName && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BtsProperties.ActualRetryCount > 3 && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3")
				};

				// ConjunctionOfBinaryDisjunctionsEachWithNestedConjunctionAndDisjunction
				yield return new[] {
					new Filter(
						() => (
							BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3
							&& (BtsProperties.AckRequired == true || BtsProperties.SendPortName == "SP")
						) && (
							BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1"
							&& (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3"))),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.SenderName == senderName && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
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
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (
								BtsProperties.AckRequired == true
								|| BtsProperties.SendPortName == "SP" && (BtsProperties.MessageDestination == "M.D" || BtsProperties.Operation == operation))),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.AckRequired == true
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D"
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation
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
						() => (BizTalkFactoryProperties.SenderName == senderName || BtsProperties.ActualRetryCount > 3)
							&& (BtsProperties.AckRequired == true || BtsProperties.SendPortName == "SP" && (BtsProperties.MessageDestination == "M.D" || BtsProperties.Operation == operation))
							&& (BtsProperties.MessageType == messageType || SBMessagingProperties.Label == "v1" && (BtsProperties.Operation == "v2" || EdiProperties.BGM1_1 == "v3"))
					),
					new Filter(
						() => BizTalkFactoryProperties.SenderName == senderName && BtsProperties.AckRequired == true && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.AckRequired == true && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && BtsProperties.MessageType == messageType
							|| BtsProperties.ActualRetryCount > 3 && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && BtsProperties.MessageType == messageType
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.AckRequired == true && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.MessageDestination == "M.D" && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && SBMessagingProperties.Label == "v1" && BtsProperties.Operation == "v2"
							|| BizTalkFactoryProperties.SenderName == senderName && BtsProperties.SendPortName == "SP" && BtsProperties.Operation == operation && SBMessagingProperties.Label == "v1" && EdiProperties.BGM1_1 == "v3"
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
