using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace D66.Hackathon.Model
{
	public class Party
	{

		[XmlAttribute()]
		public string Key { get; set; }

		[XmlAttribute()]
		public string Name { get; set; }

	}
}
