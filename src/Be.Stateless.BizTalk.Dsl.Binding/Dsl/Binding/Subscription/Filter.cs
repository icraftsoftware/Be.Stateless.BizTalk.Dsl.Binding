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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	public class Filter
	{
		#region Operators

		public static Filter operator &(Filter left, Filter right)
		{
			if (left == null) throw new ArgumentNullException(nameof(left));
			if (right == null) throw new ArgumentNullException(nameof(right));
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-and-operator
			return new(
				Expression.Lambda<Func<bool>>(
					Expression.MakeBinary(
						ExpressionType.AndAlso,
						left._predicate.Body,
						right._predicate.Body)));
		}

		public static Filter operator |(Filter left, Filter right)
		{
			if (left == null) throw new ArgumentNullException(nameof(left));
			if (right == null) throw new ArgumentNullException(nameof(right));
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/conditional-or-operator
			return new(
				Expression.Lambda<Func<bool>>(
					Expression.MakeBinary(
						ExpressionType.OrElse,
						left._predicate.Body,
						right._predicate.Body)));
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public DSL API.")]
		public static bool operator false(Filter filter)
		{
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#user-defined-conditional-logical-operators
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/false-operator
			// return opposite of what expected to prevent short-circuit evaluation as actual intent is to return a new expression tree
			return false;
		}

		public static implicit operator string(Filter filter)
		{
			if (filter == null) throw new ArgumentNullException(nameof(filter));
			var filterPredicate = FilterTranslator.Translate(filter._predicate);
			return filterPredicate.Groups.Any() ? filterPredicate.ToXml() : null;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public DSL API.")]
		public static bool operator true(Filter filter)
		{
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#user-defined-conditional-logical-operators
			// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/true-operator
			// return opposite of what expected to prevent short-circuit evaluation as actual intent is to return a new expression tree
			return false;
		}

		#endregion

		public Filter(Expression<Func<bool>> predicate)
		{
			_predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
		}

		#region Base Class Member Overrides

		public override string ToString()
		{
			return this;
		}

		#endregion

		private readonly Expression<Func<bool>> _predicate;
	}
}
