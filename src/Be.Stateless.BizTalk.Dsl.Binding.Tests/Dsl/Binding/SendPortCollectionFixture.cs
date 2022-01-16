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

using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class SendPortCollectionFixture
	{
		[Fact]
		public void SupportsAndPropagatesISupportValidation()
		{
			var sendPortMock = new Mock<ISendPort<string>>();
			sendPortMock.As<ISupportValidation>();

			var sendPortCollectionMock = new Mock<SendPortCollection<string>>(new Mock<IApplicationBinding<string>>().Object) { CallBase = true };
			sendPortCollectionMock.As<ISupportValidation>();
			sendPortCollectionMock.Object.Add(sendPortMock.Object);

			((ISupportValidation) sendPortCollectionMock.Object).Validate();

			sendPortMock.As<ISupportValidation>().Verify(m => m.Validate(), Times.Once);
		}
	}
}
