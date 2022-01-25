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
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class WcfWebHttpAdapter
	{
		#region Nested Type: Inbound

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
		public class Inbound
			: WcfWebHttpAdapter<Uri, WebHttpRLConfig>,
				IInboundAdapter,
				IAdapterConfigInboundDisableLocationOnFailure,
				IAdapterConfigInboundIncludeExceptionDetailInFaults,
				IAdapterConfigInboundSuspendRequestMessageOnFailure,
				IAdapterConfigMaxConcurrentCalls,
				IAdapterConfigServiceBehavior,
				IAdapterConfigSSO
		{
			[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
			public Inbound()
			{
				// Binding Tab - Service Throttling Behavior Settings
				MaxConcurrentCalls = 200;

				// Behavior Tab
				ServiceBehaviors = Enumerable.Empty<BehaviorExtensionElement>();

				// Messages Tab - Error Handling Settings
				DisableLocationOnFailure = false;
				SuspendRequestMessageOnFailure = true;
				IncludeExceptionDetailInFaults = true;
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

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

			#region IAdapterConfigMaxConcurrentCalls Members

			public int MaxConcurrentCalls
			{
				get => AdapterConfig.MaxConcurrentCalls;
				set => AdapterConfig.MaxConcurrentCalls = value;
			}

			#endregion

			#region IAdapterConfigServiceBehavior Members

			public IEnumerable<BehaviorExtensionElement> ServiceBehaviors { get; set; }

			#endregion

			#region IAdapterConfigSSO Members

			/// <summary>
			/// Specify whether to use Enterprise Single Sign-On (SSO) to retrieve client credentials to issue an SSO ticket.
			/// </summary>
			/// <remarks>
			/// It defaults to <c>False</c>.
			/// </remarks>
			public bool UseSSO
			{
				get => AdapterConfig.UseSSO;
				set => AdapterConfig.UseSSO = value;
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				AdapterConfig.ServiceBehaviorConfiguration = ServiceBehaviors.GetServiceBehaviorElementXml();
				base.Save(propertyBag);
			}

			#endregion

			#region Message Tab - Inbound Message Settings

			/// <summary>
			/// Specify whether to add a empty payload message for some incoming HTTP request made for some HTTP verbs.
			/// </summary>
			/// <remarks>
			/// <para>
			/// List of coma separated HTTP Verbs.
			/// </para>
			/// <para>
			/// Message body being added: <code><![CDATA[
			/// <WCF-WebHttpMessageBody xmlns="http://schemas.microsoft.com/BizTalk/2014/01/Adapters/WCF-WebHttp.EmptyMessage">
			/// ]]></code>
			/// </para>
			/// </remarks>
			public string AddMessageBodyForHttpVerbs
			{
				get => AdapterConfig.AddMessageBodyForHttpVerbs;
				set => AdapterConfig.AddMessageBodyForHttpVerbs = value;
			}

			#endregion
		}

		#endregion
	}
}
