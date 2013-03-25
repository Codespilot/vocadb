using VocaDb.Model.Utils;

namespace VocaDb.Web.Models.Ext {

	public class SongOEmbedResponse {

		public int height {
			get { return 400; }
		}

		public string html { get; set; }

		public string provider_name {
			get { return "VocaDB"; }
		}

		public string provider_url {
			get { return AppConfig.HostAddress; }
		}

		public string type {
			get { return "video"; }
		}

		public string version {
			get { return "1.0"; }
		}

		public int width {
			get { return 570; }
		}

	}

}