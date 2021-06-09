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
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Install;

namespace Be.Stateless.BizTalk.Dsl.Environment.Settings.Convention
{
	/// <summary>
	/// Elementary Microsoft BizTalk Server's Platform Settings that allows <see cref="ApplicationBinding"/> to be
	/// XML-serialized for a given target deployment environment and that provides an overriding point for <see
	/// cref="ApplicationBinding"/> written against these settings.
	/// </summary>
	/// <seealso cref="CompositeEnvironmentSettings{T,TI}"/>
	/// <seealso cref="DeploymentContext"/>
	/// <seealso cref="TargetEnvironment"/>
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API.")]
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
	public interface IPlatformEnvironmentSettings
	{
		/// <summary>
		/// Name of the Microsoft BizTalk Server isolated host in the target deployment environment.
		/// </summary>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string IsolatedHost { get; }

		/// <summary>
		/// Name of the SQL Server Database Instance hosting the management-related databases, e.g.<c>BizTalkMgmtDb</c>,
		/// <c>BizTalkFactoryMgmtDb</c>.
		/// </summary>
		/// <seealso cref="ManagementDatabaseServer"/>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string ManagementDatabaseInstance { get; }

		/// <summary>
		/// Name of the SQL Server Database Server hosting the management-related databases, e.g.<c>BizTalkMgmtDb</c>,
		/// <c>BizTalkFactoryMgmtDb</c>.
		/// </summary>
		/// <seealso cref="ManagementDatabaseInstance"/>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string ManagementDatabaseServer { get; }

		/// <summary>
		/// Name of the SQL Server Database Instance hosting the monitoring-related databases, e.g. <c>BAMPrimaryImport</c>.
		/// </summary>
		/// <seealso cref="MonitoringDatabaseServer"/>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string MonitoringDatabaseInstance { get; }

		/// <summary>
		/// Name of the SQL Server Database Server hosting the monitoring-related databases, e.g. <c>BAMPrimaryImport</c>.
		/// </summary>
		/// <seealso cref="MonitoringDatabaseInstance"/>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string MonitoringDatabaseServer { get; }

		/// <summary>
		/// Name of the SQL Server Database Instance hosting the processing-related databases, e.g. <c>BizTalkMsgBoxDb</c>,
		/// <c>BizTalkFactoryTransientStateDb</c>.
		/// </summary>
		/// <seealso cref="ProcessingDatabaseServer"/>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string ProcessingDatabaseInstance { get; }

		/// <summary>
		/// Name of the SQL Server Database Server hosting the processing-related databases, e.g. <c>BizTalkMsgBoxDb</c>,
		/// <c>BizTalkFactoryTransientStateDb</c>.
		/// </summary>
		/// <seealso cref="ProcessingDatabaseInstance"/>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string ProcessingDatabaseServer { get; }

		/// <summary>
		/// Name of the Microsoft BizTalk Server host that will host orchestrations in the target deployment environment.
		/// </summary>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string ProcessingHost { get; }

		/// <summary>
		/// Name of the Microsoft BizTalk Server host that will host receive locations in the target deployment environment.
		/// </summary>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string ReceivingHost { get; }

		/// <summary>
		/// Name of the Microsoft BizTalk Server host that will host send ports in the target deployment environment.
		/// </summary>
		/// <seealso cref="DeploymentContext"/>
		/// <seealso cref="TargetEnvironment"/>
		string TransmittingHost { get; }
	}
}
