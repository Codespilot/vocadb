using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	public class SiteSettingsSection : ConfigurationSection {

		public SiteSettingsSection() {
		}

		[ConfigurationProperty("blogUrl", DefaultValue = "blog.vocadb.net")]
		public string BlogUrl {
			get { return (string)this["blogUrl"]; }
			set { this["blogUrl"] = value; }
		}

		[ConfigurationProperty("ircUrl", DefaultValue = "irc.rizon.net/vocadb")]
		public string IRCUrl {
			get { return (string)this["ircUrl"]; }
			set { this["ircUrl"] = value; }			
		}

	}
}
