using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	public class SiteSettingsSection : ConfigurationSection {

		public SiteSettingsSection() {
			//BlogUrl = "http://blog.vocadb.net";
		}

		[ConfigurationProperty("blogUrl", DefaultValue = "http://blog.vocadb.net")]
		public string BlogUrl {
			get { return (string)this["blogUrl"]; }
			set { this["blogUrl"] = value; }
		}

	}
}
