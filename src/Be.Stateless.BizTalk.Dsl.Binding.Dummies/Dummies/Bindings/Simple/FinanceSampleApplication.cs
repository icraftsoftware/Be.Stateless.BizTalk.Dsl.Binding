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

using System.Xml;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.Finance.Income;
using Microsoft.BizTalk.DefaultPipelines;

// ReSharper disable CheckNamespace
namespace Be.Stateless.Finance
{
	internal class FinanceSampleApplication : ApplicationBinding<NamingConvention>
	{
		public FinanceSampleApplication()
		{
			ReceivePorts.Add(new Invoice.TaxAgencyReceivePort());
			SendPorts.Add(new BankSendPort());
			Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local);
		}
	}

	namespace Invoice
	{
		internal class TaxAgencyReceivePort : ReceivePort<NamingConvention>
		{
			public TaxAgencyReceivePort()
			{
				Name = ReceivePortName.Offwards("Job");
				ReceiveLocations.Add(new TaxAgencyReceiveLocation());
			}
		}

		internal class TaxAgencyReceiveLocation : ReceiveLocation<NamingConvention>
		{
			public TaxAgencyReceiveLocation()
			{
				Name = ReceiveLocationName.About("AddPart").FormattedAs.Xml;
				ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
				Transport.Host = "Host";
			}
		}
	}

	namespace Income
	{
		internal class BankSendPort : SendPort<NamingConvention>
		{
			public BankSendPort()
			{
				Name = SendPortName.Towards("Job").About("Notification").FormattedAs.Xml;
				SendPipeline = new SendPipeline<PassThruTransmit>();
				Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\drops"; });
				Transport.Host = "Host";
			}
		}

		internal class BankReceiveLocation : ReceiveLocation<NamingConvention>
		{
			public BankReceiveLocation()
			{
				Name = ReceiveLocationName.About("AddPart").FormattedAs.Xml;
				ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
				Transport.Host = "Host";
			}
		}
	}
}
