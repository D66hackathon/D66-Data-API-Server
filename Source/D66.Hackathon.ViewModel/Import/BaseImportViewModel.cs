using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace D66.Hackathon.ViewModel.Import
{
	public class BaseImportViewModel
	{
		[Required()]
		public string ElectionKey { get; set; }

		[Required()]
		public string ElectionName { get; set; }





	}
}
