using System;
using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserDetailsContract : UserContract {

		public UserDetailsContract() {}

		public UserDetailsContract(User user, IUserPermissionContext permissionContext) 
			: base(user) {

			//AlbumLinks = user.Albums.Select(a => new AlbumForUserContract(a, languagePreference)).OrderBy(a => a.Album.Name).ToArray();
			//FavoriteSongs = user.FavoriteSongs.Select(f => new FavoriteSongForUserContract(f, languagePreference)).OrderBy(s => s.Song.Name).ToArray();
			LastLogin = user.LastLogin;
			SongLists = user.SongLists.Select(l => new SongListContract(l, permissionContext)).ToArray();

		}

		public int AlbumCollectionCount { get; set; }

		public int ArtistCount { get; set; }

		public int CommentCount { get; set; }

		public int EditCount { get; set; }

		public int FavoriteSongCount { get; set; }

		public DateTime LastLogin { get; set; }

		public SongListContract[] SongLists { get; set; }

		public int SubmitCount { get; set; }

		public int TagVotes { get; set; }

	}
}
