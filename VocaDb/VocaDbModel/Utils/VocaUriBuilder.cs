using System;

namespace VocaDb.Model.Utils {

	public static class VocaUriBuilder {

		// Path to static files root, for example http://static.vocadb.net. Possible trailing slash is removed.
		private static readonly string staticResourceBase = RemoveTrailingSlash(AppConfig.StaticContentHost);

		/// <summary>
		/// Creates a full, absolute uri which includes the domain and scheme.
		/// </summary>
		/// <param name="relative">Relative address, for example /User/Profile/Test</param>
		/// <returns>Absolute address, for example http://vocadb.net/User/Profile/Test </returns>
		public static Uri CreateAbsolute(string relative) {

			return new Uri(new Uri(AppConfig.HostAddress), relative);

		}

		private static string MergeUrls_BaseNoTrailingSlash(string baseUrl, string relative) {
			
			if (relative.StartsWith("/"))
				return string.Format("{0}{1}", baseUrl, relative);
			else
				return string.Format("{0}/{1}", baseUrl, relative);

		}

		public static string MergeUrls(string baseUrl, string relative) {

			if (baseUrl.EndsWith("/")) {

				if (relative.StartsWith("/"))
					return string.Format("{0}{1}", baseUrl.Substring(0, baseUrl.Length - 1), relative);
				else
					return string.Format("{0}{1}", baseUrl, relative);

			} else {
				return MergeUrls_BaseNoTrailingSlash(baseUrl, relative);
			}

		}

		public static string RemoveTrailingSlash(string url) {

			if (string.IsNullOrEmpty(url))
				return url;

			return url.EndsWith("/") ? url.Substring(0, AppConfig.StaticContentHost.Length - 1) : url;

		}

		/// <summary>
		/// Returns a path to a resource in the static VocaDB domain (static.vocadb.net).
		/// </summary>
		/// <param name="relative">Relative URL, for example /banners/rvocaloid.png</param>
		/// <returns>
		/// Full path to that static resource, for example http://static.vocadb.net/banners/rvocaloid.png
		/// </returns>
		public static string StaticResource(string relative) {
			return MergeUrls_BaseNoTrailingSlash(staticResourceBase, relative);
		}

	}
}
