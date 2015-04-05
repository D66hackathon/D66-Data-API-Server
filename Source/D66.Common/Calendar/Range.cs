using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Calendar
{
	public class Range : Period
	{
		public Range(DateTime start, DateTime end)
		{
			if(end < start)
			{
				var tmp = start;
				start = end;
				end = tmp;
			}
			this.start = start;
			this.end = end;
		}

		private readonly DateTime start, end;

		public override DateTime StartTime { get { return start; } }
		public override DateTime EndTime { get { return end; } }

		public override string Code
		{
			get { return string.Format("{0:yyyyMMdd}-{1:yyyyMMdd}", start, end); }
		}

		public override string DisplayName
		{
			get { return string.Format("{0:yyyy-MM-dd} - {1:yyyy-MM-dd}", start, end); }
		}
	}

	public class RangePeriodType : PeriodType
	{

		public override Period ByCode(string code)
		{
			if (code != null && code.Length == 17)
			{
				var parts = code.Split('-');
				if(parts.Length == 2)
				{
					var start = Calendar.Day.Parse(parts[0]);
					var end = Calendar.Day.Parse(parts[1]);
					if(start.HasValue && end.HasValue)
					{
						return new Range(start.Value, end.Value);
					}
				}
			}
			return null;
		}

		public override Period ByTime(DateTime time)
		{
			throw new InvalidOperationException();
		}
	}
}
