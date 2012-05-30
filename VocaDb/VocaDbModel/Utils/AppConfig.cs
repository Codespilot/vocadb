using System.Configuration;

namespace VocaDb.Model.Utils {

	public static class AppConfig {

		public static string HostAddress {
			get {
				return ConfigurationManager.AppSettings["HostAddress"];
			}
		}

	}
}
