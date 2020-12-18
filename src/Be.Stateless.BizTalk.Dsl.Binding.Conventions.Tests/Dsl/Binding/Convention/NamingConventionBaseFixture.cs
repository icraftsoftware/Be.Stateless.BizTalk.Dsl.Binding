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
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dummies.Adapter;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Dummies.Bindings.Simple;
using Be.Stateless.BizTalk.Dummies.Conventions;
using Be.Stateless.Finance;
using Be.Stateless.Finance.Income;
using Be.Stateless.Finance.Invoice;
using FluentAssertions;
using Microsoft.Adapters.OracleDB;
using Microsoft.Adapters.SAP;
using Microsoft.Adapters.Sql;
using Microsoft.BizTalk.Adapter.Wcf.Config;
using Moq;
using Moq.Protected;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;
using CustomBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.CustomBindingElement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class NamingConventionBaseFixture
	{
		[Fact]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomAdapter()
		{
			var sut = new NamingConventionSpy();

			IAdapter adapter = new CustomAdapterDummy<NetTcpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomNetTcp");

			adapter = new CustomAdapterDummy<NetMsmqBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomNetMsmq");

			adapter = new CustomAdapterDummy<Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.NetMsmqBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomNetMsmq");

			adapter = new CustomAdapterDummy<OracleDBBindingConfigurationElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomOracleDB");

			adapter = new CustomAdapterDummy<SqlAdapterBindingConfigurationElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomSql");

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new MtomMessageEncodingElement(), new HttpsTransportElement()));
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomHttps");

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new TcpTransportElement()));
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomTcp");

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new SAPAdapterExtensionElement()));
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomSap");

			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new SqlAdapterBindingElementExtensionElement()));
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomSql");

			// notice that OracleDBAdapterExtensionElement is internal :(
			//adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(new OracleDBAdapterExtensionElement()));
			var type = typeof(Microsoft.Adapters.OracleDB.InboundOperation).Assembly.GetType("Microsoft.Adapters.OracleDB.OracleDBAdapterExtensionElement", true);
			var bindingElement = (BindingElementExtensionElement) Activator.CreateInstance(type);
			adapter = new WcfCustomAdapter.Outbound<CustomBindingElement>(a => a.Binding.Add(bindingElement));
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomOracleDB");
		}

		[Fact]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomIsolatedAdapter()
		{
			var sut = new NamingConventionSpy();

			IAdapter adapter = new CustomIsolatedAdapterDummy<NetTcpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomIsolatedNetTcp");

			adapter = new CustomIsolatedAdapterDummy<WSHttpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomIsolatedWsHttp");

			adapter = new CustomIsolatedAdapterDummy<BasicHttpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomIsolatedBasicHttp");
		}

		[Fact]
		public void ComputeAggregateNameIsCalledForReceiveLocation()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "SomeParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var namingConventionMock = new Mock<NamingConventionSpy>();
			namingConventionMock.Object.MessageName = "SomeMessage";
			namingConventionMock.Object.MessageFormat = "SomeFormat";

			namingConventionMock.Object.ComputeReceiveLocationNameSpy(receiveLocationMock.Object);

			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Once(), receivePortMock.Object.GetType());
			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Once(), receiveLocationMock.Object.GetType());
		}

		[Fact]
		public void ComputeAggregateNameIsCalledForReceivePort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var namingConventionMock = new Mock<NamingConventionSpy>();
			namingConventionMock.Object.Party = "SomeParty";

			namingConventionMock.Object.ComputeReceivePortNameSpy(receivePortMock.Object);

			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Once(), receivePortMock.Object.GetType());
		}

		[Fact]
		public void ComputeAggregateNameIsCalledForSendPort()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var namingConventionMock = new Mock<NamingConventionSpy>();
			namingConventionMock.Object.Party = "SomeParty";
			namingConventionMock.Object.MessageName = "SomeMessage";
			namingConventionMock.Object.MessageFormat = "SomeFormat";

			namingConventionMock.Object.ComputeSendPortNameSpy(sendPortMock.Object);

			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Once(), sendPortMock.Object.GetType());
		}

		[Fact]
		public void ComputeAggregateNameIsNotCalledForApplication()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.Setup(ab => ab.GetType()).Returns(typeof(SampleApplication));

			var namingConventionMock = new Mock<NamingConventionSpy>();
			namingConventionMock.Object.ComputeApplicationNameSpy(applicationBindingMock.Object);

			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Never(), applicationBindingMock.Object.GetType());
		}

		[Fact]
		public void ComputeAggregateNameReturnsFourthTokenOfTypeQualifiedNameMadeOfExactlyFiveTokens()
		{
			var sut = new NamingConventionSpy();
			sut.ComputeAggregateNameSpy(typeof(TaxAgencyReceivePort)).Should().Be("Invoice");
		}

		[Fact]
		public void ComputeAggregateNameReturnsNullWhenTypeQualifiedNameIsNotMadeOfExactlyFiveTokens()
		{
			var sut = new NamingConventionSpy();
			sut.ComputeAggregateNameSpy(typeof(SampleApplication)).Should().BeNull();
			sut.ComputeAggregateNameSpy(typeof(FinanceSampleApplication)).Should().BeNull();
			sut.ComputeAggregateNameSpy(typeof(StandaloneReceivePort)).Should().BeNull();
		}

		[Fact]
		public void ComputeApplicationNameReturnsGivenName()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();

			var sut = new NamingConventionSpy { ApplicationName = "SampleApplicationName" };

			sut.ComputeApplicationNameSpy(applicationBindingMock.Object).Should().Be("SampleApplicationName");
		}

		[Fact]
		public void ComputeApplicationNameReturnsTypeNameIfNotGiven()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.Setup(ab => ab.GetType()).Returns(typeof(UnnamedApplication));

			var sut = new NamingConventionSpy();

			sut.ComputeApplicationNameSpy(applicationBindingMock.Object).Should().Be(nameof(UnnamedApplication));
		}

		[Fact]
		public void ComputeReceiveLocationNameDoesNotRequireAggregateToMatchItsReceivePortOneIfItHasNone()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Action(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object)).Should().NotThrow();
		}

		[Fact]
		public void ComputeReceiveLocationNameEmbedsAggregate()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.Invoice.RL1.ReceivePortParty.SomeMessage.FILE.SomeFormat");
		}

		[Fact]
		public void ComputeReceiveLocationNameEmbedsApplicationNameAndPartyAndMessageNameAndTransportAndMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "SomeParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.RL1.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[Fact]
		public void ComputeReceiveLocationNameEmbedsEmptyMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = string.Empty };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.RL1.ReceivePortParty.SomeMessage.FILE");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiredPartyDefaultsToItsReceivePortOne()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object);

			sut.Party.Should().Be("ReceivePortParty");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresAggregateToMatchItsReceivePortOneIfItHasOne()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Action(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's Aggregate, 'Income', does not match its ReceivePort's one, 'Invoice'.");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresMessageName()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rp => rp.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionSpy();

			Action(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's MessageName is required.");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresNonNullMessageFormat()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionSpy { MessageName = "SomeMessage" };

			Action(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage("A non null MessageFormat is required.");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresParty()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy());

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rp => rp.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionSpy();

			Action(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's Party is required.");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresPartyToMatchItsReceivePortOne()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rp => rp.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionSpy { Party = "ReceiveLocationParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			Action(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's Party, 'ReceiveLocationParty', does not match its ReceivePort's one, 'ReceivePortParty'.");
		}

		[Fact]
		public void ComputeReceiveLocationNameTwoWay()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "SomeParty" });
			receivePortMock.Setup(rp => rp.IsTwoWay).Returns(true);

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport).Returns(new ReceiveLocationTransport { Adapter = new FileAdapter.Inbound(t => { }) });

			var sut = new NamingConventionSpy { MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.RL2.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[Fact]
		public void ComputeReceivePortNameEmbedsApplicationNameAndAggregateAndParty()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy { Party = "SomeParty" };

			sut.ComputeReceivePortNameSpy(receivePortMock.Object).Should().Be("SomeApplication.Invoice.RP1.SomeParty");
		}

		[Fact]
		public void ComputeReceivePortNameEmbedsApplicationNameAndParty()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy { Party = "SomeParty" };

			sut.ComputeReceivePortNameSpy(receivePortMock.Object).Should().Be("SomeApplication.RP1.SomeParty");
		}

		[Fact]
		public void ComputeReceivePortNameRequiresApplicationBinding()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));

			var sut = new NamingConventionSpy();

			Action(() => sut.ComputeReceivePortNameSpy(receivePortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneReceivePort)}' ReceivePort is not bound to application's receive port collection.");
		}

		[Fact]
		public void ComputeReceivePortNameRequiresParty()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy();

			Action(() => sut.ComputeReceivePortNameSpy(receivePortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneReceivePort)}' ReceivePort's Party is required.");
		}

		[Fact]
		public void ComputeReceivePortNameTwoWay()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.IsTwoWay).Returns(true);

			var sut = new NamingConventionSpy { Party = "SomeParty" };

			sut.ComputeReceivePortNameSpy(receivePortMock.Object).Should().Be("SomeApplication.RP2.SomeParty");
		}

		[Fact]
		public void ComputeSendPortNameEmbedsAggregate()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(BankSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var sut = new NamingConventionSpy { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.Income.SP1.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[Fact]
		public void ComputeSendPortNameEmbedsApplicationNameAndPartyAndMessageNameAndTransportAndMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var sut = new NamingConventionSpy { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.SP1.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[Fact]
		public void ComputeSendPortNameEmbedsEmptyMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });

			var sut = new NamingConventionSpy { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = string.Empty };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.SP1.SomeParty.SomeMessage.FILE");
		}

		[Fact]
		public void ComputeSendPortNameRequiresApplicationBinding()
		{
			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));

			var sut = new NamingConventionSpy();

			Action(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneSendPort)}' SendPort is not bound to application's send port collection.");
		}

		[Fact]
		public void ComputeSendPortNameRequiresMessageName()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy { Party = "SomeParty" };

			Action(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneSendPort)}' SendPort's MessageName is required.");
		}

		[Fact]
		public void ComputeSendPortNameRequiresNonNullMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy { Party = "SomeParty", MessageName = "SomeMessage" };

			Action(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage("A non null MessageFormat is required.");
		}

		[Fact]
		public void ComputeSendPortNameRequiresParty()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy();

			Action(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneSendPort)}' SendPort's Party is required.");
		}

		[Fact]
		public void ComputeSendPortNameTwoWay()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport { Adapter = new FileAdapter.Outbound(t => { }) });
			sendPortMock.Setup(sp => sp.IsTwoWay).Returns(true);

			var sut = new NamingConventionSpy { Party = "SomeParty", MessageName = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.SP2.SomeParty.SomeMessage.FILE.SomeFormat");
		}
	}
}
