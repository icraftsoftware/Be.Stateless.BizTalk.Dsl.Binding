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
using Be.Stateless.BizTalk.Dsl.Environment.Settings;
using Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention;

namespace Be.Stateless.BizTalk.Factory
{
	[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public class Platform : CompositeEnvironmentSettings<Platform, IProvideEnvironmentSettings>,
		IProvideDatabaseNames,
		IProvideHostNames,
		IProvideHostResolutionPolicy,
		IEnvironmentSettings
	{
		#region Nested Type: DefaultHostNameProvider

		private class DefaultHostNameProvider : IProvideHostNames
		{
			#region IProvideHostNames Members

			public string IsolatedHost => "BizTalkServerIsolatedHost";

			public string ProcessingHost => "BizTalkServerApplication";

			public string ReceivingHost => "BizTalkServerApplication";

			public string TransmittingHost => "BizTalkServerApplication";

			#endregion
		}

		#endregion

		#region IEnvironmentSettings Members

		string IEnvironmentSettings.ApplicationName => throw new NotSupportedException();

		#endregion

		#region IProvideDatabaseNames Members

		public string ManagementDatabaseInstance => EnvironmentSettingOverrides is IProvideDatabaseNames p ? p.ManagementDatabaseInstance : string.Empty;

		public string ManagementDatabaseServer => EnvironmentSettingOverrides is IProvideDatabaseNames p ? p.ManagementDatabaseServer : "localhost";

		public string MonitoringDatabaseInstance => EnvironmentSettingOverrides is IProvideDatabaseNames p ? p.MonitoringDatabaseInstance : string.Empty;

		public string MonitoringDatabaseServer => EnvironmentSettingOverrides is IProvideDatabaseNames p ? p.MonitoringDatabaseServer : "localhost";

		public string ProcessingDatabaseInstance => EnvironmentSettingOverrides is IProvideDatabaseNames p ? p.ProcessingDatabaseInstance : string.Empty;

		public string ProcessingDatabaseServer => EnvironmentSettingOverrides is IProvideDatabaseNames p ? p.ProcessingDatabaseServer : "localhost";

		#endregion

		#region IProvideHostNames Members

		string IProvideHostNames.IsolatedHost => ThrowIProvideHostNamesTraitNotSupported();

		string IProvideHostNames.ProcessingHost => ThrowIProvideHostNamesTraitNotSupported();

		string IProvideHostNames.ReceivingHost => ThrowIProvideHostNamesTraitNotSupported();

		string IProvideHostNames.TransmittingHost => ThrowIProvideHostNamesTraitNotSupported();

		#endregion

		#region IProvideHostResolutionPolicy Members

		public Dsl.Binding.Convention.HostResolutionPolicy HostResolutionPolicy => EnvironmentSettingOverrides is IProvideHostResolutionPolicy p
			? p.HostResolutionPolicy
			: Convention.HostResolutionPolicy.Default;

		#endregion

		internal IProvideHostNames HostNameProvider
		{
			get
			{
				return _hostNameProvider ??= EnvironmentSettingOverrides switch {
					IProvideHostNames and IProvideHostResolutionPolicy => ThrowIProvideHostNamesAndIProvideHostResolutionPolicyAmbiguity(),
					IProvideHostNames hostNameProvider => hostNameProvider,
					_ => new DefaultHostNameProvider()
				};
			}
		}

		private IProvideHostNames ThrowIProvideHostNamesAndIProvideHostResolutionPolicyAmbiguity()
		{
			throw new InvalidOperationException(
				$"{nameof(EnvironmentSettingOverrides)} '{EnvironmentSettingOverrides.GetType().Name}' should only implement either '{nameof(IProvideHostNames)}' or '{nameof(IProvideHostResolutionPolicy)}'; but it implements both.");
		}

		private string ThrowIProvideHostNamesTraitNotSupported()
		{
			throw new NotSupportedException(
				$"{nameof(Platform)} provides a {nameof(HostResolutionPolicy)}; its {nameof(IProvideHostNames)} trait is only an overriding point and must not be called explicitly.");
		}

		private IProvideHostNames _hostNameProvider;
	}
}
