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

extern alias ExplorerOM;
using System.Diagnostics.CodeAnalysis;
using ExplorerOM::Microsoft.BizTalk.ExplorerOM;
using ProtocolType = Microsoft.BizTalk.Deployment.Binding.ProtocolType;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions
{
	/// <summary>
	/// Convenient methods to determine an adapter's capabilities as described by its <see cref="Microsoft.BizTalk.Deployment.Binding.ProtocolType.Capabilities"/>
	/// bitmask value.
	/// </summary>
	/// <see href="https://docs.microsoft.com/en-us/biztalk/core/registering-an-adapter#registry-keys">Constraints</see>
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	public static class ProtocolTypeExtensions
	{
		/// <summary>
		/// Indicates that the adapter uses the Adapter Framework user interface for receive handler configuration.
		/// </summary>
		public static bool RequiresContextInitializationForReceiveHandler(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.InitInboundProtocolContext) == Capabilities.InitInboundProtocolContext;
		}

		/// <summary>
		/// Indicates that the adapter uses the Adapter Framework user interface for send handler configuration.
		/// </summary>
		public static bool RequiresContextInitializationForTransmitHandler(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.InitOutboundProtocolContext) == Capabilities.InitOutboundProtocolContext;
		}

		/// <summary>
		/// Indicates that the adapter uses the Adapter Framework user interface for receive location configuration.
		/// </summary>
		public static bool RequiresContextInitializationForReceiveLocation(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.InitReceiveLocationContext) == Capabilities.InitReceiveLocationContext;
		}

		/// <summary>
		/// Indicates that the adapter uses the Adapter Framework user interface for send port configuration.
		/// </summary>
		public static bool RequiresContextInitializationForSendPort(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.InitTransmitLocationContext) == Capabilities.InitTransmitLocationContext;
		}

		/// <summary>
		/// Adapter's receive handler of adapter is hosted in-process..
		/// </summary>
		public static bool RequiresInProcessHostForReceiveHandler(this ProtocolType protocolType)
		{
			return protocolType.SupportsReceive() && ((Capabilities) protocolType.Capabilities & Capabilities.ReceiveIsCreatable) == Capabilities.ReceiveIsCreatable;
		}

		/// <summary>
		/// Adapter's receive handler of adapter requires an isolated host.
		/// </summary>
		public static bool RequiresIsolatedHostForReceiveHandler(this ProtocolType protocolType)
		{
			return protocolType.SupportsReceive() && !protocolType.RequiresInProcessHostForReceiveHandler();
		}

		/// <summary>
		/// Indicates that the adapter supports running only in 32-bit hosts.
		/// </summary>
		public static bool Support32BitOnly(this ProtocolType protocolType)
		{
			return (protocolType.Capabilities & 0x10000) == 0x10000;
		}

		/// <summary>
		/// Indicates that the adapter supports ordered delivery of messages.
		/// </summary>
		public static bool SupportsOrderedDelivery(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.SupportsOrderedDelivery) == Capabilities.SupportsOrderedDelivery;
		}

		/// <summary>
		/// Adapter supports receive operations.
		/// </summary>
		public static bool SupportsReceive(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.SupportsReceive) == Capabilities.SupportsReceive;
		}

		/// <summary>
		/// Adapter supports request-response operations.
		/// </summary>
		public static bool SupportsRequestResponse(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.SupportsRequestResponse) == Capabilities.SupportsRequestResponse;
		}

		/// <summary>
		/// Adapter supports solicit-response operations.
		/// </summary>
		public static bool SupportsSolicitResponse(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.SupportsSolicitResponse) == Capabilities.SupportsSolicitResponse;
		}

		/// <summary>
		/// Adapter supports send operations.
		/// </summary>
		public static bool SupportsTransmit(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.SupportsSend) == Capabilities.SupportsSend;
		}

		/// <summary>
		/// Send adapter starts when the service starts instead of when it sends the first message.
		/// </summary>
		public static bool TransmitAdapterStartsOnServiceStart(this ProtocolType protocolType)
		{
			return ((Capabilities) protocolType.Capabilities & Capabilities.InitTransmitterOnServiceStart) == Capabilities.InitTransmitterOnServiceStart;
		}
	}
}
