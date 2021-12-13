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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class SendPortCollection<TNamingConvention>
		: List<ISendPort<TNamingConvention>>,
			ISendPortCollection<TNamingConvention>,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		internal SendPortCollection(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			_applicationBinding = applicationBinding;
		}

		#region ISendPortCollection<TNamingConvention> Members

		ISendPortCollection<TNamingConvention> ISendPortCollection<TNamingConvention>.Add(ISendPort<TNamingConvention> sendPort)
		{
			return ((ISendPortCollection<TNamingConvention>) this).Add(new[] { sendPort });
		}

		ISendPortCollection<TNamingConvention> ISendPortCollection<TNamingConvention>.Add(params ISendPort<TNamingConvention>[] sendPorts)
		{
			sendPorts.ForEach(
				sp => {
					((SendPortBase<TNamingConvention>) sp).ApplicationBinding = _applicationBinding;
					Add(sp);
				});
			return this;
		}

		public ISendPort<TNamingConvention> Find<T>() where T : ISendPort<TNamingConvention>
		{
			return this.OfType<T>().Single();
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			this.Cast<ISupportValidation>().ForEach(sp => sp.Validate());
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		void IVisitable<IApplicationBindingVisitor>.Accept(IApplicationBindingVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().ForEach(sp => sp.Accept(visitor));
		}

		#endregion

		private readonly IApplicationBinding<TNamingConvention> _applicationBinding;
	}
}
