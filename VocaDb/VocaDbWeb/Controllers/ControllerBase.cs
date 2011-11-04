using System.Linq;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers {

	public class ControllerBase : Controller {

		protected LoginManager LoginManager {
			get { return MvcApplication.LoginManager; }
		}

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

		protected void RestoreErrorsFromTempData() {

			var list = TempData["ModelErrors"] as ModelStateList;

			if (list == null)
				return;

			foreach (var state in list.ModelStates) {
				if (ModelState[state.Key] == null || !ModelState[state.Key].Errors.Any()) {
					foreach (var err in state.Errors) {
						if (err.Exception != null)
							ModelState.AddModelError(state.Key, err.Exception);
						else
							ModelState.AddModelError(state.Key, err.ErrorMessage);
					}
				}
			}

		}

		protected void SaveErrorsToTempData() {

			var list = new ModelStateList { ModelStates 
				= ViewData.ModelState.Select(m => new ModelStateErrors(m.Key, m.Value)).ToArray() };

			TempData["ModelErrors"] = list;

		}

	}

	class ModelStateList {

		public ModelStateErrors[] ModelStates;

	}

	class ModelStateErrors {

		public ModelStateErrors() { }

		public ModelStateErrors(string key, ModelState state) {

			Key = key;
			Errors = state.Errors;

		}

		public string Key { get; set; }

		public ModelErrorCollection Errors { get; set; }

	}

}