using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using D66.Hackathon.Model;

namespace D66.Hackathon.Data
{
	public class DataSource : IDataSource
	{
		public DataSource(string appDataPath)
		{
			this.appDataPath = appDataPath;
		}

		private readonly string appDataPath;

		public Election GetElection(string key)
		{
			lock (syncLock)
			{
				if (string.IsNullOrEmpty(key))
				{
					throw new ArgumentNullException("Key should not be null");
				}
				var path = Path.Combine(appDataPath, "Elections", string.Format("{0}.xml", key));
				if (!File.Exists(path))
				{
					return null;
				}
				using (var stream = File.OpenRead(path))
				{
					return Election.Deserialize(stream);
				}
			}
		}

		public void StoreElection(Election election)
		{
			lock (syncLock)
			{
				if (string.IsNullOrEmpty(election.Key))
				{
					throw new ArgumentNullException("Key should not be null");
				}
				var path = Path.Combine(appDataPath, "Elections", string.Format("{0}.xml", election.Key));
				using (var stream = File.Create(path))
				{
					election.Serialize(stream);
				}
				var index = GetIndex();
				var summary = index.Elections.SingleOrDefault(i => i.Key == election.Key);
				if (summary == null)
				{
					index.Elections.Add(summary = new ElectionSummary());
				}
				summary.Name = election.Name;
				summary.RegionCode = election.RegionCode;
				summary.Type = election.TypeCode;
				summary.SubdivisionType = election.SubdivisionTypeCode;
				StoreIndex(index);
			}
		}

		private void StoreIndex(Index index)
		{
			index.Serialize(Path.Combine(appDataPath, "Elections", "Index.xml"));
		}

		public IEnumerable<ElectionSummary> ListElections()
		{
			lock (syncLock)
			{
				return GetIndex().Elections.ToList();
			}
		}

		private Index GetIndex()
		{
			try
			{
				var index = Index.Deserialize(Path.Combine(appDataPath, "Elections", "Index.xml"));
				return index;
			}
			catch (Exception)
			{
				return new Index();
			}
		}

		private static object syncLock = new object();
	}

	public interface IDataSource
	{
		IEnumerable<ElectionSummary> ListElections();
		Election GetElection(string key);
		void StoreElection(Election election);
	}
}
