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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed
{
	public class NamingConvention<TParty, TSubject> : NamingConventionBase<NamingConvention<TParty, TSubject>, TParty, TSubject>,
		INamingConvention<NamingConvention<TParty, TSubject>>
	{
		#region Operators

		[SuppressMessage("ReSharper", "UnusedParameter.Global")]
		public static implicit operator string(NamingConvention<TParty, TSubject> _)
		{
			throw new NotSupportedException($"In order to support {typeof(Subscription.FilterTranslator).FullName}.");
		}

		#endregion

		#region INamingConvention<NamingConvention<TParty,TSubject>> Members

		string INamingConvention<NamingConvention<TParty, TSubject>>.ComputeApplicationName(IApplicationBinding<NamingConvention<TParty, TSubject>> application)
		{
			return base.ComputeApplicationName(application);
		}

		string INamingConvention<NamingConvention<TParty, TSubject>>.ComputeReceivePortName(IReceivePort<NamingConvention<TParty, TSubject>> receivePort)
		{
			return base.ComputeReceivePortName(receivePort);
		}

		string INamingConvention<NamingConvention<TParty, TSubject>>.ComputeReceiveLocationName(IReceiveLocation<NamingConvention<TParty, TSubject>> receiveLocation)
		{
			return base.ComputeReceiveLocationName(receiveLocation);
		}

		string INamingConvention<NamingConvention<TParty, TSubject>>.ComputeSendPortName(ISendPort<NamingConvention<TParty, TSubject>> sendPort)
		{
			return base.ComputeSendPortName(sendPort);
		}

		#endregion
	}
}
