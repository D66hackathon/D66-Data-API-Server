using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class LinearModel
	{
		public LinearModel()
		{
			Variables = new List<string>();
		}

		public List<string> Variables { get; set; }


		public double[] Beta { get; set; }
		public double Beta0 { get; set; }
		public double R { get; set; }

		public bool Fit(List<Sample> samples)
		{
			if (samples.Count == 0) { throw new ArgumentException("Not enough samples"); }
			int n = samples.Count;
			int p = Variables.Count;

			// detect collinearity
			//for (var i = 0; i < p; i++)
			//{
			//    for (var j = i + 1; j < p; j++)
			//    {
			//        var model = SimpleLinearModel.Create(samples, s => s.X[i], s => s.X[j]);
			//        if (model.R == 1)
			//        {
			//            return null; // Collinearity detected
			//        }
			//    }
			//}

			var left = new Matrix(p + 1, p + 1);
			var right = new Matrix(p + 1, 1);
			for (int i = 0; i < n; i++)
			{
				var x = new Matrix(p + 1, 1);
				x[0, 0] = 1;
				for (int k = 1; k <= p; k++)
				{
					x[k, 0] = samples[i][Variables[k - 1]];
				}
				var xt = x.Transpose();
				left += x * xt;
				right += x * samples[i].Y;
			}
			left /= n;
			left = left.Invert();

			if (left == null)
			{
				return false; // Matrix is not invertible
			}

			right /= n;
			var b = left * right;
			Beta0 = b[0, 0];
			Beta = Enumerable.Range(1, b.Rows - 1).Select(i => b[i, 0]).ToArray();
			var data = new StatRange(samples.Select(s => s.Y));
			var residual = new StatRange(samples.Select(s => s.Y - Calc(s)));
			if (Math.Abs(residual.Average) > 1e-6)
			{
				return false;
			}
			var explainedVariance = data.Variance - residual.Variance;
			R = Math.Sqrt(explainedVariance / data.Variance);
			return true;
		}
 
		public double Calc(Sample sample)
		{
			if(Variables.Count != Beta.Length)
			{
				throw new InvalidOperationException();
			}

			var result = Beta0;
			for(var i=0; i<Beta.Length; i++)
			{
				result += Beta[i] * sample[Variables[i]];
			}
			return result;
		}

	}

	public class Sample
	{
		public void SetValue(string variable, double value)
		{
			values.Add(variable, value);
		}
		private Dictionary<string, double> values = new Dictionary<string, double>();
		public double Y { get; set; }

		public double this[string variable]
		{
			get
			{
				double result;
				if (values.TryGetValue(variable, out result))
				{
					return result;
				}
				return 0;
			}
			set { values[variable] = value; }
		}
	}

}
