using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D66.Common.Collections
{
	public class KeyValueCollection<K, V> : List<KeyValuePair<K, V>>
	{

		public void Add(K key, V value)
		{
			Add(new KeyValuePair<K, V>(key, value));
		}

		/// <summary>
		/// Sets value as the ONLY value for the specified key. All existing values are discarded
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Set(K key, V value)
		{
			base.RemoveAll(kv => kv.Key.Equals(key));
			Add(key, value);
		}

	}
}
