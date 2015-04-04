using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace D66.Hackathon.Model
{
	public class Region
	{

		[XmlAttribute()]
		public string Key { get; set; }

		[XmlAttribute()]
		public string Name { get; set; }

		[XmlAttribute()]
		public int X { get; set; }
		
		[XmlAttribute()]
		public int Y { get; set; }

		public RegionUitslag Result { get; set; }

	}

	
}
