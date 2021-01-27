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
using System.Linq;
using System.Reflection;

namespace Be.Stateless.BizTalk.Dsl.Binding.Extensions
{
	public static class AssemblyExtensions
	{
		public static Type GetApplicationBindingType(this Assembly assembly, bool throwOnError = false)
		{
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));
			var types = assembly.GetExportedTypes().Where(t => t.IsApplicationBindingType());
			var type = types.SingleOrDefault();
			return type == null && throwOnError
				? throw new InvalidOperationException($"No {nameof(IApplicationBinding)}-derived type was found in assembly '{assembly.FullName}' located at '{assembly.Location}'.")
				: type;
		}
	}
}
