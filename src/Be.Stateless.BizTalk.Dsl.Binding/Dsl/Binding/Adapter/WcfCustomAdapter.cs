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

using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class WcfCustomAdapter<TBinding, TConfig> : WcfCustomAdapterBase<EndpointAddress, TBinding, TConfig>
		where TBinding : StandardBindingElement, new()
		where TConfig : AdapterConfig,
		IAdapterConfigAddress,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigIdentity,
		IAdapterConfigBinding,
		Microsoft.BizTalk.Adapter.Wcf.Config.IAdapterConfigEndpointBehavior,
		IAdapterConfigInboundMessageMarshalling,
		IAdapterConfigOutboundMessageMarshalling,
		new()
	{
		static WcfCustomAdapter()
		{
			_protocolType = GetProtocolTypeFromConfigurationClassId(new("af081f69-38ca-4d5b-87df-f0344b12557a"));
		}

		protected WcfCustomAdapter() : base(_protocolType) { }

		[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
		private static readonly ProtocolType _protocolType;
	}
}
