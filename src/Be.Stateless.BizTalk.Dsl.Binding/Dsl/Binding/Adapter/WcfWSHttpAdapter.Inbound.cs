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
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfWSHttpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// You can use the WCF-WSHttp adapter to do cross-computer communication with services and clients that can understand
		/// the next-generation Web service standards, using either the HTTP or HTTPS transport with text or Message Transmission
		/// Optimization Mechanism (MTOM) encoding. The WCF-WSHttp adapter provides full access to the SOAP security,
		/// reliability, and transaction features.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/what-is-the-wcf-wshttp-adapter">What Is the WCF-WSHttp Adapter?</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-wshttp-receive-location">How to Configure a WCF-WSHttp Receive Location</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-wshttp-transport-properties-dialog-box-receive-security-tab">WCF-WSHttp Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>.
		public class Inbound
			: WcfWSHttpAdapter<Uri, WSHttpRLConfig>,
				IInboundAdapter,
				IAdapterConfigInboundIncludeExceptionDetailInFaults,
				IAdapterConfigInboundSuspendRequestMessageOnFailure,
				IAdapterConfigMaxConcurrentCalls,
				IAdapterConfigSSO
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
				// Binding Tab - Service Throttling Behavior Settings
				MaxConcurrentCalls = 200;

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

			#region IAdapterConfigSSO Members

			/// <summary>
			/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials to issue an SSO ticket.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about the security configurations supporting SSO, see the section "Enterprise Single Sign-On
			/// Supportability for the WCF-WSHttp Receive Adapter" in <see
			/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-wshttp-transport-properties-dialog-box-receive-security-tab">WCF-WSHttp Transport Properties Dialog Box, Receive,
			/// Security Tab</see>.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool UseSSO
			{
				get => AdapterConfig.UseSSO;
				set => AdapterConfig.UseSSO = value;
			}

			#endregion
		}

		#endregion
	}
}
