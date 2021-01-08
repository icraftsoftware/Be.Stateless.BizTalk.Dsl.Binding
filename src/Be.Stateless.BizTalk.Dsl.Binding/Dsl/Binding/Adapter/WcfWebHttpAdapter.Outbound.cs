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
	public abstract partial class WcfWebHttpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// Microsoft BizTalk Server uses the WCF-WebHttp adapter to send messages to RESTful services.
		/// </summary>
		/// <remarks>
		/// The WCF-WebHttp send adapter sends HTTP messages to a service from a BizTalk message. The receive location receives
		/// messages from a RESTful service. For GET and DELETE request, the adapter does not use any payload. For POST and PUT
		/// request, the adapter uses the BizTalk message body part to the HTTP content/payload.
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-webhttp-adapter">WCF-WebHttp Adapter</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Outbound
			: WcfWebHttpAdapter<EndpointAddress, WebHttpTLConfig>,
				IOutboundAdapter,
				IAdapterConfigClientCertificate,
				IAdapterConfigOutboundCredentials,
				IAdapterConfigOptionalAccessControlService,
				IAdapterConfigOptionalSharedAccessSignature,
				IAdapterConfigProxySettings,
				IAdapterConfigProxyToUse
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Outbound()
			{
				ProxyToUse = ProxySelection.None;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigClientCertificate Members

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This property is
			/// required if the <see cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> property is set to
			/// <see cref="HttpClientCredentialType.Certificate"/>.
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

			#region IAdapterConfigOptionalAccessControlService Members

			/// <summary>
			/// Specify whether to authenticate with the Service Bus.
			/// </summary>
			/// <remarks>
			/// This is required only when invoking a REST interface for Service Bus related entities.
			/// </remarks>
			public bool UseAcsAuthentication
			{
				get => AdapterConfig.UseAcsAuthentication;
				set => AdapterConfig.UseAcsAuthentication = value;
			}

			public Uri StsUri
			{
				get => new Uri(AdapterConfig.StsUri);
				set => AdapterConfig.StsUri = (value ?? throw new ArgumentNullException(nameof(value))).ToString();
			}

			public string IssuerName
			{
				get => AdapterConfig.IssuerName;
				set => AdapterConfig.IssuerName = value;
			}

			public string IssuerSecret
			{
				get => AdapterConfig.IssuerSecret;
				set => AdapterConfig.IssuerSecret = value;
			}

			#endregion

			#region IAdapterConfigOptionalSharedAccessSignature Members

			public bool UseSasAuthentication
			{
				get => AdapterConfig.UseSasAuthentication;
				set => AdapterConfig.UseSasAuthentication = value;
			}

			public string SharedAccessKeyName
			{
				get => AdapterConfig.SharedAccessKeyName;
				set => AdapterConfig.SharedAccessKeyName = value;
			}

			public string SharedAccessKey
			{
				get => AdapterConfig.SharedAccessKey;
				set => AdapterConfig.SharedAccessKey = value;
			}

			#endregion

			#region IAdapterConfigOutboundCredentials Members

			/// <summary>
			/// Specify the affiliate application to use for Enterprise Single Sign-On (SSO).
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> and <see cref="UseSSO"/> is set to
			/// <c>True</c>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string AffiliateApplicationName
			{
				get => AdapterConfig.AffiliateApplicationName;
				set => AdapterConfig.AffiliateApplicationName = value;
			}

			/// <summary>
			/// Specify the password to use for authentication with the destination server when the <see cref="UseSSO"/> property
			/// is set to <c>False</c>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> and <see cref="UseSSO"/> is set to
			/// <c>False</c>.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string Password
			{
				get => AdapterConfig.Password;
				set => AdapterConfig.Password = value;
			}

			/// <summary>
			/// Specify the user name to use for authentication with the destination server when the <see cref="UseSSO"/> property
			/// is set to <c>False</c>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/> and <see cref="UseSSO"/> is set to
			/// <c>False</c>.
			/// </para>
			/// <para>
			/// You do not have to use the domain\user format for this property.
			/// </para>
			/// <para>
			/// It defaults to <see cref="string.Empty"/>.
			/// </para>
			/// </remarks>
			public string UserName
			{
				get => AdapterConfig.UserName;
				set => AdapterConfig.UserName = value;
			}

			/// <summary>
			/// Specify whether to use Single Sign-On to retrieve client credentials for authentication with the destination
			/// server.
			/// </summary>
			/// <remarks>
			/// <para>
			/// You must set the credentials if you selected the <see cref="HttpClientCredentialType.Basic"/> or <see
			/// cref="HttpClientCredentialType.Digest"/> option for <see
			/// cref="WcfWebHttpAdapter{TAddress,TConfig}.TransportClientCredentialType"/>.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool UseSSO
			{
				get => AdapterConfig.UseSSO;
				set => AdapterConfig.UseSSO = value;
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
			/// The WCF-BasicHttp adapter leverages the <see cref="BasicHttpBinding"/> in the buffered transfer mode to
			/// communicate with an endpoint. Proxy credentials of <see cref="BasicHttpBinding"/> are applicable only when the
			/// <see cref="WcfBasicHttpAdapter{TAddress,TConfig}.SecurityMode"/> is <see cref="BasicHttpSecurityMode.Transport"/>,
			/// <see cref="BasicHttpSecurityMode.None"/>, or <see cref="BasicHttpSecurityMode.TransportCredentialOnly"/>. If you
			/// set the <see cref="WcfBasicHttpAdapter{TAddress,TConfig}.SecurityMode"/> property to <see
			/// cref="BasicHttpSecurityMode.Message"/> or <see cref="BasicHttpSecurityMode.TransportWithMessageCredential"/>, the
			/// WCF-BasicHttp adapter does not use the credential specified in the <see cref="ProxyUserName"/> and <see
			/// cref="ProxyPassword"/> properties for authentication against the proxy.
			/// </para>
			/// <para>
			/// The WCF-BasicHttp send adapter uses Basic authentication for the proxy.
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

			#region Message Tab - Outbound Message Settings

			/// <summary>
			/// Specify whether to remove the message payload for outgoing HTTP request made for some HTTP verbs.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Based on the verb you use to invoke a REST endpoint, you may or may not require a message payload. For example,
			/// you may not need a message payload while using the GET or DELETE verbs. However, to trigger a call to the REST
			/// endpoint using the send port, you may use a dummy message that includes a message payload. Before the message is
			/// sent to the REST endpoint, the message payload from the dummy message must be removed. You can specify the verbs
			/// for which the message payload must be removed using the Suppress Body for Verbs property.
			/// </para>
			/// <para>
			/// For example, if you want to remove the message payload while using a GET verb, specify the value for this property
			/// as GET.
			/// </para>
			/// </remarks>
			public string SuppressMessageBodyForHttpVerbs
			{
				get => AdapterConfig.SuppressMessageBodyForHttpVerbs;
				set => AdapterConfig.SuppressMessageBodyForHttpVerbs = value;
			}

			#endregion
		}

		#endregion
	}
}
