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

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Adapter.SBMessaging;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class SBMessagingAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The SB-Messaging adapter allows to send and receive messages from Service Bus entities like Queues, Topics, and
		/// Relays. You can use the SB-Messaging adapters to bridge the connectivity between Windows Azure and on-premises
		/// BizTalk Server, thereby enabling users to create a typical hybrid application.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/sb-messaging-adapter">SB-Messaging Adapter</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound : SBMessagingAdapter<SBMessagingRLConfig>, IInboundAdapter
		{
			public Inbound() { }

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region General Tab

			/// <summary>
			/// Specifies a timespan value that indicates the time for a receive operation to complete.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>10</c> minutes.
			/// </remarks>
			public TimeSpan ReceiveTimeout
			{
				get => AdapterConfig.ReceiveTimeout;
				set => AdapterConfig.ReceiveTimeout = value;
			}

			/// <summary>
			/// Specifies the timespan value that indicates the period of inactivity that the session waits before timing out.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>1</c> minute.
			/// </remarks>
			public TimeSpan SessionIdleTimeout
			{
				get => AdapterConfig.SessionIdleTimeout;
				set => AdapterConfig.SessionIdleTimeout = value;
			}

			/// <summary>
			/// Specifies the maximum size, in bytes, for a message that can be processed by the Azure Service Bus binding.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>256</c> KB.
			/// </remarks>
			public int MaxReceivedMessageSize
			{
				get => AdapterConfig.MaxReceivedMessageSize;
				set => AdapterConfig.MaxReceivedMessageSize = value;
			}

			/// <summary>
			/// Specifies the number of messages that are received simultaneously from the Service Bus Queue or a topic.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Prefetching enables the queue or subscription client to load additional messages from the service when it performs
			/// a receive operation. The client stores these messages in a local cache. The size of the cache is determined by the
			/// value for the Prefetch Count property you specify here.
			/// </para>
			/// <para>
			/// For more information, refer to the section "Prefetching" at <see
			/// href="https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements#prefetching">Best Practices for performance
			/// improvements using Service Bus Messaging</see>.
			/// </para>
			/// <para>
			/// It defaults to <c>-1</c>.
			/// </para>
			/// </remarks>
			public int PrefetchCount
			{
				get => AdapterConfig.PrefetchCount;
				set => AdapterConfig.PrefetchCount = value;
			}

			/// <summary>
			/// Whether to use a Service Bus session to receive messages from a queue or a subscription.
			/// </summary>
			public bool IsSessionful
			{
				get => AdapterConfig.IsSessionful;
				set => AdapterConfig.IsSessionful = value;
			}

			/// <summary>
			/// Whether to preserve message ordering within each session when processing messages.
			/// </summary>
			public bool OrderedProcessing
			{
				get => AdapterConfig.OrderedProcessing;
				set
				{
					AdapterConfig.OrderedProcessing = value;
					if (value) IsSessionful = true;
				}
			}

			#endregion

			#region Properties Tab

			/// <summary>
			/// Specify the namespace that the adapter uses to write the brokered message properties as message context properties
			/// on the message received by BizTalk Server.
			/// </summary>
			public string CustomBrokeredPropertyNamespace
			{
				get => AdapterConfig.CustomBrokeredPropertyNamespace;
				set => AdapterConfig.CustomBrokeredPropertyNamespace = value;
			}

			/// <summary>
			/// Specify whether to promote the brokered message properties.
			/// </summary>
			public bool PromoteCustomProperties
			{
				get => AdapterConfig.PromoteCustomProperties;
				set => AdapterConfig.PromoteCustomProperties = value;
			}

			#endregion
		}

		#endregion
	}
}
