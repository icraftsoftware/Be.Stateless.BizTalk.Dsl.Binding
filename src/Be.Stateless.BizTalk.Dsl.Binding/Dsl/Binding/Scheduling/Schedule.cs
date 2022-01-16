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
	public class Schedule
	{
		static Schedule()
		{
			var rl = new Microsoft.BizTalk.Deployment.Binding.ReceiveLocation();
			// force TimeZone and AutomaticallyAdjustForDaylightSavingTime as default rl.ScheduleTimeZone and rl.ScheduleAutoAdjustToDaylightSaving
			// values do not match the values exported in the bindings for a Schedule that has not been configured
			None = new(TimeZoneInfo.Utc.Id, true, rl.StartDate, rl.EndDate, RecurringServiceWindow.None);
		}

		public static Schedule None { get; }

		public Schedule()
		{
			TimeZone = None.TimeZone;
			AutomaticallyAdjustForDaylightSavingTime = None.AutomaticallyAdjustForDaylightSavingTime;
			StartDate = None.StartDate;
			StopDate = None.StopDate;
			ServiceWindow = RecurringServiceWindow.None;
		}

		private Schedule(string timeZone, bool automaticallyAdjustForDaylightSavingTime, DateTime startDate, DateTime stopDate, RecurringServiceWindow serviceWindow)
		{
			TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
			AutomaticallyAdjustForDaylightSavingTime = automaticallyAdjustForDaylightSavingTime;
			StartDate = startDate;
			StopDate = stopDate;
			ServiceWindow = serviceWindow;
		}

		/// <summary>
		/// The <see cref="Schedule"/> automatically adjusts to the daylight saving time of the time zone you chose.
		/// </summary>
		/// <remarks>
		/// This option has no impact on the schedule if the time zone you chose doesn't have a daylight savings time, or
		/// daylight savings time isn't observed
		/// </remarks>
		public bool AutomaticallyAdjustForDaylightSavingTime { get; set; }

		public RecurrenceType RecurrenceType => ServiceWindow switch {
			MonthlyServiceWindow => RecurrenceType.Month,
			WeeklyServiceWindow => RecurrenceType.Week,
			DailyServiceWindow => RecurrenceType.Day,
			_ => RecurrenceType.None
		};

		/// <summary>
		/// Any <see cref="RecurringServiceWindow"/>-derived service window restricts the <see
		/// cref="ReceiveLocationBase{TNamingConvention}"/> to work during certain hours of the day possibly at a recurring
		/// period of time.
		/// </summary>
		/// <seealso cref="DailyServiceWindow"/>
		/// <seealso cref="WeeklyServiceWindow"/>
		/// <seealso cref="CalendricalMonthlyServiceWindow"/>
		/// <seealso cref="OrdinalMonthlyServiceWindow"/>
		public RecurringServiceWindow ServiceWindow { get; set; }

		public DateTime StartDate { get; set; }

		public bool StartDateEnabled => StartDate != None.StartDate;

		public DateTime StopDate { get; set; }

		public bool StopDateEnabled => StopDate != None.StopDate;

		/// <summary>
		/// The <see cref="TimeZone"/> determines all the date time values of the <see cref="Schedule"/> and its <see
		/// cref="ServiceWindow"/> or any of its derived <see cref="ServiceWindow"/>.
		/// </summary>
		/// <seealso cref="DailyServiceWindow"/>
		/// <seealso cref="WeeklyServiceWindow"/>
		/// <seealso cref="CalendricalMonthlyServiceWindow"/>
		/// <seealso cref="OrdinalMonthlyServiceWindow"/>
		public TimeZoneInfo TimeZone { get; set; }
	}
}
