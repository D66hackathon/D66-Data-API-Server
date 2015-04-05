using System.IO;

namespace D66.Common.Xml
{
	public class XmlComment : XmlItem
	{

		public string Comment { get; internal set; }

		internal override void Write(StreamWriter writer)
		{
			writer.Write("<!-- {0} -->", Comment);
		}

	}
}
