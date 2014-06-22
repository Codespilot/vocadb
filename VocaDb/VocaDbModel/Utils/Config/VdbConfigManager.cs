﻿using System.Web.Configuration;

namespace VocaDb.Model.Utils.Config {

	/// <summary>
	/// Manages VocaDb global configuration.
	/// </summary>
	public class VdbConfigManager {

		public AffiliatesSection Affiliates {
			get {
				var section = (AffiliatesSection)WebConfigurationManager.GetSection("vocaDb/affiliates");
				return section ?? new AffiliatesSection();
			}
		}

		public SiteSettingsSection SiteSettings {
			get {
				var section = (SiteSettingsSection)WebConfigurationManager.GetSection("vocaDb/siteSettings");
				return section ?? new SiteSettingsSection();
			}
		}

	}

}
