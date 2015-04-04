using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class WeightedOptionDistribution<T>
	{
		public WeightedOptionDistribution(Random random)
		{
			this.random = random;
		}

		public void AddOption(T item, double weight)
		{
			options.Add(item);
			cumulativeWeight.Add(GetTotalWeight() + weight);
		}

		/// <summary>
		/// Picks the next random item
		/// </summary>
		/// <returns></returns>
		public T Next()
		{
			if(!options.Any())
			{
				throw new InvalidOperationException("There are no options to pick from");
			}
			var value = random.NextDouble()*GetTotalWeight();
			// This should be a binary search but I've had some wine and I'm lazy ;)
			int index = 0;
			while(cumulativeWeight[index] < value)
			{
				index++;
			}
			return options[index];
		}

		private double GetTotalWeight()
		{
			return cumulativeWeight.LastOrDefault();
		}
		
		private readonly Random random;
		private readonly List<T> options = new List<T>();
		private readonly List<double> cumulativeWeight = new List<double>();



	}
}
