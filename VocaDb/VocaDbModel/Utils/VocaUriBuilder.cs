using System;

namespace VocaDb.Model.Utils {

	public static class VocaUriBuilder {

		public static Uri CreateAbsolute(string relative) {

			return new Uri(new Uri(AppConfig.HostAddress), relative);

		}

	}
}
