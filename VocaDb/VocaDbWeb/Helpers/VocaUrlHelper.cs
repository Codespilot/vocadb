using System;

namespace VocaDb.Web.Helpers {

	public static class VocaUrlHelper {

		// TODO: should support HTTPS
		private static readonly Uri staticResourceBase = new Uri("http://static.vocadb.net", UriKind.Absolute);

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