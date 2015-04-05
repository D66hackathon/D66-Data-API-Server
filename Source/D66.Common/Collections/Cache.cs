using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Collections
{
	public class Cache<T> : ICache<T> where T: class
	{

		public Cache(INowService nowService)
		{
			this.nowService = nowService;
			this.PurgeFrequency = TimeSpan.FromHours(1);
			this.MaxAge = TimeSpan.FromDays(1);
		}

		public TimeSpan MaxAge { get; set; }
		public TimeSpan PurgeFrequency { get; set; }

		private DateTime lastPurgeTime;
		private readonly INowService nowService;
		private readonly Dictionary<string, Item> items = new Dictionary<string, Item>(); 

		private class Item
		{
			public DateTime Timestamp { get; set; }
			public T Value { get; set; }
		}


		private void Purge()
		{
			var purgeBefore = nowService.Now.Subtract(MaxAge);
			foreach (var item in items.Where(v => v.Value.Timestamp < purgeBefore).ToList())
			{
				items.Remove(item.Key);
			}
		}

		public void Set(string key, T value)
		{
			CheckPurge();
			Item item;
			if(!items.TryGetValue(key, out item))
			{
				items.Add(key, item = new Item());
			}
			item.Timestamp = nowService.Now;
			item.Value = value;
		}

		public T Get(string key)
		{
			try
			{
				Item item;
				if (items.TryGetValue(key, out item))
				{
					item.Timestamp = nowService.Now;
					return item.Value;
				}
				return null;
			}
			finally
			{
				CheckPurge();
			}
		}

		private void CheckPurge()
		{
			if (lastPurgeTime.Add(PurgeFrequency) < nowService.Now)
			{
				lastPurgeTime = nowService.Now;
				Purge();
			}
		}
	}

	public interface ICache<T> where T: class
	{
		void Set(string key, T item);
		T Get(string key);
	}
}
