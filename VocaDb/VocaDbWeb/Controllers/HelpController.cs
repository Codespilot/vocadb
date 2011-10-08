using System;
using System.Collections.Generic;
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
            return View();
        }

    }
}
