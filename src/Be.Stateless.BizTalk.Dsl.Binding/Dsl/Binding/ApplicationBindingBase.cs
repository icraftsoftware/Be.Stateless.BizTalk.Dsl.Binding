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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class ApplicationBindingBase<TNamingConvention>
		: IApplicationBinding<TNamingConvention>,
			IApplicationBindingArtifactLookup,
			ISupportEnvironmentOverride,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		protected internal ApplicationBindingBase()
		{
			_referencedApplications = new ReferencedApplicationBindingCollection();
			_receivePorts = new ReceivePortCollection<TNamingConvention>(this);
			_sendPorts = new SendPortCollection<TNamingConvention>(this);
			_orchestrations = new OrchestrationBindingCollection<TNamingConvention>(this);
			Timestamp = DateTime.Now;
		}

		protected ApplicationBindingBase(Action<IApplicationBinding<TNamingConvention>> applicationBindingConfigurator) : this()
		{
			if (applicationBindingConfigurator == null) throw new ArgumentNullException(nameof(applicationBindingConfigurator));
			applicationBindingConfigurator(this);
			((ISupportValidation) this).Validate();
		}

		#region IApplicationBinding<TNamingConvention> Members

		public string Description { get; set; }

		public TNamingConvention Name { get; set; }

		public IOrchestrationBindingCollection Orchestrations => _orchestrations;

		public IReferencedApplicationBindingCollection ReferencedApplications => _referencedApplications;

		public IReceivePortCollection<TNamingConvention> ReceivePorts => _receivePorts;

		public ISendPortCollection<TNamingConvention> SendPorts => _sendPorts;

		public DateTime Timestamp { get; internal set; }

		#endregion

		#region IApplicationBindingArtifactLookup Members

		IApplicationBindingArtifactLookup IApplicationBindingArtifactLookup.ReferencedApplication<T>()
		{
			return ReferencedApplications.OfType<T>().Single();
		}

		ISupportNamingConvention IApplicationBindingArtifactLookup.ReceiveLocation<T>()
		{
			return ReceivePorts.Select(rp => rp.ReceiveLocations).SelectMany(rl => rl).OfType<T>().Single();
		}

		ISupportNamingConvention IApplicationBindingArtifactLookup.ReceivePort<T>()
		{
			return ReceivePorts.OfType<T>().Single();
		}

		ISupportNamingConvention IApplicationBindingArtifactLookup.SendPort<T>()
		{
			return SendPorts.OfType<T>().Single();
		}

		string ISupportNamingConvention.Name => NamingConventionThunk.ComputeApplicationName(this);

		#endregion

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty()) ApplyEnvironmentOverrides(environment);
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Name == null) throw new BindingException("Application's Name is not defined.");
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			((IVisitable<IApplicationBindingVisitor>) _referencedApplications).Accept(visitor);
			visitor.VisitApplicationBinding(this);
			((IVisitable<IApplicationBindingVisitor>) _receivePorts).Accept(visitor);
			((IVisitable<IApplicationBindingVisitor>) _sendPorts).Accept(visitor);
			((IVisitable<IApplicationBindingVisitor>) _orchestrations).Accept(visitor);
		}

		#endregion

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		private readonly OrchestrationBindingCollection<TNamingConvention> _orchestrations;
		private readonly ReceivePortCollection<TNamingConvention> _receivePorts;
		private readonly ReferencedApplicationBindingCollection _referencedApplications;
		private readonly SendPortCollection<TNamingConvention> _sendPorts;
	}
}
