using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class HelpController : ControllerBase
    {

        //
        // GET: /Help/

        public ActionResult Index()
        {

			if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja")
				return View("Index.ja");
			else
				return View();
        }

    }
}
