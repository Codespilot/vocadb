using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web {

	public static class ViewExtension {

		public static LoginManager GetLoggedUser(this WebViewPage page) {
			
			return new LoginManager();

		}

	}

}