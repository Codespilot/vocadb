using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Web.Models;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model;
using System.Drawing;
using VocaDb.Model.Helpers;
using VocaDb.Web.Models.Album;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Controllers
{
    public class AlbumController : ControllerBase
    {

		private readonly Size pictureThumbSize = new Size(250, 250);

		private AlbumService Service {
			get { return MvcApplication.Services.Albums; }
		}

        //
        // GET: /Album/

		public ActionResult Index(string filter, int? page) {

			var result = Service.Find(filter, ((page ?? 1) - 1) * 30, 30, true);

			var model = new Index(result, filter, page);

            return View(model);

        }

		public ActionResult FindJson(string term) {

			var albums = Service.Find(term, 0, 20).Items;

			return Json(albums);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetAlbumDetails(id);
            return View(new AlbumDetails(model));
        }

		public PartialViewResult Comments(int id) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		public ActionResult CoverPicture(int id) {

			var pictureData = Service.GetCoverPicture(id, Size.Empty);

			if (pictureData == null)
				return File(Server.MapPath("~/Content/unknown.png"), "image/png");

			return File(pictureData.Bytes, pictureData.Mime);

		}

		public ActionResult CoverPictureThumb(int id) {

			var pictureData = Service.GetCoverPicture(id, pictureThumbSize);

			if (pictureData == null)
				return File(Server.MapPath("~/Content/unknown.png"), "image/png");

			return File(pictureData.Bytes, pictureData.Mime);

		}

		public PartialViewResult CreateComment(int albumId, string message) {

			var comment = Service.CreateComment(albumId, message);

			return PartialView("Comment", comment);

		}

        //
        // POST: /Album/Create

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

			if (string.IsNullOrEmpty(model.NameOriginal) && string.IsNullOrEmpty(model.NameRomaji) && string.IsNullOrEmpty(model.NameEnglish))
				ModelState.AddModelError("Name", "Need at least one name.");

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

			if (!ModelState.IsValid)
				return View(new AlbumEdit(Service.GetAlbumForEdit(model.Id)));

            PictureDataContract pictureData = null;

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

				if (ImageHelper.IsValidImageExtension(Request.Files[0].FileName)) {

					var file = Request.Files[0];

					pictureData = ImageHelper.GetOriginalAndResizedImages(
						file.InputStream, file.ContentLength, file.ContentType);

				} else {

					ModelState.AddModelError("CoverPicture", "Picture format is not valid");
					return RedirectToAction("Edit", new { id = model.Id });

				}

			}

            var contract = model.ToContract();
			Service.UpdateBasicProperties(contract, pictureData);

        	return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewSong(int albumId, string newSongName) {

			var link = MvcApplication.Services.Songs.CreateForAlbum(albumId, newSongName);
			return PartialView("SongInAlbumEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingSong(int albumId, int songId) {

			var link = Service.AddSong(albumId, songId);
			return PartialView("SongInAlbumEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DeleteSongInAlbum(int songInAlbumId) {

			var songs = Service.DeleteSongInAlbum(songInAlbumId);

			return Json(songs);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MoveSongInAlbumDown(int songInAlbumId) {

			var songs = Service.MoveSongDown(songInAlbumId);

			return Json(songs);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MoveSongInAlbumUp(int songInAlbumId) {

			var songs = Service.MoveSongUp(songInAlbumId);

			return Json(songs);

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
		public ActionResult Merge(int id, int albumList) {

			Service.Merge(id, albumList);

			return RedirectToAction("Edit", new { id = albumList });

		}

		public PartialViewResult TrackProperties(int songInAlbumId) {

			var contract = Service.GetTrackProperties(songInAlbumId);

			return PartialView(contract);

		}

		[HttpPost]
		public void TrackProperties(int songId, string artistIds) {

			var idStr = artistIds.Split(',');
			var ids = idStr.Where(i => !string.IsNullOrEmpty(i)).Select(i => int.Parse(i)).ToArray();

			MvcApplication.Services.Songs.UpdateArtists(songId, ids);

		}

		public ActionResult Versions(int id) {

			return View(Service.GetAlbumWithArchivedVersions(id));

		}
    }
}
