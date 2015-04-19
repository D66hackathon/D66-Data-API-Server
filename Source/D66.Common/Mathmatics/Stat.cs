using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public static class Stat
	{

		public static double Variance(IEnumerable<double> items)
		{
			return new StatRange(items).Variance;
		}

		public static double Covariance(IEnumerable<Tuple<double, double>> items)
		{
			double avg1 = items.Average(i => i.Item1);
			double avg2 = items.Average(i => i.Item2);
			int count = items.Count();
			var sum = items.Sum(i => (i.Item1 - avg1) * (i.Item2 - avg2));
			return sum / (count - 1);
		}
	}
}
