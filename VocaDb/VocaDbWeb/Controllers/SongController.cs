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

namespace VocaDb.Web.Controllers
{
    public class SongController : ControllerBase
    {

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

		[HttpPost]
		public PartialViewResult CreateSongLink(int? songId) {

			SongContract song;

			if (songId == null)
				song = new SongContract();
			else
				song = Service.GetSong(songId.Value);

			return PartialView("SongLink", song);

		}

        //
        // GET: /Song/

        public ActionResult Index(string filter, int? page) {

        	var result = Service.Find(filter, ((page ?? 1) - 1)*30, 30, true);

			var model = new Index(result, filter, page);

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

		public ActionResult FindJsonByName(string term, bool alwaysExact = false) {

			var songs = Service.Find(term, 0, 20, onlyByName: true, nameMatchMode: (alwaysExact ? NameMatchMode.Exact : NameMatchMode.Auto));

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

			if (string.IsNullOrEmpty(model.NameOriginal) && string.IsNullOrEmpty(model.NameRomaji) && string.IsNullOrEmpty(model.NameEnglish))
				ModelState.AddModelError("Name", "Need at least one name.");

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

		[HttpPost]
		public PartialViewResult CreateArtistForSongRow(int artistId) {

			var artist = MvcApplication.Services.Artists.GetArtist(artistId);

			return PartialView("ArtistContractRow", artist);

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

			if (!ModelState.IsValid) {
				var oldContract = Service.GetSongForEdit(model.Id);
				model.Lyrics = oldContract.Lyrics.Select(l => new LyricsForSongModel(l)).ToArray();
				model.ArtistLinks = oldContract.Artists;
				model.PVs = oldContract.PVs;
				return View(model);				
			}

			var contract = model.ToContract();
			Service.UpdateBasicProperties(contract);

			return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingArtist(int songId, int artistId) {

			var link = Service.AddArtist(songId, artistId);
			return PartialView("ArtistForSongEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewArtist(int songId, string newArtistName) {

			var link = Service.AddArtist(songId, newArtistName);
			return PartialView("ArtistForSongEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteArtistForSong(int artistForSongId) {

			Service.DeleteArtistForSong(artistForSongId);

		}

		public PartialViewResult CreatePVForSong(int songId, PVService service, string pvId, PVType type) {

			ParamIs.NotNullOrEmpty(() => pvId);

			var contract = Service.CreatePVForSong(songId, service, pvId, type);

			return PartialView("PVForSongEditRow", contract);

		}

		[HttpPost]
		public ActionResult CreatePVForSongByUrl(int songId, string pvUrl, PVType type) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			try {
				var contract = Service.CreatePVForSong(songId, pvUrl, type);
				var view = RenderPartialViewToString("PVForSongEditRow", contract);
				return Json(new GenericResponse<string>(view));
				//return PartialView("PVForSongEditRow", contract);
			} catch (VideoParseException x) {
				return Json(new GenericResponse<string>(false, x.Message));
			}

		}

		[HttpPost]
		public void DeletePVForSong(int pvForSongId) {

			Service.DeletePvForSong(pvForSongId);

		}

		public PartialViewResult CreateLyrics() {
			
			//var entry = Service.CreateLyrics(songId, languageSelection, value, source);
			var entry = new LyricsForSongContract();

			return PartialView("LyricsForSongEditRow", new LyricsForSongModel(entry));

		}

		[HttpPost]
		public ActionResult EditLyrics(LyricsEditorModel model, IEnumerable<LyricsForSongModel> lyrics) {

			lyrics = lyrics ?? new LyricsForSongModel[] {};

			var contracts = lyrics.Select(l => l.ToContract());
			var song = Service.UpdateLyrics(model.Id, contracts);

			return RedirectToAction("Edit", new { id = model.Id });

		}

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
		public ActionResult Merge(int id, int? songList) {

			if (songList == null) {
				ModelState.AddModelError("songList", "Song must be selected");
				return Merge(id);
			}

			Service.Merge(id, songList.Value);

			return RedirectToAction("Edit", new { id = songList.Value });

		}

		public ActionResult Versions(int id) {

			return View(Service.GetSongWithArchivedVersions(id));

		}

    }
}
