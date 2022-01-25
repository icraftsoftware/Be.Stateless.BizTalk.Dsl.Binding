﻿#region Copyright & License

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

extern alias ExplorerOM;
using ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	/// <summary>
	/// Scheduling properties for a receive location.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-scheduling-for-a-receive-location#configure-scheduling-for-a-receive-location">Configure scheduling for a receive location</seealso>
	public class OrdinalMonthlyServiceWindow : MonthlyServiceWindow
	{
		/// <summary>
		/// Ordinality of the <see cref="WeekDays"/> on which the <see cref="OrdinalMonthlyServiceWindow"/> recurrence will be
		/// activated.
		/// </summary>
		public OrdinalType Ordinality { get; set; }

		/// <summary>
		/// Days on which the <see cref="OrdinalMonthlyServiceWindow"/> recurrence will be activated.
		/// </summary>
		public BtsDayOfWeek WeekDays { get; set; }
	}
}
