using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.DataContracts.UseCases {

	public class CreateSongContract {

		public ArtistContract[] Artists { get; set; }

		public TranslatedStringContract Name { get; set; }

		public string PVUrl { get; set; }
		
	}

}
