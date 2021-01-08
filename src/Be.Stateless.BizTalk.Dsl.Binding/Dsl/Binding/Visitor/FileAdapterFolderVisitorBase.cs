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

using Be.Stateless.BizTalk.Dsl.Binding.Adapter;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	public abstract class FileAdapterFolderVisitorBase : MainApplicationBindingVisitor
	{
		#region Base Class Member Overrides

		protected internal override void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class { }

		protected internal override void VisitOrchestration(IOrchestrationBinding orchestrationBinding) { }

		protected internal override void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			if (receiveLocation.Transport.Adapter is FileAdapter.Inbound fileAdapter) VisitDirectory(fileAdapter.ReceiveFolder);
		}

		protected internal override void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class { }

		protected internal override void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			if (sendPort.Transport.Adapter is FileAdapter.Outbound fileAdapter) VisitDirectory(fileAdapter.DestinationFolder);
		}

		#endregion

		protected abstract void VisitDirectory(string path);
	}
}
