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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
	[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Public DSL API.")]
	public interface IReceiveLocationCollection<TNamingConvention> : IFluentInterface, IEnumerable<IReceiveLocation<TNamingConvention>>
		where TNamingConvention : class
	{
		IReceiveLocationCollection<TNamingConvention> Add(IReceiveLocation<TNamingConvention> receiveLocation);

		IReceiveLocationCollection<TNamingConvention> Add(params IReceiveLocation<TNamingConvention>[] receiveLocations);

		IReceiveLocation<TNamingConvention> Find<T>() where T : IReceiveLocation<TNamingConvention>;
	}
}
