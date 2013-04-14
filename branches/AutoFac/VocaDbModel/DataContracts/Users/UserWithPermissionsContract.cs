using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// User with additional permission flags and artist ownership permissions.
	/// Used for user details page as well as checking for permissions of the logged in user.
	/// </summary>
	public class UserWithPermissionsContract : UserContract {

		public UserWithPermissionsContract() { }

		public UserWithPermissionsContract(User user, ContentLanguagePreference languagePreference)
			: base(user) {

			OwnedArtistEntries = user.OwnedArtists.Select(a => new ArtistForUserContract(a, languagePreference)).ToArray();

		}

		/// <summary>
		/// List of artist entries owned by the user. Cannot be null.
		/// </summary>
		public ArtistForUserContract[] OwnedArtistEntries { get; set; }

	}

}
