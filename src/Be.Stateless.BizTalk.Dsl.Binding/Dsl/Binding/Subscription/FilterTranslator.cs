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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.B2B.PartnerManagement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Subscription
{
	public static class FilterTranslator
	{
		public static FilterPredicate Translate(Expression<Func<bool>> expression)
		{
			if (expression == null) throw new ArgumentNullException(nameof(expression));
			try
			{
				return TranslateFilterPredicate(FilterNormalizer.Normalize(expression.Body));
			}
			catch (TpmException exception)
			{
				throw new NotSupportedException($"Cannot translate FilterPredicate \"{expression}\" because {exception.Message}", exception);
			}
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		private static FilterPredicate TranslateFilterPredicate(Expression expression)
		{
			if (expression is BinaryExpression binaryExpression)
			{
				var filterPredicate = new FilterPredicate();
				var groups = TranslateFilterGroup(binaryExpression);
				groups.ForEach(g => filterPredicate.Groups.Add(g));
				return filterPredicate;
			}
			throw new NotSupportedException($"Cannot translate FilterPredicate \"{expression}\" because {expression.NodeType} node is not supported.");
		}

		private static IEnumerable<FilterGroup> TranslateFilterGroup(Expression expression)
		{
			return expression is BinaryExpression binaryExpression
				? TranslateFilterGroup(binaryExpression)
				: throw new NotSupportedException($"Cannot translate FilterPredicate \"{expression}\" because {expression.NodeType} node is not supported.");
		}

		private static IEnumerable<FilterGroup> TranslateFilterGroup(BinaryExpression expression)
		{
			return expression.NodeType switch {
				ExpressionType.ExclusiveOr => TranslateFilterGroup(expression.Left).Concat(TranslateFilterGroup(expression.Right)),
				ExpressionType.Or => TranslateFilterGroup(expression.Left).Concat(TranslateFilterGroup(expression.Right)),
				ExpressionType.OrElse => TranslateFilterGroup(expression.Left).Concat(TranslateFilterGroup(expression.Right)),
				ExpressionType.And => new[] { BuildFilterGroup(TranslateFilterStatement(expression)) },
				ExpressionType.AndAlso => new[] { BuildFilterGroup(TranslateFilterStatement(expression)) },
				_ => new[] { BuildFilterGroup(TranslateFilterStatement(expression)) }
			};
		}

		private static FilterGroup BuildFilterGroup(IEnumerable<FilterStatement> statements)
		{
			var group = new FilterGroup();
			statements.ForEach(s => group.Statements.Add(s));
			return group;
		}

		private static IEnumerable<FilterStatement> TranslateFilterStatement(Expression expression)
		{
			return expression is BinaryExpression binaryExpression
				? TranslateFilterStatement(binaryExpression)
				: throw new NotSupportedException($"Cannot translate FilterPredicate \"{expression}\" because {expression.NodeType} node is not supported.");
		}

		[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
		private static IEnumerable<FilterStatement> TranslateFilterStatement(BinaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					foreach (var filterStatement in TranslateFilterStatement(expression.Left).Concat(TranslateFilterStatement(expression.Right)))
					{
						yield return filterStatement;
					}
					yield break;
				case ExpressionType.Equal:
					yield return new(
						TranslatePropertyExpression(expression.Left),
						FilterOperator.Equals,
						TranslateValueExpression(expression.Right));
					yield break;
				case ExpressionType.GreaterThan:
					yield return new(
						TranslatePropertyExpression(expression.Left),
						FilterOperator.GreaterThan,
						TranslateValueExpression(expression.Right));
					yield break;
				case ExpressionType.GreaterThanOrEqual:
					yield return new(
						TranslatePropertyExpression(expression.Left),
						FilterOperator.GreaterThanOrEquals,
						TranslateValueExpression(expression.Right));
					yield break;
				case ExpressionType.LessThan:
					yield return new(
						TranslatePropertyExpression(expression.Left),
						FilterOperator.LessThan,
						TranslateValueExpression(expression.Right));
					yield break;
				case ExpressionType.LessThanOrEqual:
					yield return new(
						TranslatePropertyExpression(expression.Left),
						FilterOperator.LessThanOrEquals,
						TranslateValueExpression(expression.Right));
					yield break;
				case ExpressionType.NotEqual:
					// != null is rewritten as Exists operator
					var value = TranslateValueExpression(expression.Right);
					yield return new(
						TranslatePropertyExpression(expression.Left),
						value == null ? FilterOperator.Exists : FilterOperator.NotEqual,
						value);
					yield break;
				default:
					throw new NotSupportedException($"Cannot translate FilterStatement \"{expression}\" because {expression.NodeType} node is not supported.");
			}
		}

		private static string TranslatePropertyExpression(Expression expression)
		{
			// handle MessageContextProperty<T, TR>
			if (expression is MemberExpression memberExpression && memberExpression.Type.IsSubclassOfGenericType(typeof(MessageContextProperty<,>)))
			{
				return Expression.Lambda(expression).Compile().DynamicInvoke() is IMessageContextProperty property
					? property.Type.FullName
					: throw new NotSupportedException($"Cannot translate property Expression \"{expression}\" because it evaluates to null.");
			}
			throw new NotSupportedException($"Cannot translate property Expression \"{expression}\" because only MessageContextProperty<T, TR>-derived type's member access expressions are supported.");
		}

		private static string TranslateValueExpression(Expression expression)
		{
			return expression switch {
				ConstantExpression constantExpression => TranslateValueExpression(constantExpression),
				MemberExpression memberExpression => TranslateValueExpression(memberExpression),
				MethodCallExpression methodCallExpression => TranslateValueExpression(methodCallExpression),
				UnaryExpression unaryExpression => TranslateValueExpression(unaryExpression),
				_ => throw new NotSupportedException($"Cannot translate value Expression \"{expression}\" because {expression.NodeType} node is not supported.")
			};
		}

		private static string TranslateValueExpression(ConstantExpression expression)
		{
			return expression.Type.IsEnum || expression.Type.IsPrimitive || expression.Type == typeof(string)
				? expression.Value?.ToString()
				: throw new NotSupportedException($"Cannot translate ConstantExpression \"{expression}\" because {expression.Type} constant type is not supported.");
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		private static string TranslateValueExpression(MemberExpression expression)
		{
			// handle static members
			if (expression.Expression == null)
			{
				return expression.Member switch {
					FieldInfo { IsStatic: true } fieldInfo => (string) fieldInfo.GetValue(fieldInfo.DeclaringType),
					PropertyInfo propertyInfo when propertyInfo.GetGetMethod().IsStatic => (string) propertyInfo.GetValue(propertyInfo.DeclaringType),
					_ => throw new NotSupportedException($"Cannot translate MemberExpression \"{expression}\" because {expression.Member.Name} is not static.")
				};
			}

			// handle IReceivePort<TNamingConvention>.Name and ISendPort<TNamingConvention>.Name
			var containingObjectType = expression.Expression.Type;
			if (containingObjectType.IsSubclassOfGenericType(typeof(IReceivePort<>)) || containingObjectType.IsSubclassOfGenericType(typeof(ISendPort<>)))
			{
				var port = (ISupportNameResolution) Expression.Lambda(expression.Expression).Compile().DynamicInvoke();
				return port.ResolveName();
			}

			// handle string value
			var type = expression.Type;
			if (type == typeof(string))
			{
				var value = (string) Expression.Lambda(expression).Compile().DynamicInvoke();
				return value;
			}

			throw new NotSupportedException($"Cannot translate MemberExpression \"{expression}\" because {expression.NodeType} node is not supported.");
		}

		private static string TranslateValueExpression(MethodCallExpression expression)
		{
			return expression.Object == null && expression.Method.IsStatic && !expression.Method.GetParameters().Any()
				? (string) expression.Method.Invoke(null, null)
				: expression.Object is ConstantExpression constantExpression
					? TranslateValueExpression(constantExpression)
					: throw new NotSupportedException($"Cannot translate MemberExpression \"{expression}\" because {expression.NodeType} node is not supported.");
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		private static string TranslateValueExpression(UnaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.Convert)
			{
				// handle cast operator to IConvertible, which is the case of Enum
				if (expression.Type == typeof(IConvertible))
				{
					var convertible = (IConvertible) Expression.Lambda(expression).Compile().DynamicInvoke();
					return convertible.ToString(CultureInfo.InvariantCulture);
				}

				var method = expression.Method;

				// handle cast operator to INamingConvention<TNamingConvention>
				var declaringType = method.DeclaringType;
				if (declaringType != null && declaringType.IsSubclassOfGenericType(typeof(INamingConvention<>)))
				{
					if (expression.Operand is MemberExpression memberExpression) return TranslateValueExpression(memberExpression);
				}

				// handle implicit string cast operator
				if (method.IsStatic && method.IsSpecialName && method.ReturnType == typeof(string) && method.Name == "op_Implicit")
				{
					return (string) Expression.Lambda(expression).Compile().DynamicInvoke();
				}
			}

			throw new NotSupportedException($"Cannot translate UnaryExpression \"{expression}\" because {expression.NodeType} node is not supported.");
		}
	}
}
