using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInAlbumContract {

		public SongInAlbumContract() {}

		public SongInAlbumContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => songInAlbum);

			Id = songInAlbum.Id;
			Song = new SongContract(songInAlbum.Song, languagePreference);
			TrackNumber = songInAlbum.TrackNumber;

		}

		public int Id { get; set; }

		public SongContract Song { get; private set; }

		public int TrackNumber { get; private set; }

	}

}
