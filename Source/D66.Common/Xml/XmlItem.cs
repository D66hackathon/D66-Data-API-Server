using System;
using System.Globalization;
using System.IO;

namespace D66.Common.Xml
{
	public abstract class XmlItem
	{

		internal XmlItem parent;

		internal abstract void Write(StreamWriter writer);

		protected internal string XmlEntities(string str)
		{
			return 
				str
				.Replace("<", "&lt;")
				.Replace(">", "&gt;")
				.Replace("'", "&apos;")
				.Replace("\"", "&quot;");
		}

		protected readonly IFormatProvider numberFormat = CultureInfo.InvariantCulture.NumberFormat;

	}
}
