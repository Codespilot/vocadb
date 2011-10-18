using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;

namespace VocaDb.Web.Code {

	public class HelperPage : System.Web.WebPages.HelperPage {

		// Workaround - exposes the MVC HtmlHelper instead of the normal helper
		public static new HtmlHelper Html {
			get { return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Html; }
		}

		public static ViewDataDictionary ViewData {
			get {
				return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).ViewData; 
			}
		}

	}
}
