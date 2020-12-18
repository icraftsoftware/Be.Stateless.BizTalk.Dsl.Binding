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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Constants;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public static class RetryPolicy
	{
		#region Nested Type: EnvironmentSensitiveRetryPolicy

		private class EnvironmentSensitiveRetryPolicy : Binding.RetryPolicy, ISupportEnvironmentOverride
		{
			public EnvironmentSensitiveRetryPolicy(Func<string, Binding.RetryPolicy> policySelector)
			{
				_policySelector = policySelector;
			}

			#region ISupportEnvironmentOverride Members

			public void ApplyEnvironmentOverrides(string environment)
			{
				_policy = _policySelector(environment);
			}

			#endregion

			#region Base Class Member Overrides

			public override int Count => _policy.Count;

			public override TimeSpan Interval => _policy.Interval;

			#endregion

			private readonly Func<string, Binding.RetryPolicy> _policySelector;

			private Binding.RetryPolicy _policy;
		}

		#endregion

		static RetryPolicy()
		{
			_longRunning = new Binding.RetryPolicy { Count = 300, Interval = TimeSpan.FromMinutes(15) };
			RealTime = new Binding.RetryPolicy { Count = 0 };
			_shortRunning = new Binding.RetryPolicy { Count = 15, Interval = TimeSpan.FromMinutes(2) };
		}

		public static Binding.RetryPolicy LongRunning => new EnvironmentSensitiveRetryPolicy(
			environment => environment.IsDevelopmentOrBuild()
				? RealTime
				: environment.IsAcceptance()
					? _shortRunning
					: _longRunning);

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Convention Public API.")]
		public static Binding.RetryPolicy RealTime { get; }

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Convention Public API.")]
		public static Binding.RetryPolicy ShortRunning => new EnvironmentSensitiveRetryPolicy(
			environment => environment.IsPreProductionOrProduction()
				? _shortRunning
				: RealTime);

		private static readonly Binding.RetryPolicy _longRunning;
		private static readonly Binding.RetryPolicy _shortRunning;
	}
}
