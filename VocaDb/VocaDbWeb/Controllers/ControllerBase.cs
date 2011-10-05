using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace VocaDb.Web.Controllers {

	public class ControllerBase : Controller {

		protected new ActionResult Json(object obj) {

			return Content(JsonConvert.SerializeObject(obj), "text/json");
	
		}

		protected string RenderPartialViewToString(string viewName, object model) {

			if (string.IsNullOrEmpty(viewName))
				viewName = ControllerContext.RouteData.GetRequiredString("action");

			ViewData.Model = model;

			using (var sw = new StringWriter()) {
				var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);

				return sw.GetStringBuilder().ToString();
			}

		}

	}

}