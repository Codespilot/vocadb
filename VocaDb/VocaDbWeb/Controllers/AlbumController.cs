using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Model;
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

		private readonly Size pictureThumbSize = new Size(250, 250);

		private AlbumService Service {
			get { return Services.Albums; }
		}

		public ActionResult ArchivedVersionCoverPicture(int id) {

			var contract = Service.GetArchivedAlbumPicture(id);

			return Picture(contract);

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

		[Obsolete("Integrated to saving properties")]
		[HttpPost]
		public void DeletePVForAlbum(int pvForAlbumId) {

			Service.DeletePv(pvForAlbumId);

		}

		[HttpGet]
		public ActionResult ImportFromFile(int? id) {

			var album = (id != null ? Service.GetAlbum(id.Value) : null);

			var contract = new ParseAlbumFileResultContract(album);

			return View(contract);

		}

		[HttpPost]
		public ActionResult ImportFromFileParse(int? id) {

			if (Request.Files.Count == 0)
				return RedirectToAction("ImportFromFile", new { id });

			var file = Request.Files[0];

			var parser = new AlbumFileParser();
			var imported = parser.Parse(file.InputStream);
			var album = (id != null ? Service.GetAlbum(id.Value) : null);
			var contract = new ParseAlbumFileResultContract(album) { Imported = imported };

			return View("ImportFromFile", contract);

		}

        //
        // GET: /Album/

		public ActionResult Index(string filter, DiscType? discType, AlbumSortRule? sort, EntryViewMode? view, int? page, bool? draftsOnly) {

			WebHelper.VerifyUserAgent(Request);
			var dType = discType ?? DiscType.Unknown;
			var sortRule = sort ?? AlbumSortRule.Name;
			var viewMode = view ?? EntryViewMode.Details;

			var result = Service.Find(filter, dType, ((page ?? 1) - 1) * 30, 30, draftsOnly ?? false,
				true, moveExactToTop: false, sortRule: sortRule);

			if (page == null && result.TotalCount == 1 && result.Items.Length == 1) {
				return RedirectToAction("Details", new { id = result.Items[0].Id });
			}

			var model = new Index(result, filter, dType, sortRule, viewMode, page, draftsOnly);
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

			var albums = Service.Find(term, DiscType.Unknown, 0, 20, false, false, moveExactToTop: true);

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

			var contract = Service.GetArtists(albumId, ArtistHelper.SongArtistTypes);

			return PartialView(contract);

		}

        //
        // GET: /Album/Details/5

        public ActionResult Details(int id) {

			WebHelper.VerifyUserAgent(Request);
			SetSearchEntryType(EntryType.Album);

			var model = Service.GetAlbumDetails(id, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty);

            return View(new AlbumDetails(model));

        }

		public FileContentResult DownloadTags(int id) {

			var album = Service.GetAlbumDetails(id, null);

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
		public PartialViewResult CreateComment(int entryId, string message) {

			var comment = Service.CreateComment(entryId, message);

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

			var album = Service.Create(contract);
			return RedirectToAction("Details", new { id = album.Id });

		}

        //
        // GET: /Album/Edit/5
        [Authorize]
        public ActionResult Edit(int id) {

        	var album = Service.GetAlbumForEdit(id);

			return View(new AlbumEdit(album));

        }

        //
        // POST: /Album/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(AlbumEdit model)
        {

			if (!OptionalDateTime.IsValid(model.ReleaseYear, model.ReleaseDay, model.ReleaseMonth))
				ModelState.AddModelError("ReleaseYear", "Invalid date");

			var coverPicUpload = Request.Files["coverPicUpload"];
			PictureDataContract pictureData = ParseMainPicture(coverPicUpload, "CoverPicture");

			ParseAdditionalPictures(coverPicUpload, model.Pictures);

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

		[HttpPost]
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

		[Authorize]
		public ActionResult Deleted() {

			var result = Service.GetDeleted(0, entriesPerPage);

			var data = new PagingData<AlbumWithAdditionalNamesContract>(result.Items.ToPagedList(0, entriesPerPage, result.TotalCount), null, "DeletedPaged", "albums");

			return View(data);

		}

		[Authorize]
		public ActionResult DeletedPaged(int? page) {

			var p = (page - 1) ?? 0;
			var result = Service.GetDeleted(p * entriesPerPage, entriesPerPage);

			var data = new PagingData<AlbumWithAdditionalNamesContract>(result.Items.ToPagedList(p, entriesPerPage, result.TotalCount), null, "DeletedPaged", "albums");

			return PartialView("PagedAlbums", data);

		}

		[Obsolete]
		public ActionResult MassTagSongs(int id) {

			var album = Service.GetAlbumDetails(id, null);
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

		[Authorize]
		public ActionResult MoveToTrash(int id) {

			Service.MoveToTrash(id);

			TempData.SetStatusMessage("Entry moved to trash");

			return RedirectToAction("Deleted");

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
			var ids = idStr.Where(i => !string.IsNullOrEmpty(i)).Select(int.Parse).ToArray();

			var artistString = MvcApplication.Services.Songs.UpdateArtists(songId, ids);
			return Content(artistString);

		}

		[HttpPost]
		public void UpdateArtistForAlbumIsSupport(int artistForAlbumId, bool isSupport) {

			Service.UpdateArtistForAlbumIsSupport(artistForAlbumId, isSupport);

		}

		[HttpPost]
		public void UpdateArtistForAlbumRoles(int artistForAlbumId, ArtistRoles[] roles) {

			var rolesBitField = (roles != null ? roles.Aggregate(ArtistRoles.Default, (list, item) => list |= item) : ArtistRoles.Default);

			Service.UpdateArtistForAlbumRoles(artistForAlbumId, rolesBitField);

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

		public ActionResult ViewVersion(int id, int? ComparedVersionId) {

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }
}
