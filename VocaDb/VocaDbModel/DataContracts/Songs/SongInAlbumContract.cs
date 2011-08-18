using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongInAlbumContract {

		public SongInAlbumContract(SongInAlbum songInAlbum) {
			
			ParamIs.NotNull(() => songInAlbum);

			Song = new SongContract(songInAlbum.Song);
			TrackNumber = songInAlbum.TrackNumber;

		}

		public SongContract Song { get; private set; }

		public int TrackNumber { get; private set; }

	}

}
