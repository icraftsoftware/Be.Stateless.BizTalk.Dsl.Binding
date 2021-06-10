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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public abstract class TransportBase<T> : ISupportEnvironmentOverride, ISupportHostNameResolution, ISupportValidation
		where T : class, IAdapter, ISupportEnvironmentOverride, ISupportValidation
	{
		#region Nested Type: UnknownAdapter

		protected internal abstract class UnknownAdapter : IAdapter
		{
			#region IAdapter Members

			public string Address => throw new NotSupportedException();

			public ProtocolType ProtocolType => throw new NotSupportedException();

			public string PublicAddress => throw new NotSupportedException();

			public void ApplyEnvironmentOverrides(string environment) { }

			public void Save(IPropertyBag propertyBag)
			{
				throw new NotSupportedException();
			}

			public void Validate()
			{
				throw new NotSupportedException($"{GetType().Name} is not a valid transport adapter.");
			}

			#endregion
		}

		#endregion

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (environment.IsNullOrEmpty()) return;
			ApplyEnvironmentOverrides(environment);
			Adapter?.ApplyEnvironmentOverrides(environment);
		}

		#endregion

		#region ISupportHostNameResolution Members

		/// <summary>
		/// Resolve host name that is to be bound to this transport's adapter.
		/// </summary>
		/// <returns>
		/// The name of the host.
		/// </returns>
		/// <remarks>
		/// Notice that TransportBase.ResolveHostName delegates to either ReceiveLocationTransport's or SendPortTransport's
		/// protected ResolveHostName(), which delegate in turn to actual HostResolutionPolicy instance but this time with the
		/// ReceiveLocationTransport or SendPortTransport instance being concerned passed as argument to help with resolution.
		/// </remarks>
		string ISupportHostNameResolution.ResolveHostName()
		{
			return ResolveHostName();
		}

		#endregion

		#region ISupportValidation Members

		void ISupportValidation.Validate()
		{
			if (Host == null) throw new BindingException("Transport's Host is not defined.");
			if (Adapter is null or UnknownAdapter) throw new BindingException("Transport's Adapter is not defined.");
			Adapter.Validate();
		}

		#endregion

		public T Adapter { get; set; }

		/// <summary>
		/// The BizTalk Server Host Name that will host this transport's <see cref="Adapter"/> at runtime.
		/// </summary>
		/// <remarks>
		/// The <see cref="Host"/> property can either be set directly to a <see cref="string"/> value or to a <see
		/// cref="HostResolutionPolicy"/>-derived object instance.
		/// </remarks>
		public HostResolutionPolicy Host { get; set; }

		protected abstract void ApplyEnvironmentOverrides(string environment);

		protected abstract string ResolveHostName();
	}
}
