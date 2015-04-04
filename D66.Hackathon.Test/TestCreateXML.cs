using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using D66.Common;
using D66.Common.IO;
using D66.Hackathon.Converters;
using D66.Hackathon.Data;
using D66.Hackathon.Model;
using D66.Hackathon.Uitslagen;
using D66.Hackathon.ViewModel.Import;
using NUnit.Framework;

namespace D66.Hackathon.Test
{
	[TestFixture()]
	public class TestCreateXML
	{

		[Test(), Ignore()]
		public void TestImportCSV()
		{
			var csvPath = @"D:\Documents\D66\Uitslagen\Grondwet_2005.txt";
			var service = new ImportService(new DataSource(@"D:\Data\D66\Hackathon"));
			var model = new CSVViewModel()
			{
				Separator = "\\t",
				ElectionKey = "REF_2005_0344",
				ElectionName = "Referendum europese grondwet 2005",
				ColumnFirstParty = "Voor",
				ColumnLastParty = "Tegen",
				ColumnBlank = "Blanco",
				ColumnKey = "#",
				ColumnName = "Stemdistrict"
			};
			using (var stream = File.OpenRead(csvPath))
			{
				var election = service.ConvertCSV(model, stream);
				using (var stream2 = File.Create(@"D:\Data\D66\Hackathon\Grondwet_2005.xml"))
				{
					election.Serialize(stream2);
				}
			}
		}

		[Test(), Ignore()]
		public void CreateXML()
		{
			var uitslag = CreateUitslag2014();
			var election = new ModelConverter().CreateElection("GR2014_0344", "Gemeenteraadsverkiezingen 2014 - Utrecht", uitslag);
			using (var stream = File.Create(@"C:\Projects\D66\Hackathon\D66.Hackathon\Data\GR2014_0344.xml"))
			{
				election.Serialize(stream);
			}
		}

		private Uitslag CreateUitslag2014()
		{
			var uitslag = new Uitslag();
			using(var reader = new SVReader(new StreamReader(GetType().Assembly.GetManifestResourceStream("D66.Hackathon.Test.GR2014_0344_Districten.txt"))) {Separator = '\t'})
			{
				var header = reader.ReadHeader();
				for(var i=8; i<=24; i++)
				{
					uitslag.AddPartij(header[i], header[i]);
				}
				foreach(var line in reader.ReadLines())
				{
					var district = line["districten"];
					var parts = district.Split('(');
					var nr = parts[1].Substring(0, 3);
					var name = parts[0].Trim();
					uitslag.AddStembureau(nr, name, line["wijk"], line["subwijk"], int.Parse(line["stemger."]), int.Parse(line["blanco"]), int.Parse(line["ongeldig"]), int.Parse(line["X"]), int.Parse(line["Y"]));
					foreach(var partij in uitslag.Partijen())
					{
						uitslag.AddUitslag(nr, partij.Key, int.Parse(line[partij.Key]));
					}
				}
			}

			uitslag.Tally();
			return uitslag;
		}

	}
}
