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
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.Extensions
{
	public class AssemblyExtensionsFixture
	{
		[Fact]
		public void GetEnvironmentSettingsSingleton()
		{
			typeof(Dummies.Environment.Settings.DummyApp).Assembly.GetEnvironmentSettingsSingleton()
				.Should().BeSameAs(Dummies.Environment.Settings.DummyApp.Settings);
		}

		[Fact]
		public void GetEnvironmentSettingsType()
		{
			typeof(Dummies.Environment.Settings.DummyApp).Assembly.GetEnvironmentSettingsType(true).Should().BeSameAs(typeof(Dummies.Environment.Settings.DummyApp));
		}

		[Fact]
		public void GetEnvironmentSettingsTypeDoesNotThrowWhenNotFound()
		{
			Invoking(() => typeof(int).Assembly.GetEnvironmentSettingsType()).Should().NotThrow();
		}

		[Fact]
		public void GetEnvironmentSettingsTypeThrowsWhenNotFound()
		{
			var assembly = typeof(int).Assembly;
			Invoking(() => assembly.GetEnvironmentSettingsType(true))
				.Should().Throw<InvalidOperationException>()
				.WithMessage($"No {nameof(IEnvironmentSettings)}-derived type was found in assembly '{assembly.FullName}' located at '{assembly.Location}'.");
		}
	}
}
