using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	/// <summary>
	/// Configuration section for affiliate links.
	/// </summary>
	public class AffiliatesSection : ConfigurationSection {

		[ConfigurationProperty("playAsiaAffiliateId", DefaultValue = "")]
		public string PlayAsiaAffiliateId {
			get { return (string)this["playAsiaAffiliateId"]; }
			set { this["playAsiaAffiliateId"] = value; }
		}

	}

}
