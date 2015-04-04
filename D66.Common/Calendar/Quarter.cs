using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Calendar
{
	public class Quarter : Period
	{

		public int Year { get; set; }

		public int Nr { get; set; }

		public override DateTime StartTime
		{
			get { 
				return new DateTime(Year, Nr * 3 - 2, 1);	
			}
		}

		public override DateTime EndTime
		{
			get { return StartTime.AddMonths(3); }
		}

		public override string Code
		{
			get { return string.Format("{0:0000}Q{1:0}", Year, Nr); }
		}

		public override string DisplayName
		{
			get { return string.Format("{0:0000} Q{1:0}", Year, Nr); }
		}

	}

	public class QuarterPeriodType : PeriodType
	{
		public override Period ByCode(string code)
		{
			if(code != null && code.Length == 6)
			{
				if(code[4] == 'Q')
				{
					int year, nr;
					if (int.TryParse(code.Substring(0, 4), out year) && int.TryParse(code.Substring(5, 1), out nr))
					{
						if(year >= 1900 && year < 2100 && nr >= 1 && nr <= 4)
						{
							return new Quarter()
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

		public override Period ByTime(DateTime time)
		{
			return new Quarter()
			       {
					   Year = time.Year,
					   Nr = ((time.Month - 1) / 3) + 1
			       };
		}
	}
}
