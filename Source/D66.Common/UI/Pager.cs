using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.UI
{
	public class Pager
	{
		public Pager()
		{
			Limit = 30;
		}

		public int Limit { get; set; }
		public int Page { get; set; }
		public string SortColumn { get; set; }
		public bool Descending { get; set; }
		public bool HasPrevious { get; private set; }
		public bool HasNext { get; private set; }

		public List<T> List<T>(IQueryable<T> query)
		{
			if (SortColumn != null)
			{
				if (Descending)
				{
					query = query.OrderByDescending(SortColumn);
				}
				else
				{
					query = query.OrderBy(SortColumn);
				}
			}
			if (Limit > 0)
			{
				query = query.Skip(Limit * Page).Take(Limit + 1);
			}
			var result = query.ToList();
			HasNext = result.Count > Limit;
			HasPrevious = Page > 0;
			return result.Take(Limit).ToList();
		} 
	}
}
