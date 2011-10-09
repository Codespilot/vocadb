using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class FavoriteSongForUserContract {

		public FavoriteSongForUserContract(FavoriteSongForUser favoriteSongForUser, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => favoriteSongForUser);

			Id = favoriteSongForUser.Id;
			Song = new SongWithAdditionalNamesContract(favoriteSongForUser.Song, languagePreference);

		}

		public int Id { get; set; }

		public SongWithAdditionalNamesContract Song { get; set; }

	}

}
