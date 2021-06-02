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
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class HostResolutionPolicy : IResolveTransportHost
	{
		#region Operators

		public static implicit operator HostResolutionPolicy(string name)
		{
			return new(name);
		}

		#endregion

		protected HostResolutionPolicy() { }

		private HostResolutionPolicy(string name)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));
			_name = name;
		}

		#region IResolveTransportHost Members

		string IResolveTransportHost.ResolveHostName<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport) where TNamingConvention : class
		{
			return ResolveHostName(transport);
		}

		string IResolveTransportHost.ResolveHostName<TNamingConvention>(SendPortTransport<TNamingConvention> transport) where TNamingConvention : class
		{
			return ResolveHostName(transport);
		}

		#endregion

		protected virtual string ResolveHostName<TNamingConvention>(ReceiveLocationTransport<TNamingConvention> transport)
			where TNamingConvention : class
		{
			return _name;
		}

		protected virtual string ResolveHostName<TNamingConvention>(SendPortTransport<TNamingConvention> transport)
			where TNamingConvention : class
		{
			return _name;
		}

		private readonly string _name;
	}
}
