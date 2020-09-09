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
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfBasicHttpAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// You can use the WCF-BasicHttp adapter to do cross-computer communication with legacy ASMX-based Web services and
		/// clients that conform to the WS-I Basic Profile 1.1, using either the HTTP or HTTPS transport with text encoding.
		/// However, you will not be able to take advantage of features that are supported by WS-* protocols.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-receive-security-tab">WCF-BasicHttp Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF
		/// Adapters Property Schema and Properties</seealso>.
		public class Inbound : WcfBasicHttpAdapter<Uri, BasicHttpRLConfig>,
			IInboundAdapter,
			IAdapterConfigMaxConcurrentCalls,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
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
				get => _adapterConfig.IncludeExceptionDetailInFaults;
				set => _adapterConfig.IncludeExceptionDetailInFaults = value;
			}

			#endregion

			#region IAdapterConfigInboundSuspendRequestMessageOnFailure Members

			public bool SuspendRequestMessageOnFailure
			{
				get => _adapterConfig.SuspendMessageOnFailure;
				set => _adapterConfig.SuspendMessageOnFailure = value;
			}

			#endregion

			#region IAdapterConfigMaxConcurrentCalls Members

			public int MaxConcurrentCalls
			{
				get => _adapterConfig.MaxConcurrentCalls;
				set => _adapterConfig.MaxConcurrentCalls = value;
			}

			#endregion

			#region IAdapterConfigSSO Members

			/// <summary>
			/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials to issue an SSO ticket.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about the security configurations supporting SSO, see the section "Enterprise Single Sign-On
			/// Supportability for the WCF-BasicHttp Receive Adapter" in <see
			/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-receive-security-tab">WCF-BasicHttp
			/// Transport Properties Dialog Box, Receive, Security Tab</see>.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool UseSSO
			{
				get => _adapterConfig.UseSSO;
				set => _adapterConfig.UseSSO = value;
			}

			#endregion
		}

		#endregion
	}
}
