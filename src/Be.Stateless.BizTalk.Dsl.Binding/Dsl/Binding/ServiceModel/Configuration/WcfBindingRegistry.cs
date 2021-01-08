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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Configuration;
using Be.Stateless.Extensions;

namespace Be.Stateless.BizTalk.Dsl.Binding.ServiceModel.Configuration
{
	public class WcfBindingRegistry : Dictionary<Type, string>
	{
		static WcfBindingRegistry()
		{
			_instance = new WcfBindingRegistry();

			var machineConfiguration = ConfigurationManager.OpenMachineConfiguration();
			var modelSectionGroup = ServiceModelSectionGroup.GetSectionGroup(machineConfiguration);
			foreach (var binding in modelSectionGroup!.Bindings.BindingCollections)
			{
				var baseType = binding.GetType().BaseType;
				if (baseType != null && baseType.IsSubclassOfGenericType(typeof(StandardBindingCollectionElement<,>)))
				{
					_instance.Add(baseType.GenericTypeArguments[1], binding.BindingName);
				}
			}
			_instance.Add(typeof(System.ServiceModel.Configuration.CustomBindingElement), "customBinding");
		}

		public static string GetBindingName(IBindingConfigurationElement bindingElement)
		{
			if (bindingElement == null) throw new ArgumentNullException(nameof(bindingElement));
			var bindingType = bindingElement is IBindingElementDecorator bindingElementDecorator
				? bindingElementDecorator.DecoratedBindingElement.GetType()
				: bindingElement.GetType();
			return _instance[bindingType];
		}

		private static readonly WcfBindingRegistry _instance;
	}
}
