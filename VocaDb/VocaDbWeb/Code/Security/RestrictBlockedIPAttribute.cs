using System.Web.Mvc;
using NLog;

namespace VocaDb.Web.Code.Security {

	public class RestrictBlockedIPAttribute : ActionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override void OnActionExecuting(ActionExecutingContext filterContext) {

			if (MvcApplication.IsAllowedIP(filterContext.HttpContext.Request.UserHostAddress))
				return;

			if (filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), false)) {
				log.Warn("Restricting blocked IP " + filterContext.HttpContext.Request.UserHostAddress);
				filterContext.Result = new HttpUnauthorizedResult();
			}

		}

	}

}