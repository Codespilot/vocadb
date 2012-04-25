using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
    public class ErrorController : ControllerBase
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
			Response.StatusCode = 500;
            return View();
        }

		public ActionResult NotFound() {
			Response.StatusCode = 404;
			return View();
		}

    }
}
