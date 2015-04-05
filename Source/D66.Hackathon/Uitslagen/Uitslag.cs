using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D66.Common.Mathmatics;

namespace D66.Hackathon.Uitslagen
{
	public class Uitslag
	{

		public void AddKandidaat(string partij, int nr, string naam)
		{
			var check = partijen[partij];
			kandidaten.Add(new Tuple<string, int>(partij, nr), new Kandidaat()
			{
				Partij = partij,
				Nr = nr,
				Name = naam
			});
		}

		public void AddPartij(string key, string name)
		{
			partijen.Add(key, new Partij() { Key = key, Name = name });
		}

		public void AddStembureau(string key, string name, string wijk, string subwijk, int stemgerechtigden, int blanco, int ongeldig, double x = 0, double y = 0)
		{
			stembureaus.Add(key, new Stembureau()
			{
				Key = key,
				Name = name,
				Wijk = wijk,
				Subwijk = subwijk,
				Stemgerechtigden = stemgerechtigden,
				Blanco = blanco,
				Ongeldig = ongeldig,
				X = x,
				Y = y
			});
		}


		public void AddUitslag(string stembureau, string partij, int stemmen)
		{
			partijen[partij].Stemmen += stemmen;
			stembureaus[stembureau].StemmenOpPartijen += stemmen;
			entries.Add(new Tuple<string, string>(stembureau, partij), new Entry()
			{
				Partij = partij,
				Stembureau = stembureau,
				Stemmen = stemmen
			});
		}

		public void AddVoorkeursUitslag(string stembureau, string partij, int plek, int stemmen)
		{
			kandidaten[new Tuple<string, int>(partij, plek)].Stemmen += stemmen;
			voorkeurEntries.Add(new Tuple<string, string, int>(stembureau, partij, plek), new VoorkeurEntry()
			{
				Stembureau = stembureau,
				Partij = partij,
				Nr = plek,
				Stemmen = stemmen
			});
		}

		/// <summary>
		/// Bereken rankings, z-scores, etc.
		/// </summary>
		public void Tally()
		{
			TallyPercentages();
			TallyZScores();
			TallyRanking();
		}

		private void TallyPercentages()
		{
			int stemmen = Partijen().Sum(p => p.Stemmen);
			foreach (var partij in Partijen())
			{
				partij.Percentage = 100.0 * partij.Stemmen / stemmen;
				foreach (var stembureau in Stembureaus())
				{
					var entry = this[stembureau, partij];
					entry.Percentage = 100.0 * entry.Stemmen / stembureau.StemmenOpPartijen;
				}
			}

			var kiesdeler = stemmen / 45.0;
			var voorkeursdrempel = kiesdeler / 4.0;
			foreach (var group in Kandidaten().GroupBy(k => k.Partij))
			{
				var partij = GetPartij(group.Key);
				foreach (var kandidaat in group)
				{
					kandidaat.Percentage = 100.0 * kandidaat.Stemmen / stemmen;
					kandidaat.PercentageBinnenPartij = 100.0 * kandidaat.Stemmen / partij.Stemmen;
					kandidaat.VoorkeursDrempel = kandidaat.Stemmen > voorkeursdrempel;

					foreach (var stembureau in Stembureaus())
					{
						var entry = this[stembureau, kandidaat];
						entry.Percentage = 100.0 * entry.Stemmen / stembureau.StemmenOpPartijen;
					}
				}
			}
		}

		private void TallyZScores()
		{
			foreach (var partij in Partijen())
			{
				var range = new StatRange(Entries(partij).Select(e => e.Percentage));
				foreach (var stembureau in Stembureaus())
				{
					var entry = this[stembureau, partij];
					entry.ZScore = (entry.Percentage - range.Average) / range.StandardDeviation;
				}
			}

			foreach (var kandidaat in Kandidaten())
			{
				var range = new StatRange(Entries(kandidaat).Select(e => e.Percentage));
				foreach (var stembureau in Stembureaus())
				{
					var entry = this[stembureau, kandidaat];
					entry.ZScore = (entry.Percentage - range.Average) / range.StandardDeviation;
				}
			}
		}

		public Partij GetPartij(string key)
		{
			return partijen[key];
		}

		private void TallyRanking()
		{
			foreach (var stembureau in Stembureaus())
			{
				var positie = 1;
				foreach (var g in Entries(stembureau).GroupBy(e => e.Stemmen).OrderByDescending(g => g.Key))
				{
					if (positie == 1)
					{
						stembureau.Grootste = string.Join(", ", g.Select(e => e.Partij));
					}
					foreach (var e in g)
					{
						e.RankingPartijInStembureau = positie;
					}
					positie += g.Count();
				}

				var grootste =
					voorkeurEntries.Where(kv => kv.Key.Item1 == stembureau.Key).OrderByDescending(e => e.Value.Stemmen).Select(kv => kv.Value).FirstOrDefault();
				if (grootste != null)
				{
					var kandidaat = kandidaten[new Tuple<string, int>(grootste.Partij, grootste.Nr)];

					stembureau.GrootsteKandidaat = string.Format("{0}. {1} ({2})", kandidaat.Nr, kandidaat.Name, kandidaat.Partij);
				}
			}

			foreach (var partij in Partijen())
			{
				var positie = 1;
				foreach (var g in Entries(partij).GroupBy(e => e.Percentage).OrderByDescending(g => g.Key))
				{
					foreach (var e in g)
					{
						e.RankingStembureauInStad = positie;
					}
					positie += g.Count();
				}
			}

			foreach (var kandidaat in Kandidaten())
			{
				var positie = 1;
				foreach (var g in Entries(kandidaat).GroupBy(e => e.Percentage).OrderByDescending(g => g.Key))
				{
					foreach (var e in g)
					{
						e.RankingStembureauInStad = positie;
					}
					positie += g.Count();
				}
			}
		}

