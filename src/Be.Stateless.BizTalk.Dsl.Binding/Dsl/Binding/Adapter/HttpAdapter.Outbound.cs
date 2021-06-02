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
using System.Net.Mime;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class HttpAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// You use the HTTP adapter to exchange information between Microsoft BizTalk Server and an application by means of the
		/// HTTP protocol. HTTP is the primary protocol for inter-business message exchange. Applications can send messages to a
		/// server by sending HTTP POST or HTTP GET requests to a specified HTTP URL. The HTTP adapter receives the HTTP requests
		/// and submits them to BizTalk Server for processing. Similarly, BizTalk Server can transmit messages to remote
		/// applications by sending HTTP POST requests to a specified HTTP URL.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/http-adapter">HTTP Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/configuring-the-http-adapter">Configuring the HTTP Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/http-send-adapter">HTTP Send Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-an-http-send-handler">How to Configure an HTTP Send Handler</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-an-http-send-port">How to Configure an HTTP Send Port</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/restrictions-on-the-destination-url-property">Restrictions on the Destination URL Property</seealso>
		public class Outbound : HttpAdapter, IOutboundAdapter, IAdapterConfigOutboundCredentials
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Outbound()
			{
				// General Tab
				EnableChunkedEncoding = true;
				MaxRedirects = 5;
				ContentType = MediaTypeNames.Text.Xml;

				// Proxy Tab
				UseHandlerProxySettings = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigOutboundCredentials Members

			/// <summary>
			/// Specifies if SSO will be used for the send port.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool UseSSO { get; set; }

			/// <summary>
			/// Name of the affiliate application to use for SSO.
			/// </summary>
			/// <remarks>
			/// Required if <see cref="UseSSO"/> is <c>True</c>.
			/// </remarks>
			public string AffiliateApplicationName { get; set; }

			/// <summary>
			/// User name to use for authentication with the server.
			/// </summary>
			/// <remarks>
			/// This value is required if you select <see cref="HttpAdapter.AuthenticationScheme.Basic"/> or <see
			/// cref="HttpAdapter.AuthenticationScheme.Digest"/> authentication. The HTTP adapter ignores the value of this
			/// property if <see cref="UseSSO"/> is <c>True</c>.
			/// </remarks>
			public string UserName { get; set; }

			/// <summary>
			/// User password to use for authentication with the server.
			/// </summary>
			/// <remarks>
			/// This value is required if you select <see cref="HttpAdapter.AuthenticationScheme.Basic"/> or <see
			/// cref="HttpAdapter.AuthenticationScheme.Digest"/> authentication. The HTTP adapter ignores the value of this
			/// property if <see cref="UseSSO"/> is <c>True</c>.
			/// </remarks>
			public string Password { get; set; }

			#endregion

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return Url.ToString();
			}

			protected override void Save(IPropertyBag propertyBag)
			{
				propertyBag.WriteAdapterCustomProperty(nameof(EnableChunkedEncoding), EnableChunkedEncoding);
				propertyBag.WriteAdapterCustomProperty(nameof(RequestTimeout), (int) RequestTimeout.TotalSeconds);
				propertyBag.WriteAdapterCustomProperty(nameof(MaxRedirects), MaxRedirects);
				propertyBag.WriteAdapterCustomProperty(nameof(ContentType), ContentType);
				propertyBag.WriteAdapterCustomProperty(nameof(UseHandlerProxySettings), UseHandlerProxySettings);
				if (!UseHandlerProxySettings)
				{
					propertyBag.WriteAdapterCustomProperty(nameof(UseProxy), UseProxy);
					propertyBag.WriteAdapterCustomProperty(nameof(ProxyName), ProxyName);
					propertyBag.WriteAdapterCustomProperty(nameof(ProxyPort), ProxyPort);
					propertyBag.WriteAdapterCustomProperty("ProxyUsername", ProxyUserName);
					propertyBag.WriteAdapterCustomProperty(nameof(ProxyPassword), ProxyPassword);
				}
				propertyBag.WriteAdapterCustomProperty(nameof(AuthenticationScheme), AuthenticationScheme.ToString());
				propertyBag.WriteAdapterCustomProperty("Username", UserName);
				propertyBag.WriteAdapterCustomProperty(nameof(Password), Password);
				propertyBag.WriteAdapterCustomProperty(nameof(UseSSO), UseSSO);
				propertyBag.WriteAdapterCustomProperty(nameof(AffiliateApplicationName), AffiliateApplicationName);
				propertyBag.WriteAdapterCustomProperty(nameof(Certificate), Certificate);
			}

			protected override void Validate()
			{
				if (MaxRedirects is < 0 or > 10) throw new BindingException("MaxRedirects must range from 0 to 10.");
				if (ProxyPort is < 0 or > 65535) throw new BindingException("ProxyPort must range from 0 to 65535.");
				if (AuthenticationScheme is AuthenticationScheme.Basic or AuthenticationScheme.Digest)
				{
					if (UserName.IsNullOrEmpty() || Password.IsNullOrEmpty())
						throw new BindingException(
							$"UserName and Password must neither be null nor empty when AuthenticationScheme is either {AuthenticationScheme.Basic} or {AuthenticationScheme.Digest}.");
				}
				if (UseSSO && AffiliateApplicationName.IsNullOrEmpty()) throw new BindingException("AffiliateApplicationName must neither be null nor empty when SSO is used.");
			}

			#endregion

			#region Authentication Tab

			/// <summary>
			/// Type of authentication to use with the destination server.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="HttpAdapter.AuthenticationScheme.Anonymous"/>.
			/// </remarks>
