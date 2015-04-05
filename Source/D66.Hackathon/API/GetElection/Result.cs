using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Hackathon.API.GetElection
{
	public class Result
	{

		public string Key { get; set; }
		public string Name { get; set; }

		public List<Party> Parties { get; set; }

	}


	public class Party
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public int Votes { get; set; }
		public double Percentage { get; set; }
	}
}


