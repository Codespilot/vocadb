using System;

namespace VocaDb.Model.Service.Helpers {

	public static class UrlHelper {

		public static string MakeLink(string partialLink, bool assumeWww = false) {

			if (partialLink.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || partialLink.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
				return partialLink;

			if (assumeWww && !partialLink.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
				return string.Format("http://www.{0}", partialLink);

			return string.Format("http://{0}", partialLink);

		}

	}

}
