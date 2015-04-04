using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace D66.Common.Mathmatics
{
	public class Tally<T>
	{

		public int ItemCountIncludingOther {
			get
			{
				return this.list.Count;
			} 
		}

		/// <summary>
		/// The number of distinct items
		/// </summary>
		public int TotalItemCount
		{
			get
			{
				return this.entries.Count;
			}
		}

		public T Total
		{
			get
			{
				UpdateTotals();
				return _Total;
			}
			private set { _Total = value; }
		}

		private T _Total;

		private bool areTotalsDirty = true;

		public void Add(string key, T value, string title = null)
		{
			Entry entry;
			if (!entries.TryGetValue(key, out entry))
			{
				entries.Add(key, entry = new Entry(key, title ?? key));
				list.Add(entry);
			}
			entry.Value = Operator.Add(entry.Value, value);
			areTotalsDirty = true;
		}

		private void UpdateTotals()
		{
			if (areTotalsDirty)
			{
				areTotalsDirty = false;
				Total =
					list
					.Select(i => i.Value)
					.Aggregate(Zero, (current, value) => Operator.Add(current, value));
				var totalDouble = Operator.Convert<T, double>(Total);
				foreach (var item in list)
				{
					if (Operator.Equal(Total, Zero))
					{
						item.Percentage = 0;
					}
					else
					{
						item.Percentage = 100.0 * Operator.Convert<T, double>(item.Value) / totalDouble;
					}
				}
			}
		}

		public Tally<T> SortByKey()
		{
			list =
				list
					.OrderBy(i => i.Key)
					.ToList();
			return this;
		}

		public Tally<T> SortByTitle()
		{
			list =
				list
					.OrderBy(i => i.Title)
					.ThenByDescending(i => i.Value)
					.ThenBy(i => i.Key)
					.ToList();
			return this;
		}

		public Tally<T> SortByValue()
		{
			list =
				list
					.OrderByDescending(i => i.Value)
					.ThenBy(i => i.Title)
					.ThenBy(i => i.Key)
					.ToList();
			return this;
		}

		public Tally<T> SortBy(Func<Entry, object> getSortKey, bool descending = false)
		{
			if (descending)
			{
				list = list.OrderByDescending(getSortKey).ToList();
			}
			else
			{
				list = list.OrderBy(getSortKey).ToList();
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Entry this[string key]
		{
			get
			{
				UpdateTotals();
				return entries.Get(key);
			}
		}

		public Entry TryGet(string key)
		{
			UpdateTotals();
			return entries.TryGet(key);
		}

		public IEnumerable<Entry> Entries()
		{
			UpdateTotals();
			return list.ToList();
		} 

		private List<Entry> list = new List<Entry>();
		private Dictionary<string, Entry> entries = new Dictionary<string, Entry>();
		private static readonly T Zero = Operator<T>.Zero;

		public class Entry
		{
			public Entry(string key, string title)
			{
				this.Key = key;
				this.Title = title;
			}
			public string Key { get; private set; }
			public string Title { get; private set; }
			public T Value { get; internal set; }
			public double Percentage { get; internal set; }
			public override string ToString()
			{
				return Title;
			}

			public List<Entry> Children { get; set; }
		}



		public Tally<T> LimitCategoryCount(int maxCategories, string otherTitle = "Other")
		{
			if (ItemCountIncludingOther > maxCategories - 1)
			{
				UpdateTotals();
				var other = new Entry(OtherKey, otherTitle)
				{
					Children = new List<Entry>()
				};
				foreach (var entry in entries.Values.OrderByDescending(v => v.Value))
				{
					if (maxCategories-- <= 0)
					{
						list.Remove(entry);
						other.Children.Add(entry);
						other.Percentage += entry.Percentage;
						other.Value = Operator.Add(other.Value, entry.Value);
					}
				}
				if (other.Children.Any())
				{
					if (other.Children.Count == 1)
					{
						list.Add(other.Children.Single());
					}
					else
					{
						list.Add(other);
					}
				}

			}
			return this;
		}

		public Tally<T> LimitCategoryPercentage(double minPercentage, string otherTitle = "Other")
		{
			UpdateTotals();
			var other = new Entry(OtherKey, "Other")
			{
				Children = new List<Entry>()
			};
			foreach (var entry in entries.Values.OrderByDescending(v => v.Value))
			{
				if (entry.Percentage < minPercentage)
				{
					list.Remove(entry);
					other.Children.Add(entry);
					other.Percentage += entry.Percentage;
					other.Value = Operator.Add(other.Value, entry.Value);
				}
			}
			if (other.Children.Any())
			{
				if (other.Children.Count == 1)
				{
					list.Add(other.Children.Single());
				}
				else
				{
					list.Add(other);
				}
			}
			return this;
		}

		public static readonly string OtherKey = "_OTHER";


	}

	public static class TallyExtensions
	{
		public static Tally<T> Tally<K, T>(this IEnumerable<K> items, Func<K, string> getKey, Func<K, T> getValue,
			Func<K, string> getTitle = null)
		{
			if (getTitle == null)
			{
				getTitle = i => (i == null) ? "" : i.ToString();
			}
			var result = new Tally<T>();
			foreach (var item in items)
			{
				result.Add(
					getKey(item), 
					getValue(item), 
					getTitle(item));
			}
			return result;
		} 
	}

}