		public Stembureau GetStembureau(string key)
		{
			Stembureau result;
			if (stembureaus.TryGetValue(key, out result))
			{
				return result;

			}
			return null;
		}

		public IEnumerable<Entry> Entries(Partij partij)
		{
			return entries.Where(e => e.Key.Item2 == partij.Key).Select(kv => kv.Value);
		}

		public IEnumerable<VoorkeurEntry> Entries(Kandidaat kandidaat)
		{
			return voorkeurEntries.Where(e => e.Key.Item2 == kandidaat.Partij && e.Key.Item3 == kandidaat.Nr).Select(kv => kv.Value);
		}

		public IEnumerable<Entry> Entries(Stembureau stembureau)
		{
			return entries.Where(e => e.Key.Item1 == stembureau.Key).Select(kv => kv.Value);
		}

		public Entry this[Stembureau stembureau, Partij partij]
		{
			get { return entries[new Tuple<string, string>(stembureau.Key, partij.Key)]; }
		}

		public VoorkeurEntry this[Stembureau stembureau, Kandidaat kandidaat]
		{
			get { return voorkeurEntries[new Tuple<string, string, int>(stembureau.Key, kandidaat.Partij, kandidaat.Nr)]; }
		}

		public IEnumerable<Partij> Partijen()
		{
			return partijen.Values;
		}

		public IEnumerable<Stembureau> Stembureaus()
		{
			return stembureaus.Values;
		}

		public IEnumerable<Kandidaat> Kandidaten()
		{
			return kandidaten.Values;
		}

		private readonly Dictionary<string, Stembureau> stembureaus = new Dictionary<string, Stembureau>();
		private readonly Dictionary<string, Partij> partijen = new Dictionary<string, Partij>();
		private readonly Dictionary<Tuple<string, int>, Kandidaat> kandidaten = new Dictionary<Tuple<string, int>, Kandidaat>();
		private readonly Dictionary<Tuple<string, string>, Entry> entries = new Dictionary<Tuple<string, string>, Entry>();
		private readonly Dictionary<Tuple<string, string, int>, VoorkeurEntry> voorkeurEntries = new Dictionary<Tuple<string, string, int>, VoorkeurEntry>();


		public class Partij
		{
			public string Key { get; set; }
			public string Name { get; set; }
			public int Stemmen { get; set; }
			public double Percentage { get; set; }
		}

		public class Stembureau
		{
			public string Key { get; set; }
			public string Name { get; set; }
			public string Wijk { get; set; }
			public string Subwijk { get; set; }
			public int Blanco { get; set; }
			public int Ongeldig { get; set; }
			public int StemmenOpPartijen { get; set; }
			public int StemmenTotaal { get { return StemmenOpPartijen + Blanco; } }
			public int Stemgerechtigden { get; set; }
			public string Grootste { get; set; }
			public string GrootsteKandidaat { get; set; }
			public double X { get; set; }
			public double Y { get; set; }

			public double Opkomst
			{
				get
				{
					if (Stemgerechtigden == 0)
					{
						return 0;
					}
					return 100.0 * StemmenTotaal / Stemgerechtigden;
				}
			}
		}

		public class Kandidaat
		{
			public string Partij { get; set; }
			public int Nr { get; set; }
			public string Name { get; set; }
			public int Stemmen { get; set; }
			public double Percentage { get; set; }
			public double PercentageBinnenPartij { get; set; }
			public bool VoorkeursDrempel { get; set; }
		}

		public class Entry
		{
			public string Stembureau { get; set; }
			public string Partij { get; set; }
			public int Stemmen { get; set; }

			/// <summary>
			/// Percentage in dit stembureau
			/// </summary>
			public double Percentage { get; set; }

			public int RankingStembureauInStad { get; set; }

			public int RankingPartijInStembureau { get; set; }

			/// <summary>
			/// Z-score percentage partij t.o.v. stad
			/// </summary>
			public double ZScore { get; set; }
		}

		public class VoorkeurEntry
		{
			public string Stembureau { get; set; }
			public string Partij { get; set; }
			public int Nr { get; set; }

			public int Stemmen { get; set; }

			/// <summary>
			/// Percentage in dit stembureau
			/// </summary>
			public double Percentage { get; set; }

			/// <summary>
			/// Ranking stembureau voor deze kandidaat
			/// </summary>
			public int RankingStembureauInStad { get; set; }

			/// <summary>
			/// Z-score percentage partij t.o.v. stad
			/// </summary>
			public double ZScore { get; set; }
		}

	}
}
