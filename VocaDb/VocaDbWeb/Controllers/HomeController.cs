using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
    public class HomeController : ControllerBase
    {

        //
        // GET: /Home/

        public ActionResult Index() {

			var contract = MvcApplication.Services.Other.GetFrontPageContent();

            return View(contract);

        }

		[HttpPost]
		public ActionResult GlobalSearch(GlobalSearchBoxModel model) {

			return RedirectToAction("Index", model.ObjectType.ToString(), new {filter = model.GlobalSearchTerm});

		}

    }
}
