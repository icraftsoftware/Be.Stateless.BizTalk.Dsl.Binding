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
using System.ServiceModel.Configuration;
using System.Transactions;
using Microsoft.BizTalk.Adapter.Wcf.Config;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfCustomAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The WCF-Custom adapter is used to enable the use of WCF extensibility components in BizTalk Server. The adapter
		/// enables complete flexibility of the WCF framework. It allows users to select and configure a WCF binding for the
		/// receive location and send port. It also allows users to set the endpoint behaviors and security settings.
		/// </summary>
		/// <remarks>
		/// You use the WCF-Custom send adapter to call a WCF service through the typeless contract with the bindings, service
		/// behavior, endpoint behavior, security mechanism, and the source of the outbound message body that you selected and
		/// configured.
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/wcf-custom-transport-properties-dialog-box-send-messages-tab">What Is the WCF-Custom Adapter?</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-custom-send-port">How to Configure a WCF-Custom Send Port</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>.
		public class Outbound<TBinding>
			: WcfCustomAdapter<TBinding, CustomTLConfig>,
				IOutboundAdapter,
				IAdapterConfigOutboundAction,
				IAdapterConfigOutboundCredentials,
				IAdapterConfigOutboundPropagateFaultMessage,
				IAdapterConfigOutboundTransactionIsolation,
				IAdapterConfigProxySettings,
				IAdapterConfigProxyToUse
			where TBinding : StandardBindingElement, new()
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Outbound()
			{
				// Messages Tab - Error Handling Settings
				PropagateFaultMessage = true;
			}

			public Outbound(Action<Outbound<TBinding>> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region IAdapterConfigOutboundAction Members

			public string StaticAction
			{
				get => AdapterConfig.StaticAction;
				set => AdapterConfig.StaticAction = value;
			}

			#endregion

			#region IAdapterConfigOutboundCredentials Members

			public string AffiliateApplicationName
			{
				get => AdapterConfig.AffiliateApplicationName;
				set => AdapterConfig.AffiliateApplicationName = value;
			}

			public string Password
			{
				get => AdapterConfig.Password;
				set => AdapterConfig.Password = value;
			}

			public string UserName
			{
				get => AdapterConfig.UserName;
				set => AdapterConfig.UserName = value;
			}

			public bool UseSSO
			{
				get => AdapterConfig.UseSSO;
				set => AdapterConfig.UseSSO = value;
			}

			#endregion

			#region IAdapterConfigOutboundPropagateFaultMessage Members

			public bool PropagateFaultMessage
			{
				get => AdapterConfig.PropagateFaultMessage;
				set => AdapterConfig.PropagateFaultMessage = value;
			}

			#endregion

			#region IAdapterConfigOutboundTransactionIsolation Members

			public bool EnableTransaction
			{
				get => AdapterConfig.EnableTransaction;
				set => AdapterConfig.EnableTransaction = value;
			}

			public IsolationLevel IsolationLevel
			{
				get => AdapterConfig.IsolationLevel;
				set => AdapterConfig.IsolationLevel = value;
			}

			#endregion

			#region IAdapterConfigProxySettings Members

			/// <summary>
			/// Specify the address of the proxy server. Use the <b>https</b> or the http scheme depending on the security
			/// configuration. This address can be followed by a colon and the port number. The property is required if the <see
			/// cref="ProxyToUse"/> property is set to <see cref="ProxySelection.UserSpecified"/>.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="string.Empty"/>.
			/// </remarks>
			public string ProxyAddress
			{
				get => AdapterConfig.ProxyAddress;
				set => AdapterConfig.ProxyAddress = value;
			}

			/// <summary>
			/// Specify the password to use for the proxy server specified in the <see cref="ProxyAddress"/> property.
			/// </summary>
			/// <remarks>
			/// It defaults to <see cref="string.Empty"/>.
			/// </remarks>
			public string ProxyPassword
			{
				get => AdapterConfig.ProxyPassword;
				set => AdapterConfig.ProxyPassword = value;
			}

			/// <summary>
			/// Specify the user name to use for the proxy server specified in the ProxyAddress property. The property is required
			/// if the <see cref="ProxyToUse"/> property is set to <see cref="ProxySelection.UserSpecified"/>.
			/// </summary>
			/// <remarks>
			/// For more information about this property, see <see
			/// href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-wshttp-send-port">How to Configure a
			/// WCF-WSHttp Send Port</see>.
			/// </remarks>
			public string ProxyUserName
			{
				get => AdapterConfig.ProxyUserName;
				set => AdapterConfig.ProxyUserName = value;
			}

			#endregion

			#region IAdapterConfigProxyToUse Members

			/// <summary>
			/// Specify which proxy server to use for outgoing HTTP traffic.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Applicable values are:
			/// <list type="bullet">
			/// <item>
			/// <see cref="ProxySelection.None"/> &#8212; Do not use a proxy server for this send port.
			/// </item>
			/// <item>
			/// <see cref="ProxySelection.Default"/> &#8212; Use the proxy settings in the send handler hosting this send port.
			/// </item>
			/// <item>
			/// <see cref="ProxySelection.UserSpecified"/> &#8212; Use the proxy server specified in the <see
			/// cref="ProxyAddress"/> property.
			/// </item>
			/// </list>
			/// </para>
			/// <para>
			/// It defaults to <see cref="ProxySelection.None"/>.
			/// </para>
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
