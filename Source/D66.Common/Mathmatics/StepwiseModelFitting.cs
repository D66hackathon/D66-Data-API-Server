using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class StepwiseModelFitting
	{


		public List<Sample> Samples { get; set; }

		public List<string> Variables { get; set; }

		public LinearModel Fit(Func<LinearModel, bool> isAcceptable = null)
		{
			if(isAcceptable == null)
			{
				isAcceptable = model => true;
			}
			bool improved;
			var bestSelection = new HashSet<string>();
			LinearModel best = null;
			do
			{
				improved = false;
				foreach(var variable in Variables)
				{
					var newSelection = new HashSet<string>(bestSelection);
					if (newSelection.Contains(variable))
					{
						// try to remove
						if(newSelection.Count > 1)
						{
							newSelection.Remove(variable);
						}
						else
						{
							continue;
						}
					}
					else
					{
						newSelection.Add(variable);
					}
					var model = new LinearModel()
					            {
					            	Variables = newSelection.ToList()
					            };
					if (model.Fit(Samples) && isAcceptable(model))
					{
						if(best == null || model.R > best.R)
						{
							best = model;
							bestSelection = newSelection;
							improved = true;
						}
					}
				}
			}
			while (improved);
			return best;
		}



	}
}
