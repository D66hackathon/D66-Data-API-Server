using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace D66.Hackathon.Model
{
	public class RegionUitslag
	{

		[XmlAttribute()]
		public int BlankVotes { get; set; }

		[XmlAttribute()]
		public int Invalid { get; set; }

		public List<RegionPartyResult> Parties { get; set; }

	}

	public class RegionPartyResult
	{
		/// <summary>
		/// Key of the partij
		/// </summary>
		[XmlAttribute()]
		public string Party { get; set; }

		[XmlAttribute()]
		public int Votes { get; set; }
	}
}
