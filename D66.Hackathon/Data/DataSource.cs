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

		public void StoreElection(Election election)
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
		}

		public IEnumerable<Election> ListElections()
		{
			foreach (var file in
				Directory
					.GetFiles(Path.Combine(appDataPath, "Elections"), "*.xml"))
			{
				using (var stream = File.OpenRead(file))
				{
					yield return Election.Deserialize(stream);
				}
			}
		}
	}

	public interface IDataSource
	{
		IEnumerable<Election> ListElections();
		Election GetElection(string key);
		void StoreElection(Election election);
	}
}
