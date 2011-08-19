using System.Web.Security;

namespace VocaDb.Model.Service.Security {

	public class LoginManager {

		public static string GetHashedPass(string name, string pass, int salt) {

			return FormsAuthentication.HashPasswordForStoringInConfigFile(name + pass + salt, "sha1");

		}

		public bool IsAdmin {
			get {
				return false;
			}
		}

		public bool IsLoggedIn {
			get {
				return true;
			}
		}

	}

}
