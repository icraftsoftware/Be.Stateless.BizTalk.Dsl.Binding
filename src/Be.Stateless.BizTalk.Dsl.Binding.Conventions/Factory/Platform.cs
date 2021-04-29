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
	public class Platform : CompositeEnvironmentSettings<Platform, IPlatformEnvironmentSettings>, IPlatformEnvironmentSettings, IEnvironmentSettings
	{
		#region IEnvironmentSettings Members

		public string ApplicationName => throw new NotSupportedException();

		#endregion

		#region IPlatformEnvironmentSettings Members

		public string IsolatedHost => GetOverriddenOrDefaultValue("BizTalkServerIsolatedHost");

		public string ManagementDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

		public string ManagementDatabaseServer => GetOverriddenOrDefaultValue("localhost");

		public string MonitoringDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

		public string MonitoringDatabaseServer => GetOverriddenOrDefaultValue("localhost");

		public string ProcessingDatabaseInstance => GetOverriddenOrDefaultValue(string.Empty);

		public string ProcessingDatabaseServer => GetOverriddenOrDefaultValue("localhost");

		public string ProcessingHost => GetOverriddenOrDefaultValue("BizTalkServerApplication");

		public string ReceivingHost => GetOverriddenOrDefaultValue("BizTalkServerApplication");

		public string TransmittingHost => GetOverriddenOrDefaultValue("BizTalkServerApplication");

		#endregion
	}
}
