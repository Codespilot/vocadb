using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.Artist;
using VocaDb.Web.Models;
using System.Drawing;
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

		public ActionResult Albums(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			return RedirectToActionPermanent("AlbumsPaged", new { id });

		}

		public PartialViewResult AlbumsPaged(int id, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetAlbums(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<AlbumWithAdditionalNamesContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "AlbumsPaged", "ui-tabs-2");

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

			WebHelper.VerifyUserAgent(Request);
			var sortRule = sort ?? ArtistSortRule.Name;
			var queryParams = new ArtistQueryParams(filter,
				artistType != null && artistType != ArtistType.Unknown ? new[] { artistType.Value } : new ArtistType[] { },
				((page ?? 1) - 1) * 30, 30, draftsOnly ?? false, true, NameMatchMode.Auto, sortRule, false);

			var result = Service.FindArtists(queryParams);

			if (page == null && result.TotalCount == 1 && result.Items.Length == 1) {
				return RedirectToAction("Details", new { id = result.Items[0].Id });
			}

			var model = new ArtistIndex(result, filter, artistType ?? ArtistType.Unknown, draftsOnly, sortRule, page);

			return View(model);

        }

		public ActionResult Info(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var artist = Service.GetArtist(id);
			return Json(artist);

		}

        [Authorize]
        public ActionResult RemoveTagUsage(long id) {

            var artistId = Service.RemoveTagUsage(id);
            TempData.SetStatusMessage("Tag usage removed");

            return RedirectToAction("ManageTagUsages", new { id = artistId });

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

		public ActionResult Songs(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			return RedirectToActionPermanent("SongsPaged", new { id });

		}

		public PartialViewResult SongsPaged(int id = invalidId, int? page = null) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetSongs(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<SongWithAdditionalNamesContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "SongsPaged", "ui-tabs-3");

			return PartialView("PagedSongs", data);

		}

		public PartialViewResult TagSelections(int artistId = invalidId) {

			var contract = Service.GetTagSelections(artistId, LoginManager.LoggedUserId);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int artistId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(artistId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3, string linkUrl) {

			var result = Service.FindDuplicates(new[] { term1, term2, term3 }, linkUrl);

			if (result.Any()) {
				return PartialView("DuplicateEntryMessage", result);
			} else {
				return Content("Ok");
			}

		}

		public ActionResult FindJson(string term, string artistTypes) {

			var typeVals = !string.IsNullOrEmpty(artistTypes)
				? artistTypes.Split(',').Select(EnumVal<ArtistType>.Parse).ToArray()
				: new ArtistType[] {};

			var queryParams = new ArtistQueryParams(term, typeVals, 0, 20, false, false, NameMatchMode.Auto, ArtistSortRule.Name, true);
			var artists = Service.FindArtists(queryParams);

			return Json(artists);

		}

        //
        // GET: /Artist/Details/5

        public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			WebHelper.VerifyUserAgent(Request);
			var model = Service.GetArtistDetails(id);
            return View(model);

        }

		public ActionResult Picture(int id) {

			var artist = Service.GetArtistPicture(id, Size.Empty);

			return Picture(artist);

		}

		public ActionResult PictureThumb(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var artist = Service.GetArtistPicture(id, pictureThumbSize);

			return Picture(artist);

		}

		public ActionResult PopupContent(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var artist = Service.GetArtist(id);
			return PartialView("ArtistPopupContent", artist);

		}

		public PartialViewResult Comments(int id = invalidId) {

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

			var coverPicUpload = Request.Files["pictureUpload"];
			PictureDataContract pictureData = ParseMainPicture(coverPicUpload, "Picture");

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();
			contract.PictureData = pictureData;

			var album = Service.Create(contract);
			return RedirectToAction("Edit", new { id = album.Id });

		}
        
        //
        // GET: /Artist/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {

			RestoreErrorsFromTempData();

			CheckConcurrentEdit(EntryType.Artist, id);

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

        [Authorize]
        public ActionResult ManageTagUsages(int id) {

            var artist = Service.GetEntryWithTagUsages(id);
            return View(artist);

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

		public ActionResult Name(int id) {

			var contract = Service.GetArtist(id);
			return Content(contract.Name);

		}

		public ActionResult Versions(int id) {

			var contract = Service.GetArtistWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id = invalidId, int? ComparedVersionId = null) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }
}
