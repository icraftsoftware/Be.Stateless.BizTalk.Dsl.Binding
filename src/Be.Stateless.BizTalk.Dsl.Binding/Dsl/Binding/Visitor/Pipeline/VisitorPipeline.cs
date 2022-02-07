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

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor.Pipeline
{
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "For mocking purposes.")]
	internal class VisitorPipeline<TNamingConvention>
		where TNamingConvention : class
	{
		internal VisitorPipeline(IApplicationBinding<TNamingConvention> applicationBinding)
		{
			ApplicationBinding = applicationBinding ?? throw new ArgumentNullException(nameof(applicationBinding));
			Stage = new PreprocessingStage<TNamingConvention>(this);
		}

		internal IApplicationBinding<TNamingConvention> ApplicationBinding { get; }

		internal VisitorPipelineStage<TNamingConvention> Stage { get; set; }

		internal virtual void Accept<T>(T visitor) where T : IApplicationBindingVisitor
		{
			Stage.Accept(visitor);
		}
	}
}
