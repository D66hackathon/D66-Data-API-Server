using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Diagnostics
{
	public class Stopwatch
	{

		public Stopwatch()
		{
			start = DateTime.Now;
			lap = start;
		}

		public void Lap(string action)
		{
			var now = DateTime.Now;
			var duration = now.Subtract(lap);
			var totalDuration = now.Subtract(start);
			#if DEBUG
			Console.WriteLine("{0} took {1:0.00}s, total time {2:0.00}s", action, duration.TotalSeconds, totalDuration.TotalSeconds);
			#endif
			lap = DateTime.Now;
		}

	    private readonly DateTime start;
        private DateTime lap;

	}
}
