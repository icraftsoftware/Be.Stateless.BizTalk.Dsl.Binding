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

using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.Install
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Convention Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Convention Public API.")]
	public static class TargetEnvironment
	{
		public static bool IsDevelopment(this string environment)
		{
			return environment is DEVELOPMENT;
		}

		public static bool IsBuild(this string environment)
		{
			return environment is BUILD;
		}

		public static bool IsDevelopmentOrBuild(this string environment)
		{
			return environment.IsDevelopment() || environment.IsBuild();
		}

		public static bool IsIntegration(this string environment)
		{
			return environment is INTEGRATION;
		}

		public static bool IsIntegrationUpwards(this string environment)
		{
			return environment.IsIntegration() || environment.IsAcceptanceUpwards();
		}

		public static bool IsAcceptance(this string environment)
		{
			return environment is ACCEPTANCE;
		}

		public static bool IsAcceptanceUpwards(this string environment)
		{
			return environment.IsAcceptance() || environment.IsPreProductionUpwards();
		}

		public static bool IsPreProduction(this string environment)
		{
			return environment is PREPRODUCTION;
		}

		public static bool IsPreProductionUpwards(this string environment)
		{
			return environment.IsPreProduction() || environment.IsProduction();
		}

		public static bool IsProduction(this string environment)
		{
			return environment is PRODUCTION;
		}

		// @formatter:off
		public const string DEVELOPMENT = "DEV";
		public const string BUILD = "BLD";
		public const string INTEGRATION = "INT";
		public const string ACCEPTANCE = "ACC";
		public const string PREPRODUCTION = "PRE";
		public const string PRODUCTION = "PRD";
		// @formatter:on
	}
}
