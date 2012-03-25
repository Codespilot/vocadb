using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
    public class EventController : ControllerBase
    {

		private AlbumService Service {
			get {
				return Services.Albums;
			}
		}

		/*public ActionResult Details(int index) {



		}*/

		public ActionResult EventsBySeries() {

			var events = Service.GetReleaseEventsBySeries();
			return View(events);

		}

        //
        // GET: /Event/

        public ActionResult Index()
        {
            return View();
        }

    }
}
