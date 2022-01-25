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
using Be.Stateless.BizTalk.Dsl.Pipeline;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public interface IReceiveLocation : IFluentInterface { }

	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public DSL API.")]
	public interface IReceiveLocation<TNamingConvention> : IReceiveLocation, IObjectBinding<TNamingConvention>
		where TNamingConvention : class
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
		bool Enabled { get; set; }

		ReceivePipeline ReceivePipeline { get; set; }

		IReceivePort<TNamingConvention> ReceivePort { get; }

		SendPipeline SendPipeline { get; set; }

		ReceiveLocationTransport<TNamingConvention> Transport { get; }
	}
}
