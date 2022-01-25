﻿#region Copyright & License

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
using Microsoft.Adapters.SAP;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfSapAdapter
	{
		#region Nested Type: Inbound

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
		public class Inbound : WcfSapAdapter<CustomRLConfig>,
			IInboundAdapter,
			IAdapterConfigInboundCredentials,
			IAdapterConfigInboundDisableLocationOnFailure,
			IAdapterConfigInboundIncludeExceptionDetailInFaults,
			IAdapterConfigInboundSuspendRequestMessageOnFailure,
			IAdapterConfigServiceBehavior
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
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

			#region Binding Tab - Metadata  Settings

			/// <summary>
			/// Determines the XML format of the incoming (from SAP) IDoc
			/// </summary>
			/// <remarks>
			/// There are three possible values for the ReceiveIDocFormat property:
			/// <list type="bullet">
			/// <item>
			/// <see cref="IdocReceiveFormat.String"/> specifies that the IDoc message should be represented as a single, string
			/// field in the WCF message.
			/// </item>
			/// <item>
			/// <see cref="IdocReceiveFormat.Typed"/> specifies that the IDoc message should be parsed and represented as a
			/// strongly-typed WCF message.
			/// </item>
			/// <item>
			/// <see cref="IdocReceiveFormat.Rfc"/> specifies that the SAP adapter should pass the incoming RFC call as a WCF
			/// message with RFC parameters.
			/// </item>
			/// </list>
			/// <para>
			/// The default is <see cref="IdocReceiveFormat.Typed"/>.
			/// </para>
			/// </remarks>
			public IdocReceiveFormat ReceiveIdocFormat
			{
				get => BindingElement.ReceiveIdocFormat;
				set => BindingElement.ReceiveIdocFormat = value;
			}

			#endregion

			#region Binding Tab - TrfcServer  Settings

			/// <summary>
			/// Specifies the database connection string for the SQL Server database that the SAP adapter uses to store
			/// Transaction Ids (TIDs) if the adapter should operate in TRFC server mode.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set this property to enable inbound tRFC server calls for receiving IDocs or RFCs from SAP. The default
			/// is <c>null</c>; tRFC server calls are not enabled.
			/// </para>
			/// <para>
			/// You can specify the connection string in the following format:
			/// <code>
			/// <![CDATA[Data Source=<myServerAddress>;Initial Catalog=<myDataBase>;User Id=<myUsername>;Password=<myPassword>;]]>
			/// </code>
			/// </para>
			/// <para>
			/// The BizTalk Adapter Pack setup wizard installs some SQL scripts that must be run by the SQL Server administrator
			/// against an existing database to create the SQL Server objects that are used by the adapter to store TIDs to enable
			/// inbound transactional RFC (tRFC) server calls. For more information about the SQL scripts, refer to the BizTalk
			/// Adapter Pack installation guide available at <c>&lt;installation drive&gt;:\Program Files\Microsoft BizTalk
			/// Adapter Pack\Documents</c>.
			/// </para>
			/// </remarks>
			public string TidDatabaseConnectionString
			{
				get => BindingElement.TidDatabaseConnectionString;
				set => BindingElement.TidDatabaseConnectionString = value;
			}

			#endregion

			#region Binding Tab - Idoc Settings

			/// <summary>
			/// Determines whether the received IDoc, when in flat file format, has extra spaces at the end of each segment. This
			/// is required if the flat file is converted to XML using the Flat File Parser pipeline component in BizTalk.
			/// </summary>
			/// <remarks>
			/// The default is false; lines are not padded.
			/// </remarks>
			public bool PadReceivedIdocWithSpaces
			{
				get => BindingElement.PadReceivedIdocWithSpaces;
				set => BindingElement.PadReceivedIdocWithSpaces = value;
			}

			/// <summary>
			/// Syntax: DefaultRelease;DOCTYPE1=DOCREL1;DOCTYPE2=DOCREL2... All fields are optional. If no matching release is
			/// found, DOCREL of the incoming IDOC will be used.
			/// </summary>
			[SuppressMessage("ReSharper", "CommentTypo")]
			public string ReceivedIdocRelease
			{
				get => BindingElement.ReceivedIdocRelease;
				set => BindingElement.ReceivedIdocRelease = value;
			}

			#endregion
		}

		#endregion
	}
}
