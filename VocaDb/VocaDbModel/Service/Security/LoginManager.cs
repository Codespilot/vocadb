using System.Web;
using System.Web.Security;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.Security {

	public class LoginManager {

		public static string GetHashedPass(string name, string pass, int salt) {

			return FormsAuthentication.HashPasswordForStoringInConfigFile(name + pass + salt, "sha1");

		}

		public bool HasPermission(PermissionFlags flag) {

			return IsLoggedIn;

		}

		public bool IsLoggedIn {
			get {
				return HttpContext.Current.User.Identity.IsAuthenticated;
			}
		}

	}

}
