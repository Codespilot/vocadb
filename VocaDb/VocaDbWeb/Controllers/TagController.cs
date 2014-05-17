using System;
using System.Linq;
using System.Web.Mvc;
using MvcPaging;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.Tag;
using VocaDb.Web.Resources.Controllers;

namespace VocaDb.Web.Controllers
{
    public class TagController : ControllerBase {

	    private readonly TagQueries queries;
	    private readonly TagService service;

		private TagService Service {
			get { return service; }
		}

		public TagController(TagService service, TagQueries queries) {

			this.service = service;
			this.queries = queries;

		}

		public ActionResult Albums(string id) {

			return RedirectToActionPermanent("AlbumTagUsages", new { id });

		}

		public ActionResult AlbumTagUsages(string id, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetAlbums(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<AlbumTagUsageContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "AlbumTagUsages", "ui-tabs-2");

			if (Request.IsAjaxRequest())
				return PartialView("AlbumTagUsages", data);
			else
				return View("AlbumTagUsages", data);

		}

		public ActionResult ArtistTagUsages(string id, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetArtists(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<ArtistTagUsageContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "ArtistTagUsages", "ui-tabs-1");

			if (Request.IsAjaxRequest())
				return PartialView("ArtistTagUsages", data);
			else
				return View("ArtistTagUsages", data);

		}

		public ActionResult Artists(string id) {

			return RedirectToActionPermanent("ArtistTagUsages", new { id });

		}

		public ActionResult Create(string name) {

			if (string.IsNullOrWhiteSpace(name))
				return Json(new GenericResponse<string>(false, TagControllerStrings.TagNameError));

			name = name.Trim().Replace(' ', '_');

			if (!Tag.TagNameRegex.IsMatch(name))
				return Json(new GenericResponse<string>(false, TagControllerStrings.TagNameError));

			var view = RenderPartialViewToString("TagSelection", new TagSelectionContract(name, true));

			return Json(new GenericResponse<string>(view));

		}

		public ActionResult Delete(string id) {

			Service.Delete(id);

			TempData.SetStatusMessage("Tag deleted");

			return RedirectToAction("Index");

		}

		public ActionResult Details(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var contract = Service.GetTagDetails(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);

		}

        [Authorize]
        public ActionResult Edit(string id)
        {
			var model = new TagEdit(Service.GetTagForEdit(id));
			return View(model);
		}

		[HttpPost]
        [Authorize]
        public ActionResult Edit(TagEdit model)
        {

			var coverPicUpload = Request.Files["thumbPicUpload"];
			UploadedFileContract uploadedPicture = null;
			if (coverPicUpload != null && coverPicUpload.ContentLength > 0) {

				CheckUploadedPicture(coverPicUpload, "thumbPicUpload");
				uploadedPicture = new UploadedFileContract { Mime = coverPicUpload.ContentType, Stream = coverPicUpload.InputStream };

			}

			if (!ModelState.IsValid) {
				var contract = Service.GetTagForEdit(model.Name);
				model.CopyNonEditableProperties(contract);
				return View(model);
			}

			queries.UpdateTag(model.ToContract(), uploadedPicture);

			return RedirectToAction("Details", new { id = model.Name });

		}

		[Obsolete("Moved to Web API")]
		public ActionResult Find(string term, bool allowAliases = true) {

			return Json(queries.FindNames(term, allowAliases, false, 10));

		}

		public ActionResult FindCategories(string term) {

			return Json(Service.FindCategories(term));

		}

		public ActionResult Index(string filter = null) {

			var tags = Service.GetTagsByCategories();

			if (!string.IsNullOrEmpty(filter)) {

				var tag = Service.GetTag(filter);

				if (tag != null) {
					return RedirectToAction("Details", new { id = tag.Name});
				}

			}

			return View(tags);

		}

		public ActionResult Songs(string id) {

			return RedirectToActionPermanent("SongTagUsages", new { id });

		}

		public ActionResult SongTagUsages(string id, int? page) {

			var pageIndex = (page - 1) ?? 0;
			var result = Service.GetSongs(id, pageIndex * entriesPerPage, entriesPerPage);
			var data = new PagingData<SongTagUsageContract>(result.Items.ToPagedList(pageIndex, entriesPerPage, result.TotalCount), id, "SongTagUsages", "ui-tabs-3");

			if (Request.IsAjaxRequest())
				return PartialView("SongTagUsages", data);
			else
				return View("SongTagUsages", data);

		}

		public ActionResult Versions(string id) {

			if (string.IsNullOrEmpty(id))
				return NoId();

			var contract = Service.GetTagWithArchivedVersions(id);

			if (contract == null)
				return HttpNotFound();

			return View(contract);

		}

    }
}
