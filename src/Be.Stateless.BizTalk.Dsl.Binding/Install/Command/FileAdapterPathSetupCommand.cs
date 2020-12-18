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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;

namespace Be.Stateless.BizTalk.Install.Command
{
	public class FileAdapterPathSetupCommand : ApplicationBindingBasedCommand
	{
		#region Base Class Member Overrides

		protected override void ExecuteCore(Action<string> logAppender)
		{
			if (Users == null || !Users.Any()) throw new InvalidOperationException($"{nameof(Users)} has not been set.");
			ApplicationBinding.Accept(CurrentApplicationBindingVisitor.Create(FileAdapterFolderSetUpVisitor.Create(Users)));
		}

		#endregion

		[SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
		public string[] Users { get; set; }
	}
}
