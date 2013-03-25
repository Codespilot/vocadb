using System.Xml.Serialization;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Models.Ext {

	[XmlRoot(ElementName = "oembed")]
	public class SongOEmbedResponse {

		public int height {
			get { return 400; }
			set { }
		}

		public string html { get; set; }

		public string provider_name {
			get { return "VocaDB"; }
			set { }
		}

		public string provider_url {
			get { return AppConfig.HostAddress; }
			set { }
		}

		public string type {
			get { return "video"; }
			set { }
		}

		public string version {
			get { return "1.0"; }
			set { }
		}

		public int width {
			get { return 570; }
			set { }
		}

	}

}