using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace VocaVoter.Model.Service.Security {

	public class LoginManager {

		public static string GetHashedPass(string email, string pass, int salt) {

			return FormsAuthentication.HashPasswordForStoringInConfigFile(email + pass + salt, "sha1");

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
