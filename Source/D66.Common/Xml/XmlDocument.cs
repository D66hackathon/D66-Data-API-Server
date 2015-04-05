namespace D66.Common.Xml
{
	public class XmlDocument : XmlElement
	{
		public XmlDocument(string name)
		{
			this.Name = name;
			XmlVersion = "1.0";
			Encoding = "utf-8";
		}

		public string XmlVersion { get; set; }
		public string Encoding { get; set; }

	}
}
