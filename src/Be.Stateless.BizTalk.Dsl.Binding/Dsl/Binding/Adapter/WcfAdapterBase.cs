﻿#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

extern alias ExplorerOM;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.ComponentModel;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;
using Microsoft.ServiceModel.Channels.Common;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	/// <summary>
	/// Binding DSL base class for all WCF-based BizTalk Server Adapters.
	/// </summary>
	/// <typeparam name="TAddress">
	/// The address configuration type to which the actual adapter will be connected to. It typically is either <see
	/// cref="EndpointAddress"/> or derived from <see cref="ConnectionUri"/>.
	/// </typeparam>
	/// <typeparam name="TBinding">
	/// The <see cref="StandardBindingElement"/>-derived class that matches the adapter to be configured.
	/// </typeparam>
	/// <typeparam name="TConfig">
	/// The <see cref="AdapterConfig"/>-derived class that matches the adapter to be configured.
	/// </typeparam>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters">WCF Adapters</seealso>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/what-are-the-wcf-adapters">What Are the WCF Adapters?</seealso>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>
	public abstract class WcfAdapterBase<TAddress, TBinding, TConfig>
		: AdapterBase,
			IAdapterConfigAddress<TAddress>,
			IAdapterConfigBinding<TBinding>,
			IAdapterConfigIdentity
		where TBinding : StandardBindingElement, new()
		where TConfig : AdapterConfig,
		IAdapterConfigAddress,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
		new()
	{
		protected WcfAdapterBase(ProtocolType protocolType) : base(protocolType)
		{
			AdapterConfig = new();
			BindingElement = new();
		}

		#region IAdapterConfigAddress<TAddress> Members

		public TAddress Address { get; set; }

		#endregion

		#region IAdapterConfigBinding<TBinding> Members

		TBinding IAdapterConfigBinding<TBinding>.Binding => BindingElement;

		#endregion

		#region IAdapterConfigIdentity Members

		/// <summary>
		/// Specify the identity of the service that this receive location provides.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The values that can be specified for the Identity property differ according to the security configuration. These
		/// settings enable the client to authenticate this receive location. In the handshake process between the client and
		/// service, the Windows Communication Foundation (WCF) infrastructure will ensure that the identity of the expected
		/// service matches the values of this element.
		/// </para>
		/// <para>
		/// It defaults to <see cref="string.Empty"/>.
		/// </para>
		/// </remarks>
		/// <example>
		/// XML Blob example: <code><![CDATA[
		/// &lt;identity&gt;
		///   &lt;userPrincipalName value="username@contoso.com" /&gt;
		/// &lt;/identity&gt;]]>
		/// </code>
		/// </example>
		public IdentityElement Identity
		{
			get => _identity;
			set
			{
				AdapterConfig.Identity = new IdentityElementSurrogate(value).ConfigXml;
				_identity = value;
			}
		}

		#endregion

		#region Base Class Member Overrides

		protected override void ApplyEnvironmentOverrides(string environment)
		{
			base.ApplyEnvironmentOverrides(environment);
			(BindingElement as ISupportEnvironmentOverride)?.ApplyEnvironmentOverrides(environment);
		}

		protected override string GetAddress()
		{
			// Address.ToString() because EndpointAddress or ConnectionUri ToString() internally calls Uri.ToString()
			return Equals(Address, default(TAddress)) ? null : Address.ToString();
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			AdapterConfig.Save(propertyBag as ExplorerOM::Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected override void Validate()
		{
			// TODO validate BtsActionMapping against orchestration port's actions, see Microsoft.BizTalk.Adapter.Wcf.Metadata.BtsActionMapping and Microsoft.BizTalk.Adapter.Wcf.Metadata.BtsActionMappingHelper.CreateXml(BtsActionMapping btsActionMapping)
			AdapterConfig.Address = GetAddress();
			AdapterConfig.Validate();
			AdapterConfig.Address = null;
		}

		#endregion

		protected TConfig AdapterConfig { get; }

		[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Unit test relies on reflection to call setter.")]
		protected TBinding BindingElement { get; private set; }

		private IdentityElement _identity;
	}
}
