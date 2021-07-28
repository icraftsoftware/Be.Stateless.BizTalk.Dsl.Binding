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

namespace Be.Stateless.BizTalk.Install.Command
{
	[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Public scaffolding interface for a.o. cmdlets.")]
	public interface ICommand
	{
		ICommand Execute(Action<string> logAppender);
	}

	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Scaffolding interface for a.o. cmdlets.")]
	public interface ICommand<in TA> : ICommand
		where TA : class
	{
		ICommand<TA> InitializeParameters(TA arguments);
	}
}
