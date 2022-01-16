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
using Microsoft.BizTalk.Adapter.ServiceBus;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.ServiceBus;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfBasicHttpRelayAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// Microsoft BizTalk Server uses the WCF-BasicHttpRelay adapter when receiving and sending WCF service requests through
		/// the <see cref="BasicHttpRelayBinding"/>. The WCF-BasicHttpRelay adapter enables you to send and receive messages from
		/// the Service Bus relay endpoints using the <see cref="BasicHttpRelayBinding"/>.
		/// </summary>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-basichttprelay-adapter">WCF-BasicHttpRelay Adapter</seealso>
		public class Outbound : WcfBasicHttpRelayAdapter<BasicHttpRelayTLConfig>,
			IOutboundAdapter,
			IAdapterConfigClientCertificate,
			IAdapterConfigOutboundAction,
			IAdapterConfigOutboundPropagateFaultMessage,
			IAdapterConfigProxySettings,
			IAdapterConfigProxyToUse
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

			#region IAdapterConfigClientCertificate Members

			/// <summary>
			/// Specify the thumbprint of the X.509 certificate for authenticating this send port to services. This property is
			/// required if the <see cref="WcfBasicHttpRelayAdapter{TConfig}.MessageClientCredentialType"/> property is set to
			/// <see cref="BasicHttpMessageCredentialType.Certificate"/>.
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
		}

		#endregion
	}
}
