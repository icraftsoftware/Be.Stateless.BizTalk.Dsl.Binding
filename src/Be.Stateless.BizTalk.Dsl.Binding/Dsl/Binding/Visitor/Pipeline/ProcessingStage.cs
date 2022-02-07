#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor.Pipeline
{
	internal class ProcessingStage<TNamingConvention> : VisitorPipelineStage<TNamingConvention>
		where TNamingConvention : class
	{
		internal ProcessingStage(VisitorPipeline<TNamingConvention> visitorPipeline) : base(visitorPipeline) { }

		#region Base Class Member Overrides

		internal override void Accept<T>(T visitor)
		{
			var applicationBinding = VisitorPipeline.ApplicationBinding;
			((IVisitable<IApplicationBindingVisitor>) applicationBinding.ReferencedApplications).Accept(visitor);
			visitor.VisitApplicationBinding(applicationBinding);
			((IVisitable<IApplicationBindingVisitor>) applicationBinding.ReceivePorts).Accept(visitor);
			((IVisitable<IApplicationBindingVisitor>) applicationBinding.SendPorts).Accept(visitor);
			((IVisitable<IApplicationBindingVisitor>) applicationBinding.Orchestrations).Accept(visitor);
		}

		#endregion
	}
}
