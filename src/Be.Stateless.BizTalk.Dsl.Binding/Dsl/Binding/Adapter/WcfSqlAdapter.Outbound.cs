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
using System.Transactions;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSqlAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The Microsoft BizTalk Adapter for SQL Server exposes the SQL Server database as a WCF service. Adapter clients can
		/// perform operations on the SQL Server database by exchanging SOAP messages with the adapter. The adapter consumes the
		/// SOAP message and makes appropriate ADO.NET calls to perform the operation. The adapter returns the response from the
		/// SQL Server database back to the client in the form of SOAP messages.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-sql/overview-of-biztalk-adapter-for-sql-server">Overview of BizTalk Adapter for SQL Server</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-sql/read-about-the-biztalk-adapter-for-sql-server-adapter-binding-properties">Read about the BizTalk Adapter for SQL Server adapter binding properties</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Outbound : WcfSqlAdapter<CustomTLConfig>,
			IOutboundAdapter,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundCredentials,
			IAdapterConfigOutboundPropagateFaultMessage,
			IAdapterConfigOutboundTransactionIsolation
		{
			public Outbound()
			{
				// Binding Tab - Buffering Settings
				BatchSize = 20;
				ChunkSize = 4096 * 1024;

				// Binding Tab - Miscellaneous Settings
				AllowIdentityInsert = false;

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
				get => _adapterConfig.StaticAction;
				set => _adapterConfig.StaticAction = value;
			}

			#endregion

			#region IAdapterConfigOutboundCredentials Members

			public bool UseSSO
			{
				get => _adapterConfig.UseSSO;
				set => _adapterConfig.UseSSO = value;
			}

			public string AffiliateApplicationName
			{
				get => _adapterConfig.AffiliateApplicationName;
				set => _adapterConfig.AffiliateApplicationName = value;
			}

			public string Password
			{
				get => _adapterConfig.Password;
				set => _adapterConfig.Password = value;
			}

			public string UserName
			{
				get => _adapterConfig.UserName;
				set => _adapterConfig.UserName = value;
			}

			#endregion

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get => _adapterConfig.PropagateFaultMessage;
				set => _adapterConfig.PropagateFaultMessage = value;
			}

			#endregion

			#region IAdapterConfigOutboundTransactionIsolation Members

			public bool EnableTransaction
			{
				get => _adapterConfig.EnableTransaction;
				set => _adapterConfig.EnableTransaction = value;
			}

			public IsolationLevel IsolationLevel
			{
				get => _adapterConfig.IsolationLevel;
				set => _adapterConfig.IsolationLevel = value;
			}

			#endregion

			#region Binding Tab - Miscellaneous Settings

			/// <summary>
			/// Determines whether the adapter does a "SET IDENTITY_INSERT ON" before an INSERT or UPDATE operation.
			/// </summary>
			public bool AllowIdentityInsert
			{
				get => _bindingConfigurationElement.AllowIdentityInsert;
				set => _bindingConfigurationElement.AllowIdentityInsert = value;
			}

			#endregion

			#region Binding Tab - Buffering Settings

			/// <summary>
			/// The number of rows to buffer in memory before attempting an Insert, Update or Delete operation. A higher value can
			/// improve performance, but requires more memory consumption.
			/// </summary>
			public int BatchSize
			{
				get => _bindingConfigurationElement.BatchSize;
				set => _bindingConfigurationElement.BatchSize = value;
			}

			/// <summary>
			/// The size of the internal buffer used by the adapter during the SetXXX() operations.
			/// </summary>
			public int ChunkSize
			{
				get => _bindingConfigurationElement.ChunkSize;
				set => _bindingConfigurationElement.ChunkSize = value;
			}

			#endregion
		}

		#endregion
	}
}
