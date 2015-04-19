using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace D66.Common.Reflection
{
	public static class ReflectionHelper
	{

		public static PropertyInfo GetPropertyInfo<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			Type type = typeof (TSource);
			var member = propertyLambda.Body as MemberExpression;
			if (member == null)
			{
				throw new ArgumentException(string.Format(
					"Expression '{0}' refers to a method, not a property.",
					propertyLambda));
			}

			var propInfo = member.Member as PropertyInfo;
			if (propInfo == null)
			{
				throw new ArgumentException(string.Format(
					"Expression '{0}' refers to a field, not a property.",
					propertyLambda.ToString()));
			}

			if (type != propInfo.ReflectedType &&
			    !type.IsSubclassOf(propInfo.ReflectedType))
			{
				throw new ArgumentException(string.Format(
					"Expresion '{0}' refers to a property that is not from type {1}.",
					propertyLambda,
					type));
			}

			return propInfo;
		}


		public static string GetPropertyName<TSource>(Expression<Func<TSource, object>> propertyLambda)
		{
			return GetPropertyInfo(propertyLambda).Name;
		}
	}
}
