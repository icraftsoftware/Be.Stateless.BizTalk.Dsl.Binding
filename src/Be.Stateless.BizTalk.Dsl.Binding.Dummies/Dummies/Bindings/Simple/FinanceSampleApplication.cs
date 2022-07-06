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

using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Xml;
using Be.Stateless.BizTalk.Adapter.Metadata;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Simple;
using Be.Stateless.BizTalk.Dummies.Conventions;
using Be.Stateless.BizTalk.Install;
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
			SendPorts.Add(new BankSendPort(), new IrsSendPort(), new SomePartySendPort());
			Timestamp = XmlConvert.ToDateTime("2015-02-17T22:51:04+01:00", XmlDateTimeSerializationMode.Local);
		}
	}

	namespace Invoice
	{
		internal class TaxAgencyReceivePort : ReceivePort<NamingConvention>
		{
			public TaxAgencyReceivePort()
			{
				Name = ReceivePortName.Offwards("TaxAgency");
				ReceiveLocations.Add(new TaxAgencyReceiveLocation());
			}
		}

		internal class TaxAgencyReceiveLocation : ReceiveLocation<NamingConvention>
		{
			public TaxAgencyReceiveLocation()
			{
				Name = ReceiveLocationName.About("Incomes").FormattedAs.Xml;
				ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
				Transport.Host = DummyHostResolutionPolicy.Default;
			}
		}
	}

	namespace Income
	{
		internal class BankSendPort : SendPort<NamingConvention>
		{
			public BankSendPort()
			{
				Name = SendPortName.Towards("Bank").About("Anything").FormattedAs.Xml;
				SendPipeline = new SendPipeline<PassThruTransmit>();
				Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\drops"; });
				Transport.Host = DummyHostResolutionPolicy.Default;
			}
		}

		internal class BankReceiveLocation : ReceiveLocation<NamingConvention>
		{
			public BankReceiveLocation()
			{
				Name = ReceiveLocationName.About("AddPart").FormattedAs.Xml;
				ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
				Transport.Host = DummyHostResolutionPolicy.Default;
			}
		}

		internal class IrsSendPort : SendPort<NamingConvention>
		{
			public IrsSendPort()
			{
				Name = SendPortName.Towards("Irs").About("Anything").FormattedAs.Irrelevant;
				SendPipeline = new SendPipeline<PassThruTransmit>();
				ReceivePipeline = new ReceivePipeline<PassThruReceive>();
				var adapter = new WcfCustomAdapter.Outbound<BasicHttpBindingElement>(
					a => {
						a.Address = new EndpointAddress("http://localhost:8000/soap-stub");
						a.Binding.MaxReceivedMessageSize = 10 * 1024 * 1024;
					});
				Transport.Adapter = ApplyAdapterCommonalities(adapter);
				Transport.Host = DummyHostResolutionPolicy.Default;
			}

			#region Base Class Member Overrides

			[SuppressMessage("ReSharper", "InvertIf")]
			protected override void ApplyEnvironmentOverrides(string environment)
			{
				if (environment.IsAcceptanceUpwards())
				{
					var adapter = new WcfCustomAdapter.Outbound<NetTcpBindingElement>(
						a => {
							a.Address = new EndpointAddress("net.tcp://host/api/services/");
							a.Identity = new IdentityElement { ServicePrincipalName = { Value = "spn" } };
							a.Binding.MaxReceivedMessageSize = 10 * 1024 * 1024;
							a.Binding.Security.Mode = SecurityMode.Transport;
							a.Binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
							a.Binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
						});
					Transport.Adapter = ApplyAdapterCommonalities(adapter);
				}
			}

			#endregion

			//private IOutboundAdapter ApplyAdapterCommonalities<TAdapter>(TAdapter adapter)
			//	where TAdapter : IOutboundAdapter, IAdapterConfigEndpointBehavior, IAdapterConfigOutboundAction
			private IOutboundAdapter ApplyAdapterCommonalities<TBinding>(WcfCustomAdapter.Outbound<TBinding> adapter)
				where TBinding : StandardBindingElement, new()
			{
				adapter.EndpointBehaviors = new BehaviorExtensionElement[] {
					new CallbackDebugElement()
				};
				adapter.StaticAction = new ActionMapping {
					new("operation", "action")
				};
				return adapter;
			}
		}

		internal class SomePartySendPort : SendPort<NamingConvention>
		{
			public SomePartySendPort()
			{
				Name = SendPortName.Towards("SomeParty").About("Anything").FormattedAs.Xml;
				SendPipeline = new SendPipeline<PassThruTransmit>();
				var adapter = new WcfBasicHttpAdapter.Outbound(a => { a.Address = new EndpointAddress("http://localhost:8000/soap-stub"); });
				Transport.Adapter = ApplyAdapterCommonalities(adapter);
				Transport.Host = DummyHostResolutionPolicy.Default;
			}

			#region Base Class Member Overrides

			[SuppressMessage("ReSharper", "InvertIf")]
			protected override void ApplyEnvironmentOverrides(string environment)
			{
				if (environment.IsAcceptanceUpwards())
				{
					var adapter = new WcfNetTcpAdapter.Outbound(
						a => {
							a.Address = new EndpointAddress("net.tcp://host/api/services/");
							a.Identity = new IdentityElement { ServicePrincipalName = { Value = "spn" } };
							a.SecurityMode = SecurityMode.Transport;
							a.TransportClientCredentialType = TcpClientCredentialType.Windows;
							a.TransportProtectionLevel = ProtectionLevel.EncryptAndSign;
						});
					Transport.Adapter = ApplyAdapterCommonalities(adapter);
				}
			}

			#endregion

			private IOutboundAdapter ApplyAdapterCommonalities<TAdapter>(TAdapter adapter)
				where TAdapter : IOutboundAdapter, IAdapterConfigMaxReceivedMessageSize, IAdapterConfigOutboundAction
			{
				adapter.MaxReceivedMessageSize = 10 * 1024 * 1024;
				adapter.StaticAction = new ActionMapping {
					new("operation", "action")
				};
				return adapter;
			}
		}
	}
}
