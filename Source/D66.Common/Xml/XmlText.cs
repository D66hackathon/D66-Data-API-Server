using System.IO;

namespace D66.Common.Xml
{
	public class XmlText : XmlItem
	{

		public string Text { get; internal set; }

		internal override void Write(StreamWriter writer)
		{
			writer.Write(Text);
		}
	}
}
