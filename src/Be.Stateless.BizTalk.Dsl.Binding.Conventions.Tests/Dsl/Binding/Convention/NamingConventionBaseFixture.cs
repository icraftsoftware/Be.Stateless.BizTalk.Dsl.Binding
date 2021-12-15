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
using System.ServiceModel.Configuration;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dummies.Adapter;
using Be.Stateless.BizTalk.Dummies.Bindings;
using Be.Stateless.BizTalk.Dummies.Bindings.Simple;
using Be.Stateless.BizTalk.Dummies.Conventions;
using Be.Stateless.BizTalk.Explorer;
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
using static FluentAssertions.FluentActions;
using CustomBindingElement = Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration.CustomBindingElement;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class NamingConventionBaseFixture
	{
		[SkippableFact]
		public void ComputeAdapterNameForOffice365EmailAdapter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sut = new NamingConventionSpy();

			IAdapter adapter = new Office365EmailAdapter.Inbound();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("Office365Email");
		}

		[SkippableFact]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomAdapter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

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

		[SkippableFact]
		public void ComputeAdapterNameResolvesActualProtocolTypeNameForWcfCustomIsolatedAdapter()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var sut = new NamingConventionSpy();

			IAdapter adapter = new CustomIsolatedAdapterDummy<NetTcpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomIsolatedNetTcp");

			adapter = new CustomIsolatedAdapterDummy<WSHttpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomIsolatedWsHttp");

			adapter = new CustomIsolatedAdapterDummy<BasicHttpBindingElement, CustomRLConfig>();
			sut.ComputeAdapterNameSpy(adapter).Should().Be("WCF-CustomIsolatedBasicHttp");
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void ComputeAggregateNameIsCalledForReceiveLocation()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "SomeParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var namingConventionMock = new Mock<NamingConventionSpy>();
			namingConventionMock.Object.Subject = "SomeMessage";
			namingConventionMock.Object.MessageFormat = "SomeFormat";

			namingConventionMock.Object.ComputeReceiveLocationNameSpy(receiveLocationMock.Object);

			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Once(), receivePortMock.Object.GetType());
			namingConventionMock.Protected().Verify("ComputeAggregateName", Times.Once(), receiveLocationMock.Object.GetType());
		}

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
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

		[SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
		[SkippableFact]
		public void ComputeAggregateNameIsCalledForSendPort()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport<NamingConventionSpy>(sendPortMock.Object) { Adapter = new FileAdapter.Outbound(_ => { }) });

			var namingConventionMock = new Mock<NamingConventionSpy>();
			namingConventionMock.Object.Party = "SomeParty";
			namingConventionMock.Object.Subject = "SomeMessage";
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

		[SkippableFact]
		public void ComputeReceiveLocationNameDoesNotRequireAggregateToMatchItsReceivePortOneIfItHasNone()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			Invoking(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object)).Should().NotThrow();
		}

		[SkippableFact]
		public void ComputeReceiveLocationNameEmbedsAggregate()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.Invoice.RL1.ReceivePortParty.SomeMessage.FILE.SomeFormat");
		}

		[SkippableFact]
		public void ComputeReceiveLocationNameEmbedsApplicationNameAndPartyAndSubjectAndTransportAndMessageFormat()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "SomeParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.RL1.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[SkippableFact]
		public void ComputeReceiveLocationNameEmbedsEmptyMessageFormat()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = string.Empty };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object).Should().Be("SomeApplication.RL1.ReceivePortParty.SomeMessage.FILE");
		}

		[SkippableFact]
		public void ComputeReceiveLocationNameRequiredPartyDefaultsToItsReceivePortOne()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(StandaloneReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object);

			sut.Party.Should().Be("ReceivePortParty");
		}

		[SkippableFact]
		public void ComputeReceiveLocationNameRequiresAggregateToMatchItsReceivePortOneIfItHasOne()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(TaxAgencyReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			Invoking(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's Aggregate, 'Income', does not match its ReceivePort's one, 'Invoice'.");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresNonNullMessageFormat()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionSpy { Subject = "SomeMessage" };

			Invoking(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
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

			Invoking(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
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

			var sut = new NamingConventionSpy { Party = "ReceiveLocationParty", Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			Invoking(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's Party, 'ReceiveLocationParty', does not match its ReceivePort's one, 'ReceivePortParty'.");
		}

		[Fact]
		public void ComputeReceiveLocationNameRequiresSubject()
		{
			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.Name).Returns(new NamingConventionSpy { Party = "ReceivePortParty" });

			var receiveLocationMock = new Mock<IReceiveLocation<NamingConventionSpy>>();
			receiveLocationMock.Setup(rp => rp.GetType()).Returns(typeof(BankReceiveLocation));
			receiveLocationMock.Setup(rl => rl.ReceivePort).Returns(receivePortMock.Object);

			var sut = new NamingConventionSpy();

			Invoking(() => sut.ComputeReceiveLocationNameSpy(receiveLocationMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(BankReceiveLocation)}' ReceiveLocation's Subject is required.");
		}

		[SkippableFact]
		public void ComputeReceiveLocationNameTwoWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

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
			receiveLocationMock.Setup(rl => rl.Transport)
				.Returns(new ReceiveLocationTransport<NamingConventionSpy>(receiveLocationMock.Object) { Adapter = new FileAdapter.Inbound(_ => { }) });

			var sut = new NamingConventionSpy { Subject = "SomeMessage", MessageFormat = "SomeFormat" };

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

			Invoking(() => sut.ComputeReceivePortNameSpy(receivePortMock.Object))
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

			Invoking(() => sut.ComputeReceivePortNameSpy(receivePortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneReceivePort)}' ReceivePort's Party is required.");
		}

		[SkippableFact]
		public void ComputeReceivePortNameTwoWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var receivePortMock = new Mock<IReceivePort<NamingConventionSpy>>();
			receivePortMock.Setup(rp => rp.GetType()).Returns(typeof(StandaloneReceivePort));
			receivePortMock.Setup(rp => rp.ApplicationBinding).Returns(applicationBindingMock.Object);
			receivePortMock.Setup(rp => rp.IsTwoWay).Returns(true);

			var sut = new NamingConventionSpy { Party = "SomeParty" };

			sut.ComputeReceivePortNameSpy(receivePortMock.Object).Should().Be("SomeApplication.RP2.SomeParty");
		}

		[SkippableFact]
		public void ComputeSendPortNameEmbedsAggregate()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(BankSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport<NamingConventionSpy>(sendPortMock.Object) { Adapter = new FileAdapter.Outbound(_ => { }) });

			var sut = new NamingConventionSpy { Party = "SomeParty", Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.Income.SP1.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[SkippableFact]
		public void ComputeSendPortNameEmbedsApplicationNameAndPartyAndSubjectAndTransportAndMessageFormat()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport<NamingConventionSpy>(sendPortMock.Object) { Adapter = new FileAdapter.Outbound(_ => { }) });

			var sut = new NamingConventionSpy { Party = "SomeParty", Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.SP1.SomeParty.SomeMessage.FILE.SomeFormat");
		}

		[SkippableFact]
		public void ComputeSendPortNameEmbedsEmptyMessageFormat()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport<NamingConventionSpy>(sendPortMock.Object) { Adapter = new FileAdapter.Outbound(_ => { }) });

			var sut = new NamingConventionSpy { Party = "SomeParty", Subject = "SomeMessage", MessageFormat = string.Empty };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.SP1.SomeParty.SomeMessage.FILE");
		}

		[Fact]
		public void ComputeSendPortNameRequiresApplicationBinding()
		{
			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));

			var sut = new NamingConventionSpy();

			Invoking(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneSendPort)}' SendPort is not bound to application's send port collection.");
		}

		[Fact]
		public void ComputeSendPortNameRequiresNonNullMessageFormat()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy { Party = "SomeParty", Subject = "SomeMessage" };

			Invoking(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
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

			Invoking(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneSendPort)}' SendPort's Party is required.");
		}

		[Fact]
		public void ComputeSendPortNameRequiresSubject()
		{
			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);

			var sut = new NamingConventionSpy { Party = "SomeParty" };

			Invoking(() => sut.ComputeSendPortNameSpy(sendPortMock.Object))
				.Should().Throw<NamingConventionException>()
				.WithMessage($"'{nameof(StandaloneSendPort)}' SendPort's Subject is required.");
		}

		[SkippableFact]
		public void ComputeSendPortNameTwoWay()
		{
			Skip.IfNot(BizTalkServerGroup.IsConfigured);

			var applicationBindingMock = new Mock<IApplicationBinding<NamingConventionSpy>>();
			applicationBindingMock.As<ISupportNamingConvention>().Setup(snc => snc.Name).Returns("SomeApplication");

			var sendPortMock = new Mock<ISendPort<NamingConventionSpy>>();
			sendPortMock.Setup(sp => sp.GetType()).Returns(typeof(StandaloneSendPort));
			sendPortMock.Setup(sp => sp.ApplicationBinding).Returns(applicationBindingMock.Object);
			sendPortMock.Setup(sp => sp.Transport).Returns(new SendPortTransport<NamingConventionSpy>(sendPortMock.Object) { Adapter = new FileAdapter.Outbound(_ => { }) });
			sendPortMock.Setup(sp => sp.IsTwoWay).Returns(true);

			var sut = new NamingConventionSpy { Party = "SomeParty", Subject = "SomeMessage", MessageFormat = "SomeFormat" };

			sut.ComputeSendPortNameSpy(sendPortMock.Object).Should().Be("SomeApplication.SP2.SomeParty.SomeMessage.FILE.SomeFormat");
		}
	}
}
