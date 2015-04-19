using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class ProbabilityTable
	{

		public ProbabilityTable(string name)
		{
			var format = CultureInfo.InvariantCulture.NumberFormat;
			using(var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("D66.Common.Mathmatics." + name)))
			{
				var header = reader.ReadLine();
				var parts = header.Split('\t');
				probabilities = new double[parts.Length - 1];
				for(var i=1; i<parts.Length; i++)
				{
					probabilities[i - 1] = int.Parse(parts[i].Substring(1))/1000.0;
				}
				string line;
				while(null != (line = reader.ReadLine()))
				{
					parts = line.Split('\t');
					var row = new Row()
					          {
					          	Dof = int.Parse(parts[0]),
					          	Values = new double[probabilities.Length]
					          };
					for (var i = 1; i < parts.Length; i++)
					{
						row.Values[i - 1] = double.Parse(parts[i], format);
					}
					rows.Add(row);
				}
			}
		}

		private double[] probabilities;
		private List<Row> rows = new List<Row>(); 

		public double GetP(int dof, double chiSquared)
		{
			var values = GetValues(dof);
			if(chiSquared < values[0])
			{
				return probabilities[0];
			}
			for(var i=0; i<values.Length - 1; i++)
			{
				if(chiSquared >= values[i] && chiSquared < values[i + 1])
				{
					double alpha = (chiSquared - values[i])/(values[i + 1] - values[i]);
					return probabilities[i] + alpha*(probabilities[i + 1] - probabilities[i]);
				}
			}
			return probabilities.Last();
		}

		private double[] GetValues(int dof)
		{
			int startIndex = 0;
			for(var i=0; i<rows.Count; i++)
			{
				if(rows[i].Dof <= dof)
				{
					startIndex = i;
				}
			}
			if(rows[startIndex].Dof == dof)
			{
				return rows[startIndex].Values;
			}
			// interpolate
			var low = rows[startIndex];
			var hi = rows[startIndex + 1];
			var result = new double[probabilities.Length];
			double alpha = (double) (dof - low.Dof)/(hi.Dof - low.Dof);
			for(var i=0; i<probabilities.Length; i++)
			{
				result[i] = low.Values[i] + alpha * (hi.Values[i] - low.Values[i]);
			}
			return result;
		}
		
		class Row
		{
			public int Dof { get; set; }
			public double[] Values { get; set; }
		}
	}

	public class ChiSquaredProbabilityTable : ProbabilityTable
	{
		public ChiSquaredProbabilityTable() : base("ChiSquaredTable.txt")
		{
			
		}
	}
}
