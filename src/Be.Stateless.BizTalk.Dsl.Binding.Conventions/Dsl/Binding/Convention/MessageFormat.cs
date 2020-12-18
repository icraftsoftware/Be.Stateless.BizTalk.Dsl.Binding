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

using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Convention Public API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Convention Public API.")]
	public class MessageFormat<TNamingConvention>
	{
		internal MessageFormat(TNamingConvention convention)
		{
			_convention = convention;
		}

		public TNamingConvention Csv => Custom(nameof(Csv).ToUpperInvariant());

		public TNamingConvention Edi => Custom(nameof(Edi).ToUpperInvariant());

		public TNamingConvention FF => Custom(nameof(FF).ToUpperInvariant());

		public TNamingConvention Idoc => Custom(nameof(Idoc).ToUpperInvariant());

		public TNamingConvention Irrelevant => None;

		public TNamingConvention Json => Custom(nameof(Json).ToUpperInvariant());

		public TNamingConvention Mime => Custom(nameof(Mime).ToUpperInvariant());

		public TNamingConvention None => Custom(string.Empty);

		public TNamingConvention Rfc => Custom(nameof(Rfc).ToUpperInvariant());

		public TNamingConvention Xml => Custom(nameof(Xml).ToUpperInvariant());

		public TNamingConvention Custom<T>(T messageFormat)
		{
			((IMessageFormatMemento<T>) _convention).MessageFormat = messageFormat;
			return _convention;
		}

		private readonly TNamingConvention _convention;
	}
}
