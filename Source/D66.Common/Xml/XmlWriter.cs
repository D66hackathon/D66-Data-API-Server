using System;
using System.IO;
using System.Text;

namespace D66.Common.Xml
{
	public class XmlWriter
	{

		public void Write(XmlDocument document, Stream stream, bool keepStreamOpen = false)
		{
			var writer = new StreamWriter(stream, GetEncoding(document));
			try
			{
				writer.WriteLine(
					"<?xml version=\"{0}\" encoding=\"{1}\"?>",
					document.XmlVersion,
					document.Encoding
				);
				document.Write(writer);
			}
			finally
			{
				writer.Flush();
				if(!keepStreamOpen)
				{
					writer.Dispose();
				}
			}
		}

		private Encoding GetEncoding(XmlDocument document)
		{
			switch(document.Encoding.ToLower())
			{
				case "utf-8":
					return Encoding.UTF8;
				default:
					throw new InvalidOperationException();
			}
		}

	}
}
