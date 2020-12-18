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

using Be.Stateless.BizTalk.Dsl.Binding.Xml.Serialization.Extensions;
using FluentAssertions;
using Xunit;
using static Be.Stateless.Unit.DelegateFactory;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public class FileAdapterOutboundFixture
	{
		[Fact]
		public void CredentialsAreCompatibleWithNetworkFolder()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server\folder";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().NotThrow();
		}

		[Fact]
		public void CredentialsAreNotCompatibleWithLocalFolder()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"c:\file\drops";
					a.NetworkCredentials.UserName = "user";
					a.NetworkCredentials.Password = "pwd";
				});
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
		}

		[Fact]
		public void DestinationFolderIsRequired()
		{
			var ofa = new FileAdapter.Outbound(a => { });
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Outbound file adapter has no destination folder.");
		}

		[Fact]
		public void FileNameIsRequired()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.FileName = string.Empty;
				});
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Outbound file adapter has no destination file name.");
		}

		[Fact]
		public void SerializeToXml()
		{
			var ofa = new FileAdapter.Outbound(a => { a.DestinationFolder = @"c:\file\drops"; });
			var xml = ofa.GetAdapterBindingInfoSerializer().Serialize();
			xml.Should().Be(
				"<CustomProps>" +
				"<AllowCacheOnWrite vt=\"11\">0</AllowCacheOnWrite>" +
				"<CopyMode vt=\"19\">1</CopyMode>" +
				"<FileName vt=\"8\">%MessageID%.xml</FileName>" +
				"<UseTempFileOnWrite vt=\"11\">-1</UseTempFileOnWrite>" +
				"</CustomProps>");
		}

		[Fact]
		public void UseTempFileOnWriteAndAppendFileAreNotCompatible()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.Mode = FileAdapter.CopyMode.Append;
					a.UseTempFileOnWrite = false;
				});
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().NotThrow();
		}

		[Fact]
		public void UseTempFileOnWriteAndCreateNewFileAreCompatible()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.Mode = FileAdapter.CopyMode.CreateNew;
					a.UseTempFileOnWrite = true;
				});
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().NotThrow();
		}

		[Fact]
		public void UseTempFileOnWriteAndOverwriteFileAreNotCompatible()
		{
			var ofa = new FileAdapter.Outbound(
				a => {
					a.DestinationFolder = @"\\server";
					a.Mode = FileAdapter.CopyMode.Overwrite;
					a.UseTempFileOnWrite = true;
				});
			Action(() => ((ISupportValidation) ofa).Validate())
				.Should().Throw<BindingException>()
				.WithMessage("Outbound file adapter cannot use a temporary file when it is meant to append or overwrite an existing file.");
		}
	}
}
