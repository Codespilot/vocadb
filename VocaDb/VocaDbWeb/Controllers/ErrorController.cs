using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class ErrorController : ControllerBase
    {
        //
        // GET: /Error/

        public ActionResult Index(int? code)
        {

			if (code == 404)
				return RedirectToAction("NotFound");

			Response.StatusCode = code ?? 500;
            return View();

        }

		public ActionResult NotFound() {
			Response.StatusCode = 404;
			return View();
		}

    }
}
