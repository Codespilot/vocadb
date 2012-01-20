using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Service;
using VocaDb.Web.Models.SongLists;

namespace VocaDb.Web.Controllers
{
    public class SongListController : ControllerBase
    {

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddSong(int songId) {

			var songContract = MvcApplication.Services.Songs.GetSongWithAdditionalNames(songId);
			var link = new SongInListEditContract(songContract);

			return PartialView("SongInListEditRow", link);

		}

		public ActionResult CreateFromWVR() {

			var model = new CreateFromWVR();
			return View(model);

		}

		[HttpPost]
		public ActionResult CreateFromWVR(CreateFromWVR model, bool commit) {

			if (commit) {
				var listId = MvcApplication.Services.Rankings.CreateSongListFromWVR(model.Url);
				return RedirectToAction("Details", "SongList", new { id = listId });
			}

			var parseResult = MvcApplication.Services.Rankings.ParseWVRList(model.Url);
			model.ListResult = parseResult;

			if (parseResult.IsIncomplete)
				ModelState.AddModelError("ListResult", "Some of the songs are missing");

			return View(model);

		}

		public ActionResult Delete(int id) {

			Service.DeleteSongList(id);

			return RedirectToAction("Profile", "User", new { id = LoginManager.LoggedUser.Name });

		}

		public ActionResult Details(int id) {

			var contract = Service.GetSongListDetails(id);

			return View(contract);

		}

        //
        // GET: /SongList/Edit/

        public ActionResult Edit(int? id)
        {

			var contract = id != null ? Service.GetSongListForEdit(id.Value) : new SongListForEditContract();
			var model = new SongListEdit(contract);

            return View(model);

        }

		[HttpPost]
		public ActionResult Edit(SongListEdit model) {

			if (!ModelState.IsValid) {
				return View(model);
			}

			var listId = Service.UpdateSongList(model.ToContract());

			return RedirectToAction("Details", new { id = listId });

		}

    }
}
