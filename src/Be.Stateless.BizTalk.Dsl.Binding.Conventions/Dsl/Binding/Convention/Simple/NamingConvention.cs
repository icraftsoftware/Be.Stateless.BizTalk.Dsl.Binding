#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple
{
	public class NamingConvention : NamingConventionBase<NamingConvention, string, string>, INamingConvention<NamingConvention>
	{
		#region Operators

		[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates")]
		[SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations")]
		public static implicit operator string(NamingConvention _)
		{
			throw new NotSupportedException($"In order to support {typeof(Subscription.FilterTranslator).FullName}.");
		}

		#endregion

		#region INamingConvention<NamingConvention> Members

		string INamingConvention<NamingConvention>.ComputeApplicationName(IApplicationBinding<NamingConvention> application)
		{
			return base.ComputeApplicationName(application);
		}

		string INamingConvention<NamingConvention>.ComputeReceivePortName(IReceivePort<NamingConvention> receivePort)
		{
			return base.ComputeReceivePortName(receivePort);
		}

		string INamingConvention<NamingConvention>.ComputeReceiveLocationName(IReceiveLocation<NamingConvention> receiveLocation)
		{
			return base.ComputeReceiveLocationName(receiveLocation);
		}

		string INamingConvention<NamingConvention>.ComputeSendPortName(ISendPort<NamingConvention> sendPort)
		{
			return base.ComputeSendPortName(sendPort);
		}

		#endregion
	}
}
