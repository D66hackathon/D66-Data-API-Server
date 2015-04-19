using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class MultivariateLinearModel
	{

		public static MultivariateLinearModel Create<T>(IEnumerable<T> items, Func<T, double[]> getX, Func<T, double> getY)
		{
			var samples = 
				items
				.Select(i => new XY()
			            {
							X = getX(i), Y = getY(i)
			            })
				.ToList();
			if(samples.Count == 0) { throw new ArgumentException("Not enough samples"); }
			int n = samples.Count;
			int p = samples.First().X.Length;
			if(p == 0) { throw new ArgumentException(); }
			if(samples.Any(s => s.X.Length != p)) { throw new ArgumentException("Inconsistent length of X[]"); }

			// detect collinearity
			for(var i=0; i<p; i++)
			{
				for (var j = i + 1; j < p; j++)
				{
					var model = SimpleLinearModel.Create(samples, s => s.X[i], s => s.X[j]);
					if(model.R == 1)
					{
						return null; // Collinearity detected
					}
				}
			}

			var left = new Matrix(p + 1, p + 1);
			var right = new Matrix(p + 1, 1);
			for(int i=0;i<n; i++)
			{
				var x = new Matrix(p + 1, 1);
				x[0, 0] = 1;
				for(int k=1; k<=p; k++)
				{
					x[k, 0] = samples[i].X[k - 1];
				}
				var xt = x.Transpose();
				left += x*xt;
				right += x * samples[i].Y;
			}
			left /= n;
			left = left.Invert();

			if(left == null)
			{
				return null; // Matrix is not
			}

			right /= n;
			var b = left*right;
			var result = new MultivariateLinearModel()
			       {
					   Beta0 = b[0, 0],
					   Beta = Enumerable.Range(1, b.Rows - 1).Select(i => b[i, 0]).ToArray()
				   };
			var data = new StatRange(samples.Select(s => s.Y));
			var residual = new StatRange(samples.Select(s => s.Y - result.CalcY(s.X)));
			if (Math.Abs(residual.Average) > 1e-6)
			{
				return null;
			}
			var explainedVariance = data.Variance - residual.Variance;
			result.R = Math.Sqrt(explainedVariance/data.Variance);
			return result;
		}

		public static ModelSelection StepwiseSelection<T>(IEnumerable<T> items, Func<T, double[]> getX, Func<T, double> getY, Func<MultivariateLinearModel, bool> isAcceptable = null)
		{
			if(isAcceptable == null)
			{
				isAcceptable = m => true;
			}
			var samples =
				items
				.Select(i => new XY()
				{
					X = getX(i),
					Y = getY(i)
				})
				.ToList();
			if (samples.Count == 0) { throw new ArgumentException("Not enough samples"); }
			int n = samples.Count;
			int p = samples.First().X.Length;
			if (p == 0) { throw new ArgumentException(); }
			if (samples.Any(s => s.X.Length != p)) { throw new ArgumentException("Inconsistent length of X[]"); }

			var selection = new List<int>();
			MultivariateLinearModel bestModel = null;
			for(int i=0; i<p; i++)
			{
				var bestSelection = selection;
				bool changed = false;
				for(var j=0; j<p; j++)
				{
					var newSelection = selection.ToList();
					if(selection.Contains(j))
					{
						if(selection.Count > 1)
						{
							// try to remove item j from selection
							newSelection.Remove(j);
						}
					}
					else
					{
						// try to add variable j to selection
						newSelection.Add(j);
					}
					var newModel = TryFit(samples, newSelection);
					if(newModel != null)
					{
						if (isAcceptable(newModel))
						{
							if (bestModel == null || newModel.R > bestModel.R + 0.01)
							{
								changed = true;
								bestSelection = newSelection;
								bestModel = newModel;
							}
						}
					}
				}
				if(!changed)
				{
					break; // further iterations will not improve the model
				}
				selection = bestSelection;
			}
			var result = new ModelSelection()
			       {
			       	Indices = selection,
					Model = bestModel
			       };
			if(result.Model != null)
			{
				result.R = result.Model.R;
			}
			return result;
		}

		public class ModelSelection
		{
			public double R { get; set; }
			public List<int> Indices { get; set; }
			public MultivariateLinearModel Model { get; set; }
		}

		private static MultivariateLinearModel TryFit(List<XY> samples, List<int> indices)
		{
			return Create(samples, xy => indices.Select(i => xy.X[i]).ToArray(), xy => xy.Y);
		}

		internal class XY
		{
			public double[] X { get; set; }
			public double Y { get; set; }
		}


		public double CalcY(double[] x)
		{
			var result = Beta0 + Beta.Select((b, i) => b * x[i]).Sum();
			return result;
		}

		/// <summary>
		/// Intercept
		/// </summary>
		public double Beta0 { get; private set; }
		public double[] Beta { get; private set; }
		public double R { get; private set; }


	}



}
