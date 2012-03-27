using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.ReleaseEvents;

namespace VocaDb.Web.Controllers
{
    public class EventController : ControllerBase
    {

		private ReleaseEventService Service {
			get {
				return Services.ReleaseEvents;
			}
		}

		/*public ActionResult Details(int index) {



		}*/

		[HttpPost]
		public PartialViewResult AliasForSeries(string name) {

			return PartialView(name);

		}

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

		public ActionResult SeriesEdit(int? id) {

			var series = (id != null ? Service.GetReleaseEventSeriesForEdit(id.Value) : new ReleaseEventSeriesForEditContract());
			return View(series);

		}

		[HttpPost]
		public ActionResult SeriesEdit(ReleaseEventSeriesForEditContract model) {

			Service.UpdateSeries(model);

			return RedirectToAction("EventsBySeries");

		}

    }
}
