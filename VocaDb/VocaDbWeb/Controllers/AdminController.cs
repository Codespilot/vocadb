using System.Web.Mvc;
using NHibernate.Mapping;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
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

		public ActionResult UpdateArtistStrings() {
			
			Service.UpdateArtistStrings();

			return RedirectToAction("Index");

		}

    }
}
