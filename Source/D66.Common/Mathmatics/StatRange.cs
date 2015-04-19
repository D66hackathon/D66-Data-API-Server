using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class StatRange
	{

		public StatRange(IEnumerable<double> data)
		{
			this.sorted = data.OrderBy(d => d).ToList();
			this.Count = sorted.Count;
			if(Count > 0)
			{
				Average = this.sorted.Average();
				Median = GetValueForPercentile(50);
				Minimum = sorted.First();
				Maximum = sorted.Last();
			}
			else
			{
				Average = double.NaN;
				Median = double.NaN;
				Minimum = double.NaN;
				Maximum = double.NaN;
			}
			if(Count > 1)
			{
				double ss = 0;
				foreach(var d in this.sorted)
				{
					Sum += d;
					var delta = d - Average;
					ss += delta*delta;
				}
				Variance = ss/Dof;
				StandardDeviation = System.Math.Sqrt(Variance);
			}
			else
			{
				Variance = double.NaN;
				StandardDeviation = double.NaN;
			}
		}

		public int Dof { get { return Count - 1; } }
		public int Count { get; private set; }
		public double Average { get; private set; }
		public double Median { get; private set; }
		public double Variance { get; private set; }
		public double StandardDeviation { get; private set; }
		public double Minimum { get; private set; }
		public double Maximum { get; private set; }
		public double Sum { get; private set; }

		private readonly List<double> sorted;
		
		public IEnumerable<double> Sorted()
		{
			return sorted;
		} 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p">[0, 100]</param>
		/// <returns></returns>
		public double GetValueForPercentile(double p)
		{
			if(p < 0 || p > 100)
			{
				throw new ArgumentException();
			}
			if(sorted.Count == 0)
			{
				throw new InvalidOperationException("Can't get percentile value, n = 0");
			}
			if(sorted.Count == 1)
			{
				return sorted[0];
			}
			p /= 100;
			double index = p*Dof;
			var startIndex = (int) System.Math.Floor(index);
			var alpha = index - startIndex;
			if(startIndex == sorted.Count - 1)
			{
				return sorted[startIndex];
			}
			var delta = sorted[startIndex + 1] - sorted[startIndex];
			return sorted[startIndex] + delta*alpha;
		}

		public double GetPercentileForValue(double value)
		{
			if(sorted.Count <= 1)
			{
				throw new InvalidOperationException();
			}
			if(value < sorted.First())
			{
				return 0;
			}
			if(value > sorted.Last())
			{
				return 100;
			}
			int firstLargerIndex = 1;
			while(firstLargerIndex < sorted.Count && sorted[firstLargerIndex] < value)
			{
				firstLargerIndex++;
			}
			double from = sorted[firstLargerIndex - 1];
			double to = sorted[firstLargerIndex];
			if (from > value)
			{
				throw new InvalidOperationException();
			}
			if (to < value)
			{
				throw new InvalidOperationException();
			}
			var alpha = (value - from)/(to - from);
			var percentagePerStep = 100.0/(sorted.Count - 1);
			return (alpha + firstLargerIndex - 1) * percentagePerStep;
		}



		public NormalDistribution CreateNormal()
		{
			return new NormalDistribution(Average, StandardDeviation);
		}


		public override string ToString()
		{
			return string.Format("n = {0}, M = {1:0.00}, StdDev = {2:0.00}", Count, Average, StandardDeviation);
		}


	}
}
