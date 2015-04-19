using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace D66.Hackathon.ViewModel.Import
{
	public class CSVViewModel : BaseImportViewModel
	{
		[Required()]
		public string Separator { get; set; }

		[Required()]
		public string ColumnKey { get; set; }

		[Required()]
		public string ColumnName { get; set; }

		public string ColumnX { get; set; }

		public string ColumnY { get; set; }

		public string ColumnInvalid { get; set; }

		public string ColumnBlank { get; set; }

		[Required()]
		public string ColumnFirstParty { get; set; }

		[Required()]
		public string ColumnLastParty { get; set; }

		[Display(Name = "CSV bestand (.txt, .csv)")]
		[ValidateAttachment(Extensions = ".txt,.csv", MaxSize = 50 * 1024 * 1024)]
		public HttpPostedFileBase UploadedFile { get; set; }

	}
}
