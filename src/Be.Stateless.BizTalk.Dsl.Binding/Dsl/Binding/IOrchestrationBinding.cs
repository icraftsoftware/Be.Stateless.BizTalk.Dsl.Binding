#region Copyright & License

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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public interface IOrchestrationBinding : ISupportHostResolution, ISupportNameResolution
	{
		IApplicationBinding ApplicationBinding { get; set; }

		string Description { get; set; }

		/// <summary>
		/// The BizTalk Server Host Name that will host this orchestration at runtime.
		/// </summary>
		/// <remarks>
		/// The <see cref="Host"/> property can either be set directly to a <see cref="string"/> value or to a <see
		/// cref="HostResolutionPolicy"/>-derived object instance.
		/// </remarks>
		[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API.")]
		HostResolutionPolicy Host { get; set; }

		[SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Global", Justification = "Public DSL API.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IOrchestrationPortBinding[] PortBindings { get; }

		/// <summary>
		/// The state of the orchestration.
		/// </summary>
		/// <remarks>
		/// An orchestration can be in either of the following state:
		/// <list type="bullet">
		/// <item>
		/// <see cref="ServiceState.Unenlisted"/>;
		/// </item>
		/// <item>
		/// <see cref="ServiceState.Enlisted"/>, or equivalently, <see cref="ServiceState.Stopped"/>;
		/// </item>
		/// <item>
		/// <see cref="ServiceState.Started"/>
		/// </item>
		/// </list>
		/// </remarks>
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		ServiceState State { get; set; }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Type Type { get; }
	}
}
