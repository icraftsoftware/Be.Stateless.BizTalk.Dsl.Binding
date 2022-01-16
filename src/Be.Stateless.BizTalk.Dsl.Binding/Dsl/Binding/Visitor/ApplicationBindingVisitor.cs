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

using System;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> base class that ensures that only the BizTalk artifacts belonging to the
	/// top-level application are being visited by the <see cref="ApplicationBindingVisitor"/>-derived visitors, skipping
	/// therefore any referenced applications. Note however that this class ensures that before an <see
	/// cref="ApplicationBindingVisitor"/>-derived visitor proceeds, the environment settings overrides have been applied for
	/// the top-level application as well as any of its referenced ones.
	/// </summary>
	public abstract class ApplicationBindingVisitor : IApplicationBindingVisitor
	{
		#region IApplicationBindingVisitor Members

		void IApplicationBindingVisitor.VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			_mainApplicationBinding = applicationBinding ?? throw new ArgumentNullException(nameof(applicationBinding));
			// ensure application bindings are settled for target environment before visit
			((IVisitable<IApplicationBindingVisitor>) applicationBinding).Accept(new EnvironmentOverrideApplicator());
			VisitApplicationBinding(applicationBinding);
		}

		void IApplicationBindingVisitor.VisitReferencedApplicationBinding(IVisitable<IApplicationBindingVisitor> referencedApplicationBinding)
		{
			// skip ReferencedApplicationBinding
		}

		void IApplicationBindingVisitor.VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			if (orchestrationBinding == null) throw new ArgumentNullException(nameof(orchestrationBinding));
			// visit only Orchestration belonging to this application
			if (ReferenceEquals(orchestrationBinding.ApplicationBinding, _mainApplicationBinding)) VisitOrchestration(orchestrationBinding);
		}

		void IApplicationBindingVisitor.VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			if (receivePort == null) throw new ArgumentNullException(nameof(receivePort));
			// visit only ReceivePort belonging to this application
			if (ReferenceEquals(receivePort.ApplicationBinding, _mainApplicationBinding)) VisitReceivePort(receivePort);
		}

		void IApplicationBindingVisitor.VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			if (receiveLocation == null) throw new ArgumentNullException(nameof(receiveLocation));
			// visit only ReceiveLocation belonging to this application
			if (ReferenceEquals(receiveLocation.ReceivePort.ApplicationBinding, _mainApplicationBinding)) VisitReceiveLocation(receiveLocation);
		}

		void IApplicationBindingVisitor.VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			if (sendPort == null) throw new ArgumentNullException(nameof(sendPort));
			// visit only SendPort belonging to this application
			if (ReferenceEquals(sendPort.ApplicationBinding, _mainApplicationBinding)) VisitSendPort(sendPort);
		}

		#endregion

		protected internal abstract void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding) where TNamingConvention : class;

		protected internal abstract void VisitOrchestration(IOrchestrationBinding orchestrationBinding);

		protected internal abstract void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort) where TNamingConvention : class;

		protected internal abstract void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation) where TNamingConvention : class;

		protected internal abstract void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort) where TNamingConvention : class;

		private IApplicationBinding _mainApplicationBinding;
	}
}
