using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Model;
using System.Drawing;
using VocaDb.Model.Helpers;
using VocaDb.Web.Models.Album;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Controllers
{
    public class AlbumController : ControllerBase
    {

		private readonly Size pictureThumbSize = new Size(250, 250);

		private AlbumService Service {
			get { return MvcApplication.Services.Albums; }
		}

		public ActionResult ArchivedVersionCoverPicture(int id) {

			var contract = Service.GetArchivedAlbumPicture(id);

			return Picture(contract);

		}

		[HttpPost]
		public ActionResult CreatePVForAlbumByUrl(int albumId, string pvUrl) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			try {
				//var contract = Service.CreatePV(albumId, pvUrl, PVType.Other);

				var result = VideoServiceHelper.ParseByUrl(pvUrl);
				var contract = new PVContract(result, PVType.Other);

				var view = RenderPartialViewToString("PVForSongEditRow", contract);
				return Json(new GenericResponse<string>(view));

			} catch (VideoParseException x) {
				return Json(new GenericResponse<string>(false, x.Message));
			}

		}

		[Obsolete("Integrated to saving properties")]
		[HttpPost]
		public void DeletePVForAlbum(int pvForAlbumId) {

			Service.DeletePv(pvForAlbumId);

		}

        //
        // GET: /Album/

		public ActionResult Index(string filter, int? page, bool? draftsOnly) {

			var result = Service.Find(filter, ((page ?? 1) - 1) * 30, 30, draftsOnly ?? false, true, false);

			var model = new Index(result, filter, page, draftsOnly);

            return View(model);

        }

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3) {

			var result = Service.FindByNames(new[] { term1, term2, term3 });

			if (result != null) {
				return PartialView("DuplicateEntryMessage",
					new KeyValuePair<string, string>(result.Name,
						Url.Action("Details", new { id = result.Id })));
			} else {
				return Content("Ok");
			}

		}

		public ActionResult FindJson(string term) {

			var albums = Service.Find(term, 0, 20, false, false, true);

			return Json(albums);

		}

		public ActionResult MikuDbRedirect(int id) {

			var albumId = Service.FindByMikuDbId(id);

			if (albumId == null) {

				TempData.SetStatusMessage("Sorry, album not found! Maybe it hasn't been imported yet.");
				return RedirectToAction("Index", "Home");

			} else {

				return RedirectToAction("Details", new { id = albumId });

			}

		}

		public PartialViewResult MultipleTrackProperties(int albumId) {

			var contract = Service.GetArtists(albumId);

			return PartialView(contract);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetAlbumDetails(id);
            return View(new AlbumDetails(model));
        }

		public FileContentResult DownloadTags(int id) {

			var album = Service.GetAlbumDetails(id);

			return File(Encoding.Unicode.GetBytes(TagsHelper.GetAlbumTags(album)), "text/csv", album.Name + ".csv");

		}

		public PartialViewResult Comments(int id) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		public ActionResult CoverPicture(int id) {

			var album = Service.GetCoverPicture(id, Size.Empty);

			return Picture(album);

		}

		public ActionResult CoverPictureThumb(int id) {

			var album = Service.GetCoverPicture(id, pictureThumbSize);

			return Picture(album);

		}

		[HttpPost]
		public PartialViewResult CreateComment(int albumId, string message) {

			var comment = Service.CreateComment(albumId, message);

			return PartialView("Comment", comment);

		}

        //
        // POST: /Album/Create

		[Obsolete("Disabled")]
        [HttpPost]
        public ActionResult CreateQuick(ObjectCreate model)
        {

			if (ModelState.IsValid) {

				var artist = Service.Create(model.Name);
				return RedirectToAction("Edit", new { id = artist.Id });

			} else {

				return RedirectToAction("Index");

			}

		}

		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) 
				&& string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", "Need at least one name.");

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", "Need at least one artist.");

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			var album = Service.Create(contract);
			return RedirectToAction("Details", new { id = album.Id });

		}

        //
        // GET: /Album/Edit/5
 
        public ActionResult Edit(int id) {

        	var album = Service.GetAlbumForEdit(id);

			return View(new AlbumEdit(album));

        }

        //
        // POST: /Album/Edit/5

        [HttpPost]
		public ActionResult Edit(AlbumEdit model)
        {

			if (!OptionalDateTime.IsValid(model.ReleaseYear, model.ReleaseDay, model.ReleaseMonth))
				ModelState.AddModelError("ReleaseYear", "Invalid date");

            PictureDataContract pictureData = null;

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

				var file = Request.Files[0];

				if (file.ContentLength > ImageHelper.MaxImageSizeBytes) {
					ModelState.AddModelError("CoverPicture", "Picture file is too large.");
				}

				if (!ImageHelper.IsValidImageExtension(file.FileName)) {
					ModelState.AddModelError("CoverPicture", "Picture format is not valid.");
				}

				if (ModelState.IsValid) {

					pictureData = ImageHelper.GetOriginalAndResizedImages(
						file.InputStream, file.ContentLength, file.ContentType);

				}

			}

			/*foreach (var link in model.WebLinks) {
				if (!UrlValidator.IsValid(link.Url))
					ModelState.AddModelError("WebLinks", link.Url + " is not a valid URL.");
			}*/

			if (!ModelState.IsValid) {
				var oldContract = Service.GetAlbumForEdit(model.Id);
				model.CopyNonEditableFields(oldContract);
				return View(model);
			}

            var contract = model.ToContract();
			Service.UpdateBasicProperties(contract, pictureData);

        	return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewSong(int albumId, string newSongName) {

			ParamIs.NotNullOrWhiteSpace(() => newSongName);

			var link = new SongInAlbumEditContract(newSongName.Trim());

			return PartialView("SongInAlbumEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingSong(int albumId, int songId) {

			var songContract = MvcApplication.Services.Songs.GetSongWithAdditionalNames(songId);
			var link = new SongInAlbumEditContract(songContract);

			return PartialView("SongInAlbumEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete("Integrated to saving properties")]
		public ActionResult DeleteSongInAlbum(int songInAlbumId) {

			var songs = Service.DeleteSongInAlbum(songInAlbumId);

			return Json(songs);

		}

		public ActionResult Restore(int id) {

			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });

		}

		public ActionResult RevertToVersion(int archivedAlbumVersionId) {

			var result = Service.RevertToVersion(archivedAlbumVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteArtistForAlbum(int artistForAlbumId) {

			Service.DeleteArtistForAlbum(artistForAlbumId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingArtist(int albumId, int artistId) {

			var link = MvcApplication.Services.Artists.AddAlbum(artistId, albumId);
			return PartialView("ArtistForAlbumEditRow", link);

		}

		[Obsolete("Not allowed anymore")]
		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewArtist(int albumId, string newArtistName) {

			var link = Service.AddArtist(albumId, newArtistName);
			return PartialView("ArtistForAlbumEditRow", link);

		}

        //
        // GET: /Album/Delete/5
 
        public ActionResult Delete(int id)
        
		{
			Service.Delete(id);

			return RedirectToAction("Index");

		}

		public ActionResult MassTagSongs(int id) {

			var album = Service.GetAlbumDetails(id);
			return View(new MassTagSongs(album));

		}

		[HttpPost]
		public ActionResult MassTagSongs(MassTagSongs model, IEnumerable<TrackArtistSelection> ArtistSelections) {

			// TODO

			return RedirectToAction("Details", new { id = model.Id });

		}

		public ActionResult Merge(int id) {

			var album = Service.GetAlbum(id);
			return View(album);

		}

		[HttpPost]
		public ActionResult Merge(int id, int targetAlbumId) {

			Service.Merge(id, targetAlbumId);

			return RedirectToAction("Edit", new { id = targetAlbumId });

		}

		public PartialViewResult TagSelections(int albumId) {

			var contract = Service.GetTagSelections(albumId, LoginManager.LoggedUser.Id);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int albumId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(albumId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}

		public PartialViewResult TrackProperties(int albumId, int songId) {

			var contract = Service.GetTrackProperties(albumId, songId);

			return PartialView(contract);

		}

		[HttpPost]
		public ContentResult TrackProperties(int songId, string artistIds) {

			var idStr = artistIds.Split(',');
			var ids = idStr.Where(i => !string.IsNullOrEmpty(i)).Select(i => int.Parse(i)).ToArray();

			var artistString = MvcApplication.Services.Songs.UpdateArtists(songId, ids);
			return Content(artistString);

		}

		[HttpPost]
		public ActionResult UpdateArtistsForMultipleTracks(int[] songIds, int[] artistIds, bool add) {

			if (songIds == null || artistIds == null || !songIds.Any() || !artistIds.Any())
				return Content(string.Empty);

			var artistStrings = MvcApplication.Services.Songs.UpdateArtistsForMultipleTracks(songIds, artistIds, add);

			return Json(artistStrings);

		}

		public ActionResult Versions(int id) {

			var contract = Service.GetAlbumWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id) {

			var contract = Service.GetVersionDetails(id);

			return View(contract);

		}

    }
}
