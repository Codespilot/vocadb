using System.Web.Mvc;
using System.Web.SessionState;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Home;

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

			PageProperties.Description = "VocaDB is a Vocaloid database with translated artists, albums and songs.";

			var contract = otherService.GetFrontPageContent();

            return View(contract);

        }

		[HttpPost]
		public ActionResult GlobalSearch(GlobalSearchBoxModel model) {

			string action, controller;
			if (model.ObjectType != EntryType.Undefined) {
				action = "Index";
				controller = model.ObjectType.ToString();
			} else {
				action = "Search";
				controller = "Home";
			}

			return RedirectToAction(action, controller, new {filter = model.GlobalSearchTerm});

		}

		public ActionResult PVContent(int songId = invalidId) {

			if (songId == invalidId)
				return NoId();

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

		public ActionResult Search(string filter) {

			filter = filter ?? string.Empty;
			var result = Services.Other.Find(filter, 15, true);

			if (result.OnlyOneItem) {

				if (result.Albums.Items.Length == 1)
					return RedirectToAction("Details", "Album", new { id = result.Albums.Items[0].Id });

				if (result.Artists.Items.Length == 1)
					return RedirectToAction("Details", "Artist", new { id = result.Artists.Items[0].Id });

				if (result.Songs.Items.Length == 1)
					return RedirectToAction("Details", "Song", new { id = result.Songs.Items[0].Id });

				if (result.Tags.Items.Length == 1)
					return RedirectToAction("Details", "Tag", new { id = result.Tags.Items[0].Name });

			}

			var model = new SearchEntries(filter, result.Albums, result.Artists, result.Songs, result.Tags);

			return View(model);

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
