using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class ActivityEntryController : Controller
    {
        //
        // GET: /ActivityEntry/

        public ActionResult Index()
        {

			var entries = MvcApplication.Services.Other.GetActivityEntries(500);

			return View(entries);

        }

    }
}
