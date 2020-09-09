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
using System.IO;
using log4net;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> implementation that tears down file adapters' physical paths.
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public DSL API.")]
	public class FileAdapterFolderTearDownVisitor : FileAdapterFolderVisitorBase
	{
		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
		public static IApplicationBindingVisitor Create(bool recurse)
		{
			return new FileAdapterFolderTearDownVisitor(recurse);
		}

		private FileAdapterFolderTearDownVisitor(bool recurse)
		{
			_recurse = recurse;
		}

		#region Base Class Member Overrides

		[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
		protected override void VisitDirectory(string path)
		{
			_logger.InfoFormat("Deleting directory '{0}'.", path);
			try
			{
				Directory.Delete(path, _recurse);
				_logger.InfoFormat("Deleted directory '{0}'.", path);
			}
			catch (Exception exception)
			{
				_logger.WarnFormat($"Could not delete directory '{path}'.", exception);
			}
		}

		#endregion

		private static readonly ILog _logger = LogManager.GetLogger(typeof(FileAdapterFolderTearDownVisitor));
		private readonly bool _recurse;
	}
}
