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
using System.Diagnostics.CodeAnalysis;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter.Extensions;
using Be.Stateless.Extensions;
using Be.Stateless.IO;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Adapter
{
	public abstract partial class FileAdapter
	{
		#region Nested Type: Inbound

		/// <summary>
		/// The File adapter transfers files into and out of Microsoft BizTalk Server.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use the File receive adapter to read messages from files, and submit them to the server. The receive adapter reads
		/// the file, and creates a BizTalk Message object, so that BizTalk Server can process the message. While reading from
		/// the file, the adapter locks the file to ensure that no modifications can be made to the file content
		/// </para>
		/// <para>
		/// The File receive adapter reads the messages from files on local file systems or on network shares. When the specified
		/// location on a network share is unavailable due to network problems, the receive adapter retries the read operation
		/// (the number of retries is configurable in the BizTalk Server Administration console). After the message has been read
		/// and successfully accepted by the BizTalk Messaging Engine, the receive adapter deletes the file from the file system
		/// or network share. If the message was read but the pipeline did not successfully process the message, the adapter puts
		/// the message in the suspended queue and then deletes the file from the file system or network share. If the File
		/// receive adapter cannot submit or suspend the message to the MessageBox database, it does not delete the original file
		/// from the file system or network share.
		/// </para>
		/// <para>
		/// You can also configure the File receive adapter to rename files when processing them. You should rename files to
		/// ensure that the receive adapter does not generate duplicate messages if the receive location is shut down and
		/// restarted. This is a configurable option for File receive locations. By default, renaming is disabled. When renaming
		/// is enabled, the File receive adapter appends the extension .BTS-WIP to the file. The receive adapter then reads the
		/// messages from the renamed file in the receive location and submits it to the server. After the receive adapter has
		/// successfully submitted a file, the receive adapter deletes the renamed file from the file system or network share. If
		/// a message has been read but failed processing in the pipeline, the receive adapter places the message in the
		/// MessageBox database suspended queue, and deletes the renamed file from the network share.
		/// </para>
		/// <para>
		/// If the File receive adapter successfully reads the message but did not successfully store the message in the
		/// MessageBox database, the renamed file reverts to its original name, without the .BTS-WIP extension. Note that the
		/// receive adapter does not read files with the extension .BTS-WIP if the renaming option is turned on.
		/// </para>
		/// <para>
		/// <b>Using file change notifications and polling</b>
		/// </para>
		/// <para>
		/// The File receive adapter relies on Windows File Change Notifications to determine when to pick up a file from the
		/// specified directory or share. If the File receive adapter receives a Windows File Change Notification before the file
		/// has been completely written to the specified directory or share then the file will be locked and the File receive
		/// adapter will not retrieve the file. In this scenario, the File receive adapter will actively poll the specified
		/// directory or share at the Polling interval (ms) specified on the Advanced settings dialog box available when
		/// configuring a File receive location. When the File receive adapter polls a directory or share it retrieves unlocked
		/// files from the share and submits the files to the MessageBox database.
		/// </para>
		/// <para>
		/// <b>File Receive Adapter Batching Support</b>
		/// </para>
		/// <para>
		/// The File receive adapter submits messages to the server in batches. The File receive adapter starts by building a
		/// single batch per receive location by collecting all the readable files available in the receive location. Batches are
		/// submitted to the MessageBox database by the receive adapter when all the available files have been collected or when
		/// the amount of files collected exceeds the maximum batch size.
		/// </para>
		/// <para>
		/// After all the messages within the batch have been successfully read and submitted into the MessageBox database, the
		/// File receive adapter deletes the corresponding files from the receive location. If some of the messages within the
		/// batch failed processing, the File receive adapter suspends them and deletes the corresponding files from the receive
		/// location. If some or all of the messages fail to be stored in the MessageBox database, the entire batch operation is
		/// rolled back and all corresponding files are left unchanged in the receive location.
		/// </para>
		/// </remarks>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/file-adapter">File Adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/file-adapter#file-receive-adapter">File receive adapter</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/file-adapter#using-file-change-notifications-and-polling">Using file change notifications and polling</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/file-adapter#file-receive-adapter-batching-support">File Receive Adapter Batching Support</seealso>
		/// <seealso href="https://docs.microsoft.com/en-us/biztalk/core/configure-the-file-adapter">Configure the File adapter</seealso>
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public DSL API.")]
		[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "Public DSL API.")]
		public class Inbound : FileAdapter, IInboundAdapter
		{
			private Inbound()
			{
				FileMask = "*.xml";
				BatchMessagesCount = 20;
				BatchSize = 102400;
				RenameReceivedFiles = true;
				PollingInterval = TimeSpan.FromMinutes(1);
				FileRemovingSettings = new FileRemovingSettings();
				RetryCountOnNetworkFailure = 5;
				RetryIntervalOnNetworkFailure = TimeSpan.FromMinutes(5);
			}

			public Inbound(Action<Inbound> adapterConfigurator) : this()
			{
				if (adapterConfigurator == null) throw new ArgumentNullException(nameof(adapterConfigurator));
				adapterConfigurator(this);
			}

			#region Base Class Member Overrides

			protected override string GetAddress()
			{
				return System.IO.Path.Combine(ReceiveFolder, FileMask);
			}

			protected override void Validate()
			{
				if (ReceiveFolder.IsNullOrEmpty()) throw new BindingException("Inbound file adapter has no source folder.");
				if (FileMask.IsNullOrEmpty()) throw new BindingException("Inbound file adapter has no source file mask.");
				if (!Path.IsNetworkPath(ReceiveFolder) && !NetworkCredentials.UserName.IsNullOrEmpty())
					throw new BindingException("Alternate credentials to access the file folder cannot be supplied while accessing local drive or a mapped network drive.");
			}

			#endregion

			#region Base Class Member Overrides

			protected override void Save(IPropertyBag propertyBag)
			{
				base.Save(propertyBag);
				propertyBag.WriteAdapterCustomProperty("BatchSize", Convert.ToUInt32(BatchMessagesCount));
				propertyBag.WriteAdapterCustomProperty("BatchSizeInBytes", BatchSize);
				propertyBag.WriteAdapterCustomProperty(nameof(FileMask), FileMask);
				propertyBag.WriteAdapterCustomProperty("FileNetFailRetryCount", RetryCountOnNetworkFailure);
				propertyBag.WriteAdapterCustomProperty("FileNetFailRetryInt", Convert.ToUInt32(RetryIntervalOnNetworkFailure.TotalMinutes));
				propertyBag.WriteAdapterCustomProperty(nameof(PollingInterval), Convert.ToUInt32(PollingInterval.TotalMilliseconds));
				propertyBag.WriteAdapterCustomProperty("RemoveReceivedFileDelay", Convert.ToUInt32(FileRemovingSettings.RetryInterval.TotalMilliseconds));
				propertyBag.WriteAdapterCustomProperty("RemoveReceivedFileMaxInterval", Convert.ToUInt32(FileRemovingSettings.MaxRetryInterval.TotalMilliseconds));
				propertyBag.WriteAdapterCustomProperty("RemoveReceivedFileRetryCount", FileRemovingSettings.RetryCount);
				propertyBag.WriteAdapterCustomProperty(nameof(RenameReceivedFiles), RenameReceivedFiles);
			}

			#endregion

			/// <summary>
			/// Source file mask.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <description>
			/// <a href="https://docs.microsoft.com/en-us/biztalk/core/restrictions-when-configuring-the-file-adapter#file-mask-and-file-name-gotchas">File mask and file name gotchas</a>
			/// </description>
			/// <description>
			/// <a href="https://docs.microsoft.com/en-us/biztalk/core/restrictions-when-configuring-the-file-adapter#using-macros-in-file-names">Using macros in file names</a>
			/// </description>
			/// </item>
			/// </list>
			/// </remarks>
			public string FileMask { get; set; }

			/// <summary>
			/// Source folder.
			/// </summary>
			/// <remarks>
			/// <list type="bullet">
			/// <item>
			/// <description>
			/// <a href="https://docs.microsoft.com/en-us/biztalk/core/restrictions-when-configuring-the-file-adapter#receive-folder-and-destination-location-properties-gotchas">Receive folder and destination location properties gotchas</a>
			/// </description>
			/// </item>
			/// </list>
			/// </remarks>
			public string ReceiveFolder { get; set; }

			#region Advanced Settings

			/// <summary>
			/// Renames files while reading.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Specify whether to rename files before picking them up for processing; see also <a
			/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/file-transport-properties-dialog-box-receive-advanced-settings-dialog-box">File
			/// Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box</a>.
			/// </para>
			/// </remarks>
			public bool RenameReceivedFiles { get; set; }

			/// <summary>
			/// Receive location polling interval.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Specify the interval frequency that the file adapter will use to poll the specified location for new files; see
			/// also <a
			/// href="https://docs.microsoft.com/en-us/biztalk/core/technical-reference/file-transport-properties-dialog-box-receive-advanced-settings-dialog-box">File
			/// Transport Properties Dialog Box, Receive, Advanced Settings Dialog Box</a>.
			/// </para>
			/// </remarks>
			public TimeSpan PollingInterval { get; set; }

			public FileRemovingSettings FileRemovingSettings { get; set; }

			#endregion

			#region Batching

			/// <summary>
			/// Specify the maximum number of messages to be submitted in a batch.
			/// </summary>
			public byte BatchMessagesCount { get; set; }

			/// <summary>
			/// Specify the maximum total bytes for a batch.
			/// </summary>
			public uint BatchSize { get; set; }

			#endregion

			#region Network Failure

			/// <summary>
			/// Specify the number of attempts to access the receive location on a network share if it is temporarily unavailable.
			/// </summary>
			public uint RetryCountOnNetworkFailure { get; set; }

			/// <summary>
			/// Specify the retry interval time (in minutes) between attempts to access the receive location on the network share
			/// if it is temporarily unavailable.
			/// </summary>
			public TimeSpan RetryIntervalOnNetworkFailure { get; set; }

			#endregion
		}

		#endregion
	}
}
