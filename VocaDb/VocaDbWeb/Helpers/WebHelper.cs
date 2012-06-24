using System.Linq;

namespace VocaDb.Web.Helpers {

	public static class WebHelper {

		private static readonly string[] forbiddenUserAgents = new[] {
			"Googlebot", "bingbot"
		};

		public static bool IsValidHit(string userAgent) {

			return !forbiddenUserAgents.Any(userAgent.Contains);

		}

	}

}