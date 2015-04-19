using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D66.Common.Data
{
	public class Voornamen
	{

		public Voornamen() {
			using(var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("D66.Common.Data.Voornamen.txt")))
			{
				string line;
				while(null != (line = reader.ReadLine()))
				{
					line = line.Trim();
					if(!string.IsNullOrEmpty(line))
					{
						voornamen.Add(line);
					}
				}
			}
		}

		public string Random(string seed)
		{
			var hash = (seed + "BLA").GetHashCode();
			var random = new Random(hash);
			var index = random.Next(voornamen.Count);
			return voornamen[index];
		}

		private readonly List<string> voornamen = new List<string>();

	}
}
