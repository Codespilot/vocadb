using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.User {

	public class FavoriteSongs {

		public FavoriteSongs() {
			Sort = SongSortRule.Name;
		}

		public FavoriteSongs(UserContract user, SongSortRule sort)
			: this() {

			Sort = sort;
			User = user;

		}

		public SongSortRule Sort { get; set; }

		public UserContract User { get; set; }

	}

}