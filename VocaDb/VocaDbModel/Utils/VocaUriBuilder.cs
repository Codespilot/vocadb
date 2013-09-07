using System;

namespace VocaDb.Model.Utils {

	public static class VocaUriBuilder {

		/// <summary>
		/// Creates a full, absolute uri which includes the domain and scheme.
		/// </summary>
		/// <param name="relative">Relative address, for example /User/Profile/Test</param>
		/// <returns>Absolute address, for example http://vocadb.net/User/Profile/Test </returns>
		public static Uri CreateAbsolute(string relative) {

			return new Uri(new Uri(AppConfig.HostAddress), relative);

		}

	}
}
