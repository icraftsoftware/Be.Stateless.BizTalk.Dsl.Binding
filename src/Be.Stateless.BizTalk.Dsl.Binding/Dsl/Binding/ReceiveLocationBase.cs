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

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Extensions;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class ReceiveLocationBase<TNamingConvention>
		: IReceiveLocation<TNamingConvention>,
			ISupportEnvironmentOverride,
			ISupportNamingConvention,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		protected internal ReceiveLocationBase()
		{
			Transport = new(this);
		}

		protected internal ReceiveLocationBase(Action<IReceiveLocation<TNamingConvention>> receiveLocationConfigurator) : this()
		{
			if (receiveLocationConfigurator == null) throw new ArgumentNullException(nameof(receiveLocationConfigurator));
			receiveLocationConfigurator(this);
			((ISupportValidation) this).Validate();
		}

		#region IReceiveLocation<TNamingConvention> Members

		public bool Enabled { get; set; }

		public string Description { get; set; }

		public TNamingConvention Name { get; set; }

		public ReceivePipeline ReceivePipeline { get; set; }

		public IReceivePort<TNamingConvention> ReceivePort { get; internal set; }

		public SendPipeline SendPipeline { get; set; }

		public ReceiveLocationTransport<TNamingConvention> Transport { get; }

		#endregion

		#region ISupportEnvironmentOverride Members

		[SuppressMessage("ReSharper", "InvertIf")]
		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty())
			{
				ApplyEnvironmentOverrides(environment);
				((ISupportEnvironmentOverride) ReceivePipeline)?.ApplyEnvironmentOverrides(environment);
				((ISupportEnvironmentOverride) SendPipeline)?.ApplyEnvironmentOverrides(environment);
				((ISupportEnvironmentOverride) Transport).ApplyEnvironmentOverrides(environment);
			}
		}

		#endregion

		#region ISupportNamingConvention Members

		string ISupportNamingConvention.Name => NamingConventionThunk.ComputeReceiveLocationName(this);

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Name == null) throw new BindingException("Receive Location's Name is not defined.");
			if (ReceivePipeline == null) throw new BindingException("Receive Location's Receive Pipeline is not defined.");
			Transport.Validate("Receive Location's Transport");
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			visitor.VisitReceiveLocation(this);
		}

		#endregion

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		protected virtual void ApplyEnvironmentOverrides(string environment) { }
	}
}
