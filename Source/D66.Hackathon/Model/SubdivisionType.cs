using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace D66.Hackathon.Model
{
	public class SubdivisionType
	{

		public string Key { get; set; }
		public string Name { get; set; }

		public static SubdivisionType Get(string key)
		{
			return All().SingleOrDefault(e => e.Key == key);
		}

		public static IEnumerable<SubdivisionType> All()
		{
			yield return Provincie;
			yield return Gemeente;
			yield return Stemlokaal;
		}

		public static readonly SubdivisionType Provincie = new SubdivisionType() { Key = "Provincie", Name = "Provincie" };
		public static readonly SubdivisionType Gemeente = new SubdivisionType() { Key = "Gemeente", Name = "Gemeente" };
		public static readonly SubdivisionType Stemlokaal = new SubdivisionType() { Key = "Stemlokaal", Name = "Stemlokaal" };

	}
}
