﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Utils.Search;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Controllers.DataAccess;
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

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly ArtistQueries queries;
		private readonly ArtistService service;

		public ArtistController(ArtistService service, ArtistQueries queries) {
			this.service = service;
			this.queries = queries;
		}

    	private ArtistService Service {
    		get { return service; }
    	}

		public ActionResult Albums(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			return RedirectToActionPermanent("AlbumsPaged", new { id });

		}

		public PartialViewResult AlbumsPaged(int id, ArtistAlbumParticipationStatus? artistParticipation, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var queryParams = new AlbumQueryParams {
				Paging = PagingProperties.CreateFromPage(pageIndex, entriesPerPage, true),
				SortRule = AlbumSortRule.ReleaseDateWithNulls,
				ArtistId = id,
				ArtistParticipationStatus = artistParticipation ?? ArtistAlbumParticipationStatus.Everything
			};

			var result = Services.Albums.Find(queryParams);

			var target = queryParams.ArtistParticipationStatus == ArtistAlbumParticipationStatus.OnlyCollaborations ? "ui-tabs-3" : "ui-tabs-2";
			var data = new PagingData<AlbumContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "AlbumsPaged", target);
			data.RouteValues = new RouteValueDictionary(new { artistParticipation });

			return PartialView("PagedAlbums", data);

		}

		public ActionResult ArchivedVersionPicture(int id) {

			var contract = Service.GetArchivedArtistPicture(id);

			return Picture(contract);

		}

		public ActionResult ArchivedVersionXml(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var doc = Service.GetVersionXml(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);

		}

		[HttpPost]
		public PartialViewResult CreateArtistContractRow(int artistId) {

			var artist = Service.GetArtist(artistId);

			return PartialView("ArtistContractRow", artist);

		}

		[HttpPost]
		public PartialViewResult CreateComment(int entryId, string message) {

			var comment = queries.CreateComment(entryId, message);

			return PartialView("Comment", comment);

		}

		[HttpPost]
		public void CreateReport(int artistId, ArtistReportType reportType, string notes) {

			Service.CreateReport(artistId, reportType, CfHelper.GetRealIp(Request), notes ?? string.Empty);

		}

		public ActionResult DataById(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var artist = Service.GetArtist(id);
			return new JsonNetResult { Data = artist };

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

		public ActionResult FindNames(string term) {

			return Json(Service.FindNames(term, 15));

		}

        [Obsolete]
		public ActionResult Index(IndexRouteParams routeParams) {

			return RedirectToAction("Index", "Search", new SearchRouteParams {
				searchType = EntryType.Artist, filter = routeParams.filter, sort = routeParams.sort,
				artistType = routeParams.artistType
			});

        }

		// Doesn't seem this is in use?
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
			var queryParams = new SongQueryParams {
				Paging = PagingProperties.CreateFromPage(pageIndex, entriesPerPage, true),
				SortRule = SongSortRule.Name,
				ArtistId = id
			};
			var result = Services.Songs.Find(queryParams);

			var data = new PagingData<SongContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "SongsPaged", "ui-tabs-4");

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

			var result = Service.FindDuplicates(new[] { term1, term2, term3 }, linkUrl).Select(e => new DuplicateEntryResultContract<ArtistEditableFields>(e, ArtistEditableFields.Names));
			return LowercaseJson(result);

		}

		public ActionResult FindJson(string term, string artistTypes) {

			var matchMode = NameMatchMode.Auto;
			term = FindHelpers.GetMatchModeAndQueryForSearch(term, ref matchMode);

			var typeVals = !string.IsNullOrEmpty(artistTypes)
				? artistTypes.Split(',').Select(EnumVal<ArtistType>.Parse).ToArray()
				: new ArtistType[] {};

			var queryParams = new ArtistQueryParams(term, typeVals, 0, 20, false, false, matchMode, ArtistSortRule.Name, true);
			var artists = Service.FindArtists(queryParams);

			return Json(artists);

		}

        //
        // GET: /Artist/Details/5

        public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			WebHelper.VerifyUserAgent(Request);
			var model = queries.GetDetails(id);
			PageProperties.Description = model.Description;
            return View(model);

        }

		public ActionResult Picture(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var artist = Service.GetArtistPicture(id, Size.Empty);

			return Picture(artist);

		}

		public ActionResult PictureThumb(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var artist = queries.GetPictureThumb(id);
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
			var pictureData = ParsePicture(coverPicUpload, "Picture");

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();
			contract.PictureData = pictureData;

			var album = queries.Create(contract);
			return RedirectToAction("Edit", new { id = album.Id });

		}
        
        //
        // GET: /Artist/Edit/5
        [Authorize]
        public ActionResult Edit(int id = invalidId)
        {

			if (id == invalidId)
				return NoId();

			CheckConcurrentEdit(EntryType.Artist, id);

        	var model = new ArtistEditViewModel(Service.GetArtist(id), PermissionContext);
            return View(model);

        }

        //
        // POST: /Artist/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(ArtistEditViewModel viewModel)
        {

			var model = viewModel.EditedArtist;

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names.All(n => string.IsNullOrEmpty(n.Value))) {
				ModelState.AddModelError("Names", Model.Resources.ArtistValidationErrors.UnspecifiedNames);
			}

			var coverPicUpload = Request.Files["pictureUpload"];
			var pictureData = ParsePicture(coverPicUpload, "Picture");

			ParseAdditionalPictures(coverPicUpload, model.Pictures);

			try {
				viewModel.CheckModel();
			} catch (InvalidFormException x) {
				AddFormSubmissionError(x.Message);
			}

			if (!ModelState.IsValid) {
				return View("Edit", new ArtistEditViewModel(Service.GetArtist(model.Id), PermissionContext, model));
			}

			queries.Update(model, pictureData, LoginManager);

			return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete("Happens on the album side")]
		public PartialViewResult AddNewAlbum(int artistId, string newAlbumName) {

			ParamIs.NotNullOrWhiteSpace(() => newAlbumName);

			var link = new AlbumForArtistEditContract(newAlbumName);

			return PartialView("ArtistForAlbumRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		[Obsolete("Happens on the album side")]
		public PartialViewResult AddExistingAlbum(int artistId, int albumId) {

			var album = Services.Albums.GetAlbumWithAdditionalNames(albumId);
			var link = new AlbumForArtistEditContract(album);

			return PartialView("ArtistForAlbumRow", link);

		}

        //
        // GET: /Artist/Delete/5
 
        public ActionResult Delete(int id)
        {

			Service.Delete(id);
			return RedirectToAction("Details", new { id });

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
