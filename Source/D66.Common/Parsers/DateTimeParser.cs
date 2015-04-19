using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace D66.Common.Parsers
{
	public static class DateTimeParser
	{

		public static bool TryParse(string value, string format, CultureInfo culture, out DateTime result)
		{
			result = new DateTime();
			int valueIndex = 0;
			int formatIndex = 0;
			var tempResult = new TempDate();
			while (valueIndex < value.Length)
			{
				if (delimiters.Contains(value[valueIndex]))
				{
					valueIndex++;
					formatIndex++;
				}
				else
				{
					bool found = false;
					foreach (var handler in partHandlers)
					{
						if (Matches(format, formatIndex, handler.Key))
						{
							found = true;
							formatIndex += handler.Key.Length;
							if (!handler.Value(culture, value, ref valueIndex, tempResult))
							{
								return false;
							}
							break;
						}
					}
					if (!found)
					{
						throw new InvalidOperationException("Unexpected date format: " + format);
					}
				}
			}
			result = new DateTime(tempResult.Year, tempResult.Month, tempResult.Day);
			return true;
		}

		private static bool Matches(string haystack, int index, string needle)
		{
			if (index + needle.Length <= haystack.Length)
			{
				return haystack.Substring(index, needle.Length) == needle;
			}
			return false;
		}

		private static bool TryParse_yyyy(CultureInfo culture, string str, ref int valueIndex, TempDate result)
		{
			if (valueIndex + 4 <= str.Length)
			{
				int year;
				if (int.TryParse(str.Substring(valueIndex, 4), out year))
				{
					result.Year = year;
					valueIndex += 4;
					return true;
				}
			}
			return false;
		}

		private static bool TryParse_yy(CultureInfo culture, string str, ref int valueIndex, TempDate result)
		{
			if (valueIndex + 2 <= str.Length)
			{
				int year;
				if (int.TryParse(str.Substring(valueIndex, 2), out year))
				{
					result.Year = 2000 + year;
					valueIndex += 2;
					return true;
				}
			}
			return false;
		}

		private static bool TryParse_MMM(CultureInfo culture, string str, ref int index, TempDate result)
		{
			if (index + 3 <= str.Length)
			{
				var abbreviatedName = str.Substring(index, 3).ToLower();
				for (int month = 1; month <= 12; month++)
				{
					if (culture.DateTimeFormat.GetAbbreviatedMonthName(month).ToLower() == abbreviatedName)
					{
						result.Month = month;
						index += 3;
						return true;
					}
				}
			}
			return false;
		}

		private static bool TryParse_d(CultureInfo culture, string str, ref int index, TempDate result)
		{
			if(TryParse_dd(culture, str, ref index, result))
			{
				return true;
			}
			if (index + 1 < str.Length)
			{
				int day;
				if (int.TryParse(str.Substring(index, 1), out day))
				{
					result.Day = day;
					index++;
					return true;
				}
			}
			return false;
		}

		private static bool TryParse_dd(CultureInfo culture, string str, ref int index, TempDate result)
		{
			if (index + 2 < str.Length)
			{
				int day;
				if (int.TryParse(str.Substring(index, 2), out day))
				{
					result.Day = day;
					index += 2;
					return true;
				}
			}
			return false;
		}


		private static bool TryParse_MM(CultureInfo culture, string str, ref int index, TempDate result)
		{
			if (index + 2 < str.Length)
			{
				int month;
				if (int.TryParse(str.Substring(index, 2), out month))
				{
					result.Month = month;
					index += 2;
					return true;
				}
			}
			return false;
		}

		private static bool TryParse_M(CultureInfo culture, string str, ref int index, TempDate result)
		{
			if (TryParse_MM(culture, str, ref index, result))
			{
				return true;
			}
			if (index + 1 < str.Length)
			{
				int month;
				if (int.TryParse(str.Substring(index, 1), out month))
				{
					result.Month = month;
					index++;
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// The different handlers for parsing date parts. The longer ones need to come first to prevent a partial match
		/// </summary>
		private static readonly List<KeyValuePair<string, DatePartParseDelegate>> partHandlers = new List<KeyValuePair<string, DatePartParseDelegate>>()
		                                                                        {
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("yyyy", TryParse_yyyy),
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("yy", TryParse_yy),
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("MMM", TryParse_MMM),
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("MM", TryParse_MM),
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("M", TryParse_M),
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("dd", TryParse_dd),
		                                                                        	new KeyValuePair<string,DatePartParseDelegate>("d", TryParse_d),
		                                                                        };
		private static readonly char[] delimiters = new[] { '-', '/', ':', ' ' };

		private delegate bool DatePartParseDelegate(CultureInfo culture, string str, ref int index, TempDate result);


		internal class TempDate
		{
			public int Year { get; set; }
			public int Month { get; set; }
			public int Day { get; set; }
		}

	}
}
