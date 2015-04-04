using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace D66.Common.Calendar
{
	[Serializable()]
	public class RecurringEvent
	{
		public RecurringEvent()
		{
			Scale = RecurrenceScale.Day;
			EveryCount = 1;
		}

		[XmlAttribute()]
		public RecurrenceScale Scale { get; set; }

		[XmlAttribute()]
		public int EveryCount { get; set; }

		[XmlAttribute()]
		public TimeSpan RecurrenceDelay { get; set; }

		public DateTime GetNextOccurenceAfter(DateTime time)
		{
			if (EveryCount <= 0)
			{
				throw new ArgumentException();
			}
			var epoch = new DateTime(2000, 1, 1);
			switch (Scale)
			{
				case RecurrenceScale.Second:
					var seconds = Math.Floor(time.Subtract(epoch).TotalSeconds / EveryCount);
					seconds = EveryCount * (seconds + 1);
					return epoch.AddSeconds(seconds).Add(RecurrenceDelay);

				case RecurrenceScale.Minute:
					var minutes = Math.Floor(time.Subtract(epoch).TotalMinutes / EveryCount);
					minutes = EveryCount * (minutes + 1);
					return epoch.AddMinutes(minutes).Add(RecurrenceDelay);

				case RecurrenceScale.Hour:
					var hours = Math.Floor(time.Subtract(epoch).TotalHours / EveryCount);
					hours = EveryCount * (hours + 1);
					return epoch.AddHours(hours).Add(RecurrenceDelay);

				case RecurrenceScale.Day:
					var days = Math.Floor(time.Subtract(epoch).TotalDays / EveryCount);
					days = EveryCount * (days + 1);
					return epoch.AddDays(days).Add(RecurrenceDelay);

				default:
					throw new NotImplementedException();
			}
		}

		public override string ToString()
		{
			return string.Format("Every {0} {1}(s) after {2}", EveryCount, Scale, RecurrenceDelay);
		}

	}

	public enum RecurrenceScale
	{
		Second,
		Minute,
		Hour,
		Day
	}
}
