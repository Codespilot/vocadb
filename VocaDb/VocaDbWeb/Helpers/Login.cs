using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model.Service.Security;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Helpers {

	public static class Login {

		public static LoginManager Manager {
			get {
				return MvcApplication.LoginManager;
			}
		}

		public static UserContract User {
			get {
				return Manager.LoggedUser;
			}
		}

	}

}