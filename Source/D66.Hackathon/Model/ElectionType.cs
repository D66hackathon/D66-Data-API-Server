using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace D66.Hackathon.Model
{
	public class ElectionType
	{

		public string Key { get; set; }
		public string Name { get; set; }

		public static ElectionType Get(string key)
		{
			return All().SingleOrDefault(e => e.Key == key);
		}

		public static IEnumerable<ElectionType> All()
		{
			yield return GR;
			yield return PS;
			yield return TK;
			yield return EP;
		} 

		public static readonly ElectionType GR = new ElectionType() { Key = "GR", Name = "Gemeenteraadsverkiezingen" };
		public static readonly ElectionType PS = new ElectionType() { Key = "PS", Name = "Provinciale Statenverkiezingen" };
		public static readonly ElectionType TK = new ElectionType() { Key = "TK", Name = "Tweede Kamerverkiezingen" };
		public static readonly ElectionType EP = new ElectionType() { Key = "EP", Name = "Europees Parlementsverkiezingen" };

	}
}
