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

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public interface IApplicationBindingVisitor
	{
		void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding) where TNamingConvention : class;

		void VisitReferencedApplicationBinding(IVisitable<IApplicationBindingVisitor> referencedApplicationBinding);

		void VisitOrchestration(IOrchestrationBinding orchestrationBinding);

		void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort) where TNamingConvention : class;

		void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation) where TNamingConvention : class;

		void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort) where TNamingConvention : class;
	}
}
