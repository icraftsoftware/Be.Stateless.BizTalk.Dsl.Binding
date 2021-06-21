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
	public abstract class LogicAppAdapter<TConfig>
		: AdapterBase,
			IAdapterConfigAddress<Uri>
		//IAdapterConfigOptionalAccessControlService,
		//IAdapterConfigSasCredentials
		where TConfig : AdapterConfig,
		IAdapterConfigAddress,
		//IAdapterConfigTimeouts,
		//IAdapterConfigAcsCredentials,
		//IAdapterConfigSasCredentials,
		new()
	{
		static LogicAppAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new Guid("ec50860c-24cb-4934-be2e-1694093b721f"));
		}

		protected LogicAppAdapter() : base(_protocolType)
		{
			AdapterConfig = new TConfig();
		}

		#region IAdapterConfigAddress<Uri> Members

		public Uri Address { get; set; }

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

		protected TConfig AdapterConfig { get; }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}
