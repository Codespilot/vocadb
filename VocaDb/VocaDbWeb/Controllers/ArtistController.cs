using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Service;
using VocaDb.Web.Models;
using System.Drawing;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Artists;
using VocaDb.Web.Models.Artist;
using VocaDb.Web.Helpers;
using MvcPaging;
using VocaDb.Web.Models.Shared;

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

		public PartialViewResult AlbumsPaged(int id, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetAlbums(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<AlbumWithAdditionalNamesContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "AlbumsPaged", "All_albums");

			return PartialView("PagedAlbums", data);

		}

		public ActionResult ArchivedVersionPicture(int id) {

			var contract = Service.GetArchivedArtistPicture(id);

			return Picture(contract);

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
		public void CreateReport(int artistId, ArtistReportType reportType, string notes) {

			Service.CreateReport(artistId, reportType, CfHelper.GetRealIp(Request), notes ?? string.Empty);

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

		public ActionResult FindNames(string term) {

			return Json(Service.FindNames(term, 15));

		}

        //
        // GET: /Artist/
		public ActionResult Index(string filter, ArtistType? artistType, bool? draftsOnly, ArtistSortRule? sort, int? page) {

			var sortRule = sort ?? ArtistSortRule.Name;
			var result = Service.FindArtists(filter, 
				artistType != null && artistType != ArtistType.Unknown ? new[] { artistType.Value } : new ArtistType[] {}, 
				((page ?? 1) - 1) * 30, 30, draftsOnly ?? false, true, NameMatchMode.Auto, sortRule, false);

			if (page == null && result.TotalCount == 1 && result.Items.Length == 1) {
				return RedirectToAction("Details", new { id = result.Items[0].Id });
			}

			var model = new ArtistIndex(result, filter, artistType ?? ArtistType.Unknown, draftsOnly, sortRule, page);

			return View(model);

        }

		public ActionResult Restore(int id) {

			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });

		}

		public ActionResult RevertToVersion(int archivedArtistVersionId) {

			var result = Service.RevertToVersion(archivedArtistVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });

		}

		public PartialViewResult Songs(int id) {

			var songs = Service.GetSongs(id);

			return PartialView(songs);

		}

		public PartialViewResult SongsPaged(int id, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetSongs(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<SongWithAdditionalNamesContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "SongsPaged", "All_songs");

			return PartialView("PagedSongs", data);

		}

		public PartialViewResult TagSelections(int artistId) {

			var contract = Service.GetTagSelections(artistId, LoginManager.LoggedUser.Id);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int artistId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(artistId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3) {

			var result = Service.FindDuplicates(new[] { term1, term2, term3 });

			if (result.Any()) {
				return PartialView("DuplicateEntryMessage", result);
			} else {
				return Content("Ok");
			}

		}

		public ActionResult FindJson(string term, string artistTypes) {

			var typeVals = !string.IsNullOrEmpty(artistTypes)
				? artistTypes.Split(',').Select(EnumVal<ArtistType>.Parse)
				: new ArtistType[] {};

			var albums = Service.FindArtists(term, typeVals.ToArray(), 0, 20, false, false, NameMatchMode.Auto, ArtistSortRule.Name, true);

			return Json(albums);

		}

        //
        // GET: /Artist/Details/5

        public ActionResult Details(int id) {
        	var model = Service.GetArtistDetails(id);
            return View(model);
        }

		public ActionResult Picture(int id) {

			var artist = Service.GetArtistPicture(id, Size.Empty);

			return Picture(artist);

		}

		public ActionResult PictureThumb(int id) {

			var artist = Service.GetArtistPicture(id, pictureThumbSize);

			return Picture(artist);

		}

		public PartialViewResult Comments(int id) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		[Authorize]
		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (string.IsNullOrWhiteSpace(model.Description) && string.IsNullOrWhiteSpace(model.WebLinkUrl))
				ModelState.AddModelError("Description", ViewRes.Artist.CreateStrings.NeedWebLinkOrDescription);

			//if (!string.IsNullOrWhiteSpace(model.WebLinkUrl) && !UrlValidator.IsValid(model.WebLinkUrl))
			//	ModelState.AddModelError("WebLinkUrl", model.WebLinkUrl + " is not a valid URL.");

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			var album = Service.Create(contract);
			return RedirectToAction("Details", new { id = album.Id });

		}

        //
        // POST: /Artist/Create

		[Obsolete("Disabled")]
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
        [Authorize]
        public ActionResult Edit(int id)
        {

			RestoreErrorsFromTempData();
        	var model = new ArtistEdit(Service.GetArtistForEdit(id));
            return View(model);

        }

        //
        // POST: /Artist/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult EditBasicDetails(ArtistEdit model, IEnumerable<GroupForArtistContract> groups)
        {

			var coverPicUpload = Request.Files["pictureUpload"];
			PictureDataContract pictureData = ParseMainPicture(coverPicUpload, "Picture");

			ParseAdditionalPictures(coverPicUpload, model.Pictures);

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

			ParamIs.NotNullOrWhiteSpace(() => newAlbumName);

			var link = new AlbumForArtistEditContract(newAlbumName);
			//var link = MvcApplication.Services.Albums.CreateForArtist(artistId, newAlbumName);

			return PartialView("ArtistForAlbumRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingAlbum(int artistId, int albumId) {

			//var link = Service.AddAlbum(artistId, albumId);

			var album = MvcApplication.Services.Albums.GetAlbumWithAdditionalNames(albumId);
			var link = new AlbumForArtistEditContract(album);

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
		public ActionResult Merge(int id, int targetArtistId) {

			Service.Merge(id, targetArtistId);

			return RedirectToAction("Edit", new { id = targetArtistId });

		}

		public ActionResult Versions(int id) {

			var contract = Service.GetArtistWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId) {

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }
}
