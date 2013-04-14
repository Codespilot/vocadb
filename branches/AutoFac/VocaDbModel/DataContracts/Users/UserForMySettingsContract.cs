﻿using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.DataContracts.Users {

	public class UserForMySettingsContract : UserContract {

		public UserForMySettingsContract() {
			AboutMe = Location = string.Empty;
			WebLinks = new WebLinkContract[] {};
		}

		public UserForMySettingsContract(User user)
			: base(user) {

			AboutMe = user.Options.AboutMe;
			HashedAccessKey = LoginManager.GetHashedAccessKey(user.AccessKey);
			HasPassword = !string.IsNullOrEmpty(user.Password);
			HasTwitterToken = !string.IsNullOrEmpty(user.Options.TwitterOAuthToken);
			Location = user.Options.Location;
			PublicRatings = user.Options.PublicRatings;
			TwitterId = user.Options.TwitterId;
			TwitterName = user.Options.TwitterName;
			WebLinks = user.WebLinks.OrderBy(w => w.DescriptionOrUrl).Select(w => new WebLinkContract(w)).ToArray();

		}

		public string AboutMe { get; set; }

		public string HashedAccessKey { get; set; }

		public bool HasPassword { get; set; }

		public bool HasTwitterToken { get; set; }

		public string Location { get; set; }

		public bool PublicRatings { get; set; }

		public int TwitterId { get; set; }

		public string TwitterName { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}
}
