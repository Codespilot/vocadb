﻿using System;
using System.Linq;
using System.Web.Mvc;
using NHibernate;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Admin;

namespace VocaDb.Web.Controllers
{
    public class AdminController : ControllerBase {

	    private readonly ISessionFactory sessionFactory;
		private AdminService Service { get; set; }

		public AdminController(AdminService service, ISessionFactory sessionFactory) {
			Service = service;
			this.sessionFactory = sessionFactory;
		}

		[Authorize]
		public ActionResult AuditLogEntries(ViewAuditLogModel model, int start = 0) {

			LoginManager.VerifyPermission(PermissionToken.ViewAuditLog);

			var excludeUsers = (!string.IsNullOrEmpty(model.ExcludeUsers) 
				? model.ExcludeUsers.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(u => u.Trim()).ToArray() 
				: new string[0]);
			var entries = Service.GetAuditLog(model.Filter, start, 200, 365, excludeUsers, model.OnlyNewUsers, model.GroupId);

			return PartialView(entries);

		}

		[Authorize]
		public ActionResult BannedIPs() {

			LoginManager.VerifyPermission(PermissionToken.ManageIPRules);

			var hosts = MvcApplication.BannedIPs.ToArray();
			return Json(hosts);

		}

		public ActionResult CleanupOldLogEntries() {

			var count = Service.CleanupOldLogEntries();

			TempData.SetStatusMessage("Cleanup complete - " + count + " entries removed.");

			return RedirectToAction("Index");

		}

		public ActionResult CreateMissingThumbs() {

			Service.CreateMissingThumbs();

			TempData.SetStatusMessage("Operation completed");

			return RedirectToAction("Index");

		}

		public ActionResult CreateXmlDump() {

			Service.CreateXmlDump();

			TempData.SetStatusMessage("Dump created");

			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult DeleteEntryReport(int id) {

			LoginManager.VerifyPermission(PermissionToken.ManageEntryReports);

			Service.DeleteEntryReports(new[] { id });
			TempData.SetStatusMessage("Reports deleted");

			return RedirectToAction("ViewEntryReports");

		}

		[Authorize]
		public ActionResult DeletePVsByAuthor(string author) {

			var count = Service.DeletePVsByAuthor(author, PVService.Youtube);

			TempData.SetSuccessMessage(string.Format("Deleted {0} PVs by '{1}'.", count, author));

			return View("PVsByAuthor", new PVsByAuthor(author ?? string.Empty, new PVForSongContract[] { }));

		}

        //
        // GET: /Admin/
		[Authorize]
		public ActionResult Index()
        {

			LoginManager.VerifyPermission(PermissionToken.AccessManageMenu);

            return View();

        }

		public ActionResult GeneratePictureThumbs() {
			
			var count = Service.GeneratePictureThumbs();

			TempData.SetStatusMessage(count + " picture thumbnails recreated.");

			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult ManageIPRules() {

			LoginManager.VerifyPermission(PermissionToken.ManageIPRules);

			var rules = Services.Other.GetIPRules();
			return View(rules);

		}

		[Authorize]
		[HttpPost]
		public ActionResult ManageIPRules([FromJson] IPRule[] rules) {

			LoginManager.VerifyPermission(PermissionToken.ManageIPRules);

			Service.UpdateIPRules(rules);
			MvcApplication.IPRules.Reset();

			TempData.SetSuccessMessage("IP rules updated.");
			
			return View(rules);

		}

		public ActionResult PVAuthorNames(string term) {

			var authors = Service.FindPVAuthorNames(term);

			return Json(authors);

		}

		[Authorize]
		public ActionResult PVsByAuthor(string author) {

			var songs = Service.GetSongPVsByAuthor(author ?? string.Empty);

			var model = new PVsByAuthor(author ?? string.Empty, songs);

			return View(model);

		}

		public ActionResult RefreshDbCache() {

			DatabaseHelper.ClearSecondLevelCache(sessionFactory);

			return RedirectToAction("Index");

		}

		public ActionResult UpdateAdditionalNames() {

			Service.UpdateAdditionalNames();
			TempData.SetStatusMessage("Updated additional names strings");
			return RedirectToAction("Index");

		}

		public ActionResult UpdateAlbumRatingTotals() {

			Service.UpdateAlbumRatingTotals();
			TempData.SetStatusMessage("Updated album rating totals");
			return RedirectToAction("Index");

		}

		public ActionResult UpdateArtistStrings() {
			
			Service.UpdateArtistStrings();

			return RedirectToAction("Index");

		}

		public ActionResult UpdateEntryStatuses() {

			var count = Service.UpdateEntryStatuses();

			TempData.SetStatusMessage(count + " entries updated");

			return RedirectToAction("Index");

		}

		public ActionResult UpdateLinkCategories() {

			Service.UpdateWebLinkCategories();
			TempData.SetStatusMessage("Updated link categories");

			return RedirectToAction("Index");

		}

		public ActionResult UpdateNicoIds() {
			
			Service.UpdateNicoIds();

			return RedirectToAction("Index");

		}

		public ActionResult UpdatePVIcons() {

			Service.UpdatePVIcons();

			return RedirectToAction("Index");

		}

		public ActionResult UpdateSongFavoritedTimes() {

			Service.UpdateSongFavoritedTimes();
			TempData.SetStatusMessage("Updated favorited song counts");
			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult UpdateTagVoteCounts() {

			var count = Service.UpdateTagVoteCounts();
			TempData.SetStatusMessage(string.Format("Updated tag vote counts, {0} corrections made", count));
			return RedirectToAction("Index");

		}

		[Authorize]
		public ActionResult ViewAuditLog(ViewAuditLogModel model) {

			LoginManager.VerifyPermission(PermissionToken.ViewAuditLog);

			return View(model ?? new ViewAuditLogModel());

		}

		[Authorize]
		public ActionResult ViewEntryReports() {

			LoginManager.VerifyPermission(PermissionToken.ManageEntryReports);

			var reports = Service.GetEntryReports();

			return View(reports);

		}

		[Authorize]
		public ActionResult ViewSysLog() {

			LoginManager.VerifyPermission(PermissionToken.ViewAuditLog);

			var logContents = new LogFileReader().GetLatestLogFileContents();

			return Content(logContents, "text/plain");

			//return View(new ViewSysLog(logContents));

		}

    }
}
