using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserForMySettingsContract : UserContract {

		public UserForMySettingsContract() { }

		public UserForMySettingsContract(User user, ContentLanguagePreference languagePreference)
			: base(user) {

			AlbumLinks = user.Albums.Select(a => new AlbumForUserContract(a, languagePreference)).ToArray();

		}

		public AlbumForUserContract[] AlbumLinks { get; set; }

	}
}
