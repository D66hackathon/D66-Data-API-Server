using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace D66.Common.Calendar
{

	/// <summary>
	/// Implements ISO 8601 week numbering
	/// </summary>
	public class Week : Period
	{

		public override DateTime StartTime { get { return monday; } }
		public override DateTime EndTime { get { return monday.AddDays(7); } }


		public override string Code { get { return string.Format("{0:0000}W{1:00}", Year, Nr); } }

		public override string DisplayName
		{
			get
			{
				var culture = CultureInfo.CurrentUICulture;
				return string.Format("week {1}, {0}", Year, Nr);
			}
		}

		public int Year
		{
			get { return _Year; }
			set
			{
				_Year = value;
				Update();
			}
		}

		private int _Year;

	
		public int Nr
		{
			get { return _Nr; }
			set
			{
				_Nr = value;
				Update();
			}
		}

		private int _Nr;

		private void Update()
		{
			if(Nr < 1 || Nr > 53)
			{
				return;
			}
			var jan1 = new DateTime(Year, 1, 1);
			int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

			DateTime firstThursday = jan1.AddDays(daysOffset);
			var cal = CultureInfo.CurrentCulture.Calendar;
			int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

			var weekNum = Nr;
			if (firstWeek <= 1)
			{
				weekNum -= 1;
			}
			monday = firstThursday.AddDays(weekNum * 7 - 3);
		}

		private DateTime monday;


		internal static int GetIso8601WeekOfYear(DateTime time)
		{
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

	}


	public class WeekPeriodType : PeriodType
	{

		public override Period ByTime(DateTime time)
		{
			var date = time.Date;
			var nr = Calendar.Week.GetIso8601WeekOfYear(date);
			var monday = date;
			// TODO: Ugly but it works
			while (monday.DayOfWeek != DayOfWeek.Monday)
			{
				monday = monday.AddDays(-1);
			}
			var year = monday.AddDays(3).Year;
			return new Week()
			       {
			       	Year = year,
					Nr = nr
			       };
		}


		

		public override Period ByCode(string code)
		{
			if (code != null && code.Length == 7)
			{
				if (code[4] == 'W')
				{
					int year, nr;
					if (int.TryParse(code.Substring(0, 4), out year) && int.TryParse(code.Substring(5, 2), out nr))
					{
						if (year >= 1900 && year < 2100 && nr >= 1 && nr <= 53)
						{
							return new Week()
							{
								Year = year,
								Nr = nr
							};
						}
					}
				}
			}
			return null;
		}

		//public static Week ByNumber(int year, int nr)
		//{
		//    var firstWeek = ByDate(new DateTime(year, 1, 1));
		//    int skip;
		//    if (firstWeek.Year != year)
		//    {
		//        skip = nr;
		//    }
		//    else
		//    {
		//        skip = nr - 1;
		//    }
		//    return firstWeek.AddWeeks(skip);
		//}



		//public IEnumerable<DateTime> GetWeekDays()
		//{
		//    var date = Monday;
		//    do
		//    {
		//        yield return date;
		//        date = date.AddDays(1);
		//    } while (date <= Sunday);
		//} 

		//public Week Next()
		//{
		//    return AddWeeks(1);
		//}

		//public Week Previous()
		//{
		//    return AddWeeks(-1);
		//}

		//public Week AddWeeks(int amount)
		//{
		//    return ByDate(GetWeekDay(DayOfWeek.Thursday).AddDays(amount * 7));
		//}
	}
}
