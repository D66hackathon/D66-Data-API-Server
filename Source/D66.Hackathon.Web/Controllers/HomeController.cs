using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using D66.Hackathon.API;
using D66.Hackathon.Data;
using D66.Hackathon.ViewModel.Home;

namespace D66.Hackathon.Web.Controllers
{
	public class HomeController: Controller
	{

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			this.data = new DataSource(Server.MapPath("~/App_Data"));
		}

		//
		// GET: /API/GetElection/{id}


		private IDataSource data;


		[HttpGet()]
		public ActionResult Index()
		{
			var model = new IndexViewModel
			{
				Elections = data.ListElections().ToList()
			};
			return View(model);
		}

	}
}