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

using System.CodeDom.Compiler;

namespace Be.Stateless.BizTalk.Factory
{
	[GeneratedCode("EnvironmentSettings", "2.0.0.0")]
	public partial class BizTalkFactorySettings : Be.Stateless.BizTalk.Dsl.Environment.Settings.ExcelEnvironmentSettings
	{

		public static int BamArchiveWindowTimeLength => _instance.ValueForTargetEnvironment(new int?[] { null, 30, 30, null, null });

		public static int BamOnlineWindowTimeLength => _instance.ValueForTargetEnvironment(new int?[] { null, 15, 15, null, null });

		public static string ClaimStoreAgentTargetHosts => _instance.ValueForTargetEnvironment(new string[] { null, "-", "-", "*", "*" });

		public static string ClaimStoreCheckInDirectory => _instance.ValueForTargetEnvironment(new string[] { null, "C:\\Files\\Drops\\BizTalk.Factory\\CheckIn", "C:\\Files\\Drops\\BizTalk.Factory\\CheckIn", null, null });

		public static string ClaimStoreCheckOutDirectory => _instance.ValueForTargetEnvironment(new string[] { null, "C:\\Files\\Drops\\BizTalk.Factory\\CheckOut", "C:\\Files\\Drops\\BizTalk.Factory\\CheckOut", null, null });

		protected override string ExcelFileName => "BizTalk.Factory.Settings.xml";

		protected override string[] TargetEnvironments => _targetEnvironments;

		private static readonly BizTalkFactorySettings _instance = new BizTalkFactorySettings();
		private static readonly string[] _targetEnvironments = { null, "DEV", "BLD", "ACC", "PRD" };
	}
}

