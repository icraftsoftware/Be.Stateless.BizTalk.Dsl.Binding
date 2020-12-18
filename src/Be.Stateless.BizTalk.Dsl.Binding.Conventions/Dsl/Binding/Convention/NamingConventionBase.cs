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
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention.Extensions;
using Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public abstract class NamingConventionBase<TNamingConvention, TParty, TMessageName> :
		IApplicationNameMemento<string>,
		IPartyMemento<TParty>,
		IMessageNameMemento<TMessageName>,
		IMessageFormatMemento<string>
		where TNamingConvention : NamingConventionBase<TNamingConvention, TParty, TMessageName>, INamingConvention<TNamingConvention>
	{
		#region IApplicationNameMemento<string> Members

		public string ApplicationName { get; set; }

		#endregion

		#region IMessageFormatMemento<string> Members

		public string MessageFormat { get; set; }

		#endregion

		#region IMessageNameMemento<TMessageName> Members

		public TMessageName MessageName { get; set; }

		#endregion

		#region IPartyMemento<TParty> Members

		public TParty Party { get; set; }

		#endregion

		protected string ComputeApplicationName(IApplicationBinding<TNamingConvention> application)
		{
			if (application == null) throw new ArgumentNullException(nameof(application));
			return ApplicationName.IsNullOrEmpty() ? application.GetType().Name : ApplicationName;
		}

		protected string ComputeReceivePortName(IReceivePort<TNamingConvention> receivePort)
		{
			if (receivePort == null) throw new ArgumentNullException(nameof(receivePort));
			if (receivePort.ApplicationBinding == null)
				throw new NamingConventionException($"'{receivePort.GetType().Name}' ReceivePort is not bound to application's receive port collection.");
			if (Equals(Party, default(TParty))) throw new NamingConventionException($"'{receivePort.GetType().Name}' ReceivePort's Party is required.");

			var aggregate = ComputeAggregateName(receivePort.GetType());
			return string.Format(
				CultureInfo.InvariantCulture,
				"{0}{1}.RP{2}.{3}",
				((ISupportNamingConvention) receivePort.ApplicationBinding).Name,
				aggregate.IsNullOrEmpty() ? string.Empty : $".{aggregate}",
				receivePort.IsTwoWay ? "2" : "1",
				Party);
		}

		protected string ComputeReceiveLocationName(IReceiveLocation<TNamingConvention> receiveLocation)
		{
			if (receiveLocation == null) throw new ArgumentNullException(nameof(receiveLocation));
			if (receiveLocation.ReceivePort == null) throw new NamingConventionException($"'{receiveLocation.GetType().Name}' ReceiveLocation is not bound to any receive port.");
			if (Equals(Party, default(TParty))) Party = receiveLocation.ReceivePort.Name.Party;
			if (Equals(Party, default(TParty))) throw new NamingConventionException($"'{receiveLocation.GetType().Name}' ReceiveLocation's Party is required.");
			if (!Equals(Party, receiveLocation.ReceivePort.Name.Party))
				throw new NamingConventionException(
					$"'{receiveLocation.GetType().Name}' ReceiveLocation's Party, '{Party}', does not match its ReceivePort's one, '{receiveLocation.ReceivePort.Name.Party}'.");
			if (Equals(MessageName, default(TMessageName))) throw new NamingConventionException($"'{receiveLocation.GetType().Name}' ReceiveLocation's MessageName is required.");
			if (MessageFormat == null) throw new NamingConventionException("A non null MessageFormat is required.");

			var aggregate = ComputeAggregateName(receiveLocation.GetType());
			var receivePortAggregate = ComputeAggregateName(receiveLocation.ReceivePort.GetType());
			if (aggregate.IsNullOrEmpty() && !receivePortAggregate.IsNullOrEmpty()) aggregate = receivePortAggregate;
			if (!receivePortAggregate.IsNullOrEmpty() && receivePortAggregate != aggregate)
				throw new NamingConventionException(
					$"'{receiveLocation.GetType().Name}' ReceiveLocation's Aggregate, '{aggregate}', does not match its ReceivePort's one, '{receivePortAggregate}'.");

			return string.Format(
				CultureInfo.InvariantCulture,
				"{0}{1}.RL{2}.{3}.{4}.{5}{6}",
				((ISupportNamingConvention) receiveLocation.ReceivePort.ApplicationBinding).Name,
				aggregate.IsNullOrEmpty() ? string.Empty : $".{aggregate}",
				receiveLocation.ReceivePort.IsTwoWay ? "2" : "1",
				Party,
				MessageName,
				ComputeAdapterName(receiveLocation.Transport.Adapter),
				MessageFormat.IsNullOrEmpty() ? string.Empty : $".{MessageFormat}");
		}

		protected string ComputeSendPortName(ISendPort<TNamingConvention> sendPort)
		{
			if (sendPort == null) throw new ArgumentNullException(nameof(sendPort));
			if (sendPort.ApplicationBinding == null)
				throw new NamingConventionException($"'{sendPort.GetType().Name}' SendPort is not bound to application's send port collection.");
			if (Equals(Party, default(TParty))) throw new NamingConventionException($"'{sendPort.GetType().Name}' SendPort's Party is required.");
			if (Equals(MessageName, default(TMessageName))) throw new NamingConventionException($"'{sendPort.GetType().Name}' SendPort's MessageName is required.");
			if (MessageFormat == null) throw new NamingConventionException("A non null MessageFormat is required.");

			var aggregate = ComputeAggregateName(sendPort.GetType());
			return string.Format(
				CultureInfo.InvariantCulture,
				"{0}{1}.SP{2}.{3}.{4}.{5}{6}",
				((ISupportNamingConvention) sendPort.ApplicationBinding).Name,
				aggregate.IsNullOrEmpty() ? string.Empty : $".{aggregate}",
				sendPort.IsTwoWay ? "2" : "1",
				Party,
				MessageName,
				ComputeAdapterName(sendPort.Transport.Adapter),
				MessageFormat.IsNullOrEmpty() ? string.Empty : $".{MessageFormat}");
		}

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Convention Public API.")]
		protected virtual string ComputeAdapterName(IAdapter adapter)
		{
			if (adapter == null) throw new ArgumentNullException(nameof(adapter));
			var name = adapter.ProtocolType.Name;
			if (!adapter.GetType().IsSubclassOfGenericType(typeof(WcfCustomAdapterBase<,,>))) return name;

			// cast to dynamic in order to access Binding property which is declared by WcfCustomAdapterBase<,,>
			dynamic dynamicAdapter = adapter;
			var binding = dynamicAdapter.Binding;
			if (binding is CustomBindingElement customBindingElement)
			{
				var actualBindingElement = (System.ServiceModel.Configuration.CustomBindingElement) customBindingElement.DecoratedBindingElement;
				var transportElementType = actualBindingElement
					.Select(be => be.BindingElementType)
					.Single(bet => typeof(TransportBindingElement).IsAssignableFrom(bet));
				return typeof(Microsoft.ServiceModel.Channels.Common.Adapter).IsAssignableFrom(transportElementType)
					? name + transportElementType.Name
						.TrimSuffix(nameof(Microsoft.ServiceModel.Channels.Common.Adapter))
						.Capitalize()
					: name + transportElementType.Name
						.TrimSuffix(nameof(TransportBindingElement))
						.Capitalize();
			}

			var bindingName = ((string) binding.Name)
				.TrimSuffix("Binding")
				.Capitalize();
			return name + bindingName;
		}

		[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Convention Public API.")]
		protected virtual string ComputeAggregateName(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			if (type.IsGenericType) type = type.GetGenericTypeDefinition();
			var tokens = type.FullName!.Split('.');
			return tokens.Length == 5 ? tokens[3] : null;
		}
	}
}
