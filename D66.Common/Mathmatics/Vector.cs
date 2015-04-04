using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	/// <summary>
	/// An n-dimensional vector.
	/// </summary>
	public class Vector
	{

		public Vector(double[] values)
		{
			this.values = values;
		}

		public Vector(int dimension)
		{
			values = new double[dimension];
		}

		public int Dimension { get { return values.Length; } }

		private readonly double[] values;

		public double this[int index]
		{
			get { return values[index]; }
			set { values[index] = value; }
		}

		public IEnumerable<double> Values()
		{
			foreach(var d in values)
			{
				yield return d;
			}
		}


		public static Vector operator /(Vector v, double n)
		{
			return new Vector(v.Values().Select(d => d / n).ToArray());
		}

		public static Vector operator *(Vector v, double n)
		{
			return new Vector(v.Values().Select(d => d * n).ToArray());
		}

		public static Vector operator +(Vector v1, Vector v2)
		{
			if(v1.Dimension != v2.Dimension)
			{
				throw new ArgumentException("Vectors should have same dimension");
			}
			var result = new Vector(v1.Dimension);
			for(var i=0; i<result.Dimension; i++)
			{
				result[i] = v1[i] + v2[i];
			}
			return result;
		}

		public static Vector operator -(Vector v1, Vector v2)
		{
			if (v1.Dimension != v2.Dimension)
			{
				throw new ArgumentException("Vectors should have same dimension");
			}
			var result = new Vector(v1.Dimension);
			for (var i = 0; i < result.Dimension; i++)
			{
				result[i] = v1[i] - v2[i];
			}
			return result;
		}


		public double DistanceTo(Vector other)
		{
			if (Dimension != other.Dimension)
			{
				throw new ArgumentException("Vectors should have same dimension");
			}
			var ss = 0.0;
			for (var i = 0; i < Dimension; i++)
			{
				var d = this[i] - other[i];
				ss += d*d;
			}
			return Math.Sqrt(ss);
		}

		public override string ToString()
		{
			return string.Format("({0})", string.Join(", ", values.Select(v => v.ToString("0.0", numberFormat))));
		}

		private static readonly IFormatProvider numberFormat = CultureInfo.InvariantCulture.NumberFormat;
	}
}
