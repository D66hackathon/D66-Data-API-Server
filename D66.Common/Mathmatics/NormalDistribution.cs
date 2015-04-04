using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class NormalDistribution
	{
		public NormalDistribution(double mean, double standardDeviation)
		{
			Mean = mean;
			StandardDeviation = standardDeviation;
		}

		public double Mean { get; private set; }
		public double StandardDeviation { get; private set; }

		/// <summary>
		/// returns the probability density for x
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public double F(double x)
		{
			double translated = x - Mean;
			double result = Math.Exp(-translated*translated/2.0*StandardDeviation*StandardDeviation);
			return result/(StandardDeviation*Math.Sqrt(2.0*System.Math.PI));
		}

		/// <summary>
		/// Returns the cumulative probability density for x
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public double CDF(double x)
		{
			var a1 = 0.254829592;
			var a2 = -0.284496736;
			var a3 = 1.421413741;
			var a4 = -1.453152027;
			var a5 = 1.061405429;
			var p = 0.3275911;

			// Save the sign of x
			var sign = 1;
			if (x < 0)
			{
				sign = -1;
			}
			x = Math.Abs(x)/Math.Sqrt(2.0);

			//# A&S formula 7.1.26
			var t = 1.0/(1.0 + p*x);
			var y = 1.0 - (((((a5*t + a4)*t) + a3)*t + a2)*t + a1)*t*Math.Exp(-x*x);

			return 0.5*(1.0 + sign*y);
		}


		/// <summary>
		/// Returns the inverse cumulative distribution for x
		/// </summary>
		/// <param name="p">&lt;0, 1&gt;</param>
		/// <returns></returns>
		public double Quantile(double p)
		{
			if (p == 0)
			{
				return double.NaN;
			}
			if(p == 1)
			{
				return double.NaN;
			}
			if (double.IsNaN(p) || double.IsNaN(Mean) || double.IsNaN(StandardDeviation)) return (p + Mean + StandardDeviation);
			if (StandardDeviation < 0) return (double.NaN);
			if (StandardDeviation == 0) return (Mean);

			double p_ = p;
			double q = p_ - 0.5;
			double r, val;

			if (Math.Abs(q) <= 0.425)  // 0.075 <= p <= 0.925
			{
				r = .180625 - q * q;
				val = q * (((((((r * 2509.0809287301226727 +
						   33430.575583588128105) * r + 67265.770927008700853) * r +
						 45921.953931549871457) * r + 13731.693765509461125) * r +
					   1971.5909503065514427) * r + 133.14166789178437745) * r +
					 3.387132872796366608)
				/ (((((((r * 5226.495278852854561 +
						 28729.085735721942674) * r + 39307.89580009271061) * r +
					   21213.794301586595867) * r + 5394.1960214247511077) * r +
					 687.1870074920579083) * r + 42.313330701600911252) * r + 1.0);
			}
			else
			{
				r = q > 0 ? R_D_Cval(p) : p_;
				r = Math.Sqrt(-Math.Log(r));

				if (r <= 5)              // <==> min(p,1-p) >= exp(-25) ~= 1.3888e-11
				{
					r -= 1.6;
					val = (((((((r * 7.7454501427834140764e-4 +
							.0227238449892691845833) * r + .24178072517745061177) *
						  r + 1.27045825245236838258) * r +
						 3.64784832476320460504) * r + 5.7694972214606914055) *
					   r + 4.6303378461565452959) * r +
					  1.42343711074968357734)
					 / (((((((r *
							  1.05075007164441684324e-9 + 5.475938084995344946e-4) *
							 r + .0151986665636164571966) * r +
							.14810397642748007459) * r + .68976733498510000455) *
						  r + 1.6763848301838038494) * r +
						 2.05319162663775882187) * r + 1.0);
				}
				else                     // very close to  0 or 1 
				{
					r -= 5.0;
					val = (((((((r * 2.01033439929228813265e-7 +
							2.71155556874348757815e-5) * r +
						   .0012426609473880784386) * r + .026532189526576123093) *
						 r + .29656057182850489123) * r +
						1.7848265399172913358) * r + 5.4637849111641143699) *
					  r + 6.6579046435011037772)
					 / (((((((r *
							  2.04426310338993978564e-15 + 1.4215117583164458887e-7) *
							 r + 1.8463183175100546818e-5) * r +
							7.868691311456132591e-4) * r + .0148753612908506148525)
						  * r + .13692988092273580531) * r +
						 .59983220655588793769) * r + 1.0);
				}
				if (q < 0.0) val = -val;
			}

			return (Mean + StandardDeviation * val);
		}

		private static double R_D_Cval(double p)
		{
			return 0.5 - p + 0.5;
		}



	}
}
