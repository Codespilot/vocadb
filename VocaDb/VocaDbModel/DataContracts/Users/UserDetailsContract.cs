using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserDetailsContract : UserContract {

		public UserDetailsContract() {}

		public UserDetailsContract(User user, ContentLanguagePreference languagePreference) 
			: base(user) {

			AlbumLinks = user.Albums.Select(a => new AlbumForUserContract(a, languagePreference)).OrderBy(a => a.Album.Name).ToArray();

		}

		public AlbumForUserContract[] AlbumLinks { get; set; }

	}
}
