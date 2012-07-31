using System.Linq;
using System.Web;
using NLog;

namespace VocaDb.Web.Helpers {

	public static class WebHelper {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static readonly string[] forbiddenUserAgents = new[] {
			"Googlebot", "bingbot"
		};

		public static string GetRealHost(HttpRequestBase request) {

			return CfHelper.GetRealIp(request);

		}

		public static bool IsValidHit(HttpRequestBase request) {

			var ua = request.UserAgent;

			if (string.IsNullOrEmpty(ua)) {
				log.Warn("Blank user agent from {0}", GetRealHost(request));
				return false;
			}

			return !forbiddenUserAgents.Any(ua.Contains);

		}

	}

}