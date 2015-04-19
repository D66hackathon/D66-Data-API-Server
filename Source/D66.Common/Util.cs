using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common
{
	public static class Util
	{

		public static void CopyProperties<TSource, TTarget>(TSource source, TTarget target)
		{
			var sourceProperties =
				typeof (TSource)
					.GetProperties()
					.Where(p => p.CanRead)
					.ToDictionary(p => p.Name);
			foreach (var targetProperty in 
					typeof (TTarget)
					.GetProperties()
					.Where(p => p.CanWrite))
			{
				var sourceProperty =
					sourceProperties.TryGet(targetProperty.Name);
				if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
				{
					var sourceValue = sourceProperty.GetValue(source, null);
					targetProperty.SetValue(target, sourceValue, null);
				}
			}
		}

	}
}
