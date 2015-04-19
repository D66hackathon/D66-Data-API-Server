using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D66.Common.Data
{
	public class Achternamen
	{

		public Achternamen()
		{
			using(var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("D66.Common.Data.Achternamen.txt")))
			{
				string line;
				while(null != (line = reader.ReadLine()))
				{
					line = line.Trim();
					if(!string.IsNullOrEmpty(line))
					{
						achternamen.Add(line);
					}
				}
			}
		}

		public string Random(string seed)
		{
			var hash = (seed + "hsakjdhas").GetHashCode();
			var random = new Random(hash);
			var index = random.Next(achternamen.Count);
			return achternamen[index];
		}

		private readonly List<string> achternamen = new List<string>();

	}
}
