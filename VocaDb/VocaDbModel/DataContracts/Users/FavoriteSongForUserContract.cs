using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class FavoriteSongForUserContract {

		public FavoriteSongForUserContract(FavoriteSongForUser favoriteSongForUser, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => favoriteSongForUser);

			Id = favoriteSongForUser.Id;
			Rating = favoriteSongForUser.Rating;
			Song = new SongContract(favoriteSongForUser.Song, languagePreference);

		}

		public int Id { get; set; }

		public SongVoteRating Rating { get; set; }

		public SongContract Song { get; set; }

	}

}
