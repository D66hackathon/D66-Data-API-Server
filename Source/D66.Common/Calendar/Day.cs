using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Calendar
{
	public class Day : Period
	{

		public int Year { get; set; }
		public int Month { get; set; }
		public int Nr { get; set; }

		public override DateTime StartTime { get
		{
			return new DateTime(Year, Month, Nr);
		} }
		public override DateTime EndTime { get { return StartTime.AddDays(1); } }


		public override string Code
		{
			get { return string.Format("{0:0000}{1:00}{2:00}", Year, Month, Nr); }
		}

		public override string DisplayName
		{
			get { return StartTime.ToShortDateString(); }
		}

		public static DateTime? Parse(string str)
		{
			if (str != null && str.Length == 8)
			{
				int year, month, nr;
				if (int.TryParse(str.Substring(0, 4), out year) && int.TryParse(str.Substring(4, 2), out month) && int.TryParse(str.Substring(6, 2), out nr))
				{
					if (year >= 1900 && year < 2100 && month >= 1 && month <= 12 && nr >= 1 && nr <= 31)
					{
						try
						{
							return new DateTime(year, month, nr);
						}
						catch(Exception ex)
						{
							return null;
						}
					}

				}
			}
			return null;
		}

	}

	public class DayPeriodType : PeriodType
	{

		public override Period ByCode(string code)
		{
			var date = Calendar.Day.Parse(code);
			if(date.HasValue)
			{
				return new Day()
				       {
				       	Year = date.Value.Year,
						Month = date.Value.Month,
						Nr = date.Value.Day
				       };
			}
			return null;
		}


		public override Period ByTime(DateTime time)
		{
			return new Day()
			       {
			       	Year = time.Year,
					Month = time.Month,
			       	Nr = time.Day
			       };
		}
	}
}
