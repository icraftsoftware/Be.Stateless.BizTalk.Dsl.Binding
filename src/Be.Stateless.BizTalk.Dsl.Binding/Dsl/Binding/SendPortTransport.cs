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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public sealed class SendPortTransport : TransportBase<IOutboundAdapter>
	{
		#region Nested Type: UnknownOutboundAdapter

		internal class UnknownOutboundAdapter : UnknownAdapter, IOutboundAdapter
		{
			public static readonly IOutboundAdapter Instance = new UnknownOutboundAdapter();
		}

		#endregion

		public SendPortTransport()
		{
			Adapter = UnknownOutboundAdapter.Instance;
			RetryPolicy = RetryPolicy.Default;
			ServiceWindow = ServiceWindow.None;
		}

		#region Base Class Member Overrides

		[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		protected override void ApplyEnvironmentOverrides(string environment)
		{
			(RetryPolicy as ISupportEnvironmentOverride)?.ApplyEnvironmentOverrides(environment);
			(ServiceWindow as ISupportEnvironmentOverride)?.ApplyEnvironmentOverrides(environment);
		}

		#endregion

		public RetryPolicy RetryPolicy { get; set; }

		/// <summary>
		/// <see cref="ServiceWindow"/> restricts the <see cref="SendPortBase{TNamingConvention}"/> to work during certain hours
		/// of the day.
		/// </summary>
		public ServiceWindow ServiceWindow { get; set; }
	}
}
