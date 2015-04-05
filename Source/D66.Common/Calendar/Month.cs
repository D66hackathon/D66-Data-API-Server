using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace D66.Common.Calendar
{
	public class Month : Period
	{

		public int Year { get; set; }
		public int Nr { get; set; }

		public override DateTime StartTime { get
		{
			return new DateTime(Year, Nr, 1);
		} }
		public override DateTime EndTime { get { return StartTime.AddMonths(1); } }


		public override string Code
		{
			get { return string.Format("{0:0000}{1:00}", Year, Nr); }
		}

		public override string DisplayName
		{
			get { 
				var culture = CultureInfo.CurrentUICulture;
				return string.Format("{0}, {1}", Year, culture.DateTimeFormat.GetMonthName(Nr));
			}
		}

	}

	public class MonthPeriodType : PeriodType
	{

		public override Period ByCode(string code)
		{
			if(code != null && code.Length == 6)
			{
				int year, nr;
				if (int.TryParse(code.Substring(0, 4), out year) && int.TryParse(code.Substring(4, 2), out nr))
				{
					if (year >= 1900 && year < 2100 && nr >= 1 && nr <= 12)
					{
						return new Month()
						{
							Year = year,
							Nr = nr
						};
					}

				}
			}
			return null;
		}

		public override Period ByTime(DateTime time)
		{
			return new Month()
			       {
			       	Year = time.Year,
			       	Nr = time.Month
			       };
		}
	}
}
