using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases {

	public class SongForEditContract : SongContract {

		public SongForEditContract(Song song) {
			
			ParamIs.NotNull(() => song);

			Artists = song.Artists.Select(a => new ArtistForSongContract(a)).ToArray();

		}

		public ArtistForSongContract[] Artists { get; set; }

	}

}
