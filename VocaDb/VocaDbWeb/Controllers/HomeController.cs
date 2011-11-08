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

        	var result = MvcApplication.Services.Other.GetFrontPageContent();
        	ViewBag.Albums = result.LatestAlbums;
        	ViewBag.Songs = result.LatestSongs;

            return View();

        }

		[HttpPost]
		public ActionResult GlobalSearch(GlobalSearchBoxModel model) {

			return RedirectToAction("Index", model.ObjectType.ToString(), new {filter = model.GlobalSearchTerm});

		}

    }
}
