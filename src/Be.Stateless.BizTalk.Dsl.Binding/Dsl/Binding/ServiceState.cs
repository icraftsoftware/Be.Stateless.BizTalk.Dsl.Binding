﻿#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
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

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	/// <summary>
	/// The state of a BizTalk Server service, for instance, a send port or an orchestration.
	/// </summary>
	public enum ServiceState
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		Undefined = 0,
		Unenlisted = 1,
		Enlisted = 2,
		Stopped = Enlisted,
		Started = 3
	}
}
