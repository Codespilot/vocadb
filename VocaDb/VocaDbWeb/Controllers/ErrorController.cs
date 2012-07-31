using System.Web.Mvc;
using NLog;
using VocaDb.Web.Code;

namespace VocaDb.Web.Controllers
{
    public class ErrorController : ControllerBase
    {


        //
        // GET: /Error/

		public ActionResult Forbidden(bool? redirect) {

			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, ErrorLogger.Code_Forbidden);

			Response.StatusCode = ErrorLogger.Code_Forbidden;
			return View();

		}

        public ActionResult Index(int? code, bool? redirect)
        {

			var realCode = code ?? ErrorLogger.Code_InternalServerError;

			if (realCode == ErrorLogger.Code_Forbidden)
				return RedirectToAction("Forbidden", new { redirect });

			if (realCode == ErrorLogger.Code_NotFound)
				return RedirectToAction("NotFound", new { redirect });

			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, realCode);

			Response.StatusCode = realCode;
            return View();

        }

		public ActionResult NotFound(bool? redirect) {

			if (redirect.HasValue && redirect.Value == false)
				ErrorLogger.LogHttpError(Request, ErrorLogger.Code_NotFound);

			Response.StatusCode = ErrorLogger.Code_NotFound;
			return View();

		}

    }
}
