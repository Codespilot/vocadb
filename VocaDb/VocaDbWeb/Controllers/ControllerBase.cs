using System.Web.Mvc;
using Newtonsoft.Json;

namespace VocaDb.Web.Controllers {

	public class ControllerBase : Controller {

		protected new ActionResult Json(object obj) {

			return Content(JsonConvert.SerializeObject(obj), "text/json");
	
		}

	}

}