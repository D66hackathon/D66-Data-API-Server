using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using D66.Hackathon.API;
using D66.Hackathon.Data;

namespace D66.Hackathon.Web.Controllers
{
    public class APIController : Controller
    {
	    public APIController()
	    {
	    }

	    protected override void Initialize(RequestContext requestContext)
	    {
		    base.Initialize(requestContext);
			this.api = new APIService(new DataSource(Server.MapPath("~/App_Data")));
		}

	    //
        // GET: /API/GetElection/{id}

	    
	    private APIService api;

		public ActionResult ListElections()
		{
			var result = api.GetElections();
			return Json(result, JsonRequestBehavior.AllowGet);
		}


        public ActionResult GetElection(string id)
        {
	        var result = api.GetElection(id);
	        if (result == null)
	        {
		        HttpNotFound();
	        }
	        return Json(result, JsonRequestBehavior.AllowGet);
        }

		public ActionResult GetRegions(string id)
		{
			var result = api.GetRegions(id);
			if (result == null)
			{
				HttpNotFound();
			}
			return Json(result, JsonRequestBehavior.AllowGet);
		}

    }
}
