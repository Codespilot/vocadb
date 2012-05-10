using System;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers {

	public class ControllerBase : Controller {

		protected static readonly TimeSpan imageExpirationTime = TimeSpan.FromMinutes(5);
		protected const int entriesPerPage = 30;

		protected int LoggedUserId {
			get {

				LoginManager.VerifyLogin();

				return LoginManager.LoggedUser.Id;

			}
		}

		protected LoginManager LoginManager {
			get { return MvcApplication.LoginManager; }
		}

		protected ServiceModel Services {
			get { return MvcApplication.Services; }
		}

		protected ActionResult Picture(EntryForPictureDisplayContract contract) {

			ParamIs.NotNull(() => contract);

			Response.Cache.SetETag(string.Format("Artist{0}{1}v{2}", contract.EntryType, contract.EntryId, contract.Version));

			return Picture(contract.Picture, contract.Name);

		}

		protected ActionResult Picture(PictureContract pictureData, string title) {

			if (pictureData == null)
				return File(Server.MapPath("~/Content/unknown.png"), "image/png");

			var ext = ImageHelper.GetExtensionFromMime(pictureData.Mime);

			if (ext != null) {
				//var encoded = Url.Encode(title);
				Response.AddHeader("content-disposition", "inline;filename=\"" + title + ext + "\"");
				return File(pictureData.Bytes, pictureData.Mime);
			} else {
				return File(pictureData.Bytes, pictureData.Mime);
			}

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

		protected void SetSearchEntryType(EntryType entryType) {

			ViewData["GlobalSearchObjectType"] = entryType;

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