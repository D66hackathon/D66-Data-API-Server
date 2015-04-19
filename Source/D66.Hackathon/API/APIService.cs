using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D66.Common.Geo;
using D66.Common.Mathmatics;
using D66.Hackathon.API.ListElections;
using D66.Hackathon.Data;

namespace D66.Hackathon.API
{
	public class APIService
	{

		public APIService(IDataSource data)
		{
			this.data = data;
		}

		private readonly IDataSource data;

		public GetElection.Result GetElection(string key)
		{
			var election = data.GetElection(key);
			if (election == null)
			{
				return null;
			}
			return new GetElection.Result()
			{
				Key = election.Key,
				Name = election.Name,
				Parties = 
					election
					.Parties
					.OrderBy(p => p.Key)
					.Select(p => new GetElection.Party()
					{
						Key = p.Key,
						Name = p.Name,
						Votes = election.GetTotalVotes(p),
						Percentage = election.GetPercentage(p)
					})
					.ToList()
			};
		}

		private readonly Projection projection = new RijksDriehoeksProjection();

		public GetRegions.Result GetRegions(string key)
		{
			var election = data.GetElection(key);
			if (election == null)
			{
				return null;
			}
			return new GetRegions.Result()
			{
				Key = election.Key,
				Name = election.Name,
				Regions = 
					election
					.Regions
					.Select(r => new GetRegions.Region()
					{
						Key = r.Key,
						Name = r.Name,
						X = r.X,
						Y = r.Y,
						Latitude = projection.ToLatLng(new Vector2(r.X, r.Y)).Latitude,
						Longitude = projection.ToLatLng(new Vector2(r.X, r.Y)).Longitude,
						Parties = 
							r
							.Result
							.Parties
							.OrderBy(p => p.Party)
							.Select(p => CreateEntry(election, r,  p))
							.ToList()
					})
					.ToList()
			};
		}

		private GetRegions.PartyEntry CreateEntry(Model.Election election, Model.Region region, Model.RegionPartyResult party)
		{
			var electionParty = election.GetParty(party.Party);
			var result = new GetRegions.PartyEntry()
			{
				Key = party.Party,
				Name = electionParty.Name,
				Votes = election.GetTotalVotes(region, electionParty),
				Percentage = election.GetPercentage(region, electionParty)
			};
			return result;
		}


		public ListElections.Result GetElections()
		{
			return new Result()
			{
				Elections =
					data
					.ListElections()
					.Select(e => new ListElections.Election()
					{
						Key = e.Key,
						Name = e.Name
					}).ToList()
			};
		}
	}
}
