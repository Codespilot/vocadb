using System;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserDetailsContract : UserContract {

		public UserDetailsContract() {}

		public UserDetailsContract(User user, ContentLanguagePreference languagePreference) 
			: base(user) {

			AlbumLinks = user.Albums.Select(a => new AlbumForUserContract(a, languagePreference)).OrderBy(a => a.Album.Name).ToArray();
			FavoriteSongs = user.FavoriteSongs.Select(f => new FavoriteSongForUserContract(f, languagePreference)).ToArray();
			LastLogin = user.LastLogin;

		}

		public AlbumForUserContract[] AlbumLinks { get; set; }

		public FavoriteSongForUserContract[] FavoriteSongs { get; set; }

		public DateTime LastLogin { get; set; }

	}
}
