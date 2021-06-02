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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public sealed class SendPortTransport<TNamingConvention> : TransportBase<IOutboundAdapter>
		where TNamingConvention : class
	{
		#region Nested Type: UnknownOutboundAdapter

		private class UnknownOutboundAdapter : UnknownAdapter, IOutboundAdapter
		{
			public static readonly IOutboundAdapter Instance = new UnknownOutboundAdapter();
		}

		#endregion

		public SendPortTransport(ISendPort<TNamingConvention> sendPort) : this()
		{
			SendPort = sendPort ?? throw new ArgumentNullException(nameof(sendPort));
		}

		private SendPortTransport()
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

		protected override string ResolveHostName()
		{
			var name = ((IResolveTransportHost) Host)?.ResolveHostName(this);
			if (name.IsNullOrEmpty())
				throw new BindingException(
					$"{(ReferenceEquals(SendPort.Transport, this) ? "Primary" : "Backup")} Transport's Host could not be resolved for SendPort '{((ISupportNamingConvention) SendPort).Name}'.");
			return name;
		}

		#endregion

		public RetryPolicy RetryPolicy { get; set; }

		public ISendPort<TNamingConvention> SendPort { get; }

		/// <summary>
		/// <see cref="ServiceWindow"/> restricts the <see cref="SendPortBase{TNamingConvention}"/> to work during certain hours
		/// of the day.
		/// </summary>
		public ServiceWindow ServiceWindow { get; set; }
	}
}
