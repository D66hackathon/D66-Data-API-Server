using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using D66.Common;

namespace D66.Hackathon.Model
{
	public class Election
	{

		public Election()
		{
			Parties = new List<Party>();
			Regions = new List<Region>();
		}

		[XmlAttribute()]
		public string Key { get; set; }

		public string Name { get; set; }

		[XmlElement("Type")]
		public string TypeCode { get; set; }

		[XmlIgnore()]
		public ElectionType Type
		{
			get { return ElectionType.Get(TypeCode); }
			set
			{
				if (value == null)
				{
					TypeCode = null;
				}
				else
				{
					TypeCode = value.Key;
				}
			}
		}

		/// <summary>
		/// Code indicating the region:
		/// NL for the Netherlands
		/// GMxxxx for a gemeente
		/// </summary>
		public string RegionCode { get; set; }

		[XmlElement("SubdivisionType")]
		public string SubdivisionTypeCode { get; set; }

		[XmlIgnore()]
		public SubdivisionType SubdivisionType
		{
			get { return SubdivisionType.Get(SubdivisionTypeCode); }
			set
			{
				if (value == null)
				{
					SubdivisionTypeCode = null;
				}
				else
				{
					SubdivisionTypeCode = value.Key;
				}
			}

		}

	
		public List<Party> Parties { get; set; }

		public List<Region> Regions { get; set; }



		#region Serialization

		public void Serialize(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Election));
			serializer.Serialize(stream, this);
		}


		public static Election Deserialize(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Election));
			var result = (Election)serializer.Deserialize(stream);
			return result;
		}

		/// <summary>
		/// Creates a copy of this bundle
		/// </summary>
		/// <returns></returns>
		public Election Clone()
		{
			var ms = new MemoryStream();
			Serialize(ms);
			ms.Seek(0, SeekOrigin.Begin);
			return Deserialize(ms);
		}

		#endregion


		public int GetTotalVotes()
		{
			return
				this
					.Regions
					.Select(r => GetTotalVotes(r))
					.DefaultIfEmpty()
					.Sum();
		}

		public int GetTotalVotes(Region region)
		{
			return
				region
					.Result
					.Parties
					.Select(p => p.Votes)
					.DefaultIfEmpty()
					.Sum();

		}

		public int GetTotalVotes(Region region, Party party)
		{
			return
				region
					.Result
					.Parties
					.Where(p => p.Party == party.Key)
					.Select(p => p.Votes)
					.DefaultIfEmpty()
					.Sum();

		}


		public int GetTotalVotes(Party party)
		{
			return 
				Regions
				.Select(r => GetTotalVotes(r, party))
				.DefaultIfEmpty()
					.Sum();
		}

		public double GetPercentage(Region region, Party party)
		{
			var total = GetTotalVotes(region);
			if (total == 0)
			{
				return double.NaN;
			}
			var forParty = GetTotalVotes(region, party);
			return 100.0 * forParty / total;
		}

		public double GetPercentage(Party p)
		{
			var total = GetTotalVotes();
			if (total == 0)
			{
				return double.NaN;
			}
			var forParty = GetTotalVotes(p);
			return 100.0*forParty/total;
		}

		public Party GetParty(string key)
		{
			return Parties.SingleOrDefault(p => p.Key == key);
		}

	
	}

	

}
