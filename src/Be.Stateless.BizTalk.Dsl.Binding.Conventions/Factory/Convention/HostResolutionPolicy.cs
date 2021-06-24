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

using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;

namespace Be.Stateless.BizTalk.Factory.Convention
{
	public class HostResolutionPolicy : Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy
	{
		public static Be.Stateless.BizTalk.Dsl.Binding.Convention.HostResolutionPolicy Default { get; } = new HostResolutionPolicy();

		#region Base Class Member Overrides

		protected override string ResolveHostName(IOrchestrationBinding orchestration)
		{
			return Platform.Settings.HostNameProvider.ProcessingHost;
		}

		protected override string ResolveHostName<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
		{
			// see also https://docs.microsoft.com/en-us/biztalk/install-and-config-guides/whats-new-in-biztalk-server-2020#deprecated--removed-list
			return transport.Adapter.ProtocolType.RequiresIsolatedHostForReceiveHandler()
				? Platform.Settings.HostNameProvider.IsolatedHost
				: Platform.Settings.HostNameProvider.ReceivingHost;
		}

		protected override string ResolveHostName<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
		{
			return Platform.Settings.HostNameProvider.TransmittingHost;
		}

		#endregion
	}
}
