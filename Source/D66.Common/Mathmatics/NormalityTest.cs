using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class NormalityTest
	{

		private const double BinWidth = 0.5;
		private const int BinCount = 15;

		/// <summary>
		/// Calculates the chance that this data set is normally distributed
		/// </summary>
		/// <returns></returns>
		public double TestNormality(IEnumerable<double> samples)
		{
			var observed = new int[BinCount];
			var range = new StatRange(samples);
			var distribution = range.CreateNormal();
			foreach(var sample in range.Sorted())
			{
				var normalized = (sample - distribution.Mean)/distribution.StandardDeviation;
				var binIndex = GetBinIndex(normalized);
				if(binIndex.HasValue)
				{
					observed[binIndex.Value]++;
				}
			}

			var standard = new NormalDistribution(0, 1);
			// now compare to standard deviations
			double chiSquared = 0;
			for(var i=0; i<BinCount; i++)
			{
				var expected = (standard.CDF(GetBinEnd(i)) - standard.CDF(GetBinStart(i))) * range.Count;
				var difference = observed[i] - expected;
				chiSquared += difference*difference / expected;
			}
			return new ChiSquaredProbabilityTable().GetP(BinCount - 1, chiSquared);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">normalized value</param>
		/// <returns></returns>
		private int? GetBinIndex(double value)
		{
			value += (BinWidth*BinCount) / 2.0;
			var index = (int) Math.Floor(value/BinWidth);
			if(index < 0 || index >= BinCount)
			{
				return null;
			}
			return index;
		}

		private double GetBinStart(int bin)
		{
			return bin*BinWidth - (BinWidth * BinCount / 2.0);
		}

		private double GetBinEnd(int bin)
		{
			return GetBinStart(bin) + BinWidth;
		}

	}



}
