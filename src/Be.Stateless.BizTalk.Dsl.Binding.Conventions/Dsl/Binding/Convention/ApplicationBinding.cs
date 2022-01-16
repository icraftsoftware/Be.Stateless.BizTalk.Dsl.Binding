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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class ApplicationBinding<TNamingConvention> : ApplicationBindingBase<TNamingConvention>
		where TNamingConvention : class, INamingConvention<TNamingConvention>, new()
	{
		protected ApplicationBinding()
		{
			Name = new();
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Convention Public API.")]
		protected internal ApplicationBinding(Action<IApplicationBinding<TNamingConvention>> applicationBindingConfigurator) : base(applicationBindingConfigurator)
		{
			// do not override NamingConvention set by applicationBindingConfigurator
			Name ??= new();
		}

		protected ApplicationNamingConvention<TNamingConvention> ApplicationName => new();

		protected ReceiveLocationNamingConvention<TNamingConvention> ReceiveLocationName => new();

		protected ReceivePortNamingConvention<TNamingConvention> ReceivePortName => new();

		protected SendPortNamingConvention<TNamingConvention> SendPortName => new();

		protected IReceiveLocation<TNamingConvention> ReceiveLocation(Action<IReceiveLocation<TNamingConvention>> receiveLocationConfigurator)
		{
			var receiveLocation = new ReceiveLocation<TNamingConvention>(receiveLocationConfigurator);
			return receiveLocation;
		}

		protected IReceivePort<TNamingConvention> ReceivePort(Action<IReceivePort<TNamingConvention>> receivePortConfigurator)
		{
			var receivePort = new ReceivePort<TNamingConvention>(receivePortConfigurator);
			return receivePort;
		}

		protected ISendPort<TNamingConvention> SendPort(Action<ISendPort<TNamingConvention>> sendPortConfigurator)
		{
			var sendPort = new SendPort<TNamingConvention>(sendPortConfigurator);
			return sendPort;
		}
	}
}
