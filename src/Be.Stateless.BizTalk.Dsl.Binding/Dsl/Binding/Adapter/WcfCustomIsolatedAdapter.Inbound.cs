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
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfCustomIsolatedAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-CustomIsolated adapter is used to enable the use of WCF extensibility components in BizTalk Server with an
		/// isolated host. The adapter enables complete flexibility of the WCF framework. It allows users to select and configure
		/// a WCF binding for the receive location, and to specify the endpoint behaviors and security settings. This adapter can
		/// only be used by transports that are hosted in Internet Information Services (IIS).
		/// </summary>
		/// <remarks>
		/// The WCF-CustomIsolated adapter consists of a receive adapter only. You use the WCF-CustomIsolated receive adapter to
		/// receive WCF service requests through WCF bindings, service behavior, endpoint behavior, security mechanism, and the
		/// source of the inbound message body that you selected and configured for the receive location running in an isolated
		/// host. A receive location that uses the WCF-CustomIsolated receive adapter can be configured as one-way or
		/// request-response (two-way).
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-customisolated-adapter">WCF-CustomIsolated Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/what-is-the-wcf-customisolated-adapter">What Is the WCF-CustomIsolated Adapter?</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-customisolated-receive-location">How to Configure a WCF-CustomIsolated Receive Location</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound<TBinding>
			: WcfCustomIsolatedAdapter<TBinding, CustomRLConfig>,
				IInboundAdapter,
				IAdapterConfigInboundCredentials,
				IAdapterConfigInboundDisableLocationOnFailure,
				IAdapterConfigInboundIncludeExceptionDetailInFaults,
				IAdapterConfigInboundSuspendRequestMessageOnFailure,
				IAdapterConfigOrdering,
				IAdapterConfigServiceBehavior
			where TBinding : StandardBindingElement, new()
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
				// Other Tab - Credentials Settings
				CredentialType = CredentialSelection.None;

				// Messages Tab - Error Handling Settings
				DisableLocationOnFailure = false;
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;

				ServiceBehaviors = Enumerable.Empty<BehaviorExtensionElement>();
			}

			public Inbound(Action<Inbound<TBinding>> adapterConfigurator) : this()
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

			#region IAdapterConfigOrdering Members

			/// <summary>
			/// Specify whether to preserve message order when processing messages received over the NetMsmq binding.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>
			/// </remarks>
			public bool OrderedProcessing
			{
				get => AdapterConfig.OrderedProcessing;
				set => AdapterConfig.OrderedProcessing = value;
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

			/// <summary>
			/// On the service side, initialize the session-idle timeout which controls how long a session can be idle before
			/// timing out.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>10</c> minutes.
			/// </remarks>
			public TimeSpan ReceiveTimeout
			{
				get => Binding.ReceiveTimeout;
				set => Binding.ReceiveTimeout = value;
			}
		}

		#endregion
	}
}
