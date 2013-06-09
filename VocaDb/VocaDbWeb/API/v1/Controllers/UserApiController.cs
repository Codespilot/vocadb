using System.Web.Mvc;
using System.Web.Security;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.API.v1.Controllers {

	public class UserApiController : Web.Controllers.ControllerBase {

		private UserService Service {
			get { return Services.Users; }
		}

		//[HttpPost]
		public ActionResult Authenticate(string username, string accesskey) {
			
			var user = Service.CheckAccessWithKey(username, accesskey, WebHelper.GetRealHost(Request));

			if (user == null) {
				return Content("Username or password doesn't match");
			} else {

				FormsAuthentication.SetAuthCookie(user.Name, true);
				return Content("OK");

			}

		}

 	}
}