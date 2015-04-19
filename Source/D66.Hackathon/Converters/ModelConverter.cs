using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D66.Common.Calendar;
using D66.Hackathon.Model;
using D66.Hackathon.Uitslagen;

namespace D66.Hackathon.Converters
{
	public class ModelConverter
	{

		public Election CreateElection(string key, string name, Uitslag uitslag)
		{
			return new Election()
			{
				Key = key,
				Name = name,
				Parties = CreateParties(uitslag).ToList(),
				Regions = CreateRegions(uitslag).ToList()
			};
		}

		private IEnumerable<Party> CreateParties(Uitslag uitslag)
		{
			return
				uitslag
					.Partijen()
					.Select(p => new Party()
					{
						Key = p.Key,
						Name = p.Name
					});
		}

		private IEnumerable<Region> CreateRegions(Uitslag uitslag)
		{
			return
				uitslag
					.Stembureaus()
					.Select(b => new Region()
					{
						Key = b.Key,
						Name = b.Name,
						X = (int)b.X,
						Y = (int)b.Y,
						Result = CreateRegionUitslag(uitslag, b)
					});
		}

		private RegionUitslag CreateRegionUitslag(Uitslag uitslag, Uitslag.Stembureau bureau)
		{
			return new RegionUitslag()
			{
				BlankVotes = bureau.Blanco,
				Invalid = bureau.Ongeldig,
				Parties =
					uitslag
						.Partijen()
						.Select(p => new RegionPartyResult()
						{
							Party = p.Key,
							Votes = uitslag[bureau, p].Stemmen
						})
						.ToList()
			};
		}

	}
}
