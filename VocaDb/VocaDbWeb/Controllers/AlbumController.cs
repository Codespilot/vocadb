﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using MvcPaging;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Resources;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.TagFormatting;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using System.Drawing;
using VocaDb.Model.Helpers;
using VocaDb.Web.Models.Album;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Controllers
{
    public class AlbumController : ControllerBase
    {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IEntryThumbPersister imagePersister;
		private readonly Size pictureThumbSize = new Size(250, 250);
	    private readonly AlbumQueries queries;
	    private readonly UserQueries userQueries;

		private AlbumService Service { get; set; }

		public AlbumController(AlbumService service, AlbumQueries queries, UserQueries userQueries, IEntryThumbPersister imagePersister) {

			Service = service;
			this.queries = queries;
			this.userQueries = userQueries;
			this.imagePersister = imagePersister;

		}

		public ActionResult ArchivedVersionCoverPicture(int id) {

			var contract = Service.GetArchivedAlbumPicture(id);

			return Picture(contract);

		}

		public ActionResult ArchivedVersionXml(int id) {

			var doc = Service.GetVersionXml(id);
			var content = XmlHelper.SerializeToUTF8XmlString(doc);

			return Xml(content);

		}

		[HttpPost]
		public ActionResult CreatePVForAlbumByUrl(int albumId, string pvUrl) {

			var result = VideoServiceHelper.ParseByUrl(pvUrl, true);

			if (!result.IsOk) {
				return Json(new GenericResponse<string>(false, result.Exception.Message));
			}

			var contract = new PVContract(result, PVType.Other);

			var view = RenderPartialViewToString("PVForSongEditRow", contract);
			return Json(new GenericResponse<string>(view));

		}

		[HttpPost]
		public void CreateReport(int albumId, AlbumReportType reportType, string notes) {

			Service.CreateReport(albumId, reportType, CfHelper.GetRealIp(Request), notes ?? string.Empty);

		}

        //
        // GET: /Album/

		public ActionResult Index(IndexRouteParams routeParams) {

			WebHelper.VerifyUserAgent(Request);

			var filter = routeParams.filter;
			var page = routeParams.page;
			var draftsOnly = routeParams.draftsOnly;
			var dType = routeParams.discType ?? DiscType.Unknown;
			var matchMode = routeParams.matchMode ?? NameMatchMode.Auto;
			var sortRule = routeParams.sort ?? AlbumSortRule.Name;
			var viewMode = routeParams.view ?? EntryViewMode.Details;

			filter = FindHelpers.GetMatchModeAndQueryForSearch(filter, ref matchMode);

			var queryParams = new AlbumQueryParams(filter, dType, ((page ?? 1) - 1) * 30, 30, draftsOnly ?? false,
				true, moveExactToTop: false, sortRule: sortRule, nameMatchMode: matchMode);

			var result = Service.Find(queryParams);

			if (page == null && result.TotalCount == 1 && result.Items.Length == 1) {
				return RedirectToAction("Details", new { id = result.Items[0].Id });
			}

			var model = new Index(result, filter, dType, sortRule, viewMode, page, draftsOnly, routeParams);
			SetSearchEntryType(EntryType.Album);

            return View(model);

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

		public ActionResult FindNames(string term) {

			return Json(Service.FindNames(term, 15));

		}

		public ActionResult FindReleaseEvents(string term) {

			return Json(Service.FindReleaseEvents(term));

		}

		public ActionResult FindJson(string term) {

			var matchMode = NameMatchMode.Auto;
			term = FindHelpers.GetMatchModeAndQueryForSearch(term, ref matchMode);
			var albums = Service.Find(term, DiscType.Unknown, 0, 20, false, false, moveExactToTop: true, nameMatchMode: matchMode);

			return Json(albums);

		}

		public ActionResult MikuDbRedirect() {

			return RedirectToActionPermanent("Index", "Home");

		}

		public ActionResult Name(int id) {

			var contract = Service.GetAlbum(id);
			return Content(contract.Name);

		}

		public ActionResult PopupContent(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			var album = Service.GetAlbum(id);
			return PartialView("AlbumPopupContent", album);

		}

		public ActionResult PopupWithCoverContent(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			var album = Service.GetAlbum(id);
			return PartialView("AlbumWithCoverPopupContent", album);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			WebHelper.VerifyUserAgent(Request);
			SetSearchEntryType(EntryType.Album);

			var model = Service.GetAlbumDetails(id, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty);
			PageProperties.Description = model.Description;

            return View(new AlbumDetails(model));

        }

		public FileContentResult DownloadTags(int id, string formatString = "", bool setFormatString = false, bool includeHeader = false) {

			if (setFormatString) {
				userQueries.SetAlbumFormatString(formatString);
			} else if (string.IsNullOrEmpty(formatString) && LoginManager.IsLoggedIn) {
				formatString = LoginManager.LoggedUser.AlbumFormatString;
			}

			if (string.IsNullOrEmpty(formatString))
				formatString = TagFormatter.TagFormatStrings[0];

			var album = Service.GetAlbum(id);
			var tagString = Service.GetAlbumTagString(id, formatString, includeHeader);

			var enc = new UTF8Encoding(true);
			var data = enc.GetPreamble().Concat(enc.GetBytes(tagString)).ToArray();

			return File(data, "text/csv", album.Name + ".csv");

		}

		public PartialViewResult Comments(int id = invalidId) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		//[OutputCache(Duration = pictureCacheDurationSec, Location = OutputCacheLocation.Any, VaryByParam = "id,v")]
		public ActionResult CoverPicture(int id = invalidId) {

			if (id == invalidId)
				return HttpNotFound();

			var album = Service.GetCoverPicture(id, Size.Empty);

			return Picture(album);

		}

		public ActionResult CoverPictureThumb(int id = invalidId) {

			if (id == invalidId) 
				return HttpNotFound();

			var data = queries.GetCoverPictureThumb(id);
			return Picture(data);

		}

		[HttpPost]
		public PartialViewResult CreateComment(int entryId, string message) {

			var comment = queries.CreateComment(entryId, message);

			return PartialView("Comment", comment);

		}

		[Authorize]
		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) 
				&& string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Album.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			var album = queries.Create(contract);
			return RedirectToAction("Edit", new { id = album.Id });

		}

        //
        // GET: /Album/Edit/5
        [Authorize]
        public ActionResult Edit(int id) {

			CheckConcurrentEdit(EntryType.Album, id);

        	var album = Service.GetAlbumForEdit(id);
			return View(new AlbumEdit(album));

        }

        //
        // POST: /Album/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(AlbumEdit model)
        {

			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names.AllNames.All(n => string.IsNullOrEmpty(n.Value))) {
				ModelState.AddModelError("Names", AlbumValidationErrors.UnspecifiedNames);
			}

			if (!OptionalDateTime.IsValid(model.ReleaseYear, model.ReleaseDay, model.ReleaseMonth))
				ModelState.AddModelError("ReleaseYear", "Invalid date");

			var coverPicUpload = Request.Files["coverPicUpload"];
			var pictureData = ParsePicture(coverPicUpload, "CoverPicture");

			if (coverPicUpload == null) {
				AddFormSubmissionError("Cover picture was null");
			}

			if (model.Pictures == null) {
				AddFormSubmissionError("List of pictures was null");
			}

			if (coverPicUpload != null && model.Pictures != null) {
				ParseAdditionalPictures(coverPicUpload, model.Pictures);				
			}

			if (!ModelState.IsValid) {
				var oldContract = Service.GetAlbumForEdit(model.Id);
				model.CopyNonEditableFields(oldContract);
				return View(model);
			}

			AlbumForEditContract contract;
			try {
				contract = model.ToContract();
			} catch (InvalidFormException x) {
				AddFormSubmissionError(x.Message);
				var oldContract = Service.GetAlbumForEdit(model.Id);
				model.CopyNonEditableFields(oldContract);
				return View(model);				
			}

			queries.UpdateBasicProperties(contract, pictureData);

        	return RedirectToAction("Details", new { id = model.Id });

        }

		public ActionResult Related(int id) {

			var related = queries.GetRelatedAlbums(id);
			return PartialView("RelatedAlbums", related);

		}

		[Authorize]
		public ActionResult RemoveTagUsage(long id) {

			var albumId = Service.RemoveTagUsage(id);
			TempData.SetStatusMessage("Tag usage removed");

			return RedirectToAction("ManageTagUsages", new { id = albumId });

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

        //
        // GET: /Album/Delete/5
 
        public ActionResult Delete(int id)
        
		{
			Service.Delete(id);
			return RedirectToAction("Details", new { id });

		}

		[Authorize]
		public ActionResult Deleted() {

			var result = Service.GetDeleted(0, entriesPerPage);

			var data = new PagingData<AlbumContract>(result.Items.ToPagedList(0, entriesPerPage, result.TotalCount), null, "DeletedPaged", "albums");
			data.RouteValues = new RouteValueDictionary(new { action = "DeletedPaged" });

			return View(data);

		}

		[Authorize]
		public ActionResult DeletedPaged(int? page) {

			var p = (page - 1) ?? 0;
			var result = Service.GetDeleted(p * entriesPerPage, entriesPerPage);

			var data = new PagingData<AlbumContract>(result.Items.ToPagedList(p, entriesPerPage, result.TotalCount), null, "DeletedPaged", "albums");
			data.RouteValues = new RouteValueDictionary(new { action = "DeletedPaged" });

			return PartialView("PagedAlbums", data);

		}

		[Authorize]
		public ActionResult ManageTagUsages(int id) {

			var album = Service.GetEntryWithTagUsages(id);
			return View(album);

		}

		public ActionResult Merge(int id) {

			var album = Service.GetAlbum(id);
			return View(album);

		}

		[HttpPost]
		public ActionResult Merge(int id, int targetAlbumId) {

			queries.Merge(id, targetAlbumId);

			return RedirectToAction("Edit", new { id = targetAlbumId });

		}

		[Authorize]
		public ActionResult MoveToTrash(int id) {

			Service.MoveToTrash(id);

			TempData.SetStatusMessage("Entry moved to trash");

			return RedirectToAction("Deleted");

		}

		public PartialViewResult TagSelections(int albumId = invalidId) {

			var contract = Service.GetTagSelections(albumId, LoginManager.LoggedUserId);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int albumId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(albumId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}

		public ActionResult UsersWithAlbumInCollection(int albumId = invalidId) {

			if (albumId == invalidId)
				return NoId();

			var users = Service.GetUsersWithAlbumInCollection(albumId);
			return PartialView(users);
			//return Json(users);

		}

		public ActionResult Versions(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetAlbumWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id = invalidId, int? ComparedVersionId = 0) {

			if (id == invalidId)
				return NoId();

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }
}
