using System;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Service;
using VocaDb.Web.Models;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model;

namespace VocaDb.Web.Controllers
{
    public class AlbumController : ControllerBase
    {

		private AlbumService Service {
			get { return MvcApplication.Services.Albums; }
		}

        //
        // GET: /Album/

        public ActionResult Index() {
        	ViewBag.Albums = Service.GetAlbums();
            return View();
        }

		public ActionResult FindJson(string term) {

			var albums = Service.Find(term, 20);

			return Json(albums);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetAlbumDetails(id);
            return View(model);
        }

		public ActionResult CoverPicture(int id) {

			var pictureData = Service.GetCoverPicture(id);

			if (pictureData == null)
				return new EmptyResult();

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

			if (!ModelState.IsValid)
				return View();

            PictureDataContract pictureData = null;

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

				var file = Request.Files[0];
				var buf = new Byte[file.ContentLength];
				file.InputStream.Read(buf, 0, file.ContentLength);

				pictureData = new PictureDataContract(buf, file.ContentType);

			}

            var contract = model.ToContract();
			Service.UpdateBasicProperties(contract, pictureData);

        	return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CreateName(int artistId, string nameVal, ContentLanguageSelection language) {

			var name = Service.CreateName(artistId, nameVal, language);

			return Json(name);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CreateWebLink(int artistId, string description, string url) {

			var name = Service.CreateWebLink(artistId, description, url);

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

    }
}
