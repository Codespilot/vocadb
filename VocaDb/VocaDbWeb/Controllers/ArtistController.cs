using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Models;
using System.Drawing;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Artists;
using VocaDb.Web.Models.Artist;

namespace VocaDb.Web.Controllers
{
    public class ArtistController : ControllerBase
    {

		private readonly Size pictureThumbSize = new Size(250, 250);

    	private ArtistService Service {
    		get { return MvcApplication.Services.Artists; }
    	}

		public PartialViewResult Albums(int id) {

			var albums = Service.GetAlbums(id);

			return PartialView(albums);

		}

		[HttpPost]
		public PartialViewResult CreateArtistContractRow(int artistId) {

			var artist = Service.GetArtist(artistId);

			return PartialView("ArtistContractRow", artist);

		}

		[HttpPost]
		public PartialViewResult CreateComment(int entryId, string message) {

			var comment = Service.CreateComment(entryId, message);

			return PartialView("Comment", comment);

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

        //
        // GET: /Artist/
		public ActionResult Index(string filter, ArtistType? artistType, int? page) {

			var result = Service.FindArtists(filter, 
				artistType != null && artistType != ArtistType.Unknown ? new[] { artistType.Value } : new ArtistType[] {}, 
				((page ?? 1) - 1) * 30, 30, true);

			var model = new ArtistIndex(result, filter, artistType ?? ArtistType.Unknown, page);

			return View(model);

        }

		public PartialViewResult Songs(int id) {

			var songs = Service.GetSongs(id);

			return PartialView(songs);

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

		public ActionResult FindJson(string term, string artistTypes) {

			var typeVals = !string.IsNullOrEmpty(artistTypes)
				? artistTypes.Split(',').Select(EnumVal<ArtistType>.Parse)
				: new ArtistType[] {};

			var albums = Service.FindArtists(term, typeVals.ToArray(), 0, 20);

			return Json(albums.Items);

		}

        //
        // GET: /Artist/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetArtistDetails(id);
            return View(model);
        }

		public ActionResult Picture(int id) {

			var artist = Service.GetArtistPicture(id, Size.Empty);

			Response.Cache.SetETag(string.Format("Artist{0}v{1}", id, artist.Version));

			return Picture(artist.CoverPicture, artist.Name);

		}

		public ActionResult PictureThumb(int id) {

			var artist = Service.GetArtistPicture(id, pictureThumbSize);

			Response.Cache.SetETag(string.Format("Artist{0}v{1}t", id, artist.Version));

			return Picture(artist.CoverPicture, artist.Name);

		}

		public PartialViewResult Comments(int id) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Name", "Need at least one name.");

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			var album = Service.Create(contract);
			return RedirectToAction("Details", new { id = album.Id });

		}

        //
        // POST: /Artist/Create

        [HttpPost]
        public ActionResult CreateQuick(ObjectCreate model)
        {

			if (ModelState.IsValid) {

				var artist = Service.Create(model.Name, MvcApplication.LoginManager);
				return RedirectToAction("Edit", new { id = artist.Id });

			} else {
			
				return RedirectToAction("Index");

			}

        }
        
        //
        // GET: /Artist/Edit/5
 
        public ActionResult Edit(int id) {

			RestoreErrorsFromTempData();
        	var model = new ArtistEdit(Service.GetArtistForEdit(id));
            return View(model);

        }

        //
        // POST: /Artist/Edit/5

        [HttpPost]
		public ActionResult EditBasicDetails(ArtistEdit model, IEnumerable<GroupForArtistContract> groups)
        {

            PictureDataContract pictureData = null;

			if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0) {

				if (ImageHelper.IsValidImageExtension(Request.Files[0].FileName)) {

					var file = Request.Files[0];

					pictureData = ImageHelper.GetOriginalAndResizedImages(
						file.InputStream, file.ContentLength, file.ContentType);

				} else {

					ModelState.AddModelError("Picture", "Picture format is not valid");

				}

			}

			if (!ModelState.IsValid) {
				SaveErrorsToTempData();
				return RedirectToAction("Edit", new { id = model.Id });
			}

			Service.UpdateBasicProperties(model.ToContract(), pictureData, LoginManager);

			return RedirectToAction("Details", new { id = model.Id });

        }

		[HttpPost]
		public PartialViewResult AddCircle(int artistId, int circleId) {

			var circle = Service.GetArtistWithAdditionalNames(circleId);

			return PartialView("GroupForArtistEditRow", new GroupForArtistContract { Group = circle });

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewAlbum(int artistId, string newAlbumName) {

			var link = MvcApplication.Services.Albums.CreateForArtist(artistId, newAlbumName);
			return PartialView("ArtistForAlbumRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingAlbum(int artistId, int albumId) {

			var link = Service.AddAlbum(artistId, albumId);
			return PartialView("ArtistForAlbumRow", link);

		}

        //
        // GET: /Artist/Delete/5
 
        public ActionResult Delete(int id)
        {

			Service.Delete(id);

            return RedirectToAction("Index");

        }

		public ActionResult Merge(int id) {

			var artist = Service.GetArtist(id);
			return View(artist);

		}

		[HttpPost]
		public ActionResult Merge(int id, FormCollection collection) {

			var targetId = collection["artistList"];

			if (string.IsNullOrEmpty(targetId)) {
				ModelState.AddModelError("artistList", "Artist must be selected");
				return Merge(id);
			}

			var targetIdInt = int.Parse(targetId);

			Service.Merge(id, targetIdInt);

			return RedirectToAction("Edit", new { id = targetIdInt });

		}

		public ActionResult Versions(int id) {

			var contract = Service.GetArtistWithArchivedVersions(id);

			return View(new Versions(contract));

		}

    }
}
