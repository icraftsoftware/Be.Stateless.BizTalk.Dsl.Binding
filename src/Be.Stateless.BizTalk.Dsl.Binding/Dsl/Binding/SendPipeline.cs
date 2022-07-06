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

using System;
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Pipeline;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global", Justification = "Public DSL API.")]
	public class SendPipeline<T> : SendPipeline, ISupportEnvironmentOverride, ITypeDescriptor
		where T : Microsoft.BizTalk.PipelineOM.SendPipeline, new()
	{
		public static SendPipeline<T> Configure(Action<SendPipeline<T>> sendPipelineConfigurator)
		{
			return new(sendPipelineConfigurator);
		}

		public SendPipeline()
		{
			PipelineObjectModel<T>.CloneStageDefinitions(Stages);
		}

		public SendPipeline(Action<SendPipeline<T>> sendPipelineConfigurator) : this()
		{
			if (sendPipelineConfigurator == null) throw new ArgumentNullException(nameof(sendPipelineConfigurator));
			sendPipelineConfigurator(this);
		}

		#region ISupportEnvironmentOverride Members

		void ISupportEnvironmentOverride.ApplyEnvironmentOverrides(string environment)
		{
			ApplyEnvironmentOverrides(environment);
		}

		#endregion

		#region ITypeDescriptor Members

		string ITypeDescriptor.FullName => typeof(T).FullName;

		string ITypeDescriptor.AssemblyQualifiedName => typeof(T).AssemblyQualifiedName;

		#endregion

		[SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Public DSL API.")]
		protected virtual void ApplyEnvironmentOverrides(string environment) { }
	}
}
