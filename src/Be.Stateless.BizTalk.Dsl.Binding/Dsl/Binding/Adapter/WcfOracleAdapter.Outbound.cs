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
	public abstract partial class WcfOracleAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The Microsoft BizTalk Adapter for Oracle Database exposes the Oracle database as a WCF service. Adapter clients can
		/// perform operations on the Oracle database by exchanging SOAP messages with the adapter. The adapter consumes the WCF
		/// message and makes appropriate ODP.NET calls to perform the operation. The adapter returns the response from the
		/// Oracle database back to the client in the form of SOAP messages.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/overview-of-biztalk-adapter-for-oracle-database">Overview of BizTalk Adapter for Oracle Database</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/read-about-the-oracle-database-adapter-binding-properties">Read about the Oracle Database adapter binding properties</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>.
		public class Outbound : WcfOracleAdapter<CustomTLConfig>,
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
		}

		#endregion
	}
}
