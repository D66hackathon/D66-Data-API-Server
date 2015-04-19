using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Mathmatics
{
	public static class GenericMathExtensions
	{

		public static T GenericSum<T>(this IEnumerable<T> items)
		{
			var result = Operator<T>.Zero;
			foreach(var item in items)
			{
				result = Operator<T>.Add(result, item);
			}
			return result;
		}

	}
}
