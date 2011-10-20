using System;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Web.Models;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model;
using System.Drawing;
using VocaDb.Model.Helpers;
using PagedList;
using VocaDb.Model.DataContracts.Albums;

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

			ViewBag.Filter = filter;
			ViewBag.Albums = new StaticPagedList<AlbumWithAdditionalNamesContract>(result.Items.OrderBy(a => a.Name), page ?? 1, 30, result.TotalCount);

            return View();

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

        //
        // POST: /Album/Create

        [HttpPost]
        public ActionResult Create(ObjectCreate model)
        {

			if (ModelState.IsValid) {

				var artist = Service.Create(model.Name);
				return RedirectToAction("Edit", new { id = artist.Id });

			} else {

				return RedirectToAction("Index");

			}

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
						file.InputStream, file.ContentLength, file.ContentType, pictureThumbSize);

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
		public ActionResult CreateName(int albumId, string nameVal, ContentLanguageSelection language) {

			var name = Service.CreateName(albumId, nameVal, language);

			return Json(name);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CreateWebLink(int albumId, string description, string url) {

			var name = Service.CreateWebLink(albumId, description, url);

			return Json(name);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteName(int nameId) {

			Service.DeleteName(nameId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteWebLink(int linkId) {

			Service.DeleteWebLink(linkId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void EditNameLanguage(int nameId, string nameLanguage) {

			Service.UpdateNameLanguage(nameId, EnumVal<ContentLanguageSelection>.Parse(nameLanguage));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void EditNameValue(int nameId, string nameVal) {

			Service.UpdateNameValue(nameId, nameVal);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void EditWebLinkDescription(int linkId, string description) {

			Service.UpdateWebLinkDescription(linkId, description);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void EditWebLinkUrl(int linkId, string url) {

			Service.UpdateWebLinkUrl(linkId, url);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewSong(int albumId, string newSongName) {

			var link = Service.AddSong(albumId, newSongName);
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

		public ActionResult Merge(int id) {

			var album = Service.GetAlbumDetails(id);
			return View(album);

		}

    }
}
