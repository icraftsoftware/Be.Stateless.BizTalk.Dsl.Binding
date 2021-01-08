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

extern alias ExplorerOM;
using System;
using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Be.Stateless.BizTalk.Explorer;
using log4net;
using OrchestrationStatus = ExplorerOM::Microsoft.BizTalk.ExplorerOM.OrchestrationStatus;
using PortStatus = ExplorerOM::Microsoft.BizTalk.ExplorerOM.PortStatus;

namespace Be.Stateless.BizTalk.Dsl.Binding.Visitor
{
	/// <summary>
	/// <see cref="IApplicationBindingVisitor"/> implementation that manages BizTalk Server services' state.
	/// </summary>
	/// <remarks>
	/// <see cref="IApplicationBindingVisitor"/> implementation that either
	/// <list type="bullet">
	/// <item>
	/// Enable or disable a receive location according to its DSL-based binding;
	/// </item>
	/// <item>
	/// Start, stop, enlist or unenlist a send port according to its DSL-based binding;
	/// </item>
	/// <item>
	/// Start, stop, enlist or unenlist an orchestration according to its DSL-based binding.
	/// </item>
	/// </list>
	/// </remarks>
	public sealed class BizTalkServiceConfiguratorVisitor : MainApplicationBindingVisitor, IDisposable
	{
		#region IDisposable Members

		public void Dispose()
		{
			_application?.Dispose();
		}

		#endregion

		#region Base Class Member Overrides

		protected internal override void VisitApplicationBinding<TNamingConvention>(IApplicationBinding<TNamingConvention> applicationBinding)
			where TNamingConvention : class
		{
			if (applicationBinding == null) throw new ArgumentNullException(nameof(applicationBinding));
			var name = ((ISupportNamingConvention) applicationBinding).Name;
			_application = BizTalkServerGroup.Applications[name];
		}

		protected internal override void VisitOrchestration(IOrchestrationBinding orchestrationBinding)
		{
			if (orchestrationBinding == null) throw new ArgumentNullException(nameof(orchestrationBinding));
			var name = orchestrationBinding.Type.FullName;
			var orchestration = _application.Orchestrations[name];
			if (_logger.IsDebugEnabled)
				_logger.Debug(
					orchestrationBinding.State switch {
						ServiceState.Unenlisted => $"Unenlisting orchestration '{name}'.",
						ServiceState.Enlisted => $"Enlisting or stopping orchestration '{name}'.",
						_ => $"Starting orchestration '{name}'."
					});
			orchestration.Status = (OrchestrationStatus) orchestrationBinding.State;
		}

		protected internal override void VisitReceiveLocation<TNamingConvention>(IReceiveLocation<TNamingConvention> receiveLocation)
			where TNamingConvention : class
		{
			if (receiveLocation == null) throw new ArgumentNullException(nameof(receiveLocation));
			var name = ((ISupportNamingConvention) receiveLocation).Name;
			var rl = _receivePort.ReceiveLocations[name];
			if (_logger.IsDebugEnabled) _logger.Debug($"{(receiveLocation.Enabled ? "Enabling" : "Disabling")} receive location '{name}'.");
			rl.Enabled = receiveLocation.Enabled;
		}

		protected internal override void VisitReceivePort<TNamingConvention>(IReceivePort<TNamingConvention> receivePort)
			where TNamingConvention : class
		{
			if (receivePort == null) throw new ArgumentNullException(nameof(receivePort));
			var name = ((ISupportNamingConvention) receivePort).Name;
			_receivePort = _application.ReceivePorts[name];
		}

		protected internal override void VisitSendPort<TNamingConvention>(ISendPort<TNamingConvention> sendPort)
			where TNamingConvention : class
		{
			if (sendPort == null) throw new ArgumentNullException(nameof(sendPort));
			var name = ((ISupportNamingConvention) sendPort).Name;
			var sp = _application.SendPorts[name];
			if (_logger.IsDebugEnabled)
				_logger.Debug(
					sendPort.State switch {
						ServiceState.Indefinite => $"Leaving send port '{name}''s state as it is.",
						ServiceState.Unenlisted => $"Unenlisting send port '{name}'.",
						ServiceState.Enlisted => $"Enlisting or stopping send port '{name}'.",
						_ => $"Starting send port '{name}'."
					});
			if (sendPort.State != ServiceState.Indefinite) sp.Status = (PortStatus) sendPort.State;
		}

		#endregion

		public void Commit()
		{
			_application.ApplyChanges();
		}

		private static readonly ILog _logger = LogManager.GetLogger(typeof(BizTalkServiceConfiguratorVisitor));
		private Application _application;
		private Explorer.ReceivePort _receivePort;
	}
}
