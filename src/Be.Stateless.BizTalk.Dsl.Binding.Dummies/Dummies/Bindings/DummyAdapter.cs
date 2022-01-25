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
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dummies.Bindings
{
	internal class DummyAdapter : AdapterBase, IInboundAdapter, IOutboundAdapter
	{
		public DummyAdapter() : base(new() { Name = "Test Dummy" }) { }

		#region IInboundAdapter Members

		public string Address => @"c:\file\drops\*.xml";

		#endregion

		#region Base Class Member Overrides

		protected override string GetAddress()
		{
			throw new NotSupportedException();
		}

		protected override void Save(IPropertyBag propertyBag) { }

		protected override void Validate() { }

		#endregion
	}
}
