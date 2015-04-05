using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using D66.Hackathon.API;
using D66.Hackathon.Data;
using D66.Hackathon.ViewModel.Import;

namespace D66.Hackathon.Web.Controllers
{
    public class UploadController : Controller
    {
		public UploadController()
	    {
	    }

	    protected override void Initialize(RequestContext requestContext)
	    {
		    base.Initialize(requestContext);
			this.upload = new ImportService(new DataSource(Server.MapPath("~/App_Data")));
		}

	    //
        // GET: /API/GetElection/{id}

	    
	    private ImportService upload;

		[HttpGet()]
	    public ActionResult CSV()
	    {
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
			return View(model);
	    }

		[HttpPost()]
		public ActionResult CSV(CSVViewModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					upload.CSV(model);
					return RedirectToAction("Index", "Home");
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}
			return View(model);
		}

    }
}
