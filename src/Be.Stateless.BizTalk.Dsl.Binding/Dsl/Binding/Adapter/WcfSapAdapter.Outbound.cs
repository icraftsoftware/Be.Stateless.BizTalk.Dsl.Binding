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
using System.Transactions;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSapAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The Microsoft BizTalk Adapter for mySAP Business Suite exposes the SAP system as a WCF service. Adapter
		/// clients can perform operations on the SAP system by exchanging SOAP messages with the adapter. The adapter
		/// consumes the WCF message and makes appropriate calls to the SAP system to perform the operation. The adapter
		/// returns the response from the SAP system back to the client in the form of SOAP messages.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-sap/overview-of-the-sap-adapter">Overview of BizTalk Adapter for mySAP Business Suite</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-sap/read-about-biztalk-adapter-for-mysap-business-suite-binding-properties">Read about BizTalk Adapter for mySAP Business Suite binding properties</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-sap/understand-biztalk-adapter-for-mysap-business-suite">Understanding BizTalk Adapter for mySAP Business Suite</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Outbound : WcfSapAdapter<CustomTLConfig>,
			IOutboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundCredentials,
			IAdapterConfigOutboundPropagateFaultMessage,
			IAdapterConfigOutboundTransactionIsolation
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Outbound()
			{
				// Messages Tab - Error Handling Settings
				PropagateFaultMessage = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigOutboundAction Members

			public string StaticAction
			{
				get => AdapterConfig.StaticAction;
				set => AdapterConfig.StaticAction = value;
			}

			#endregion

			#region IAdapterConfigOutboundCredentials Members

			public bool UseSSO
			{
				get => AdapterConfig.UseSSO;
				set => AdapterConfig.UseSSO = value;
			}

			public string AffiliateApplicationName
			{
				get => AdapterConfig.AffiliateApplicationName;
				set => AdapterConfig.AffiliateApplicationName = value;
			}

			public string Password
			{
				get => AdapterConfig.Password;
				set => AdapterConfig.Password = value;
			}

			public string UserName
			{
				get => AdapterConfig.UserName;
				set => AdapterConfig.UserName = value;
			}

			#endregion

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get => AdapterConfig.PropagateFaultMessage;
				set => AdapterConfig.PropagateFaultMessage = value;
			}

			#endregion

			#region IAdapterConfigOutboundTransactionIsolation Members

			public bool EnableTransaction
			{
				get => AdapterConfig.EnableTransaction;
				set => AdapterConfig.EnableTransaction = value;
			}

			public IsolationLevel IsolationLevel
			{
				get => AdapterConfig.IsolationLevel;
				set => AdapterConfig.IsolationLevel = value;
			}

			#endregion

			#region Binding Tab - Idoc Settings

			/// <summary>
			/// Determines whether the adapter automatically calls RfcConfirmTransID() after sending an IDoc.
			/// </summary>
			/// <remarks>
			/// Specifies whether the SAP adapter auto-commits tRFC client calls used for sending IDocs. The default is false;
			/// auto-commit is disabled. If auto-commit is disabled, the client application must explicitly commit the tRFC call
			/// by invoking the RfcConfirmTransID operation. The RfcConfirmTransID operation is a special operation surfaced by
			/// the SAP adapter. It appears under the TRFC node when you use the Add Adapter Service Reference Visual Studio
			/// Plug-in or the Consume Adapter Service BizTalk Project Add-in.
			/// </remarks>
			public bool AutoConfirmSentIdocs
			{
				get => BindingElement.AutoConfirmSentIdocs;
				set => BindingElement.AutoConfirmSentIdocs = value;
			}

			#endregion

			#region Binding Tab - Connection Settings

			/// <summary>
			/// Determines whether the state acquired by the R/3 user context should be reset before returning a connection back
			/// to the connection pool.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool ClearRfcContext
			{
				get => BindingElement.ClearRfcContext;
				set => BindingElement.ClearRfcContext = value;
			}

			#endregion
		}

		#endregion
	}
}
