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
using Microsoft.Adapters.OracleDB;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfOracleAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The Microsoft BizTalk Adapter for Oracle Database exposes the Oracle database as a WCF service. Adapter clients can
		/// perform operations on the Oracle database by exchanging SOAP messages with the adapter. The adapter consumes the WCF
		/// message and makes appropriate ODP.NET calls to perform the operation. The adapter returns the response from the
		/// Oracle database back to the client in the form of SOAP messages.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/overview-of-biztalk-adapter-for-oracle-database">Overview of BizTalk Adapter for Oracle Database</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/read-about-the-oracle-database-adapter-binding-properties">Read about the Oracle Database adapter binding properties</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound : WcfOracleAdapter<CustomRLConfig>,
			IInboundAdapter,
			IAdapterConfigInboundCredentials,
			IAdapterConfigInboundDisableLocationOnFailure,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigServiceBehavior
		{
			public Inbound()
			{
				// Binding Tab - General Settings
				InboundOperationType = InboundOperation.Polling;

				// Binding Tab - Notification Settings
				NotificationPort = -1;
				NotifyOnListenerStart = true;

				// Binding Tab - Polling Receive Settings
				PolledDataAvailableStatement = "SELECT 1 FROM DUAL";
				PollingInterval = TimeSpan.FromSeconds(500);
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

			#region Binding Tab - General Settings

			/// <summary>
			/// Specifies whether you want to perform <see cref="InboundOperation.Polling"/> or <see
			/// cref="InboundOperation.Notification"/> inbound operation.
			/// </summary>
			/// <remarks>
			/// <para>
			/// For more information about <see cref="InboundOperation.Polling"/> see <see
			/// href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/support-for-receiving-polling-based-data-changed-messages-in-oracle-database">Support
			/// for Receiving Polling-based Data-changed Messages in Oracle Database</see> For more information about <see
			/// cref="InboundOperation.Notification"/>, see <see
			/// href="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/receive-oracle-database-change-notifications">Receiving
			/// Oracle Database Change Notifications</see>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="InboundOperation.Polling"/>.
			/// </para>
			/// </remarks>
			public InboundOperation InboundOperationType
			{
				get => BindingElement.InboundOperationType;
				set => BindingElement.InboundOperationType = value;
			}

			#endregion

			#region Binding Tab - Notification Settings

			/// <summary>
			/// Specifies the port number that ODP.NET must open to listen for database change notification from Oracle database.
			/// </summary>
			/// <remarks>
			/// <para>
			/// If there is more than one application in an application domain receiving notifications using the Oracle Database
			/// adapter, the <see cref="NotificationPort"/> binding property for all applications must be set to the same port
			/// number. This is because ODP.NET creates only one listener that listens on one port within an application domain.
			/// </para>
			/// <para>
			/// Adapter clients will not receive database change notifications if Windows Firewall is turned on. Also, turning off
			/// Windows Firewall to receive notifications is not advisable. So, to receive notifications without compromising the
			/// security of the client-side computers, we recommend specifying a positive integer value as a port number and then
			/// adding that port number to the Windows Firewall exceptions list. If you set this binding property to the default
			/// value of -1, ODP.NET uses a random port and adapter clients will not know which port to add to Windows Firewall
			/// exceptions list.
			/// </para>
			/// <para>
			/// It defaults to -1, which signifies that ODP.NET uses a valid, random, unused port number.
			/// </para>
			/// </remarks>
			public int NotificationPort
			{
				get => BindingElement.NotificationPort;
				set => BindingElement.NotificationPort = value;
			}

			/// <summary>
			/// Specifies the SELECT statement used to register for getting notifications from Oracle database.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The adapter gets a notification message from Oracle database only when the result set for the specified SELECT
			/// statement changes.
			/// </para>
			/// <para>
			/// An example SELECT statement could resemble the following:
			/// <code>SELECT TID,ACCOUNT,PROCESSED FROM SCOTT.ACCOUNTACTIVITY WHERE PROCESSED = 'n'</code>
			/// </para>
			/// <para>
			/// Notice that you must specify the database object name along with the schema name. For example,
			/// <c>SCOTT.ACCOUNTACTIVITY</c>.
			/// </para>
			/// </remarks>
			[SuppressMessage("ReSharper", "CommentTypo")]
			public string NotificationStatement
			{
				get => BindingElement.NotificationStatement;
				set => BindingElement.NotificationStatement = value;
			}

			/// <summary>
			/// Specifies whether the adapter sends a notification message to the adapter clients, informing that the receive
			/// location is running, when the listener starts.
			/// </summary>
			/// <remarks>
			/// It defaults is to <c>True</c>.
			/// </remarks>
			public bool NotifyOnListenerStart
			{
				get => BindingElement.NotifyOnListenerStart;
				set => BindingElement.NotifyOnListenerStart = value;
			}

			#endregion

			#region Binding Tab - Polling Receive Settings

			/// <summary>
			/// Specifies the SELECT statement executed to determine whether any data is available for polling.
			/// </summary>
			/// <remarks>
			/// <para>
			/// This specified statement should return a single result set consisting of one row and one column, the value in
			/// which should reflect the number of rows available to be read.
			/// </para>
			/// <para>
			/// The default value of this binding property is set to: <c>SELECT 1 FROM DUAL</c>, which implies that the adapter
			/// will continue polling irrespectively of whether the table being polled has data or not.
			/// </para>
			/// <para>
			/// You must not specify stored procedures for this binding property. Also, this statement must not modify the
			/// underlying Oracle database.
			/// </para>
			/// </remarks>
			public string PolledDataAvailableStatement
			{
				get => BindingElement.PolledDataAvailableStatement;
				set => BindingElement.PolledDataAvailableStatement = value;
			}

			/// <summary>
			/// Specifies the action for the polling operation. You can determine the polling action for a specific operation from
			/// the metadata you generate for the operation using the Consume Adapter Service Add-in.
			/// </summary>
			public string PollingAction
			{
				get => BindingElement.PollingAction;
				set => BindingElement.PollingAction = value;
			}

			/// <summary>
			/// Specifies the transacted polling interval, that is, the time interval at which the Oracle Database adapter
			/// executes the polling statement against the Oracle database.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The polling interval is used by the adapter for the following:
			/// <list type="bullet">
			/// <item>
			/// The time interval between successive polls. This interval is used to run the poll and post-poll queries. If these
			/// queries are executed within the specified interval, the adapter sleeps for the remaining time in the interval.
			/// </item>
			/// <item>
			/// The polling transaction timeout value. This value must be set large enough to include the polling statement
			/// execution time, the post-poll statement (if specified) execution time, and the time to receive the reply from the
			/// client application to commit the transaction.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// If the client application sends a reply before the polling interval expires, the adapter commits the transaction
			/// and waits until the polling interval is reached to execute the next poll.
			/// </para>
			/// <para>
			/// If the client application returns a fault, the adapter terminates the transaction.
			/// </para>
			/// <para>
			/// If the polling interval expires before the client application sends the reply, the transaction will time out. For
			/// more information about polling, see <see
			/// ref="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/receive-polling-based-data-changed-messages-in-oracle-database-adapter">Receive
			/// polling-based data-changed messages in Oracle Database adapter</see>.
			/// </para>
			/// <para>
			/// The default is 500 seconds.
			/// </para>
			/// </remarks>
			public TimeSpan PollingInterval
			{
				get => TimeSpan.FromSeconds(BindingElement.PollingInterval);
				set => BindingElement.PollingInterval = (int) value.TotalSeconds;
			}

			/// <summary>
			/// Specifies the polling statement.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You can specify a simple SELECT statement or a stored procedure, function, or a packaged procedure or function for
			/// polling.
			/// <list type="bullet">
			/// <item>
			/// If you want to poll a table or view, you must specify a SELECT query in this binding property.
			/// </item>
			/// <item>
			/// If you want to poll using a stored procedure, function, or procedure or function within a package, you must
			/// specify the entire request message for the respective operation in this binding property.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// The polling statement is executed only if the statement executed by the <see cref="PolledDataAvailableStatement"/>
			/// property returns some data.
			/// </para>
			/// <para>
			/// The Oracle Database adapter executes the polling statement and the post-poll statement (if specified) inside of an
			/// Oracle transaction. If you are using a SELECT statement in the <see cref="PollingStatement"/> property, we
			/// recommend that you specify a FOR UPDATE clause in your SELECT statement. This will ensure that the selected
			/// records are locked during the transaction and that the post-poll statement can perform any required updates on the
			/// selected records.
			/// </para>
			/// <para>
			/// For more information about polling, including the use of the FOR UPDATE clause, see <see
			/// ref="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/receive-polling-based-data-changed-messages-in-oracle-database-adapter">Receive
			/// polling-based data-changed messages in Oracle Database adapter</see>.
			/// </para>
			/// </remarks>
			public string PollingStatement
			{
				get => BindingElement.PollingStatement;
				set => BindingElement.PollingStatement = value;
			}

			/// <summary>
			/// Specifies whether the Oracle Database adapter ignores the polling interval and continuously polls the Oracle
			/// database, if data is available in the table being polled. If no data is available in the table, the adapter
			/// reverts to execute the SQL statement at the specified polling interval.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Consider a scenario where the polling interval is set to 60 seconds, and the statement specified for <see
			/// cref="PolledDataAvailableStatement"/> returns that data is available for polling. The adapter then executes the
			/// statement specified for the <see cref="PollingStatement"/> property. Assuming that the adapter takes just 10
			/// seconds to execute the statement, it will now have to wait for 50 seconds before executing the <see
			/// cref="PolledDataAvailableStatement"/> again, and then subsequently execute the <see cref="PollingStatement"/>.
			/// Instead, to optimize the performance you can set the <see cref="PollWhileDataFound"/> property to <c>True</c> so
			/// that the adapter can start executing the next polling cycle as soon as the previous polling cycle ends.
			/// </para>
			/// <para>
			/// This property is applicable both for polling on tables and views and polling using stored procedures, functions,
			/// or packaged procedures or functions.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool PollWhileDataFound
			{
				get => BindingElement.PollWhileDataFound;
				set => BindingElement.PollWhileDataFound = value;
			}

			/// <summary>
			/// Specifies a PL/SQL block that is executed after the <see cref="PollingStatement"/> and before the /POLLINGSTMT
			/// message is sent to the consumer.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The post-poll statement executes inside the polling transaction. Two common uses for the post-poll statement are
			/// to:
			/// <list type="bullet">
			/// <item>
			/// Update a column in the rows returned in the polling statement to indicate that they have been processed and should
			/// be excluded from subsequent polling queries.
			/// </item>
			/// <item>
			/// Move processed records to a different table.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// If a <see cref="PostPollStatement"/> is specified, <see cref="PollingInterval"/> should be set large enough for
			/// the PL/SQL block to complete before the interval expires.
			/// </para>
			/// <para>
			/// For more information about polling, see <see
			/// ref="https://docs.microsoft.com/en-us/biztalk/adapters-and-accelerators/adapter-oracle-database/receive-polling-based-data-changed-messages-in-oracle-database-adapter">Receive
			/// polling-based data-changed messages in Oracle Database adapter</see>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>; no post-poll statement is executed.
			/// </para>
			/// </remarks>
			public string PostPollStatement
			{
				get => BindingElement.PostPollStatement;
				set => BindingElement.PostPollStatement = value;
			}

			#endregion
		}

		#endregion
	}
}
