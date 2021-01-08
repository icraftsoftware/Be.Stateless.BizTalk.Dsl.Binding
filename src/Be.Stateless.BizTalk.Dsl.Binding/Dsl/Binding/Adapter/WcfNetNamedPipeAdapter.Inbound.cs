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
	public abstract partial class WcfNetNamedPipeAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-NetNamedPipe adapter provides cross-process communication on the same computer in an environment in which
		/// both services and clients are WCF based. It provides full access to SOAP reliability and transaction features. The
		/// adapter uses the named pipe transport, and messages have binary encoding. This adapter cannot be used in
		/// cross-computer communication.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/what-is-the-wcf-netnamedpipe-adapter">What Is the WCF-NetNamedPipe Adapter?</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-netnamedpipe-receive-location">How to Configure a WCF-NetNamedPipe Receive Location</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-netnamedpipe-transport-properties-dialog-box-receive-security-tab">WCF-NetNamedPipe Transport Properties Dialog Box, Receive, Security Tab</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF
		/// Adapters Property Schema and Properties</seealso>.
		public class Inbound : WcfNetNamedPipeAdapter<NetNamedPipeRLConfig>,
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
