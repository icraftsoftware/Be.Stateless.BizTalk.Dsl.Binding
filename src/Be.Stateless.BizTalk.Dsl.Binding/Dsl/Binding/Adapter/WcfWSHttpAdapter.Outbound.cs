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
using System.ServiceModel;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfWSHttpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// You can use the WCF-WSHttp adapter to do cross-computer communication with services and clients that can understand
		/// the next-generation Web service standards, using either the HTTP or HTTPS transport with text or Message Transmission
		/// Optimization Mechanism (MTOM) encoding. The WCF-WSHttp adapter provides full access to the SOAP security,
		/// reliability, and transaction features.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/what-is-the-wcf-wshttp-adapter">What Is the WCF-WSHttp Adapter?</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-wshttp-send-port">How to Configure a WCF-WSHttp Send Port</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-wshttp-transport-properties-dialog-box-send-security-tab">WCF-WSHttp Transport Properties Dialog Box, Send, Security Tab</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>.
		public class Outbound
			: WcfWSHttpAdapter<EndpointAddress, WSHttpTLConfig>,
				IOutboundAdapter,
				IAdapterConfigClientCertificate,
				IAdapterConfigOutboundAction,
				IAdapterConfigOutboundCredentials,
				IAdapterConfigOutboundPropagateFaultMessage,
				IAdapterConfigProxySettings,
				IAdapterConfigProxyToUse
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Outbound()
			{
				// Proxy Tab - General Settings
				ProxyToUse = ProxySelection.None;

				// Messages Tab - Error Handling Settings
				PropagateFaultMessage = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigClientCertificate Members

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This property is
			/// required if the <see cref="WcfWSHttpAdapter{TAddress,TConfig}.MessageClientCredentialType"/> property is set to
			/// <see cref="MessageCredentialType.Certificate"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The certificate to be used for this property must be installed into the My store in the Current User location of
			/// the user account for the send handler hosting this send port.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ClientCertificate
			{
				get => AdapterConfig.ClientCertificate;
				set => AdapterConfig.ClientCertificate = value;
			}

			#endregion

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

			#endregion

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get => AdapterConfig.PropagateFaultMessage;
				set => AdapterConfig.PropagateFaultMessage = value;
			}

			#endregion

			#region IAdapterConfigProxySettings Members

			/// <summary>
			/// Specify the address of the proxy server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Use the https or the http scheme depending on the security configuration. This address can be followed by a colon
			/// and the port number. For example, <c>http://127.0.0.1:8080</c>.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ProxyAddress
			{
				get => AdapterConfig.ProxyAddress;
				set => AdapterConfig.ProxyAddress = value;
			}

			/// <summary>
			/// Specify the user name to use for the proxy.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The WCF-WSHttp adapter leverages the <see cref="WSHttpBinding"/> in the buffered transfer mode to communicate with
			/// an endpoint. Proxy credentials of <see cref="WSHttpBinding"/> are applicable only when the <see
			/// cref="WcfWSHttpAdapter{TAddress,TConfig}.SecurityMode"/> is <see cref="SecurityMode.Transport"/> or <see
			/// cref="SecurityMode.None"/>. If you set the <see cref="WcfWSHttpAdapter{TAddress,TConfig}.SecurityMode"/> property
			/// to
			/// <see cref="SecurityMode.Message"/>
			/// or <see cref="SecurityMode.TransportWithMessageCredential"/>, the WCF-WSHttp adapter does not use the credential
			/// specified in the <see cref="ProxyUserName"/> and <see cref="ProxyPassword"/> properties for authentication against
			/// the proxy.
			/// </para>
			/// <para>
			/// The WCF-WSHttp send adapter uses Basic authentication for the proxy.
			/// </para>
			/// <para>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </para>
			/// </remarks>
			public string ProxyUserName
			{
				get => AdapterConfig.ProxyUserName;
				set => AdapterConfig.ProxyUserName = value;
			}

			/// <summary>
			/// Specify the password to use for the proxy.
			/// </summary>
			/// <remarks>
			/// It defaults to an <see cref="string.Empty"/> string.
			/// </remarks>
			public string ProxyPassword
			{
				get => AdapterConfig.ProxyPassword;
				set => AdapterConfig.ProxyPassword = value;
			}

			#endregion

			#region IAdapterConfigProxyToUse Members

			/// <summary>
			/// Specify which proxy server to use for outgoing HTTP traffic.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <see cref="ProxySelection.None"/> &#8212; Do not use a proxy server for this send port.
			/// </item>
			/// <item>
			/// <see cref="ProxySelection.Default"/> &#8212; Use the proxy settings in the send handler hosting this send port.
			/// </item>
			/// <item>
			/// <see cref="ProxySelection.UserSpecified"/> &#8212; Use the proxy server specified in the <see cref="ProxyAddress"/>
			/// property.
			/// </item>
			/// </list>
			/// It defaults to <see cref="ProxySelection.None"/>.
			/// </remarks>
			public ProxySelection ProxyToUse
			{
				get => AdapterConfig.ProxyToUse;
				set => AdapterConfig.ProxyToUse = value;
			}

			#endregion
		}

		#endregion
	}
}
