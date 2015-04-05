using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common
{
	public static class StringUtil
	{
		public static string Replace(string s, Func<char, char> getReplacement)
		{
			return new string(s.Select(c => getReplacement(c)).ToArray());
		}

		public static string StripAll(string s, Func<char, bool> retainIf)
		{
			return new string(s.Where(c => retainIf(c)).ToArray());
		}

		public static string PascalCase(string str)
		{
			return
				string.Join(
					"",
					StripAll(str, c => c == ' ' || char.IsLetterOrDigit(c))
						.Split(' ')
						.Select(s => CapitalizeFirst(s)));
		}

		public static string CapitalizeFirst(string s)
		{
			if(string.IsNullOrEmpty(s))
			{
				return s;
			}
			if(s.Length == 1)
			{
				return s.ToUpper();
			}
			return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
		}

		public static string CreateIdentifier(string s)
		{
			return s.Replace(" ", "").Replace(".", "").Replace("-", "").Replace("&", "").Replace("'", "");
		}

	}
}
