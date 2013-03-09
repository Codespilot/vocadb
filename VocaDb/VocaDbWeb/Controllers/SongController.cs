using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Feeds;
using VocaDb.Web.Models;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.DataContracts;
using VocaDb.Web.Models.Song;
using System;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Controllers
{
    public class SongController : ControllerBase
    {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

		[HttpPost]
		public void AddSongToList(int listId, int songId) {

			Service.AddSongToList(listId, songId);

		}

		public PartialViewResult Comments(int id = invalidId) {

			var comments = Service.GetComments(id);
			return PartialView("DiscussionContent", comments);

		}

		[HttpPost]
		public PartialViewResult CreateComment(int songId, string message) {

			var comment = Service.CreateComment(songId, message);

			return PartialView("Comment", comment);

		}

		[HttpPost]
		public void CreateReport(int songId, SongReportType reportType, string notes) {

			Service.CreateReport(songId, reportType, CfHelper.GetRealIp(Request), notes ?? string.Empty);

		}

		[HttpPost]
		public PartialViewResult CreateSongLink(int? songId) {

			SongContract song;

			if (songId == null)
				song = new SongContract();
			else
				song = Service.GetSong(songId.Value);

			return PartialView("SongLink", song);

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

		public ActionResult Index(IndexRouteParams indexParams) {

			WebHelper.VerifyUserAgent(Request);

			var pageSize = (indexParams.pageSize.HasValue ? Math.Min(indexParams.pageSize.Value, 30) : 30);
			var page = indexParams.page ?? 1;
			var sortRule = indexParams.sort ?? SongSortRule.Name;
			var timeFilter = DateTimeUtils.ParseFromSimpleString(indexParams.since);
			var filter = indexParams.filter;
			var songType = indexParams.songType ?? SongType.Unspecified;
			var draftsOnly = indexParams.draftsOnly ?? false;
			var matchMode = indexParams.matchMode ?? NameMatchMode.Auto;
			var onlyWithPVs = indexParams.onlyWithPVs ?? false;
			var view = indexParams.view ?? SongViewMode.Details;

			if (matchMode == NameMatchMode.Auto && filter != null && filter.Length <= 2)
				matchMode = NameMatchMode.StartsWith;

			var queryParams = new SongQueryParams(filter,
				songType != SongType.Unspecified ? new[] { songType } : new SongType[] { },
				(page - 1) * pageSize, pageSize, draftsOnly, true, matchMode, sortRule, false, false, null) {

				TimeFilter = timeFilter,
				OnlyWithPVs = onlyWithPVs,
				ArtistId = indexParams.artistId ?? 0
			};

			var result = Service.FindWithAlbum(queryParams, view == SongViewMode.Preview);

			if (page == 1 && result.TotalCount == 1 && result.Items.Length == 1) {
				return RedirectToAction("Details", new { id = result.Items[0].Id });
			}

			SetSearchEntryType(EntryType.Song);
			var model = new Index(result, filter, matchMode, songType, indexParams.since, onlyWithPVs, sortRule, view, draftsOnly, page, pageSize, indexParams);

        	return View(model);

        }

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3, string pv1, string pv2) {

			var result = Service.FindDuplicates(new[] { term1, term2, term3 }, new[] { pv1, pv2 });

			return Json(result);

			/*if (result.Any()) {
				return PartialView("DuplicateEntryMessage", result);
			} else {
				return Content("Ok");
			}*/

		}

		public ActionResult FindNames(string term) {

			return Json(Service.FindNames(term, 15));

		}

		public ActionResult FindJsonByName(string term, string songTypes, bool alwaysExact = false, int[] ignoredIds = null) {

			var typeVals = !string.IsNullOrEmpty(songTypes)
				? songTypes.Split(',').Select(EnumVal<SongType>.Parse).ToArray()
				: new SongType[] { };

			var songs = Service.Find(new SongQueryParams(term, typeVals, 0, 40, 
				draftsOnly: false, 
				getTotalCount: false, 
				onlyByName: true, 
				nameMatchMode: (alwaysExact ? NameMatchMode.Exact : NameMatchMode.Auto), 
				sortRule: SongSortRule.Name, 
				ignoredIds: ignoredIds,
				moveExactToTop: true));

			return Json(songs);

		}

		public ActionResult SongListsForUser(int ignoreSongId) {

			var result = Service.GetSongListsForCurrentUser(ignoreSongId);
			return Json(result);

		}

        //
        // GET: /Song/Details/5

        public ActionResult Details(int id = invalidId) {

			if (id == invalidId)
				return NoId();

			WebHelper.VerifyUserAgent(Request);
			SetSearchEntryType(EntryType.Song);

			var model = new SongDetails(Service.GetSongDetails(id, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty));

            return View(model);

        }

		[Authorize]
		public ActionResult Create() {

			return View(new Create());

		}

		[HttpPost]
		public ActionResult Create(Create model) {

			if (string.IsNullOrWhiteSpace(model.NameOriginal) && string.IsNullOrWhiteSpace(model.NameRomaji) && string.IsNullOrWhiteSpace(model.NameEnglish))
				ModelState.AddModelError("Names", ViewRes.EntryCreateStrings.NeedName);

			if (model.Artists == null || !model.Artists.Any())
				ModelState.AddModelError("Artists", ViewRes.Song.CreateStrings.NeedArtist);

			if (!ModelState.IsValid)
				return View(model);

			var contract = model.ToContract();

			try {
				var song = Service.Create(contract);
				return RedirectToAction("Edit", new { id = song.Id });
			} catch (VideoParseException x) {
				ModelState.AddModelError("PVUrl", x.Message);
				return View(model);
			}

		}
       
        //
        // GET: /Song/Edit/5 
        [Authorize]
        public ActionResult Edit(int id)
        {

			CheckConcurrentEdit(EntryType.Song, id);

			var model = new SongEdit(Service.GetSongForEdit(id));
			return View(model);

		}

        //
        // POST: /Song/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(SongEdit model)
        {

			if (!ModelState.IsValid) {
				var oldContract = Service.GetSongForEdit(model.Id);
				model.CopyNonEditableFields(oldContract);
				return View(model);				
			}

			var contract = model.ToContract();
			Service.UpdateBasicProperties(contract);

			return RedirectToAction("Details", new { id = model.Id });

        }

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingArtist(int songId, int artistId) {

			//var link = Service.AddArtist(songId, artistId);
			var artist = MvcApplication.Services.Artists.GetArtistWithAdditionalNames(artistId);
			var link = new ArtistForSongContract(artist);
			return PartialView("ArtistForSongEditRow", link);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AddNewArtist(int songId, string newArtistName) {

			if (string.IsNullOrWhiteSpace(newArtistName)) {
				log.Warn("Artist name for song was null or whitespace");
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Artist name cannot be null or whitespace");
			}

			var link = new ArtistForSongContract(newArtistName);
			return PartialView("ArtistForSongEditRow", link);

		}

		[Obsolete("Integrated to saving properties")]
		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteArtistForSong(int artistForSongId) {

			Service.DeleteArtistForSong(artistForSongId);

		}

		/*
		[Obsolete("Integrated to saving properties")]
		public PartialViewResult CreatePVForSong(int songId, PVService service, string pvId, PVType type) {

			ParamIs.NotNullOrEmpty(() => pvId);

			var contract = Service.CreatePVForSong(songId, service, pvId, type);

			return PartialView("PVForSongEditRow", contract);

		}*/

		[HttpPost]
		public ActionResult CreatePVForSongByUrl(int songId, string pvUrl, PVType type) {

			var result = VideoServiceHelper.ParseByUrl(pvUrl, true);

			if (!result.IsOk) {
				return Json(new GenericResponse<string>(false, result.Exception.Message));
			}

			var contract = new PVContract(result, type);

			var view = RenderPartialViewToString("PVForSongEditRow", contract);
			return Json(new GenericResponse<string>(view));

		}

		[Obsolete("Integrated to saving properties")]
		[HttpPost]
		public void DeletePVForSong(int pvForSongId) {

			Service.DeletePvForSong(pvForSongId);

		}

		public PartialViewResult CreateLyrics() {
			
			var entry = new LyricsForSongContract();

			return PartialView("LyricsForSongEditRow", new LyricsForSongModel(entry));

		}

        //
        // GET: /Song/Delete/5

        public ActionResult Delete(int id)
        {
            
			Service.Delete(id);
			return RedirectToAction("Index");

        }

		public FeedResult Feed(IndexRouteParams indexParams) {

			WebHelper.VerifyUserAgent(Request);

			var pageSize = (indexParams.pageSize.HasValue ? Math.Min(indexParams.pageSize.Value, 30) : 30);
			var sortRule = indexParams.sort ?? SongSortRule.Name;
			var timeFilter = DateTimeUtils.ParseFromSimpleString(indexParams.since);
			var filter = indexParams.filter;
			var songType = indexParams.songType ?? SongType.Unspecified;
			var draftsOnly = indexParams.draftsOnly ?? false;
			var matchMode = indexParams.matchMode ?? NameMatchMode.Auto;
			var onlyWithPVs = indexParams.onlyWithPVs ?? false;

			var queryParams = new SongQueryParams(filter,
				songType != SongType.Unspecified ? new[] { songType } : new SongType[] { },
				0, pageSize, draftsOnly, false, matchMode, sortRule, false, false, null) {

					TimeFilter = timeFilter,
					OnlyWithPVs = onlyWithPVs,
					ArtistId = indexParams.artistId ?? 0,
				};

			var result = Service.FindWithThumbPreferNotNico(queryParams);

			var fac = new SongFeedFactory();
			var feed = fac.Create(result.Items, 
				VocaUriBuilder.CreateAbsolute(Url.Action("Index", indexParams)), 
				song => RenderPartialViewToString("SongItem", song), 
				song => Url.Action("Details", new { id = song.Id }));

			return new FeedResult(new Atom10FeedFormatter(feed));

		}

		public FeedResult LatestVideos() {

			return Feed(new IndexRouteParams { onlyWithPVs = true, pageSize = 20, sort = SongSortRule.AdditionDate });

		}

        [Authorize]
        public ActionResult ManageTagUsages(int id) {

            var song = Service.GetEntryWithTagUsages(id);
            return View(song);

        }

		public ActionResult Merge(int id) {

			var song = Service.GetSong(id);
			return View(song);

		}

		[HttpPost]
		public ActionResult Merge(int id, int? targetSongId) {

			if (targetSongId == null) {
				ModelState.AddModelError("songList", "Song must be selected");
				return Merge(id);
			}

			Service.Merge(id, targetSongId.Value);

			return RedirectToAction("Edit", new { id = targetSongId.Value });

		}

		public ActionResult PVForSong(int pvId = invalidId) {

			if (pvId == invalidId)
				return NoId();

			var pv = Service.PVForSong(pvId);
			return PartialView("PVEmbedDynamic", pv);

		}

		public ActionResult PVEmbedNND(int pvId = invalidId) {

		    if (pvId == invalidId)
		        return NoId();

			var pv = Service.PVForSong(pvId);
			return PartialView("PVEmbedNND", pv);

		}

		public ActionResult PVRedirect(PVService service, string pvId) {

			var song = Service.GetSongWithPV(service, pvId);

			if (song == null) {

				TempData.SetWarnMessage("Sorry, song not found! Maybe it hasn't been added yet.");
				return RedirectToAction("Index", "Home");

			} else {

				return RedirectToAction("Details", new { id = song.Id });

			}

		}

        [Authorize]
        public ActionResult RemoveTagUsage(long id) {

            var songId = Service.RemoveTagUsage(id);
            TempData.SetStatusMessage("Tag usage removed");

            return RedirectToAction("ManageTagUsages", new { id = songId });

        }

		public ActionResult Restore(int id) {

			Service.Restore(id);

			return RedirectToAction("Edit", new { id = id });

		}

		public ActionResult RevertToVersion(int archivedSongVersionId) {

			var result = Service.RevertToVersion(archivedSongVersionId);

			TempData.SetStatusMessage(string.Join("\n", result.Warnings));

			return RedirectToAction("Edit", new { id = result.Id });

		}

		public PartialViewResult TagSelections(int songId = invalidId) {

			var contract = Service.GetTagSelections(songId, LoginManager.LoggedUserId);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int songId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(songId, tagNameParts);

			return PartialView("TagList", tagUsages);

		}

		public ActionResult UsersWithSongRating(int songId = invalidId) {

			if (songId == invalidId)
				return NoId();

			var users = Service.GetUsersWithSongRating(songId);
			return PartialView(users);
			//return Json(users);

		}

		public ActionResult Versions(int id) {

			var contract = Service.GetSongWithArchivedVersions(id);

			return View(new Versions(contract));

		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId) {

			var contract = Service.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(contract);

		}

    }
}
