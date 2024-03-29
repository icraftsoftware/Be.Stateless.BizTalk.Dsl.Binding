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

using System.Collections.Generic;
using System.Linq;
using Be.Stateless.Linq.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	internal class ReceiveLocationCollection<TNamingConvention>
		: List<IReceiveLocation<TNamingConvention>>,
			IReceiveLocationCollection<TNamingConvention>,
			IVisitable<IApplicationBindingVisitor>
		where TNamingConvention : class
	{
		internal ReceiveLocationCollection(IReceivePort<TNamingConvention> receivePort)
		{
			_receivePort = receivePort;
		}

		#region IReceiveLocationCollection<TNamingConvention> Members

		IReceiveLocationCollection<TNamingConvention> IReceiveLocationCollection<TNamingConvention>.Add(IReceiveLocation<TNamingConvention> receiveLocation)
		{
			return ((IReceiveLocationCollection<TNamingConvention>) this).Add(new[] { receiveLocation });
		}

		IReceiveLocationCollection<TNamingConvention> IReceiveLocationCollection<TNamingConvention>.Add(params IReceiveLocation<TNamingConvention>[] receiveLocations)
		{
			receiveLocations.ForEach(
				rl => {
					((ReceiveLocationBase<TNamingConvention>) rl).ReceivePort = _receivePort;
					Add(rl);
				});
			return this;
		}

		public IReceiveLocation<TNamingConvention> Find<T>() where T : IReceiveLocation<TNamingConvention>
		{
			return this.OfType<T>().Single();
		}

		#endregion

		#region IVisitable<IApplicationBindingVisitor> Members

		TVisitor IVisitable<IApplicationBindingVisitor>.Accept<TVisitor>(TVisitor visitor)
		{
			this.Cast<IVisitable<IApplicationBindingVisitor>>().ForEach(rl => rl.Accept(visitor));
			return visitor;
		}

		#endregion

		private readonly IReceivePort<TNamingConvention> _receivePort;
	}
}
