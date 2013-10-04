using System.Configuration;

namespace VocaDb.Model.Utils {

	public static class AppConfig {

		private static string Val(string key) {
			return ConfigurationManager.AppSettings[key];
		}

		public static string BilibiliAppKey {
			get { return Val("BilibiliAppKey"); }
		}

		public static string DbDumpFolder {
			get {
				return Val("DbDumpFolder");
			}
		}

		/// <summary>
		/// Host address contains full path to the web application's root, including hostname.
		/// For example http://vocadb.net
		/// </summary>
		public static string HostAddress {
			get {
				return Val("HostAddress");
			}
		}

		public static string LockdownMessage {
			get {
				return Val("LockdownMessage");
			}
		}

		public static string ReCAPTCHAKey {
			get {
				return Val("ReCAPTCHAKey");
			}
		}

		public static string ReCAPTCHAPublicKey {
			get {
				return Val("ReCAPTCHAPublicKey");
			}
		}

		public static string TwitterConsumerKey {
			get {
				return Val("TwitterConsumerKey");
			}
		}

		public static string TwitterConsumerSecret {
			get {
				return Val("TwitterConsumerSecret");
			}
		}

	}
}
