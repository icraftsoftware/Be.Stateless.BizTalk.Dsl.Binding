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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSqlAdapter
	{
		#region Nested Type: Inbound

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
		public class Inbound : WcfSqlAdapter<CustomRLConfig>,
			IInboundAdapter,
			IAdapterConfigInboundCredentials,
			IAdapterConfigInboundDisableLocationOnFailure,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigServiceBehavior
		{
			public Inbound()
			{
				// Binding Tab - Inbound Settings
				InboundOperationType = InboundOperation.Polling;

				// Binding Tab - Notification Settings
				NotifyOnListenerStart = true;

				// Binding Tab - Polling Settings
				PollingInterval = TimeSpan.FromSeconds(30);
				PollWhileDataFound = false;

				// Other Tab - Polling Credentials Settings
				CredentialType = CredentialSelection.None;

				// Messages Tab - Error Handling Settings
				DisableLocationOnFailure = false;
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;

				ServiceBehaviors = Enumerable.Empty<BehaviorExtensionElement>();
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigInboundCredentials Members

			public CredentialSelection CredentialType
			{
				get => AdapterConfig.CredentialType;
				set => AdapterConfig.CredentialType = value;
			}

			public string UserName
			{
				get => AdapterConfig.UserName;
				set => AdapterConfig.UserName = value;
			}

			public string Password
			{
				get => AdapterConfig.Password;
				set => AdapterConfig.Password = value;
			}

			public string AffiliateApplicationName
			{
				get => AdapterConfig.AffiliateApplicationName;
				set => AdapterConfig.AffiliateApplicationName = value;
			}

			#endregion

			#region IAdapterConfigInboundDisableLocationOnFailure Members

			public bool DisableLocationOnFailure
			{
				get => AdapterConfig.DisableLocationOnFailure;
				set => AdapterConfig.DisableLocationOnFailure = value;
			}

			#endregion

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

			#region IAdapterConfigServiceBehavior Members

			public IEnumerable<BehaviorExtensionElement> ServiceBehaviors { get; set; }

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				AdapterConfig.ServiceBehaviorConfiguration = ServiceBehaviors.GetServiceBehaviorElementXml();
				base.Save(propertyBag);
			}

			#endregion

			#region Binding Tab - Inbound Settings

			/// <summary>
			/// The inbound operation which needs to be performed.
			/// </summary>
			public InboundOperation InboundOperationType
			{
				get => BindingElement.InboundOperationType;
				set => BindingElement.InboundOperationType = value;
			}

			#endregion

			#region Binding Tab - Notification Settings

			/// <summary>
			/// The SQL SELECT or EXEC statement against which notifications will be registered.
			/// </summary>
			public string NotificationStatement
			{
				get => BindingElement.NotificationStatement;
				set => BindingElement.NotificationStatement = value;
			}

			/// <summary>
			/// Determines whether the adapter should send a notification message when the Listener is started.
			/// </summary>
			public bool NotifyOnListenerStart
			{
				get => BindingElement.NotifyOnListenerStart;
				set => BindingElement.NotifyOnListenerStart = value;
			}

			#endregion

			#region Binding Tab - Polling Settings

			/// <summary>
			/// This statement is executed to determine whether data is available to be polled. Execution of this statement should
			/// return a single result set consisting of one row and one column, the value in which should reflect the number of
			/// rows available to be read.
			/// </summary>
			public string PolledDataAvailableStatement
			{
				get => BindingElement.PolledDataAvailableStatement;
				set => BindingElement.PolledDataAvailableStatement = value;
			}

			/// <summary>
			/// The SQL statement used to retrieve data from SQL, and optionally update the database. This statement will be
			/// executed within a transaction. In BizTalk Server, this same transaction will be used to insert the message into
			/// the Message Box.
			/// </summary>
			public string PollingStatement
			{
				get => BindingElement.PollingStatement;
				set => BindingElement.PollingStatement = value;
			}

			/// <summary>
			/// The interval at which the adapter will execute the Polled Data Available Statement to determine whether data is
			/// available to be polled.
			/// </summary>
			public TimeSpan PollingInterval
			{
				get => TimeSpan.FromSeconds(BindingElement.PollingIntervalInSeconds);
				set => BindingElement.PollingIntervalInSeconds = (int) value.TotalSeconds;
			}

			/// <summary>
			/// Controls whether the adapter should execute the Polled Data Available Statement even before the polling interval
			/// has elapsed, if the previous execution of the polling statement returned data.
			/// </summary>
			public bool PollWhileDataFound
			{
				get => BindingElement.PollWhileDataFound;
				set => BindingElement.PollWhileDataFound = value;
			}

			#endregion
		}

		#endregion
	}
}
