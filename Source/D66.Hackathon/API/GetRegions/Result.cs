using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Hackathon.API.GetRegions
{
	public class Result
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public List<Region> Regions { get; set; }

	}

	public class Region
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public List<PartyEntry> Parties { get; set; }
	}

	public class PartyEntry
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public int Votes { get; set; }
		public double Percentage { get; set; }
	}
}
