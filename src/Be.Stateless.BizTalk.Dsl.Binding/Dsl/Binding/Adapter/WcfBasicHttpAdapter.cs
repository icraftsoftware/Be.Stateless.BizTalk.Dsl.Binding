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
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfBasicHttpAdapter<TAddress, TConfig>
		: WcfTwoWayAdapterBase<TAddress, BasicHttpBindingElement, TConfig>,
			IAdapterConfigBasicHttpSecurity,
			IAdapterConfigMaxReceivedMessageSize,
			IAdapterConfigMessageEncoding,
			IAdapterConfigServiceCertificate
		where TConfig : AdapterConfig,
		IAdapterConfigAddress,
		IAdapterConfigBasicHttpBinding,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigBasicHttpSecurity,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
		IAdapterConfigInboundMessageMarshalling,
		IAdapterConfigOutboundMessageMarshalling,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigServiceCertificate,
		IAdapterConfigTimeouts,
		new()
	{
		static WcfBasicHttpAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new("467c1a52-373f-4f09-9008-27af6b985f14"));
		}

		protected WcfBasicHttpAdapter() : base(_protocolType)
		{
			// Binding Tab - General Settings
			MaxReceivedMessageSize = ushort.MaxValue;

			// Binding Tab - Encoding Settings
			MessageEncoding = WSMessageEncoding.Text;
			TextEncoding = Encoding.UTF8;

			// Security Tab - Security Mode Settings
			SecurityMode = BasicHttpSecurityMode.None;

			// Security Tab - Transport Security Settings
			TransportClientCredentialType = HttpClientCredentialType.None;

			// Security Tab - Message Security Settings
			MessageClientCredentialType = BasicHttpMessageCredentialType.UserName;
			AlgorithmSuite = SecurityAlgorithmSuiteValue.Basic256;
		}

		#region IAdapterConfigBasicHttpSecurity Members

		/// <summary>
		/// Specify the type of security that is used.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="SecurityMode"/> property, see the Security mode
		/// property in <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-receive-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-send-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Send, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="BasicHttpSecurityMode.None"/>.
		/// </para>
		/// </remarks>
		public BasicHttpSecurityMode SecurityMode
		{
			get => AdapterConfig.SecurityMode;
			set => AdapterConfig.SecurityMode = value;
		}

		/// <summary>
		/// Specify the type of credential to be used when performing the client authentication.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="TransportClientCredentialType"/> property, see the
		/// Transport client credential type property in <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-receive-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-send-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Send, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="HttpClientCredentialType.None"/>.
		/// </para>
		/// </remarks>
		public HttpClientCredentialType TransportClientCredentialType
		{
			get => AdapterConfig.TransportClientCredentialType;
			set => AdapterConfig.TransportClientCredentialType = value;
		}

		/// <summary>
		/// Specify the type of credential to be used when performing client authentication using message-based security.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="MessageClientCredentialType"/> property, see the
		/// Message client credential type property in <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-receive-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-send-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Send, Security Tab</see>.
		/// </para>
		/// <para>
		/// It defaults to <see cref="BasicHttpMessageCredentialType.UserName"/>.
		/// </para>
		/// </remarks>
		public BasicHttpMessageCredentialType MessageClientCredentialType
		{
			get => AdapterConfig.MessageClientCredentialType;
			set => AdapterConfig.MessageClientCredentialType = value;
		}

		/// <summary>
		/// Specify the message encryption and key-wrap algorithms. These algorithms map to those specified in the Security
		/// Policy Language (WS-SecurityPolicy) specification.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For more information about the member names for the <see cref="AlgorithmSuite"/> property, see the Algorithm suite
		/// property in <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-receive-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Receive, Security Tab</see> and <see
		/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-basichttp-transport-properties-dialog-box-send-security-tab">WCF-BasicHttp
		/// Transport Properties Dialog Box, Send, Security Tab</see>..
		/// </para>
		/// <para>
		/// It defaults to <see cref="SecurityAlgorithmSuiteValue.Basic256"/>.
		/// </para>
		/// </remarks>
		public SecurityAlgorithmSuiteValue AlgorithmSuite
		{
			get => AdapterConfig.AlgorithmSuite;
			set => AdapterConfig.AlgorithmSuite = value;
		}

		#endregion

		#region IAdapterConfigMaxReceivedMessageSize Members

		/// <summary>
		/// Specify the maximum size, in bytes, for a message (including headers) that can be received on the wire. The size of
		/// the messages is bounded by the amount of memory allocated for each message. You can use this property to limit
		/// exposure to denial of service (DoS) attacks. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// The WCF-BasicHttp adapter leverages the <see cref="BasicHttpBinding"/> class in the buffered transfer mode to
		/// communicate with an endpoint. For the buffered transport mode, the <see cref="HttpBindingBase"/>.<see
		/// cref="HttpBindingBase.MaxBufferSize"/> property is always equal to the value of this property.
		/// </para>
		/// <para>
		/// It defaults to roughly <see cref="ushort"/>.<see cref="ushort.MaxValue"/>, 65536.
		/// </para>
		/// </remarks>
		public int MaxReceivedMessageSize
		{
			get => AdapterConfig.MaxReceivedMessageSize;
			set => AdapterConfig.MaxReceivedMessageSize = value;
		}

		#endregion

		#region IAdapterConfigMessageEncoding Members

		public WSMessageEncoding MessageEncoding
		{
			get => AdapterConfig.MessageEncoding;
			set => AdapterConfig.MessageEncoding = value;
		}

		public Encoding TextEncoding
		{
			get => Encoding.GetEncoding(AdapterConfig.TextEncoding);
			set => AdapterConfig.TextEncoding = (value ?? throw new ArgumentNullException(nameof(value))).WebName;
		}

		#endregion

		#region IAdapterConfigServiceCertificate Members

		public string ServiceCertificate
		{
			get => AdapterConfig.ServiceCertificate;
			set => AdapterConfig.ServiceCertificate = value;
		}

		#endregion

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}
