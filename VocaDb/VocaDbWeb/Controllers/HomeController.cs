using System.Web.Mvc;
using System.Web.SessionState;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{

	[SessionState(SessionStateBehavior.ReadOnly)]
    public class HomeController : ControllerBase
    {

		private readonly OtherService otherService;

		public HomeController(OtherService otherService) {
			this.otherService = otherService;
		}

		public ActionResult Chat() {

			return View();

		}

		public ActionResult FindNames(string term) {

			var result = Services.Other.FindNames(term);

			return Json(result);

		}

        //
        // GET: /Home/

        public ActionResult Index() {

			PageProperties.Description = "VocaDB is a Vocaloid music database with translated artists, albums and songs.";

			var contract = otherService.GetFrontPageContent();

            return View(contract);

        }

		[HttpPost]
		public ActionResult GlobalSearch(GlobalSearchBoxModel model) {

			switch (model.ObjectType) {
				case EntryType.Undefined:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm });

				case EntryType.Album:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				case EntryType.Artist:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				case EntryType.Song:
					return RedirectToAction("Index", "Search", new { filter = model.GlobalSearchTerm, searchType = model.ObjectType });

				default:
					var controller = model.ObjectType.ToString();
					return RedirectToAction("Index", controller, new {filter = model.GlobalSearchTerm});

			}


		}

		public ActionResult PVContent(int songId = invalidId) {

			if (songId == invalidId)
				return NoId();

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

		public ActionResult Search(string filter) {
			return RedirectToAction("Index", "Search", new { filter });
		}

		public ActionResult SetContentPreferenceCookie(ContentLanguagePreference languagePreference) {

			LoginManager.SetLanguagePreferenceCookie(languagePreference);

			if (LoginManager.HasPermission(PermissionToken.EditProfile))
				Services.Users.UpdateContentLanguagePreference(languagePreference);

			return Content(string.Empty);

		}

		public ActionResult Wiki() {
			return View();
		}

    }
}
