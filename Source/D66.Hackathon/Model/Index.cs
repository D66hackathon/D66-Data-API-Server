using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace D66.Hackathon.Model
{
	[Serializable()]
	public class Index
	{
		public Index()
		{
			Elections = new List<ElectionSummary>();
		}

		public List<ElectionSummary> Elections { get; set; }

		#region Serialization

		public void Serialize(string path)
		{
			using (var stream = File.Create(path))
			{
				Serialize(stream);
			}
		}

		public void Serialize(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Index));
			serializer.Serialize(stream, this);
		}

		public static Index Deserialize(string path)
		{
			using(var stream = File.OpenRead(path))
			{
				return Deserialize(stream);
			}
		}

		public static Index Deserialize(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(Index));
			var result = (Index)serializer.Deserialize(stream);
			return result;
		}

		/// <summary>
		/// Creates a copy of this bundle
		/// </summary>
		/// <returns></returns>
		public Index Clone()
		{
			var ms = new MemoryStream();
			Serialize(ms);
			ms.Seek(0, SeekOrigin.Begin);
			return Deserialize(ms);
		}

		#endregion
	}

	[Serializable()]
	public class ElectionSummary
	{
		[XmlAttribute()]
		public string Key { get; set; }
		[XmlAttribute()]
		public string Name { get; set; }
		[XmlAttribute(DataType = "date")]
		public DateTime Date { get; set; }
		[XmlAttribute()]
		public string RegionCode { get; set; }
		[XmlAttribute()]
		public string Type { get; set; }
		[XmlAttribute()]
		public string SubdivisionType { get; set; }

	}
}
