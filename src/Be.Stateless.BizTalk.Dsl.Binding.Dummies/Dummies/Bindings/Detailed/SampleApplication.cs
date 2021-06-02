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
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.Dsl.Binding.Subscription;
using Be.Stateless.BizTalk.Dummies.Bindings.Detailed;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Schema.Annotation;
using Microsoft.Adapters.Sql;
using Microsoft.XLANGs.BaseTypes;
using Party = Be.Stateless.BizTalk.Dummies.Bindings.Detailed.Party;

// ReSharper disable CheckNamespace
namespace Be.Stateless.BizTalk.Dsl.Binding.Convention.Detailed
{
	internal class SampleApplication : ApplicationBinding<NamingConvention<Party, Subject>>
	{
		public SampleApplication()
		{
			Name = ApplicationName.Is("Detailed.SampleApplication");
			ReceivePorts.Add(
				CustomerOneWayReceivePort = ReceivePort(
					p => {
						p.Name = ReceivePortName.Offwards(Party.Customer);
						p.ReceiveLocations
							.Add(
								ReceiveLocation(
									l => {
										l.Name = ReceiveLocationName.About(Subject.Invoice).FormattedAs.Xml;
										l.Enabled = false;
										l.ReceivePipeline = new ReceivePipeline<XmlReceive>();
										l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
										l.Transport.Host = Host.RECEIVING_HOST;
									}),
								ReceiveLocation(
									l => {
										l.Name = ReceiveLocationName.About(Subject.CreditNote).FormattedAs.Edi;
										l.Enabled = false;
										l.ReceivePipeline = new ReceivePipeline<XmlReceive>();
										l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
										l.Transport.Host = Host.RECEIVING_HOST;
									})
							);
					}),
				CustomerTwoWayReceivePort = ReceivePort(
					p => {
						p.Name = ReceivePortName.Offwards(Party.Customer);
						p.Description = "Receives ledgers from customers";
						p.ReceiveLocations.Add(
							ReceiveLocation(
								l => {
									l.Name = ReceiveLocationName.About(Subject.Statement).FormattedAs.Csv;
									l.Enabled = true;
									l.ReceivePipeline = new ReceivePipeline<PassThruReceive>(pl => { pl.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
									l.SendPipeline = new SendPipeline<PassThruTransmit>(pl => { pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
									l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
									l.Transport.Host = Host.RECEIVING_HOST;
								}));
					}),
				ReceivePort(
					p => {
						p.Name = ReceivePortName.Offwards(Party.Bank);
						p.Description = "Receives financial movements from bank";
						p.ReceiveLocations.Add(
							ReceiveLocation(
								l => {
									l.Name = ReceiveLocationName.About(Subject.Statement).FormattedAs.Xml;
									l.Enabled = true;
									l.ReceivePipeline = new ReceivePipeline<XmlReceive>(
										pl => {
											pl.Decoder<MicroPipelineComponent>(
												c => {
													c.Enabled = false;
													c.Components = new IMicroComponent[] {
														new FailedMessageRoutingEnabler { EnableFailedMessageRouting = true, SuppressRoutingFailureReport = false },
														new ContextPropertyExtractor {
															Extractors = new PropertyExtractorCollection(
																new ConstantExtractor(BizTalkFactoryProperties.OutboundTransportLocation, TargetEnvironment.ACCEPTANCE))
														}
													};
												});
										});
									l.Transport.Adapter = new WcfSqlAdapter.Inbound(
										a => {
											a.Address = new SqlAdapterConnectionUri { InboundId = "FinancialMovements", InitialCatalog = "BankDb", Server = "localhost" };
											a.InboundOperationType = InboundOperation.XmlPolling;
											a.PolledDataAvailableStatement = "select count(1) from data";
											a.PollingStatement = "select * from data for XML";
											a.PollingInterval = TimeSpan.FromHours(2);
											a.PollWhileDataFound = true;
										});
									l.Transport.Host = Host.RECEIVING_HOST;
								}));
					}),
				TaxAgencyOneWayReceivePort = new TaxAgencyReceivePort()
			);
			SendPorts.Add(
				BankOneWaySendPort = new BankSendPort(),
				CustomerTwoWaySendPort = SendPort(
					p => {
						p.Name = SendPortName.Towards(Party.Customer).About(Subject.Statement).FormattedAs.Csv;
						p.SendPipeline = new SendPipeline<PassThruTransmit>(pl => { pl.PreAssembler<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
						p.ReceivePipeline = new ReceivePipeline<PassThruReceive>(pl => { pl.Decoder<FailedMessageRoutingEnablerComponent>(c => { c.Enabled = false; }); });
						p.Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\drops"; });
						p.Transport.RetryPolicy = RetryPolicy.LongRunning;
						p.Transport.Host = Host.SENDING_HOST;
					})
			);
			// TODO Orchestrations.Add(
			//	new ProcessOrchestrationBinding(
			//		o => {
			//			o.ReceivePort = CustomerOneWayReceivePort;
			//			o.RequestResponsePort = CustomerTwoWayReceivePort;
			//			o.SendPort = BankOneWaySendPort;
			//			o.SolicitResponsePort = CustomerTwoWaySendPort;
			//			o.Host = Host.PROCESSING_HOST;
			//		}));
		}

		internal IReceivePort<NamingConvention<Party, Subject>> CustomerOneWayReceivePort { get; }

		internal ISendPort<NamingConvention<Party, Subject>> CustomerTwoWaySendPort { get; }

		internal TaxAgencyReceivePort TaxAgencyOneWayReceivePort { get; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private BankSendPort BankOneWaySendPort { get; }

		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private IReceivePort<NamingConvention<Party, Subject>> CustomerTwoWayReceivePort { get; }
	}

	internal class BankSendPort : SendPort<NamingConvention<Party, Subject>>
	{
		public BankSendPort()
		{
			Name = SendPortName.Towards(Party.Bank).About(Subject.CreditNote).FormattedAs.Edi;
			SendPipeline = new SendPipeline<XmlTransmit>();
			Transport.Adapter = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\drops"; });
			Transport.Host = Host.SENDING_HOST;
			Transport.RetryPolicy = RetryPolicy.LongRunning;
			Filter = new Filter(() => BtsProperties.MessageType == Schema<Any>.MessageType);
		}
	}

	internal class TaxAgencyReceivePort : ReceivePort<NamingConvention<Party, Subject>>
	{
		public TaxAgencyReceivePort()
		{
			Name = ReceivePortName.Offwards(Party.TaxAgency);
			ReceiveLocations.Add(
				ReceiveLocation(
					l => {
						l.Name = ReceiveLocationName.About(Subject.Statement).FormattedAs.Xml;
						l.Enabled = false;
						l.ReceivePipeline = new ReceivePipeline<PassThruReceive>(
							pl => {
								pl.Decoder<MicroPipelineComponent>(
									c => {
										c.Components = new IMicroComponent[] {
											new ContextBuilder {
												BuilderType = typeof(IContextBuilder),
												ExecutionTime = PluginExecutionTime.Deferred
											}
										};
									});
							});
						l.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\drops"; });
						l.Transport.Host = Host.ISOLATED_HOST;
						l.Transport.Schedule = new Schedule {
							StartDate = new DateTime(2015, 2, 17),
							StopDate = new DateTime(2015, 2, 17).AddDays(12),
							ServiceWindow = new DailyServiceWindow {
								Interval = 10,
								From = new DateTime(2020, 1, 30),
								StartTime = new Time(13, 15),
								StopTime = new Time(14, 15)
							}
						};
					})
			);
		}
	}
}
