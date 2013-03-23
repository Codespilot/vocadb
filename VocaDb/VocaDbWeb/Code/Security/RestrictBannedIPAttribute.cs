using System.Net;
using System.Web.Mvc;
using NLog;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Denies access to an action or controller for IPs that are banned.
	/// </summary>
	public class RestrictBannedIPAttribute : ActionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override void OnActionExecuting(ActionExecutingContext filterContext) {

			var host = filterContext.HttpContext.Request.UserHostAddress;

			if (MvcApplication.BannedIPs.Contains(host)) {
				log.Warn("Restricting banned host " + host);
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Sorry, access from your host is restricted. It is possible this restriction is no longer valid. If you think this is the case, please contact support.");
			}

		}

	}
}