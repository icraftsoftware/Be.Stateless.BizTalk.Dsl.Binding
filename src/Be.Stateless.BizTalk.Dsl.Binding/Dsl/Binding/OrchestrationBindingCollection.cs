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
	internal class OrchestrationBindingCollection<TNamingConvention>
		: List<IOrchestrationBinding>,
			IOrchestrationBindingCollection,
			ISupportValidation,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		public OrchestrationBindingCollection(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			_applicationBinding = applicationBinding;
		}

		#region IOrchestrationBindingCollection Members

		IOrchestrationBindingCollection IOrchestrationBindingCollection.Add(IOrchestrationBinding orchestrationBinding)
		{
			return ((IOrchestrationBindingCollection) this).Add(new[] { orchestrationBinding });
		}

		IOrchestrationBindingCollection IOrchestrationBindingCollection.Add(params IOrchestrationBinding[] orchestrationBindings)
		{
			orchestrationBindings.ForEach(
				ob => {
					ob.ApplicationBinding = _applicationBinding;
					Add(ob);
				});
			return this;
		}

		public T Find<T>() where T : IOrchestrationBinding
		{
			return (T) this.Single(ob => ob.GetType() == typeof(T));
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			this.Cast<ISupportValidation>().ForEach(ob => ob.Validate());
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		TVisitor IVisitable<IApplicationBindingVisitor>.Accept<TVisitor>(TVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().ForEach(ob => ob.Accept(visitor));
			return visitor;
		}

		#endregion

		private readonly IApplicationBinding<TNamingConvention> _applicationBinding;
	}
}
