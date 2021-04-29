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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Dsl.Environment.Settings;

namespace Be.Stateless.BizTalk.Dummies.Environment.Settings
{
	internal class DummyExcelEnvironmentSettings : ExcelEnvironmentSettings
	{
		#region Base Class Member Overrides

		protected override string ExcelFileName => "BizTalk.Factory.Settings";

		protected override string[] TargetEnvironments => _targetEnvironments;

		#endregion

		public int BamArchiveWindowTimeLength => ValueForTargetEnvironment(new int?[] { null, 1, 2, 3, 4 });

		public string ClaimStoreCheckInDirectory => ValueForTargetEnvironment(new[] { null, "C:\\Files\\CheckIn", null, null, null });

		public string UninitializedReferenceTypeSetting => ValueForTargetEnvironment(new string[] { null, null, null, null, null });

		public int UninitializedValueTypeSetting => ValueForTargetEnvironment(new int?[] { null, null, null, null, null });

		public string UnoverriddenReferenceTypeSetting => ValueForTargetEnvironment(new[] { null, "unoverridden", null, null, null });

		public int UnoverriddenValueTypeSetting => ValueForTargetEnvironment(new int?[] { null, -1, -2, -3, -4 });

		internal readonly string[] _targetEnvironments = { null, TargetEnvironment.DEVELOPMENT, TargetEnvironment.BUILD, TargetEnvironment.ACCEPTANCE, TargetEnvironment.PRODUCTION };
	}
}
