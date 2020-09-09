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
using System.Linq;
using System.Management;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Adapter.Sftp;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract class AdapterBase : IAdapter
	{
		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		protected static ProtocolType GetProtocolTypeFromConfigurationClassId(Guid configurationClassId)
		{
			var scope = new ManagementScope(@"\\.\root\MicrosoftBizTalkServer");
			var query = new SelectQuery(
				"MSBTS_AdapterSetting",
				$"MgmtCLSID='{configurationClassId:B}'",
				new[] { "Name", "Constraints" });
			using (var mos = new ManagementObjectSearcher(scope, query))
			using (var mo = mos.Get().Cast<ManagementObject>().Single())
			{
				return new ProtocolType {
					Capabilities = (int) (uint) mo["Constraints"],
					ConfigurationClsid = configurationClassId.ToString(),
					Name = (string) mo["Name"]
				};
			}
		}

		protected AdapterBase(ProtocolType protocolType)
		{
			_protocolType = protocolType ?? throw new ArgumentNullException(nameof(protocolType));
		}

		#region IAdapter Members

		[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types")]
		string IAdapter.Address => GetAddress();

		[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types")]
		ProtocolType IAdapter.ProtocolType => _protocolType;

		[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types")]
		string IAdapter.PublicAddress => GetPublicAddress();

		void IAdapter.Save(IPropertyBag propertyBag)
		{
			Save(propertyBag);
		}

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			if (!environment.IsNullOrEmpty()) ApplyEnvironmentOverrides(environment);
		}

		void ISupportValidation.Validate()
		{
			Validate();
		}

		#endregion

		protected abstract string GetAddress();

		protected virtual string GetPublicAddress()
		{
			return null;
		}

		protected virtual void ApplyEnvironmentOverrides(string environment) { }

		protected abstract void Save(IPropertyBag propertyBag);

		protected abstract void Validate();

		[SuppressMessage("ReSharper", "RedundantCaseLabel")]
		protected TimeSpan BuildTimeSpan(int quantity, PollingIntervalUnit unit)
		{
			return unit switch {
				PollingIntervalUnit.Seconds => TimeSpan.FromSeconds(quantity),
				PollingIntervalUnit.Minutes => TimeSpan.FromMinutes(quantity),
				PollingIntervalUnit.Hours => TimeSpan.FromHours(quantity),
				PollingIntervalUnit.Days => TimeSpan.FromDays(quantity),
				_ => TimeSpan.FromDays(quantity)
			};
		}

		protected void UnbuildTimeSpan(TimeSpan interval, Action<int, PollingIntervalUnit> quantityAndUnitSetter)
		{
			if (quantityAndUnitSetter == null) throw new ArgumentNullException(nameof(quantityAndUnitSetter));
			if (interval.Seconds != 0 || interval == TimeSpan.Zero)
			{
				quantityAndUnitSetter((int) interval.TotalSeconds, PollingIntervalUnit.Seconds);
			}
			else if (interval.Minutes != 0)
			{
				quantityAndUnitSetter((int) interval.TotalMinutes, PollingIntervalUnit.Minutes);
			}
			else if (interval.Hours != 0)
			{
				quantityAndUnitSetter((int) interval.TotalHours, PollingIntervalUnit.Hours);
			}
			else // if (interval.Days != 0)
			{
				quantityAndUnitSetter((int) interval.TotalDays, PollingIntervalUnit.Days);
			}
		}

		private readonly ProtocolType _protocolType;
	}
}
