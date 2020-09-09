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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Be.Stateless.Extensions;
using Microsoft.BizTalk.Component.Interop;

namespace Be.Stateless.BizTalk.Dsl.Binding.Interop
{
	/// <summary>
	/// Provides an object with a property bag in which the object can save its properties persistently.
	/// </summary>
	/// <seealso href="Microsoft.BizTalk.Internal.BTMPropertyBag" />
	public class PropertyBag : IPropertyBag, Microsoft.BizTalk.ExplorerOM.IPropertyBag, IXmlSerializable
	{
		internal PropertyBag()
		{
			Bag = new Dictionary<string, object>();
		}

		#region IPropertyBag Members

		/// <summary>
		/// Reads a named property from the property bag.
		/// </summary>
		/// <param name="name">
		/// The name of the property to read.
		/// </param>
		/// <param name="value">
		/// The value of the property that is read.
		/// </param>
		/// <param name="unused"></param>
		public void Read(string name, out object value, int unused)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));
			value = Bag[name];
		}

		/// <summary>
		/// Saves a named property into the property bag.
		/// </summary>
		/// <param name="name">
		/// The name of the property to write.
		/// </param>
		/// <param name="value">
		/// The value of the property that is written.
		/// </param>
		public void Write(string name, ref object value)
		{
			if (name.IsNullOrEmpty()) throw new ArgumentNullException(nameof(name));
			if (Bag.ContainsKey(name)) Bag.Remove(name);
			if (value != null) Bag.Add(name, value);
		}

		#endregion

		#region IXmlSerializable Members

		public XmlSchema GetSchema()
		{
			throw new NotSupportedException();
		}

		public void ReadXml(XmlReader reader)
		{
			throw new NotSupportedException();
		}

		public void WriteXml(XmlWriter writer)
		{
			if (writer == null) throw new ArgumentNullException(nameof(writer));
			foreach (var property in Properties)
			{
				writer.WriteStartElement(property.Key);
				writer.WriteAttributeString("vt", ToVariantTypeCode(property.Value.GetType()).ToString("D"));
				writer.WriteValue(ToVariantValue(property.Value));
				writer.WriteEndElement();
			}
		}

		#endregion

		#region Base Class Member Overrides

		public override string ToString()
		{
			var xElement = new XElement(
				"Properties",
				Properties.Select(
					p => new XElement(
						p.Key,
						new XAttribute("vt", ToVariantTypeCode(p.Value.GetType()).ToString("D")),
						ToVariantValue(p.Value))));
			return xElement.ToString(SaveOptions.None);
		}

		#endregion

		public int Count => Properties.Count();

		protected IDictionary<string, object> Bag { get; }

		protected virtual IEnumerable<KeyValuePair<string, object>> Properties => Bag;

		#region Variant Helper

		private static VarEnum ToVariantTypeCode(Type type)
		{
			// see Microsoft.BizTalk.Adapter.Framework.VariantHelper
			return Type.GetTypeCode(type) switch {
				TypeCode.Empty => VarEnum.VT_EMPTY,
				TypeCode.Object => VarEnum.VT_UNKNOWN,
				TypeCode.DBNull => VarEnum.VT_NULL,
				TypeCode.Boolean => VarEnum.VT_BOOL,
				TypeCode.Char => VarEnum.VT_UI2,
				TypeCode.SByte => VarEnum.VT_I1,
				TypeCode.Byte => VarEnum.VT_UI1,
				TypeCode.Int16 => VarEnum.VT_I2,
				TypeCode.UInt16 => VarEnum.VT_UI2,
				TypeCode.Int32 => VarEnum.VT_I4,
				TypeCode.UInt32 => VarEnum.VT_UI4,
				TypeCode.Int64 => VarEnum.VT_I8,
				TypeCode.UInt64 => VarEnum.VT_UI8,
				TypeCode.Single => VarEnum.VT_R4,
				TypeCode.Double => VarEnum.VT_R8,
				TypeCode.Decimal => VarEnum.VT_DECIMAL,
				TypeCode.DateTime => VarEnum.VT_DATE,
				TypeCode.String => VarEnum.VT_BSTR,
				_ => throw new ArgumentException($"Type not supported [TypeCode:{Type.GetTypeCode(type)}].", nameof(type))
			};
		}

		private static string ToVariantValue(object obj)
		{
			// see Microsoft.BizTalk.Adapter.Framework.VariantHelper
			return obj is bool boolean
				? boolean ? "-1" : "0"
				: Convert.ToString(obj, CultureInfo.InvariantCulture);
		}

		#endregion
	}
}
