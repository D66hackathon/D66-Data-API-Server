using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace D66.Common
{
	public static partial class Extensions
	{

		public static IEnumerable<T[]> NChooseK<T>(this List<T> items, int minCount, int maxCount)
		{
			return Enumerable.Range(minCount, maxCount + 1 - minCount).SelectMany(c => items.NChooseK(c));
		}

		public static IEnumerable<T[]> NChooseK<T>(this List<T> items, int count)
		{
			if(count > items.Count)
			{
				throw new InvalidOperationException();
			}
			var result = new T[count];
			foreach(var r in FillPermutation(items, result, 0, 0))
			{
				yield return r;
			}
		}

		public static IEnumerable<T> Apply<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach(var item in items)
			{
				action(item);
				yield return item;
			}
		}

		private static IEnumerable<T[]> FillPermutation<T>(List<T> items, T[] result, int currentIndex, int done)
		{
			var left = result.Length - done;
			if(left == 0)
			{
				yield return result;
				yield break;
			}
			if (currentIndex + left > items.Count)
			{
				yield break;
			}
			for(int i=currentIndex; i<items.Count; i++)
			{
				result[done] = items[i];
				foreach(var r in FillPermutation(items, result, i + 1, done + 1))
				{
					yield return r;
				}
			}
		}

		public static TimeSpan Sum(this IEnumerable<TimeSpan> items)
		{
			return TimeSpan.FromHours(items.SumOrDefault(t => t.TotalHours));
		}


		public static TimeSpan Sum<T>(this IEnumerable<T> items, Func<T, TimeSpan> getValue)
		{
			return items.Select(getValue).Sum();
		}


		public static TimeSpan SumOrDefault<T>(this IEnumerable<T> items, Func<T, TimeSpan> getValue)
		{
			return items.Select(getValue).DefaultIfEmpty().Sum();
		}

		public static int SumOrDefault<T>(this IEnumerable<T> items, Func<T, int> getValue)
		{
			return items.Select(getValue).DefaultIfEmpty().Sum();
		}
		public static double SumOrDefault<T>(this IEnumerable<T> items, Func<T, double> getValue)
		{
			return items.Select(getValue).DefaultIfEmpty().Sum();
		}
		public static decimal SumOrDefault<T>(this IEnumerable<T> items, Func<T, decimal> getValue)
		{
			return items.Select(getValue).DefaultIfEmpty().Sum();
		}

		public static IEnumerable<IGrouping<TKey, TValue>> GroupSequential<TKey, TValue>(this IEnumerable<TValue> items, Func<TValue, TKey> getKey)
		{
			Grouping<TKey, TValue> current = null;
			foreach(var value in items)
			{
				var key = getKey(value);
				if(current == null)
				{
					current = new Grouping<TKey, TValue>(key);
				}
				else if(!object.Equals(key, current.Key))
				{
					yield return current;
					current = new Grouping<TKey, TValue>(key);
				}
				current.Add(value);
			}
			if(current != null)
			{
				yield return current;
			}
		} 

		public class Grouping<TKey, TValue> : IGrouping<TKey, TValue>
		{
			public Grouping(TKey key)
			{
				this.key = key;
				this.values = new List<TValue>();
			}

			private readonly TKey key;
			private readonly List<TValue> values; 

			public TKey Key
			{
				get { return key; }
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return values.GetEnumerator();
			}

			internal void Add(TValue value)
			{
				values.Add(value);
			}
		}

		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
		{
			return ApplyOrder<T>(source, property, "OrderBy");
		}

		public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
		{
			return ApplyOrder<T>(source, property, "OrderByDescending");
		}

		public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
		{
			return ApplyOrder<T>(source, property, "ThenBy");
		}

		public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
		{
			return ApplyOrder<T>(source, property, "ThenByDescending");
		}

		static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
		{
			string[] props = property.Split('.');
			Type type = typeof(T);
			ParameterExpression arg = Expression.Parameter(type, "x");
			Expression expr = arg;
			foreach (string prop in props)
			{
				// use reflection (not ComponentModel) to mirror LINQ
				var pi = type.GetProperty(prop);
				if(pi == null)
				{
					throw new InvalidOperationException("Unknown property: " + property);
				}
				expr = Expression.Property(expr, pi);
				type = pi.PropertyType;
			}
			Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
			LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

			object result = typeof(Queryable).GetMethods().Single(
					method => method.Name == methodName
							&& method.IsGenericMethodDefinition
							&& method.GetGenericArguments().Length == 2
							&& method.GetParameters().Length == 2)
					.MakeGenericMethod(typeof(T), type)
					.Invoke(null, new object[] { source, lambda });
			return (IOrderedQueryable<T>)result;
		} 

	}
}
