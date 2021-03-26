﻿#region Copyright & License

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

using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Environment.Settings;
using Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Factory
{
	[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public class Platform : CompositeEnvironmentSettings<Platform, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
	{
		#region IEnvironmentSettings Members

		public string ApplicationName => "BizTalk.Factory";

		#endregion

		#region IPlatformEnvironmentSettings Members

		public string IsolatedHost => "BizTalkServerIsolatedHost";

		public string ManagementDatabaseInstance => string.Empty;

		public string ManagementDatabaseServer => "localhost";

		public string MonitoringDatabaseInstance => string.Empty;

		public string MonitoringDatabaseServer => "localhost";

		public string ProcessingDatabaseInstance => string.Empty;

		public string ProcessingDatabaseServer => "localhost";

		public string ProcessingHost => "BizTalkServerApplication";

		public string ReceivingHost => "BizTalkServerApplication";

		public string TransmittingHost => "BizTalkServerApplication";

		#endregion

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Property that needs to be deployed in SSO.")]
		[SsoSetting]
		public string TargetEnvironment => DeploymentContext.TargetEnvironment;
	}
}
