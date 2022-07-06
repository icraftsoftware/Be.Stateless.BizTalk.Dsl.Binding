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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class ReceivePortBase<TNamingConvention>
		: IReceivePort<TNamingConvention>,
			ISupportEnvironmentOverride,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		protected internal ReceivePortBase()
		{
			_receiveLocations = new(this);
		}

		protected internal ReceivePortBase(Action<IReceivePort<TNamingConvention>> receivePortConfigurator) : this()
		{
			if (receivePortConfigurator == null) throw new ArgumentNullException(nameof(receivePortConfigurator));
			receivePortConfigurator(this);
		}

		#region IReceivePort<TNamingConvention> Members

		public IApplicationBinding<TNamingConvention> ApplicationBinding { get; internal set; }

		public bool IsTwoWay => ComputeIsTwoWay();

		public string Description { get; set; }

		public TNamingConvention Name { get; set; }

		public IReceiveLocationCollection<TNamingConvention> ReceiveLocations => _receiveLocations;

		public string ResolveName()
		{
			return NamingConventionThunk.ComputeReceivePortName(this);
		}

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
			if (Name == null) throw new BindingException("Receive Port's Name is not defined.");
			if (!_receiveLocations.Any()) throw new BindingException($"[{ResolveName()}] Receive Port's Receive Locations are not defined.");
			ComputeIsTwoWay();
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		TVisitor IVisitable<IApplicationBindingVisitor>.Accept<TVisitor>(TVisitor visitor)
		{
			visitor.VisitReceivePort(this);
			return ((IVisitable<IApplicationBindingVisitor>) _receiveLocations).Accept(visitor);
		}

		#endregion

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		private bool ComputeIsTwoWay()
		{
			var isOneWay = _receiveLocations.All(rl => rl.SendPipeline == null);
			var isTwoWay = _receiveLocations.All(rl => rl.SendPipeline != null);
			if (!isOneWay && !isTwoWay) throw new BindingException($"[{ResolveName()}] Receive Port cannot define both one-way and two-way Receive Locations.");
			return isTwoWay;
		}

		private readonly ReceiveLocationCollection<TNamingConvention> _receiveLocations;
	}
}
