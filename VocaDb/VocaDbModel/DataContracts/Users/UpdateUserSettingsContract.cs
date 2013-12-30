﻿using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UpdateUserSettingsContract : UserContract {

		public UpdateUserSettingsContract() {}

		public UpdateUserSettingsContract(User user)
			: base(user) {

			AboutMe = user.Options.AboutMe;
			Location = user.Options.Location;
			PublicAlbumCollection = user.Options.PublicAlbumCollection;
			PublicRatings = user.Options.PublicRatings;
			WebLinks = user.WebLinks.Select(w => new WebLinkContract(w)).ToArray();

		}

		public string AboutMe { get; set; }

		public string Location { get; set; }

		public string NewPass { get; set; }

		public string OldPass { get; set; }

		public bool PublicAlbumCollection { get; set; }

		public bool PublicRatings { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
