using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class SimpleLinearModel
	{
		private SimpleLinearModel()
		{

		}

		public static SimpleLinearModel Create<T>(IEnumerable<T> items, Func<T, double> getX, Func<T, double> getY)
		{
			var rangeX = new StatRange(items.Select(getX));
			var rangeY = new StatRange(items.Select(getY));
			double covarXY = Stat.Covariance(items.Select(i => new Tuple<double, double>(getX(i), getY(i))));
			double beta = covarXY / rangeX.Variance;
			double r = covarXY / (rangeX.StandardDeviation * rangeY.StandardDeviation);
			double alpha = rangeY.Average - beta * rangeX.Average;
			return new SimpleLinearModel()
			{
				Alpha = alpha,
				Beta = beta,
				R = r
			};
		}

		public double CalcY(double x)
		{
			return Alpha + Beta * x;
		}

		/// <summary>
		/// Intercept
		/// </summary>
		public double Alpha { get; private set; }
		/// <summary>
		/// Slope
		/// </summary>
		public double Beta { get; private set; }
		public double R { get; private set; }

		public override string ToString()
		{
			return string.Format("Alpha = {0:0.0}, Beta = {1:0.00}, R = {2:0.00}", Alpha, Beta, R);
		}

	}
}
