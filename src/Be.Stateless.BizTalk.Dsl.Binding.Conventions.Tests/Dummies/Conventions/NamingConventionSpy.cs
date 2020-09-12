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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;

namespace Be.Stateless.BizTalk.Dummies.Conventions
{
	internal class NamingConventionSpy : NamingConventionBase<NamingConventionSpy, string, string>, INamingConvention<NamingConventionSpy>
	{
		#region INamingConvention<NamingConventionSpy> Members

		string INamingConvention<NamingConventionSpy>.ComputeApplicationName(IApplicationBinding<NamingConventionSpy> application)
		{
			throw new NotSupportedException();
		}

		string INamingConvention<NamingConventionSpy>.ComputeReceivePortName(IReceivePort<NamingConventionSpy> receivePort)
		{
			throw new NotSupportedException();
		}

		string INamingConvention<NamingConventionSpy>.ComputeReceiveLocationName(IReceiveLocation<NamingConventionSpy> receiveLocation)
		{
			throw new NotSupportedException();
		}

		string INamingConvention<NamingConventionSpy>.ComputeSendPortName(ISendPort<NamingConventionSpy> sendPort)
		{
			throw new NotSupportedException();
		}

		#endregion

		public new string ApplicationName
		{
			get => base.ApplicationName;
			set => base.ApplicationName = value;
		}

		public string ComputeReceivePortNameSpy(IReceivePort<NamingConventionSpy> receivePort)
		{
			return ComputeReceivePortName(receivePort);
		}

		public string ComputeReceiveLocationNameSpy(IReceiveLocation<NamingConventionSpy> receiveLocation)
		{
			return ComputeReceiveLocationName(receiveLocation);
		}

		public string ComputeSendPortNameSpy(ISendPort<NamingConventionSpy> sendPort)
		{
			return ComputeSendPortName(sendPort);
		}

		public string ComputeApplicationNameSpy(IApplicationBinding<NamingConventionSpy> application)
		{
			return ComputeApplicationName(application);
		}

		public string ComputeAdapterNameSpy(IAdapter adapter)
		{
			return ComputeAdapterName(adapter);
		}

		public string ComputeAggregateSpy(Type type)
		{
			return ComputeAggregate(type);
		}
	}
}
