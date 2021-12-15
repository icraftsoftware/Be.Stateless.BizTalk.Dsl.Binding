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
using Microsoft.BizTalk.Adapter.Common.Config;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class Office365EmailAdapter<TConfig> : AdapterBase
		where TConfig : AdapterConfig, new()
	{
		static Office365EmailAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new("48b96e09-bd96-4f46-95ef-57accc55f23d"));
		}

		protected Office365EmailAdapter() : base(_protocolType)
		{
			AdapterConfig = new();
		}

		#region Base Class Member Overrides

		protected override void Save(IPropertyBag propertyBag)
		{
			AdapterConfig.Save(propertyBag as ExplorerOM::Microsoft.BizTalk.ExplorerOM.IPropertyBag);
		}

		#endregion

		protected TConfig AdapterConfig { get; }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}
