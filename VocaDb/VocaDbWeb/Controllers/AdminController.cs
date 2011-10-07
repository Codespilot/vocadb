using System.Web.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
    public class AdminController : ControllerBase
    {

    	private AdminService Service {
			get { return MvcApplication.Services.Admin; }
    	}

        //
        // GET: /Admin/

        public ActionResult Index()
        {

			LoginManager.VerifyPermission(PermissionFlags.ManageDatabase);

            return View();

        }

		public ActionResult UpdateArtistStrings() {
			
			Service.UpdateArtistStrings();

			return RedirectToAction("Index");

		}

    }
}
