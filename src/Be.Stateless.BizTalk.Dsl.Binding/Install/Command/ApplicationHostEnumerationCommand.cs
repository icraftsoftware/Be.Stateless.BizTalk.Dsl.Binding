﻿#region Copyright & License

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
using System.Collections.Generic;
using System.Linq;
using Be.Stateless.BizTalk.Dsl;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Visitor;

namespace Be.Stateless.BizTalk.Install.Command
{
	public class ApplicationHostEnumerationCommand<T> : ApplicationBindingCommand<T>, IApplicationHostEnumerationCommand
		where T : class, IVisitable<IApplicationBindingVisitor>, new()
	{
		#region IApplicationHostEnumerationCommand Members

		public IEnumerable<string> Hosts { get; private set; }

		#endregion

		#region Base Class Member Overrides

		protected override void ExecuteCore(Action<string> logAppender)
		{
			var visitor = new HostEnumerationVisitor();
			ApplicationBinding.Accept(visitor);
			Hosts = visitor.ToArray();
		}

		#endregion
	}
}