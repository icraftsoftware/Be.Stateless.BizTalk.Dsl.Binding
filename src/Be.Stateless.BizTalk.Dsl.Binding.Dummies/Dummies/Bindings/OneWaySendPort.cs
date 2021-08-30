﻿#region Copyright & License

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

using System;
using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.ContextProperties;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Schema;
using BTS;

namespace Be.Stateless.BizTalk.Dummies.Bindings
{
	internal class OneWaySendPort : SendPortBase<string>
	{
		public OneWaySendPort()
		{
			Name = nameof(OneWaySendPort);
			Description = "Some Useless One-Way Test Send Port";
			Filter = new(() => BtsProperties.MessageType == SchemaMetadata.For<soap_envelope_1__2.Envelope>().MessageType);
			Priority = Priority.Highest;
			OrderedDelivery = true;
			StopSendingOnOrderedDeliveryFailure = true;
			SendPipeline = new SendPipeline<PassThruTransmit>(
				pl => pl.PreAssembler<MicroPipelineComponent>(
					c => {
						c.Components = new IMicroComponent[] {
							new ContextBuilder {
								ExecutionTime = PluginExecutionTime.Deferred,
								BuilderType = typeof(DummyContextBuilder)
							}
						};
					}));
			Transport.Adapter = new DummyAdapter();
			Transport.Host = "Send Host Name";
			Transport.RetryPolicy = new() { Count = 30, Interval = TimeSpan.FromMinutes(60) };
			Transport.ServiceWindow = new() { StartTime = new(8, 0), StopTime = new(20, 0) };
		}
	}
}
