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

using System;
using System.Globalization;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FileAdapter
	{
		#region Nested Type: Outbound

		/// <summary>
		/// The File adapter transfers files into and out of Microsoft BizTalk Server.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The File send adapter transmits messages from the MessageBox database to a specified destination address (URL). You
		/// define the URL, which is a file path and file name, by using wildcard characters related to the message context
		/// properties. The File send adapter resolves the wildcard characters to the actual file name before writing the message
		/// to the file.
		/// </para>
		/// <para>
		/// </para>
		/// When writing a message to a file, the File send adapter gets the message content from the body part of the BizTalk
		/// Message object. The File send adapter ignores other message parts in the BizTalk Message object. After the File
		/// adapter writes the message to a file, it deletes the message from the MessageBox database. The File adapter writes
		/// files to the file system either directly or by using the file system cache, which can improve performance,
		/// particularly for large files.
		/// <para>
		/// <b>File Send Adapter Batching Support</b>
		/// </para>
		/// <para>
		/// The File send adapter gets batches of messages from the MessageBox database and writes them to files in destination
		/// locations on the file system or the network share. The size of File send adapter batches is not configurable and is
		/// preset to 20. If BizTalk Server fails to write some of the messages within a batch to files, the system resubmits
		/// those messages to the MessageBox database for retry processing. You can configure the retry interval and retry count
		/// by using the BizTalk Server Administration console.
		/// </para>
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/file-adapter">File Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/file-adapter#file-send-adapter">File Send Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/configure-the-file-adapter">Configure the File adapter</seealso>
		public class Outbound : FileAdapter, IOutboundAdapter
		{
			private Outbound()
			{
				FileName = "%MessageID%.xml";
				Mode = CopyMode.CreateNew;
				UseTempFileOnWrite = true;
			}

			public Outbound(Action<Outbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return System.IO.Path.Combine(DestinationFolder, FileName);
			}

			protected override void Validate()
			{
				if (DestinationFolder.IsNullOrEmpty()) throw new BindingException("Outbound file adapter has no destination folder.");
				if (FileName.IsNullOrEmpty()) throw new BindingException("Outbound file adapter has no destination file name.");
				if (UseTempFileOnWrite && Mode != CopyMode.CreateNew)
					throw new BindingException("Outbound file adapter cannot use a temporary file when it is meant to append or overwrite an existing file.");
				if (!Path.IsNetworkPath(DestinationFolder) && !NetworkCredentials.UserName.IsNullOrEmpty())
					throw new BindingException("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				base.Save(propertyBag);
				propertyBag.WriteAdapterCustomProperty(nameof(AllowCacheOnWrite), AllowCacheOnWrite);
				propertyBag.WriteAdapterCustomProperty("CopyMode", Convert.ToUInt32(Mode, NumberFormatInfo.InvariantInfo));
				propertyBag.WriteAdapterCustomProperty(nameof(FileName), FileName);
				propertyBag.WriteAdapterCustomProperty(nameof(UseTempFileOnWrite), UseTempFileOnWrite);
			}

			#endregion

			/// <summary>
			/// Allow cache on write.
			/// </summary>
			public bool AllowCacheOnWrite { get; set; }

			/// <summary>
			/// Destination folder.
			/// </summary>
			public string DestinationFolder { get; set; }

			/// <summary>
			/// Destination file name.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <description>
			/// <a href="https://docs.microsoft.com/en-us/biztalk/core/restrictions-when-configuring-the-file-adapter#file-mask-and-file-name-gotchas">File mask and file name gotchas</a>
			/// </description>
			/// </item>
			/// </list>
			/// </remarks>
			public string FileName { get; set; }

			/// <summary>
			/// File content writing mode.
			/// </summary>
			public CopyMode Mode { get; set; }

			/// <summary>
			/// Use temporary file while writing.
			/// </summary>
			/// <remarks>
			/// Can be set to <c>true</c> only when the <see cref="Mode"/> is set to <see cref="FileAdapter.CopyMode.CreateNew"/>.
			/// </remarks>
			public bool UseTempFileOnWrite { get; set; }
		}

		#endregion
	}
}
