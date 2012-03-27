using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Web.Models.Event;

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

			return PartialView("AliasForSeries", name);

		}

		public ActionResult Delete(int id) {

			Service.DeleteEvent(id);

			return RedirectToAction("EventsBySeries");

		}

		public ActionResult DeleteSeries(int id) {

			Service.DeleteSeries(id);

			return RedirectToAction("EventsBySeries");

		}

		public ActionResult Details(int id) {

			var ev = Service.GetReleaseEventDetails(id);
			return View(ev);

		}

		public ActionResult Edit(int? id, int? seriesId) {

			var model = (id != null ? new EventEdit(Service.GetReleaseEventForEdit(id.Value)) 
				: new EventEdit(seriesId != null ? Service.GetReleaseEventSeriesForEdit(seriesId.Value): null));

			return View(model);

		}

		[HttpPost]
		public ActionResult Edit(EventEdit model) {

			if (!ModelState.IsValid) {
				var contract = Service.GetReleaseEventForEdit(model.Id);
				model.CopyNonEditableProperties(contract);
				return View(model);
			}

			Service.UpdateEvent(model.ToContract());

			return RedirectToAction("EventsBySeries");

		}

		public ActionResult EditSeries(int? id) {

			var contract = (id != null ? Service.GetReleaseEventSeriesForEdit(id.Value) : new ReleaseEventSeriesForEditContract());
			return View(new SeriesEdit(contract));

		}

		[HttpPost]
		public ActionResult EditSeries(SeriesEdit model) {

			if (!ModelState.IsValid) {
				return View(model);
			}

			Service.UpdateSeries(model.ToContract());

			return RedirectToAction("EventsBySeries");

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

    }
}
