using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Collections
{
	public class ManyToManyMapping<T1, T2>
	{

		public IEnumerable<T1> Links(T2 item)
		{
			HashSet<T1> links;
			if(links2.TryGetValue(item, out links))
			{
				foreach(var l in links)
				{
					yield return l;
				}
			}
		}

		public int CountLinks(T2 item)
		{
			HashSet<T1> links;
			if (links2.TryGetValue(item, out links))
			{
				return links.Count;
			}
			return 0;
		}

		public IEnumerable<T2> Links(T1 item)
		{
			HashSet<T2> links;
			if (links1.TryGetValue(item, out links))
			{
				foreach (var l in links)
				{
					yield return l;
				}
			}
		}

		public int CountLinks(T1 item)
		{
			HashSet<T2> links;
			if (links1.TryGetValue(item, out links))
			{
				return links.Count;
			}
			return 0;
		}

		public void AddLink(T1 item1, T2 item2)
		{
			HashSet<T1> links2;
			HashSet<T2> links1;
			if (!this.links1.TryGetValue(item1, out links1))
			{
				this.links1.Add(item1, links1 = new HashSet<T2>());
			}
			if (!this.links2.TryGetValue(item2, out links2))
			{
				this.links2.Add(item2, links2 = new HashSet<T1>());
			}
			links1.Add(item2);
			links2.Add(item1);
		}

		private readonly Dictionary<T1, HashSet<T2>> links1 = new Dictionary<T1, HashSet<T2>>();
		private readonly Dictionary<T2, HashSet<T1>> links2 = new Dictionary<T2, HashSet<T1>>();

	}
}
