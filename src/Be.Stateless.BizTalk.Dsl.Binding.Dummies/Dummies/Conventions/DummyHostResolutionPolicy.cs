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

using System.Linq;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.Dummies.MicroComponent;

namespace Be.Stateless.BizTalk.Dummies.Conventions
{
	internal class DummyHostResolutionPolicy : HostResolutionPolicy
	{
		private DummyHostResolutionPolicy() { }

		#region Base Class Member Overrides

		protected override string ResolveHostName<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
		{
			// determine host according to port's associated party
			return transport.ReceiveLocation.ReceivePort.Name is NamingConvention { Party: "TaxAgency" }
				? "TaxAgency_Host"
				// determine host type according to transport's adapter
				: transport.Adapter is WcfBasicHttpAdapter.Inbound
					? "BizTalkServerIsolatedHost"
					: "BizTalkServerApplication";
		}

		protected override string ResolveHostName<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
		{
			// determine host according to port's associated message
			return transport.SendPort.Name is NamingConvention { Subject: "Anything" }
				? "Anything_Host"
				// determine host bitness according to some pipeline component
				: transport.SendPort.SendPipeline.Encoder<MicroPipelineComponent>().Components.OfType<MimeDecoder>().Any()
					? "BizTalkServerApplication_x86"
					: "BizTalkServerApplication";
		}

		#endregion

		// !! Watch Out !! should the policy be stateful then Default should be written as follows, so as to return a new stateful instance with each invocation
		//public static HostResolutionPolicy Default => new DummyHostResolutionPolicy();
		public static readonly HostResolutionPolicy Default = new DummyHostResolutionPolicy();
	}
}
