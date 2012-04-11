using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.DataContracts.Users {

	public class UserForMySettingsContract : UserContract {

		public UserForMySettingsContract() { }

		public UserForMySettingsContract(User user, ContentLanguagePreference languagePreference)
			: base(user) {

			AlbumLinks = user.Albums.Select(a => new AlbumForUserContract(a, languagePreference)).ToArray();
			HashedAccessKey = LoginManager.GetHashedAccessKey(user.AccessKey);

		}

		public string HashedAccessKey { get; set; }

		public AlbumForUserContract[] AlbumLinks { get; set; }

	}
}
