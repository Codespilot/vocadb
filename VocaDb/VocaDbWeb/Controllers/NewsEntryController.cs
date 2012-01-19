using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class NewsEntryController : Controller
    {
        //
        // GET: /NewsEntry/

        public ActionResult Index()
        {

			var entries = MvcApplication.Services.Other.GetNewsEntries(500);

            return View(entries);

        }

    }
}
