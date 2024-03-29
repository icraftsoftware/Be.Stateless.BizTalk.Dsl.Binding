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

namespace Be.Stateless.BizTalk.Dsl.Binding.Convention
{
	public class ReceiveLocationNamingConvention<TNamingConvention> :
		IMessageSubjectConvention<TNamingConvention>,
		IMessageFormatConvention<TNamingConvention>
		where TNamingConvention : new()
	{
		public ReceiveLocationNamingConvention()
		{
			_convention = new();
		}

		#region IMessageFormatConvention<TNamingConvention> Members

		MessageFormatConvention<TNamingConvention> IMessageFormatConvention<TNamingConvention>.FormattedAs => new(_convention);

		#endregion

		#region IMessageSubjectConvention<TNamingConvention> Members

		public IMessageFormatConvention<TNamingConvention> About<T>(T subject)
		{
			((IMessageSubjectMemento<T>) _convention).Subject = subject;
			return this;
		}

		#endregion

		private readonly TNamingConvention _convention;
	}
}
