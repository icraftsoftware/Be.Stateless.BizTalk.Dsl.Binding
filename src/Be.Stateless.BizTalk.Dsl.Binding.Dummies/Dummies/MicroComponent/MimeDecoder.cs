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

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Be.Stateless.BizTalk.MicroComponent;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Be.Stateless.BizTalk.Dummies.MicroComponent
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Test dummy.")]
	internal class MimeDecoder : IMicroComponent
	{
		public MimeDecoder()
		{
			_wrappedComponent = new MIME_SMIME_Decoder {
				AllowNonMIMEMessage = false,
				BodyPartContentType = MediaTypeNames.Text.Xml,
				BodyPartIndex = 0,
				ValidateCRL = false
			};
		}

		#region IMicroComponent Members

		public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage message)
		{
			return _wrappedComponent.Execute(pipelineContext, message);
		}

		#endregion

		private readonly MIME_SMIME_Decoder _wrappedComponent;
	}
}
