using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service;
using VocaDb.Web.Models;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.DataContracts;
using VocaDb.Web.Models.Song;
using System;
using VocaDb.Model.Helpers;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Controllers
{
    public class SongController : ControllerBase
    {

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

		[HttpPost]
		public PartialViewResult CreateSongLink(int? songId) {

			SongWithAdditionalNamesContract song;

			if (songId == null)
				song = new SongWithAdditionalNamesContract();
			else
				song = Service.GetSongWithAdditionalNames(songId.Value);

			return PartialView("SongLink", song);

		}

        //
        // GET: /Song/

		public ActionResult Index(string filter, bool? draftsOnly, int? page) {

			var result = Service.Find(filter, ((page ?? 1) - 1) * 30, 30, draftsOnly ?? false, true, NameMatchMode.Auto, false, null);

			var model = new Index(result, filter, draftsOnly, page);

        	return View(model);

        }

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3) {

			var result = Service.FindFirst(new[] { term1, term2, term3 });

			if (result != null) {
				return PartialView("DuplicateEntryMessage",
					new KeyValuePair<string, string>(result.Name,
						Url.Action("Details", new { id = result.Id })));
			} else {
				return Content("Ok");
			}

		}

		public ActionResult FindJsonByName(string term, bool alwaysExact = false, int[] ignoredIds = null) {

			var songs = Service.Find(term, 0, 20, 
				draftOnly: false, 
				getTotalCount: false, 
				onlyByName: true, 
				nameMatchMode: (alwaysExact ? NameMatchMode.Exact : NameMatchMode.Auto), 
				ignoredIds: ignoredIds);

			return Json(songs);

		}

        //
        // GET: /Song/Details/5

        public ActionResult Details(int id) {
        	var model = new SongDetails(Service.GetSongDetails(id));
            return View(model);
        }

        //
        // POST: /Song/Create

		[Obsolete("Disabled")]
		[HttpPost]
		public ActionResult CreateQuick(ObjectCreate model)
        {

			if (ModelState.IsValid) {

				var song = Service.Create(model.Name);
				return RedirectToAction("Edit", new { id = song.Id });

			} else {

				return RedirectToAction("Index");

			}

        }

		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", "Need at least one name.");

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", "Need at least one artist.");

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			try {
				var song = Service.Create(contract);
				return RedirectToAction("Details", new { id = song.Id });
			} catch (VideoParseException x) {
				ModelState.AddModelError("PVUrl", x);
				return View(model);
			}

		}
       
        //
        // GET: /Song/Edit/5 
        public ActionResult Edit(int id)
        {
			var model = new SongEdit(Service.GetSongForEdit(id));
			return View(model);
		}

        //
        // POST: /Song/Edit/5
        [HttpPost]
        public ActionResult Edit(SongEdit model) {

			/*foreach (var link in model.WebLinks) {
				if (!UrlValidator.IsValid(link.Url))
					ModelState.AddModelError("WebLinks", link.Url + " is not a valid URL.");
			}*/

			if (!ModelState.IsValid) {
				var oldContract = Service.GetSongForEdit(model.Id);
				model.CopyNonEditableFields(oldContract);
				return View(model);				
			}

			var contract = model.ToContract();
			Service.UpdateBasicProperties(contract);

			return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingArtist(int songId, int artistId) {

			//var link = Service.AddArtist(songId, artistId);
			var artist = MvcApplication.Services.Artists.GetArtistWithAdditionalNames(artistId);
			var link = new ArtistForSongContract(artist);
			return PartialView("ArtistForSongEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete]
		public PartialViewResult AddNewArtist(int songId, string newArtistName) {

			var link = Service.AddArtist(songId, newArtistName);
			return PartialView("ArtistForSongEditRow", link);

		}

		[Obsolete("Integrated to saving properties")]
		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteArtistForSong(int artistForSongId) {

			Service.DeleteArtistForSong(artistForSongId);

		}

		[Obsolete("Integrated to saving properties")]
		public PartialViewResult CreatePVForSong(int songId, PVService service, string pvId, PVType type) {

			ParamIs.NotNullOrEmpty(() => pvId);

			var contract = Service.CreatePVForSong(songId, service, pvId, type);

			return PartialView("PVForSongEditRow", contract);

		}

		[HttpPost]
		public ActionResult CreatePVForSongByUrl(int songId, string pvUrl, PVType type) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			try {

				var result = VideoServiceHelper.ParseByUrl(pvUrl);
				var contract = new PVContract(result, type);

				//var contract = Service.CreatePVForSong(songId, pvUrl, type);
				var view = RenderPartialViewToString("PVForSongEditRow", contract);
				return Json(new GenericResponse<string>(view));

			} catch (VideoParseException x) {
				return Json(new GenericResponse<string>(false, x.Message));
			}

		}

		[Obsolete("Integrated to saving properties")]
		[HttpPost]
		public void DeletePVForSong(int pvForSongId) {

			Service.DeletePvForSong(pvForSongId);

		}

		public PartialViewResult CreateLyrics() {
			
			var entry = new LyricsForSongContract();

			return PartialView("LyricsForSongEditRow", new LyricsForSongModel(entry));

		}

		/*
		[HttpPost]
		[Obsolete("Integrated to song editing")]
		public ActionResult EditLyrics(LyricsEditorModel model, IEnumerable<LyricsForSongModel> lyrics) {

			lyrics = lyrics ?? new LyricsForSongModel[] {};

			var contracts = lyrics.Select(l => l.ToContract());
			var song = Service.UpdateLyrics(model.Id, contracts);

			TempData.SetStatusMessage("Lyrics saved.");

			return RedirectToAction("Edit", new { id = model.Id });

		}*/

        //
        // GET: /Song/Delete/5

        public ActionResult Delete(int id)
        {
            
			Service.Delete(id);
			return RedirectToAction("Index");

        }

		public ActionResult Merge(int id) {

			var song = Service.GetSong(id);
			return View(song);

		}

		[HttpPost]
		public ActionResult Merge(int id, int? targetSongId) {

			if (targetSongId == null) {
				ModelState.AddModelError("songList", "Song must be selected");
				return Merge(id);
			}

			Service.Merge(id, targetSongId.Value);

			return RedirectToAction("Edit", new { id = targetSongId.Value });

		}

		public ActionResult Restore(int id) {

			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });

		}

		public ActionResult RevertToVersion(int archivedSongVersionId) {

			var result = Service.RevertToVersion(archivedSongVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });

		}

		public PartialViewResult TagSelections(int songId) {

			var contract = Service.GetTagSelections(songId, LoginManager.LoggedUser.Id);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int songId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(songId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}

		public ActionResult Versions(int id) {

			var contract = Service.GetSongWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id) {

			var contract = Service.GetVersionDetails(id);

			return View(contract);

		}

    }
}
