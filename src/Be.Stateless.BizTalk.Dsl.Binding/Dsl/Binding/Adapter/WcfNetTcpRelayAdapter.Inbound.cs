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
using Microsoft.BizTalk.Adapter.ServiceBus;
using Microsoft.ServiceBus;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfNetTcpRelayAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// Microsoft BizTalk Server uses the WCF-NetTcpRelay adapter when receiving and sending WCF service requests through the
		/// <see cref="NetTcpRelayBinding"/>. The WCF-NetTcpRelay adapter enables you to send and receive messages from the
		/// Service Bus relay endpoints using the <see cref="NetTcpRelayBinding"/>.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-nettcprelay-adapter">WCF-NetTcpRelay Adapter</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound : WcfNetTcpRelayAdapter<NetTcpRelayRLConfig>,
			IInboundAdapter,
			IAdapterConfigMaxConcurrentCalls,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigServiceCertificate
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
				// Binding Tab - Service Throttling Behavior Settings
				MaxConcurrentCalls = 200;

				// Security Tab - Client Security Settings
				RelayClientAuthenticationType = RelayClientAuthenticationType.RelayAccessToken;

				// Messages Tab - Error Handling Settings
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigInboundIncludeExceptionDetailInFaults Members

			public bool IncludeExceptionDetailInFaults
			{
				get => AdapterConfig.IncludeExceptionDetailInFaults;
				set => AdapterConfig.IncludeExceptionDetailInFaults = value;
			}

			#endregion

			#region IAdapterConfigInboundSuspendRequestMessageOnFailure Members

			public bool SuspendRequestMessageOnFailure
			{
				get => AdapterConfig.SuspendMessageOnFailure;
				set => AdapterConfig.SuspendMessageOnFailure = value;
			}

			#endregion

			#region IAdapterConfigMaxConcurrentCalls Members

			public int MaxConcurrentCalls
			{
				get => AdapterConfig.MaxConcurrentCalls;
				set => AdapterConfig.MaxConcurrentCalls = value;
			}

			#endregion

			#region IAdapterConfigServiceCertificate Members

			public string ServiceCertificate
			{
				get => AdapterConfig.ServiceCertificate;
				set => AdapterConfig.ServiceCertificate = value;
			}

			#endregion

			#region Security Tab - Client Security Settings

			/// <summary>
			/// Specify the option to authenticate with the Service Bus relay endpoint from where the message is received.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Valid values include the following:
			/// <list type="bullet">
			/// <item>
			/// <term><see cref="Microsoft.ServiceBus.RelayClientAuthenticationType.None"/></term>
			/// <description>
			/// No authentication is required.
			/// </description>
			/// </item>
			/// <item>
			/// <term><see cref="Microsoft.ServiceBus.RelayClientAuthenticationType.RelayAccessToken"/></term>
			/// <description>
			/// Specify this to use a security token to authorize with the Service Bus Relay endpoint.
			/// </description>
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// It defaults to <see cref="Microsoft.ServiceBus.RelayClientAuthenticationType.RelayAccessToken"/>.
			/// </para>
			/// </remarks>
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public RelayClientAuthenticationType RelayClientAuthenticationType
			{
				get => AdapterConfig.RelayClientAuthenticationType;
				set => AdapterConfig.RelayClientAuthenticationType = value;
			}

			#endregion

			#region Security Tab - Service Discovery Settings

			/// <summary>
			/// Specify whether the behavior of the service is published in the Service Registry.
			/// </summary>
			public bool EnableServiceDiscovery
			{
				get => AdapterConfig.EnableServiceDiscovery;
				set => AdapterConfig.EnableServiceDiscovery = value;
			}

			/// <summary>
			/// Specify the name with which the service is published to the Service Registry.
			/// </summary>
			public string ServiceDisplayName
			{
				get => AdapterConfig.ServiceDisplayName;
				set => AdapterConfig.ServiceDisplayName = value;
			}

			/// <summary>
			/// Set the discovery mode for the service published in the Service Registry.
			/// </summary>
			/// <remarks>
			/// For more information about the discovery modes, see <see cref="DiscoveryType"/>.
			/// </remarks>
			public DiscoveryType DiscoveryMode
			{
				get => AdapterConfig.DiscoveryMode;
				set => AdapterConfig.DiscoveryMode = value;
			}

			#endregion
		}

		#endregion
	}
}
