using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D66.Common.Data
{
	public class NameList
	{

		public NameList(string resourceName)
		{
			using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("D66.Common.Data." + resourceName)))
			{
				string line;
				while (null != (line = reader.ReadLine()))
				{
					line = Normalize(line);
					if (!string.IsNullOrEmpty(line))
					{
						names.Add(line);
					}
				}
			}
			names.Sort();
		}

		public string Random(string seed)
		{
			var hash = seed.GetHashCode();
			var random = new Random(hash);
			var index = random.Next(names.Count);
			return names[index];
		}

		private readonly List<string> names = new List<string>();

		protected virtual string Normalize(string word)
		{
			return word.Trim();
		}

		protected string CapitalizeEnglish(string word)
		{
			return string.Join(" ",
			                   word
			                   	.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
								.Select(w => CapitalizeEnglishWord(w))
					);
		}

		protected string CapitalizeEnglishWord(string word)
		{
			if(word == "and" || word == "or")
			{
				return word;
			}
			return word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower();
		}



	}
}
