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

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.BizTalk.Deployment.Binding;

namespace Be.Stateless.BizTalk.Dsl.Binding.Scheduling
{
	/// <summary>
	/// Service window properties for a receive location's transport's <see cref="Schedule"/> or send port's transport.
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-scheduling-for-a-receive-location#configure-scheduling-for-a-receive-location">Configure scheduling for a receive location</seealso>
	/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/how-to-configure-transport-advanced-options-for-a-send-port#configure-the-transport-options">Configure the transport options</seealso>
	public class ServiceWindow
	{
		static ServiceWindow()
		{
			var ti = new TransportInfo();
			None = new(ti.FromTime, ti.ToTime);
		}

		public static ServiceWindow None { get; }

		public ServiceWindow()
		{
			StartTime = None.StartTime;
			StopTime = None.StopTime;
		}

		private ServiceWindow(DateTime startTime, DateTime stopTime)
		{
			_startTime = startTime;
			_stopTime = stopTime;
		}

		public bool Enabled => _startTime != None._startTime || _stopTime != None._stopTime;

		public Time StartTime
		{
			get => _startTime;
			set => _startTime = BuildDateTime(value);
		}

		public Time StopTime
		{
			get => _stopTime;
			set => _stopTime = BuildDateTime(value);
		}

		[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
		private DateTime BuildDateTime(Time time)
		{
			var date = None._startTime.Date;
			var timeOfDay = ((DateTime) time).TimeOfDay;
			return new(date.Year, date.Month, date.Day, timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds);
		}

		private DateTime _startTime;
		private DateTime _stopTime;
	}
}
