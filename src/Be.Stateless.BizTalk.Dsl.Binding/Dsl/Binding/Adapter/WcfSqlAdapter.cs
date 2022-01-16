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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
	public abstract class WcfSqlAdapter<TConfig> : WcfLobAdapterBase<SqlAdapterConnectionUri, SqlAdapterBindingConfigurationElement, TConfig>,
		IAdapterConfigBizTalkCompatibilityMode,
		IAdapterConfigPerformanceCounters
		where TConfig : AdapterConfig,
		IAdapterConfigAddress,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
		IAdapterConfigBinding,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigEndpointBehavior,
		IAdapterConfigInboundMessageMarshalling,
		IAdapterConfigOutboundMessageMarshalling,
		new()
	{
		static WcfSqlAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new("59b35d03-6a06-4734-a249-ef561254ecf7"));
		}

		protected WcfSqlAdapter() : base(_protocolType)
		{
			// Binding Tab - BizTalk Settings
			EnableBizTalkCompatibilityMode = true;

			// Binding Tab - Connection Settings
			Encrypt = false;
			MaxConnectionPoolSize = 100;

			// Binding Tab - Diagnostics Settings
			EnablePerformanceCounters = false;

			// Binding Tab - Metadata Settings
			UseDatabaseNameInXsdNamespace = false;

			// Binding Tab - Transaction Settings
			UseAmbientTransaction = true;
		}

		#region IAdapterConfigBizTalkCompatibilityMode Members

		public bool EnableBizTalkCompatibilityMode
		{
			get => BindingElement.EnableBizTalkCompatibilityMode;
			set => BindingElement.EnableBizTalkCompatibilityMode = value;
		}

		#endregion

		#region IAdapterConfigPerformanceCounters Members

		public bool EnablePerformanceCounters
		{
			get => BindingElement.EnablePerformanceCounters;
			set => BindingElement.EnablePerformanceCounters = value;
		}

		#endregion

		#region Binding Tab - Transaction Settings

		/// <summary>
		/// Determines whether the adapter performs the operations on the SQL Server within the context of the ambient
		/// transaction. In BizTalk Server, the same transaction is used to publish/delete messages from the MessageBox.
		/// </summary>
		public bool UseAmbientTransaction
		{
			get => BindingElement.UseAmbientTransaction;
			set => BindingElement.UseAmbientTransaction = value;
		}

		#endregion

		#region Binding Tab - Metadata Settings

		/// <summary>
		/// Determines whether the database name should be used in the XSD namespaces.
		/// </summary>
		public bool UseDatabaseNameInXsdNamespace
		{
			get => BindingElement.UseDatabaseNameInXsdNamespace;
			set => BindingElement.UseDatabaseNameInXsdNamespace = value;
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;

		#region Binding Tab - Connection Settings

		/// <summary>
		/// Determines whether SQL Server uses SSL encryption for all data sent between the client and server if the server has a
		/// certificate installed.
		/// </summary>
		public bool Encrypt
		{
			get => BindingElement.Encrypt;
			set => BindingElement.Encrypt = value;
		}

		/// <summary>
		/// The maximum number of connections allowed in the connection pool for a particular connection URI.
		/// </summary>
		public int MaxConnectionPoolSize
		{
			get => BindingElement.MaxConnectionPoolSize;
			set => BindingElement.MaxConnectionPoolSize = value;
		}

		/// <summary>
		/// Gets or sets the value for WorkstationId which is the name of the workstation connecting to SQL Server.
		/// </summary>
		public string WorkstationId
		{
			get => BindingElement.WorkstationId;
			set => BindingElement.WorkstationId = value;
		}

		#endregion

		#region Binding Tab - FOR XML Settings

		/// <summary>
		/// Gets or sets the name of the root node which will be used for executing FOR XML stored procedures.
		/// </summary>
		public string XmlStoredProcedureRootNodeName
		{
			get => BindingElement.XmlStoredProcedureRootNodeName;
			set => BindingElement.XmlStoredProcedureRootNodeName = value;
		}

		/// <summary>
		/// Gets or sets the namespace of the root node which will be used for executing FOR XML stored procedures.
		/// </summary>
		public string XmlStoredProcedureRootNodeNamespace
		{
			get => BindingElement.XmlStoredProcedureRootNodeNamespace;
			set => BindingElement.XmlStoredProcedureRootNodeNamespace = value;
		}

		#endregion
	}
}
