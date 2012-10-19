using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Feeds;
using VocaDb.Web.Models;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.DataContracts;
using VocaDb.Web.Models.Song;
using System;
using VocaDb.Model.Helpers;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Controllers
{
    public class SongController : ControllerBase
    {

		private SongService Service {
			get { return MvcApplication.Services.Songs; }
		}

		[HttpPost]
		public void AddSongToList(int listId, int songId) {

			Service.AddSongToList(listId, songId);

		}

		public PartialViewResult Comments(int id) {

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

			SongWithAdditionalNamesContract song;

			if (songId == null)
				song = new SongWithAdditionalNamesContract();
			else
				song = Service.GetSongWithAdditionalNames(songId.Value);

			return PartialView("SongLink", song);

		}

		[HttpPost]
		public void DeleteComment(int commentId) {

			Service.DeleteComment(commentId);

		}

		public ActionResult Index(IndexParams indexParams) {

			WebHelper.VerifyUserAgent(Request);

			var pageSize = Math.Min(indexParams.pageSize, 30);
			var page = indexParams.page;
			var sortRule = indexParams.sort;
			var timeFilter = DateTimeUtils.ParseFromSimpleString(indexParams.since);
			var filter = indexParams.filter;
			var songType = indexParams.songType;
			var draftsOnly = indexParams.draftsOnly;
			var onlyWithPVs = indexParams.onlyWithPVs;
			var view = indexParams.view;

			var queryParams = new SongQueryParams(filter,
				songType != SongType.Unspecified ? new[] { songType } : new SongType[] { },
				(page - 1) * pageSize, pageSize, draftsOnly, true, NameMatchMode.Auto, sortRule, false, false, null) {

				TimeFilter = timeFilter,
				OnlyWithPVs = onlyWithPVs
			};

			var result = Service.FindWithAlbum(queryParams, view == SongViewMode.Preview);

			if (page == 1 && result.TotalCount == 1 && result.Items.Length == 1) {
				return RedirectToAction("Details", new { id = result.Items[0].Id });
			}

			SetSearchEntryType(EntryType.Song);
			var model = new Index(result, filter, songType, indexParams.since, onlyWithPVs, sortRule, view, draftsOnly, page, pageSize);

        	return View(model);

        }

		[HttpPost]
		public ActionResult FindDuplicate(string term1, string term2, string term3, string pv1, string pv2) {

			var result = Service.FindDuplicates(new[] { term1, term2, term3 }, new[] { pv1, pv2 });

			if (result.Any()) {
				return PartialView("DuplicateEntryMessage", result);
			} else {
				return Content("Ok");
			}

		}

		public ActionResult FindNames(string term) {

			return Json(Service.FindNames(term, 15));

		}

		public ActionResult FindJsonByName(string term, string songTypes, bool alwaysExact = false, int[] ignoredIds = null) {

			var typeVals = !string.IsNullOrEmpty(songTypes)
				? songTypes.Split(',').Select(EnumVal<SongType>.Parse).ToArray()
				: new SongType[] { };

			var songs = Service.Find(new SongQueryParams(term, typeVals, 0, 20, 
				draftsOnly: false, 
				getTotalCount: false, 
				onlyByName: true, 
				nameMatchMode: (alwaysExact ? NameMatchMode.Exact : NameMatchMode.Auto), 
				sortRule: SongSortRule.Name, 
				ignoredIds: ignoredIds,
				moveExactToTop: false));

			return Json(songs);

		}

		public ActionResult SongListsForUser(int ignoreSongId) {

			var result = Service.GetSongListsForCurrentUser(ignoreSongId);
			return Json(result);

		}

        //
        // GET: /Song/Details/5

        public ActionResult Details(int id) {

			WebHelper.VerifyUserAgent(Request);
			SetSearchEntryType(EntryType.Song);

			var model = new SongDetails(Service.GetSongDetails(id, WebHelper.IsValidHit(Request) ? WebHelper.GetRealHost(Request) : string.Empty));

            return View(model);

        }

        //
        // POST: /Song/Create

		/*
		[Obsolete("Disabled")]
		[HttpPost]
		public ActionResult CreateQuick(ObjectCreate model)
        {

			if (ModelState.IsValid) {

				var song = Service.Create(model.Name);
				return RedirectToAction("Edit", new { id = song.Id });

			} else {

				return RedirectToAction("Index");

			}

        }
		*/

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
				return RedirectToAction("Details", new { id = song.Id });
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
			var model = new SongEdit(Service.GetSongForEdit(id));
			return View(model);
		}

        //
        // POST: /Song/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Edit(SongEdit model)
        {

			/*foreach (var link in model.WebLinks) {
				if (!UrlValidator.IsValid(link.Url))
					ModelState.AddModelError("WebLinks", link.Url + " is not a valid URL.");
			}*/

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
		public PartialViewResult AddNewArtist(int songId, string newArtistName) {

			//var link = Service.AddArtist(songId, newArtistName);
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

		public FeedResult LatestVideos() {

			var songs = Service.GetNewSongsWithVideos();
			var items = songs.Select(s => 
				new SyndicationItem(s.Name, new TextSyndicationContent(RenderPartialViewToString("SongItem", s), TextSyndicationContentKind.Html), 
					VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id = s.Id })), s.Id.ToString(), s.CreateDate));

			var feed = new SyndicationFeed("Latest songs with videos", string.Empty, VocaUriBuilder.CreateAbsolute(Url.Action("Index")), items);

			return new FeedResult(new Atom10FeedFormatter(feed));

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

		public PartialViewResult PVForSong(int pvId) {

			var pv = Service.PVForSong(pvId);
			return PartialView("PVEmbedDynamic", pv);

		}

		public PartialViewResult PVEmbedNND(int pvId) {

			var pv = Service.PVForSong(pvId);
			return PartialView("PVEmbedNND", pv);

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

		public PartialViewResult TagSelections(int songId) {

			var contract = Service.GetTagSelections(songId, LoginManager.LoggedUserId);

			return PartialView(contract);

		}

		[HttpPost]
		public PartialViewResult TagSelections(int songId, string tagNames) {

			string[] tagNameParts = (tagNames != null ? tagNames.Split(',').Where(s => s != string.Empty).ToArray() : new string[] { });

			var tagUsages = Service.SaveTags(songId, tagNameParts);

			return PartialView("TagList", tagUsages);

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
