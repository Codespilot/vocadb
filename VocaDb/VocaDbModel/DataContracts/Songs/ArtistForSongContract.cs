using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	public class ArtistForSongContract {

		public ArtistForSongContract(ArtistForSong artistForSong) {
			
			ParamIs.NotNull(() => artistForSong);

			Artist = new ArtistContract(artistForSong.Artist);
			Id = artistForSong.Id;

		}

		public ArtistContract Artist { get; set; }

		public int Id { get; set; }

	}

}
