using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly OtherService services;

		public SearchController(OtherService services) {
			this.services = services;
		}

		public ActionResult Index(string filter, EntryType searchType = EntryType.Undefined) {

			filter = filter ?? string.Empty;
			var result = services.Find(filter, 1, true);

			if (result.OnlyOneItem) {

				if (result.Albums.TotalCount == 1)
					return RedirectToAction("Details", "Album", new { id = result.Albums.Items[0].Id });

				if (result.Artists.TotalCount == 1)
					return RedirectToAction("Details", "Artist", new { id = result.Artists.Items[0].Id });

				if (result.Songs.TotalCount == 1)
					return RedirectToAction("Details", "Song", new { id = result.Songs.Items[0].Id });

				if (result.Tags.TotalCount == 1)
					return RedirectToAction("Details", "Tag", new { id = result.Tags.Items[0].Name });

			}

			ViewBag.Query = filter;
			ViewBag.SearchType = searchType;
			return View();

		}

    }
}
