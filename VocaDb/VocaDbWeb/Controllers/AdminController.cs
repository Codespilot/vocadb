using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Admin;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Controllers
{
    public class AdminController : ControllerBase
    {

		private CommentViewModel CreateCommentView(UnifiedCommentContract contract) {

			string name;
			string url;

			if (contract.Album != null) {
				name = contract.Album.Name;
				url = Url.Action("Details", "Album", new { id = contract.Album.Id });
			} else {
				name = contract.Artist.Name;
				url = Url.Action("Details", "Artist", new { id = contract.Artist.Id });
			}

			return new CommentViewModel(contract, name, url);
				
		}

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

		public ActionResult CreateXmlDump() {

			Service.CreateXmlDump();

			TempData.SetStatusMessage("Dump created");

			return RedirectToAction("Index");

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

		public ActionResult RecentComments() {

			var comments = Service.GetRecentComments();
			var models = comments.Select(c => CreateCommentView(c)).ToArray();

			return View(models);

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

    }
}
