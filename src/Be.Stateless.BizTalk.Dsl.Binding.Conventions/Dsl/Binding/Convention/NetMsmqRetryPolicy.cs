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
	public static class NetMsmqRetryPolicy
	{
		#region Nested Type: EnvironmentSensitiveNetMsmqRetryPolicy

		private class EnvironmentSensitiveNetMsmqRetryPolicy : ServiceModel.Configuration.NetMsmqRetryPolicy, ISupportEnvironmentOverride
		{
			public EnvironmentSensitiveNetMsmqRetryPolicy(Func<string, ServiceModel.Configuration.NetMsmqRetryPolicy> policySelector)
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

			public override int MaxRetryCycles => _policy.MaxRetryCycles;

			public override int ReceiveRetryCount => _policy.ReceiveRetryCount;

			public override TimeSpan RetryCycleDelay => _policy.RetryCycleDelay;

			public override TimeSpan TimeToLive => _policy.TimeToLive;

			#endregion

			private readonly Func<string, ServiceModel.Configuration.NetMsmqRetryPolicy> _policySelector;

			private ServiceModel.Configuration.NetMsmqRetryPolicy _policy;
		}

		#endregion

		static NetMsmqRetryPolicy()
		{
			_longRunning = new ServiceModel.Configuration.NetMsmqRetryPolicy {
				MaxRetryCycles = 71,
				ReceiveRetryCount = 1,
				RetryCycleDelay = TimeSpan.FromHours(1),
				TimeToLive = TimeSpan.FromDays(3)
			};
			RealTime = new ServiceModel.Configuration.NetMsmqRetryPolicy {
				MaxRetryCycles = 0,
				ReceiveRetryCount = 2,
				RetryCycleDelay = TimeSpan.Zero,
				TimeToLive = TimeSpan.FromMinutes(1)
			};
			_shortRunning = new ServiceModel.Configuration.NetMsmqRetryPolicy {
				MaxRetryCycles = 3,
				ReceiveRetryCount = 3,
				RetryCycleDelay = TimeSpan.FromMinutes(9),
				TimeToLive = TimeSpan.FromMinutes(30)
			};
		}

		public static ServiceModel.Configuration.NetMsmqRetryPolicy LongRunning => new EnvironmentSensitiveNetMsmqRetryPolicy(
			environment => environment.IsDevelopmentOrBuild()
				? RealTime
				: environment.IsAcceptance()
					? _shortRunning
					: _longRunning);

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Convention Public API.")]
		public static ServiceModel.Configuration.NetMsmqRetryPolicy RealTime { get; }

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Convention Public API.")]
		public static ServiceModel.Configuration.NetMsmqRetryPolicy ShortRunning => new EnvironmentSensitiveNetMsmqRetryPolicy(
			environment => environment.IsPreProductionOrProduction()
				? _shortRunning
				: RealTime);

		private static readonly ServiceModel.Configuration.NetMsmqRetryPolicy _longRunning;
		private static readonly ServiceModel.Configuration.NetMsmqRetryPolicy _shortRunning;
	}
}
