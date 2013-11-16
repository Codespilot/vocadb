using System;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Helpers {

	public static class VocaUrlHelper {

		// TODO: should support HTTPS
		private static readonly Uri staticResourceBase = new Uri("http://static.vocadb.net", UriKind.Absolute);

		public static string GetUrlFriendlyName(TranslatedString names) {
			return GetUrlFriendlyName(names.GetBestMatch(ContentLanguagePreference.English));
		}

		public static string GetUrlFriendlyName(string name) {

			if (string.IsNullOrEmpty(name))
				return string.Empty;

			var cleanedName = Regex.Replace(name.Replace(' ', '-'), @"[^a-zA-Z_-]", string.Empty);
			return cleanedName.Truncate(30).ToLowerInvariant();

		}

		/// <summary>
		/// Returns a path to a resource in the static VocaDB domain (static.vocadb.net).
		/// </summary>
		/// <param name="relative">Relative URL, for example /banners/rvocaloid.png</param>
		/// <returns>
		/// Full path to that static resource, for example http://static.vocadb.net/banners/rvocaloid.png
		/// </returns>
		public static string StaticResource(string relative) {
			return (new Uri(staticResourceBase, relative)).ToString();
		}
	
	}

}