#pragma warning disable 108
			public AuthenticationScheme AuthenticationScheme { get; set; }
#pragma warning restore 108

			/// <summary>
			/// Specify the thumbprint of the client certificate to use for establishing a Secure Sockets Layer (SSL) connection.
			/// </summary>
			public string Certificate { get; set; }

			#endregion

			#region Proxy Tab

			/// <summary>
			/// Specifies whether the HTTP send port will use the proxy configuration for the send handler.
			/// </summary>
			/// <remarks>
			/// <para>
			/// When true, the send port will use the proxy settings specified at the handler level. When false, the send adapter
			/// will use the proxy information specified on the send port.
			/// </para>
			/// <para>
			/// It defaults to <c>True</c>.
			/// </para>
			/// </remarks>
			public bool UseHandlerProxySettings { get; set; }

			/// <summary>
			/// Specifies whether the HTTP adapter will use the proxy server. The proxy server can be shared by all HTTP send
			/// ports.
			/// </summary>
			/// <remarks>
			/// <para>
			/// This property is ignored if <see cref="UseHandlerProxySettings"/> is <c>True</c>.
			/// </para>
			/// <para>
			/// It defaults to <c>False</c>.
			/// </para>
			/// </remarks>
			public bool UseProxy { get; set; }

			/// <summary>
			/// Specifies the proxy server name.
			/// </summary>
			/// <remarks>
			/// The HTTP send adapter ignores this property if the <see cref="UseHandlerProxySettings"/> property is set to
			/// <c>True</c>. Otherwise, the HTTP send adapter uses this property only if <see cref="UseProxy"/> is <c>True</c>.
			/// This property is required if <see cref="UseProxy"/> is <c>True</c>.
			/// </remarks>
			public string ProxyName { get; set; }

			/// <summary>
			/// Specifies the proxy server port.
			/// </summary>
			/// <remarks>
			/// <para>
			/// The HTTP send adapter ignores this property if <see cref="UseHandlerProxySettings"/> is <c>True</c>. Otherwise,
			/// HTTP send adapter uses this property only if <see cref="UseProxy"/> is <c>True</c>. This property is required if
			/// <see cref="UseProxy"/> is <c>True</c>.
			/// </para>
			/// <para>
			/// Values between <c>0</c> and <c>65535</c> are accepted.
			/// </para>
			/// </remarks>
			public int ProxyPort { get; set; }

			/// <summary>
			/// Specifies the user name for authentication with the proxy server.
			/// </summary>
			/// <remarks>
			/// The HTTP send adapter ignores this property if <see cref="UseHandlerProxySettings"/> is <c>True</c>. Otherwise,
			/// HTTP send adapter uses this property only if <see cref="UseProxy"/> is <c>True</c>.
			/// </remarks>
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public DSL API.")]
			public string ProxyUserName { get; set; }

			/// <summary>
			/// Specifies the user password for authentication with the proxy server.
			/// </summary>
			/// <remarks>
			/// The HTTP send adapter ignores this property if <see cref="UseHandlerProxySettings"/> is <c>True</c>. Otherwise,
			/// HTTP send adapter uses this property only if <see cref="UseProxy"/> is <c>True</c>.
			/// </remarks>
			public string ProxyPassword { get; set; }

			#endregion

			#region General Tab

			/// <summary>
			/// Specify the address to send HTTP requests. Include query strings appended to the base URL.
			/// </summary>
			/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/restrictions-on-the-destination-url-property">Restrictions on the Destination URL Property</seealso>
			public Uri Url { get; set; }

			/// <summary>
			/// Specifies whether or not chunked encoding is used by the HTTP adapter.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>True</c>.
			/// </remarks>
			public bool EnableChunkedEncoding { get; set; }

			/// <summary>
			/// Specify the time-out for the HTTP/HTTPS transmission. If the HTTP adapter does not receive the response within
			/// this time, the service logs the error and resubmits the message based on the retry infrastructure.
			/// </summary>
			/// <remarks>
			/// If set to zero (0), the BizTalk Messaging Engine calculates the time-out based on the request message size. If you
			/// do not provide a value, the value for the handler is used.
			/// </remarks>
			public TimeSpan RequestTimeout { get; set; }

			/// <summary>
			/// Maximum number of times that the HTTP adapter can redirect the request.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>5</c>. Values between <c>0</c> and <c>10</c> are accepted.
			/// </remarks>
			public int MaxRedirects { get; set; }

			/// <summary>
			/// Content type of the request messages.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="MediaTypeNames.Text.Xml"/>.
			/// </remarks>
			public string ContentType { get; set; }

			#endregion
		}

		#endregion
	}
}
