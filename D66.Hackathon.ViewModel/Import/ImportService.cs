using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using D66.Common.IO;
using D66.Hackathon.Data;
using D66.Hackathon.Model;

namespace D66.Hackathon.ViewModel.Import
{
	public class ImportService
	{
		public ImportService(IDataSource data)
		{
			this.data = data;
		}

		private IDataSource data;

		public void CSV(CSVViewModel model)
		{
			if (model.Separator == "\\t")
			{
				model.Separator = "\t";
			}
			if (model.Separator.Length != 1)
			{
				throw new ArgumentNullException("Separator should have 1 character");
			}
			var election = ConvertCSV(model, model.UploadedFile.InputStream);
			data.StoreElection(election);
		}

		public Election ConvertCSV(CSVViewModel model, Stream stream)
		{
			if (model.Separator == "\\t")
			{
				model.Separator = "\t";
			}
			if (model.Separator.Length != 1)
			{
				throw new ArgumentNullException("Separator should have 1 character");
			}
			using (var reader = new SVReader(new StreamReader(stream))
			{
				Separator = model.Separator[0],
				AutoQuote = true,
				QuoteCharacter = '"'
			})
			{
				var header = reader.ReadHeader();
				ValidateHeader(model, header);
				var lines = reader.ReadLines().ToList();
				var election = CreateElection(model, header, lines);
				return election;
			}
		}

		private Election CreateElection(CSVViewModel model, TableBase.Line header, List<TableBase.Line> lines)
		{
			var result = new Election()
			{
				Key = model.ElectionKey,
				Name = model.ElectionName
			};
			var startIndex = header.IndexOf(model.ColumnFirstParty);
			var endIndex = header.IndexOf(model.ColumnLastParty);

			for (var index = startIndex; index <= endIndex; index++)
			{
				var party = header.GetValue(index);
				result.Parties.Add(new Party()
				{
					Key = party,
					Name = party
				});
			}

			foreach (var line in lines)
			{
				var region = new Region()
				{
					Key = line[model.ColumnKey],
					Name = line[model.ColumnName],
				};

				int x, y;
				if (!string.IsNullOrEmpty(model.ColumnX) && int.TryParse(line[model.ColumnX], out x))
				{
					region.X = x;
				}
				if (!string.IsNullOrEmpty(model.ColumnY) && int.TryParse(line[model.ColumnY], out y))
				{
					region.Y = y;
				}

				region.Result = new RegionUitslag()
				{
					Parties = new List<RegionPartyResult>()
				};
				for (var index = startIndex; index <= endIndex; index++)
				{
					var party = header.GetValue(index);
					var votes = int.Parse(line[party]);
					region.Result.Parties.Add(new RegionPartyResult()
					{
						Party = party,
						Votes = votes
					});
				}

				result.Regions.Add(region);
			}
			return result;
		}

		private void ValidateHeader(CSVViewModel model, TableBase.Line header)
		{
			if (!header.ContainsValue(model.ColumnKey))
			{
				throw new KeyNotFoundException(string.Format("Column for key: {0}", model.ColumnKey));
			}	


		}


	}
}
