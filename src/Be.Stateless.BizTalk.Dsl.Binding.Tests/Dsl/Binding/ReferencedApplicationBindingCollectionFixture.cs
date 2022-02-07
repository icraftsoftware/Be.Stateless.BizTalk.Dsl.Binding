#region Copyright & License

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

using Moq;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl.Binding
{
	public class ReferencedApplicationBindingCollectionFixture
	{
		[Fact]
		public void AcceptsVisitorAndVisitReferencedApplications()
		{
			var applicationBindingMock = new Mock<ApplicationBindingBase<string>> { CallBase = true };

			var referencedApplicationBindingCollection = new ReferencedApplicationBindingCollection { applicationBindingMock.Object };

			var visitorMock = new Mock<IApplicationBindingVisitor>();
			((IVisitable<IApplicationBindingVisitor>) referencedApplicationBindingCollection).Accept(visitorMock.Object);

			visitorMock.Verify(m => m.VisitReferencedApplicationBinding(applicationBindingMock.Object), Times.Once);
		}
	}
}
