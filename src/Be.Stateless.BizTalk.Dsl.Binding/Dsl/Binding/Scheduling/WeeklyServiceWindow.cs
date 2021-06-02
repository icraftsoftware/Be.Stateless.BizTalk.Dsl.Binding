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

extern alias ExplorerOM;
using System;
using ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	/// <summary>
	/// Scheduling properties for a receive location.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-scheduling-for-a-receive-location#configure-scheduling-for-a-receive-location">Configure scheduling for a receive location</seealso>
	public class WeeklyServiceWindow : RecurringServiceWindow
	{
		static WeeklyServiceWindow()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();
			Default = new WeeklyServiceWindow(rl.ScheduleRecurFrom, rl.ScheduleRecurInterval, rl.ScheduleDaysOfWeek);
		}

		private static WeeklyServiceWindow Default { get; }

		public WeeklyServiceWindow()
		{
			From = Default.From;
			Interval = Default.Interval;
			WeekDays = Default.WeekDays;
		}

		private WeeklyServiceWindow(DateTime from, int interval, BtsDayOfWeek weekDays)
		{
			From = from;
			Interval = interval;
			WeekDays = weekDays;
		}

		/// <summary>
		/// The <see cref="DateTime"/> on which the <see cref="WeeklyServiceWindow"/> recurrence starts.
		/// </summary>
		public DateTime From { get; set; }

		/// <summary>
		/// Number of weeks between <see cref="WeeklyServiceWindow"/> activations.
		/// </summary>
		/// <remarks>
		/// When configured a <see cref="WeeklyServiceWindow"/> will recur every <see cref="Interval"/> weeks; from <c>1</c> to
		/// <c>999</c>.
		/// </remarks>
		public int Interval
		{
			get => _interval;
			set
			{
				if (value is < 1 or > 999) throw new ArgumentOutOfRangeException(nameof(value), "Interval value must be between 1 and 999.");
				_interval = value;
			}
		}

		/// <summary>
		/// Days on which the <see cref="WeeklyServiceWindow"/> recurrence will be activated.
		/// </summary>
		public BtsDayOfWeek WeekDays { get; set; }

		private int _interval;
	}
}
