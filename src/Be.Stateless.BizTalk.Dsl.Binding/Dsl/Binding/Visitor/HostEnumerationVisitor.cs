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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public class HostEnumerationVisitor : MainApplicationBindingVisitor, IEnumerable<string>
	{
		#region IEnumerable<string> Members

		public IEnumerator<string> GetEnumerator()
		{
			return _hosts.Distinct().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Base Class Member Overrides

		protected internal override void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class { }

		protected internal override void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			_hosts.Add(orchestrationBinding.Host);
		}

		protected internal override void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			_hosts.Add(((ISupportHostNameResolution) receiveLocation.Transport).ResolveHostName());
		}

		protected internal override void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class { }

		protected internal override void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			_hosts.Add(((ISupportHostNameResolution) sendPort.Transport).ResolveHostName());
			if (sendPort.BackupTransport.IsValueCreated) _hosts.Add(((ISupportHostNameResolution) sendPort.BackupTransport.Value).ResolveHostName());
		}

		#endregion

		private readonly List<string> _hosts = new();
	}
}
