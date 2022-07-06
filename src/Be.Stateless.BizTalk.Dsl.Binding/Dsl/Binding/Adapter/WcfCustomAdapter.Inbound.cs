#region Copyright & License

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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Linq.Extensions;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfCustomAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The WCF-Custom adapter is used to enable the use of WCF extensibility components in BizTalk Server. The adapter
		/// enables complete flexibility of the WCF framework. It allows users to select and configure a WCF binding for the
		/// receive location and send port. It also allows users to set the endpoint behaviors and security settings.
		/// </summary>
		/// <remarks>
		/// You use the WCF-Custom receive adapter to receive WCF service requests through the bindings, service behavior,
		/// endpoint behavior, security mechanism, and the source of the inbound message body that you selected and configured in
		/// the transport properties dialog in the receive location. A receive location that uses the WCF-Custom receive adapter
		/// can be configured as one-way or request-response (two-way).
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/what-is-the-wcf-custom-adapter">What Is the WCF-Custom Adapter?</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-a-wcf-custom-receive-location">How to Configure a WCF-Custom Receive Location</seealso>.
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/wcf-adapters-property-schema-and-properties">WCF Adapters Property Schema and Properties</seealso>.
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public class Inbound<TBinding>
			: WcfCustomAdapter<TBinding, CustomRLConfig>,
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

			#region Base Class Member Overrides

			[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
			protected override void ApplyEnvironmentOverrides(string environment)
			{
				base.ApplyEnvironmentOverrides(environment);
				ServiceBehaviors.Select(sb => sb as ISupportEnvironmentOverride).ForEach(sb => sb.ApplyEnvironmentOverrides(environment));
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
