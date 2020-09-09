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

using Microsoft.BizTalk.PipelineOM;

namespace Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions
{
	public static class PipelineExtensions
	{
		public static IDslSerializer GetPipelineBindingInfoSerializer(this Be.Stateless.BizTalk.Dsl.Pipeline.ReceivePipeline pipeline)
		{
			return new PipelineBindingInfoSerializer(pipeline);
		}

		public static IDslSerializer GetPipelineBindingInfoSerializer<T>(this ReceivePipeline<T> pipeline) where T : ReceivePipeline, new()
		{
			return new PipelineBindingInfoSerializer(pipeline);
		}

		public static IDslSerializer GetPipelineBindingInfoSerializer(this Be.Stateless.BizTalk.Dsl.Pipeline.SendPipeline pipeline)
		{
			return new PipelineBindingInfoSerializer(pipeline);
		}

		public static IDslSerializer GetPipelineBindingInfoSerializer<T>(this SendPipeline<T> pipeline) where T : SendPipeline, new()
		{
			return new PipelineBindingInfoSerializer(pipeline);
		}
	}
}
