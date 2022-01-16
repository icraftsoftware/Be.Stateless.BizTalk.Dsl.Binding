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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class ReceivePortCollection<TNamingConvention>
		: List<IReceivePort<TNamingConvention>>,
			IReceivePortCollection<TNamingConvention>,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		internal ReceivePortCollection(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			_applicationBinding = applicationBinding;
		}

		#region IReceivePortCollection<TNamingConvention> Members

		IReceivePortCollection<TNamingConvention> IReceivePortCollection<TNamingConvention>.Add(IReceivePort<TNamingConvention> receivePort)
		{
			return ((IReceivePortCollection<TNamingConvention>) this).Add(new[] { receivePort });
		}

		IReceivePortCollection<TNamingConvention> IReceivePortCollection<TNamingConvention>.Add(params IReceivePort<TNamingConvention>[] receivePorts)
		{
			receivePorts.ForEach(
				rp => {
					((ReceivePortBase<TNamingConvention>) rp).ApplicationBinding = _applicationBinding;
					Add(rp);
				});
			return this;
		}

		public IReceivePort<TNamingConvention> Find<T>() where T : IReceivePort<TNamingConvention>
		{
			return this.OfType<T>().Single();
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			this.Cast<ISupportValidation>().ForEach(rp => rp.Validate());
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		TVisitor IVisitable<IApplicationBindingVisitor>.Accept<TVisitor>(TVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().ForEach(rp => rp.Accept(visitor));
			return visitor;
		}

		#endregion

		private readonly IApplicationBinding<TNamingConvention> _applicationBinding;
	}
}
