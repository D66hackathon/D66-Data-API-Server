using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Hackathon.API.ListElections
{
	public class Result
	{

		public List<Election> Elections { get; set; }

	}

	public class Election
	{
		public string Key { get; set; }
		public string Name { get; set; }

	}
}
