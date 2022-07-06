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

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public DSL API.")]
	public interface IAdapterConfigMaxReceivedMessageSize
	{
		/// <summary>
		/// Specify the maximum size, in bytes, for a message (including headers) that can be received on the wire. The size of
		/// the messages is bounded by the amount of memory allocated for each message. You can use this property to limit
		/// exposure to denial of service (DoS) attacks.
		/// </summary>
		/// <remarks>
		/// It defaults to roughly <see cref="ushort.MaxValue">UInt16.MaxValue</see>, 65536.
		/// </remarks>
		int MaxReceivedMessageSize { get; set; }
	}
}
