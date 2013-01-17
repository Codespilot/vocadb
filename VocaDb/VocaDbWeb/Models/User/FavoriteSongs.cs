using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.User {

	public class FavoriteSongs {

		public FavoriteSongs() {
			Rating = SongVoteRating.Nothing;
			Sort = SongSortRule.Name;
		}

		public FavoriteSongs(UserContract user, SongVoteRating rating, SongSortRule sort)
			: this() {

			Rating = rating;
			Sort = sort;
			User = user;

		}

		public SongVoteRating Rating { get; set; }

		public SongSortRule Sort { get; set; }

		public UserContract User { get; set; }

	}

}