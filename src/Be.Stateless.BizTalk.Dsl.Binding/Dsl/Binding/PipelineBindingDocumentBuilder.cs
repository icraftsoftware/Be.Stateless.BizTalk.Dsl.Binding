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

using Be.Stateless.BizTalk.Dsl.Binding.Interop;
using Be.Stateless.BizTalk.Dsl.Pipeline;
using Microsoft.BizTalk.PipelineEditor.PipelineFile;
using StageDocument = Microsoft.BizTalk.PipelineEditor.PipelineFile.Stage;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	/// <summary>
	/// Translates a <see cref="Pipeline"/>-derived pipeline into a structure of objects that support binding-compliant
	/// serialization.
	/// </summary>
	internal class PipelineBindingDocumentBuilder : PipelineVisitor
	{
		#region Base Class Member Overrides

		protected override ComponentInfo CreateComponentInfo(IPipelineComponentDescriptor componentDescriptor)
		{
			// save overridden component's default property values as set by the pipeline *binding*
			componentDescriptor.Save(componentDescriptor.Properties, false, false);
			var componentBinding = new ComponentBinding {
				QualifiedNameOrClassId = componentDescriptor.FullName,
				Properties = (PropertyBag) componentDescriptor.Properties
			};
			return componentBinding;
		}

		protected override Document CreatePipelineDocument<T>(Pipeline<T> pipeline)
		{
			return new();
		}

		protected override StageDocument CreateStageDocument(IStage stage)
		{
			return new() { CategoryId = stage.Category.Id };
		}

		#endregion
	}
}
