using System.Web.Mvc;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Home;

namespace VocaDb.Web.Controllers
{
    public class HomeController : ControllerBase
    {

		public ActionResult FindNames(string filter) {

			var result = Services.Other.FindNames(filter);

			return Json(result);

		}

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

		public ActionResult Search(string filter) {

			filter = filter ?? string.Empty;
			var result = Services.Other.Find(filter, 15, true);
			var model = new SearchEntries(filter, result.Albums, result.Artists, result.Songs);

			return View(model);

		}

    }
}
