using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Helpers {

	public static class VocaUrlHelper {

		// Path to static files root, for example http://static.vocadb.net. Possible trailing slash is removed.
		private static readonly string staticResourceBase = RemoveTrailingSlash(AppConfig.StaticContentHost);

		private static string MergeUrls_BaseNoTrailingSlash(string baseUrl, string relative) {
			
			if (relative.StartsWith("/"))
				return string.Format("{0}{1}", baseUrl, relative);
			else
				return string.Format("{0}/{1}", baseUrl, relative);

		}

		/// <summary>
		/// Gets an URL-friendly name from any entry name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// English or Romanized name is preferred.
		/// </summary>
		public static string GetUrlFriendlyName(ITranslatedString names) {

			string raw;

			if (names.DefaultLanguage == ContentLanguageSelection.Romaji && !string.IsNullOrEmpty(names.Romaji))
				raw = names.Romaji;
			else
				raw = names.English;

			return GetUrlFriendlyName(raw);

		}

		/// <summary>
		/// Gets an URL-friendly name from any entry name.
		/// The processed name can be used as user-friendly part of an URL.
		/// 
		/// - Spaces are replaced with dashes '-'.
		/// - All non-ASCII characters except digits, underscore '_' and dash '-' are removed.
		/// - Name is truncated to 30 characters.
		/// - Name is lowercased.
		/// </summary>
		/// <param name="name">Entry name, for example "Hatsune Miku". Can be null or empty.</param>
		/// <returns>Processed name, for example "hatsune-miku". Can be empty. Cannot be null.</returns>
		public static string GetUrlFriendlyName(string name) {

			if (string.IsNullOrEmpty(name))
				return string.Empty;

			var cleanedName = Regex.Replace(name.Replace(' ', '-'), @"[^a-zA-Z0-9_-]", string.Empty);
			return cleanedName.Truncate(30).ToLowerInvariant();

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