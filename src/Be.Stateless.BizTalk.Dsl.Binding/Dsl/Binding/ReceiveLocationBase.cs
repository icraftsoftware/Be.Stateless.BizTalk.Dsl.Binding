﻿#region Copyright & License

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
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Extensions;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class ReceiveLocationBase<TNamingConvention>
		: IReceiveLocation<TNamingConvention>,
			ISupportEnvironmentOverride,
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
		}

		#region IReceiveLocation<TNamingConvention> Members

		public bool Enabled { get; set; }

		public string Description { get; set; }

		public TNamingConvention Name { get; set; }

		public ReceivePipeline ReceivePipeline { get; set; }

		public IReceivePort<TNamingConvention> ReceivePort { get; internal set; }

		public SendPipeline SendPipeline { get; set; }

		public ReceiveLocationTransport<TNamingConvention> Transport { get; }

		public string ResolveName()
		{
			return NamingConventionThunk.ComputeReceiveLocationName(this);
		}

		#endregion

		#region ISupportEnvironmentOverride Members

		[SuppressMessage("ReSharper", "InvertIf")]
		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty())
			{
				ApplyEnvironmentOverrides(environment);
				((ISupportEnvironmentOverride) Transport).ApplyEnvironmentOverrides(environment);
			}
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Name == null) throw new BindingException("Receive Location's Name is not defined.");
			if (ReceivePipeline == null) throw new BindingException($"[{ResolveName()}] Receive Location's Receive Pipeline is not defined.");
			Transport.Validate($"[{ResolveName()}] Receive Location's Transport");
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		TVisitor IVisitable<IApplicationBindingVisitor>.Accept<TVisitor>(TVisitor visitor)
		{
			visitor.VisitReceiveLocation(this);
			return visitor;
		}

		#endregion

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		protected virtual void ApplyEnvironmentOverrides(string environment) { }
	}
}
