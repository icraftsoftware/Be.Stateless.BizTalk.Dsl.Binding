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
using System.Linq;
using Be.Stateless.Extensions;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter.Utils
{
	internal static class OdpNet
	{
		internal static bool IsConfigured
		{
			get
			{
				var minVersion = new Version(4, 122, 18, 3);
				using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
				using (var odpKey = baseKey.OpenSubKey(@"SOFTWARE\Oracle\ODP.NET"))
				{
					return odpKey
						.IfNotNull(k => k.GetSubKeyNames())
						.Any(n => Version.TryParse(n, out var actualVersion) && actualVersion >= minVersion);
				}
			}
		}
	}
}
