using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index(string filter)
        {
			return RedirectToAction("Search", "Home", new { filter });
        }

    }
}
