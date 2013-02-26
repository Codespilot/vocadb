using System.Web.Mvc;
using MvcPaging;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
    public class ActivityEntryController : Controller
    {

		private const int entriesPerPage = 50;

		private ActivityFeedService Service {
			get { return MvcApplication.Services.ActivityFeed; }
		}

		public ActionResult Entries(int page = 1) {

			var pageIndex = page - 1;
			var result = Service.GetActivityEntries(PagingProperties.CreateFromPage(pageIndex, entriesPerPage, false));

			return PartialView("ActivityEntryContracts", result.Items);

		}

		/*public ActionResult EntriesPaged(int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetActivityEntries(PagingProperties.CreateFromPage(pageIndex, entriesPerPage, true));
			var data = new PagingData<ActivityEntryContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), null, "Index", "activityEntriesPaged");

			return PartialView("ActivityEntriesPaged", data);

		}*/

		public ActionResult FollowedArtistActivity() {

			var result = Service.GetFollowedArtistActivity(entriesPerPage);
			return View(result.Items);

		}

        //
        // GET: /ActivityEntry/

        public ActionResult Index(int page = 1)
        {

			if (Request.IsAjaxRequest())
				return Entries(page);
			else {
				ViewBag.Page = page;
				return View();
			}

        }

    }
}
