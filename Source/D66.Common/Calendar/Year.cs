using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Calendar
{
	public class Year : Period
	{

		public int Nr { get; set; }

		public override DateTime StartTime
		{
			get
			{
				return new DateTime(Nr, 1, 1);
			}
		}
		public override DateTime EndTime { get { return StartTime.AddYears(1); } }


		public override string Code
		{
			get { return string.Format("{0:0000}", Nr); }
		}

		public override string DisplayName
		{
			get { return Code; }
		}

	}

	public class YearPeriodType : PeriodType
	{

		public override Period ByCode(string code)
		{
			if (code != null && code.Length == 4)
			{
				int year;
				if (int.TryParse(code, out year))
				{
					if (year >= 1900 && year < 2100)
					{
						return new Year()
						{
							Nr = year,
						};
					}

				}
			}
			return null;
		}

		public override Period ByTime(DateTime time)
		{
			return new Year()
			{
				Nr = time.Year
			};
		}
	}
}
