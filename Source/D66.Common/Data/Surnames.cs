using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Data
{
	public class Surnames : NameList
	{
		public Surnames() : base("Surnames.txt") {}

		protected override string Normalize(string word)
		{
			return CapitalizeEnglish(base.Normalize(word));
		}
	}
}
