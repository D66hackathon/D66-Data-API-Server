using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using D66.Common.Collections;

namespace D66.Common.Xml
{
	public class XmlElement : XmlItem
	{
		public string Name { get; internal set; }


		private readonly KeyValueCollection<string, string> attributes = new KeyValueCollection<string, string>();
		private readonly List<XmlItem> children = new List<XmlItem>();

		public XmlElement Attr(string key, double value)
		{
			return Attr(key, value.ToString(numberFormat));
		}

		public XmlElement Attr(string key, string value)
		{
			attributes.Set(key, value);
			return this;
		}

		public T AddChild<T>() where T : XmlItem, new()
		{
			var child = new T();
			child.parent = this;
			children.Add(child);
			return child;
		}

		public T AddChildElement<T>(string name) where T : XmlElement, new()
		{
			if(name.Contains(' '))
			{
				throw new InvalidOperationException();
			}
			var result = AddChild<T>();
			result.Name = name;
			return result;
		}

		public XmlElement AddChildElement(string name)
		{
			return AddChildElement<XmlElement>(name);
		}

		public T AddSibling<T>() where T : XmlItem, new()
		{
			var parentElement = parent as XmlElement;
			if (parentElement == null)
			{
				throw new InvalidOperationException("Can't add sibling, this is the root element");
			}
			return parentElement.AddChild<T>();
		}


		public XmlItem Text(string text)
		{
			var result = AddChild<XmlText>();
			result.Text = text;
			return result;
		}

		public XmlItem Parent()
		{
			return parent;
		}

		public string GetAttribute(string name)
		{
			return
				attributes
					.Where(a => a.Key == name)
					.Select(a => a.Value)
					.FirstOrDefault();
		}

		public double GetDouble(string name)
		{
			var value = GetAttribute(name);
			if(string.IsNullOrEmpty(value))
			{
				return 0;
			}
			return double.Parse(value, numberFormat);
		}


		internal override void Write(StreamWriter writer)
		{
			writer.Write('<');
			writer.Write(Name);
			foreach(var attribute in attributes.Where(a => a.Value != null))
			{
				writer.Write(
					" {0}=\"{1}\"",
					attribute.Key,
					XmlEntities(attribute.Value)
				);
			}
			if(children.Any())
			{
				writer.WriteLine(">");
				foreach(var child in children)
				{
					child.Write(writer);
				}
				writer.WriteLine("</{0}>", Name);
			}
			else
			{
				writer.WriteLine(" />");
			}
		}


	}
}
