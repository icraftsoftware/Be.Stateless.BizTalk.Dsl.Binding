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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed;
using Be.Stateless.BizTalk.Dummies.Bindings.Detailed;

// ReSharper disable CheckNamespace
namespace Be.Stateless.Banking
{
	internal class BankingSampleApplication : ApplicationBinding<NamingConvention<Party, Subject>>
	{
		public BankingSampleApplication()
		{
			ReceivePorts.Add(new Invoice.TaxAgencyReceivePort());
			SendPorts.Add(new Invoice.BankSendPort());
		}
	}

	namespace Invoice
	{
		internal class BankSendPort : BizTalk.Dsl.Binding.Convention.Detailed.BankSendPort { }

		internal class TaxAgencyReceivePort : BizTalk.Dsl.Binding.Convention.Detailed.TaxAgencyReceivePort { }
	}
}
