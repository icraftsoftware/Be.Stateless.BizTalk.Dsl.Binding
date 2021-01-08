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
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
	public abstract class SBMessagingAdapter<TConfig>
		: AdapterBase,
			IAdapterConfigAddress<Uri>,
			IAdapterConfigOptionalAccessControlService,
			IAdapterConfigSasCredentials
		where TConfig : AdapterConfig,
		IAdapterConfigAddress,
		IAdapterConfigTimeouts,
		IAdapterConfigAcsCredentials,
		IAdapterConfigSasCredentials,
		new()
	{
		static SBMessagingAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("9c458d4a-a73c-4cb3-89c4-86ae0103de2f"));
		}

		protected SBMessagingAdapter() : base(_protocolType)
		{
			AdapterConfig = new TConfig();
		}

		#region IAdapterConfigAddress<Uri> Members

		/// <summary>
		/// Specify the URL where the Service Bus queue is deployed.
		/// </summary>
		/// <remarks>
		/// The URL is generally formatted as follows: <![CDATA[sb://<namespace>.servicebus.windows.net/<queue_name>/]]>.
		/// </remarks>
		public Uri Address { get; set; }

		#endregion

		#region IAdapterConfigOptionalAccessControlService Members

		public bool UseAcsAuthentication
		{
			get => AdapterConfig.UseAcsAuthentication;
			set => AdapterConfig.UseAcsAuthentication = value;
		}

		public Uri StsUri
		{
			get => new Uri(AdapterConfig.StsUri);
			set => AdapterConfig.StsUri = value?.ToString();
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

		#region IAdapterConfigSasCredentials Members

		/// <summary>
		/// Specify the SAS key value.
		/// </summary>
		public string SharedAccessKey
		{
			get => AdapterConfig.SharedAccessKey;
			set => AdapterConfig.SharedAccessKey = value;
		}

		/// <summary>
		/// Specify the SAS key name.
		/// </summary>
		public string SharedAccessKeyName
		{
			get => AdapterConfig.SharedAccessKeyName;
			set => AdapterConfig.SharedAccessKeyName = value;
		}

		/// <summary>
		/// Whether to use Shared Access Signature for authentication.
		/// </summary>
		public bool UseSasAuthentication
		{
			get => AdapterConfig.UseSasAuthentication;
			set => AdapterConfig.UseSasAuthentication = value;
		}

		#endregion

		#region Base Class Member Overrides

		protected override string GetAddress()
		{
			return Address?.ToString();
		}

		protected override void Save(IPropertyBag propertyBag)
		{
			AdapterConfig.Save(propertyBag as ExplorerOM::Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		protected override void Validate()
		{
			AdapterConfig.Address = GetAddress();
			AdapterConfig.Validate();
			AdapterConfig.Address = null;
		}

		#endregion

		/// <summary>
		/// Specifies a timespan value that indicates the time for a channel close operation to complete.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>1</c> minute.
		/// </remarks>
		public TimeSpan CloseTimeout
		{
			get => AdapterConfig.CloseTimeout;
			set => AdapterConfig.CloseTimeout = value;
		}

		/// <summary>
		/// Specifies a timespan value that indicates the time for a channel open operation to complete.
		/// </summary>
		/// <remarks>
		/// It defaults to <c>1</c> minute.
		/// </remarks>
		public TimeSpan OpenTimeout
		{
			get => AdapterConfig.OpenTimeout;
			set => AdapterConfig.OpenTimeout = value;
		}

		protected TConfig AdapterConfig { get; }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}
