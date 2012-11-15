using System.Web.Mvc;
using Newtonsoft.Json;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Admin;

namespace VocaDb.Web.Controllers
{
    public class AdminController : ControllerBase
    {

    	private AdminService Service {
			get { return MvcApplication.Services.Admin; }
    	}

		[Authorize]
		public ActionResult AuditLogEntries(int? start, string Filter) {

			LoginManager.VerifyPermission(PermissionToken.ViewAuditLog);

			var entries = Service.GetAuditLog(Filter, start ?? 0, 200);

			return PartialView(entries);

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

        //
        // GET: /Admin/
		[Authorize]
		public ActionResult Index()
        {

			LoginManager.VerifyPermission(PermissionToken.AccessManageMenu);

            return View();

        }

		public ActionResult GeneratePictureThumbs() {
			
			Service.GeneratePictureThumbs();

			TempData.SetStatusMessage("Picture thumbnails recreated.");

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
			
			return View(rules);

		}

		public ActionResult RecentComments() {

			var comments = Service.GetRecentComments();

			return View(comments);

		}

		public ActionResult RefreshDbCache() {

			var sessionFactory = MvcApplication.SessionFactory;

			var classMetadata = sessionFactory.GetAllClassMetadata();
			foreach (var ep in classMetadata.Values) {
				sessionFactory.EvictEntity(ep.EntityName);
			}
 
			var collMetadata = sessionFactory.GetAllCollectionMetadata();
			foreach (var acp in collMetadata.Values) {
				sessionFactory.EvictCollection(acp.Role);
			}

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

    }
}
