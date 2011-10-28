using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.DataContracts.UseCases {

	public class CreateSongContract {

		public ArtistContract[] Artists { get; set; }

		public string NameEnglish { get; set; }

		public string NameOriginal { get; set; }

		public string NameRomaji { get; set; }

		public string PVUrl { get; set; }
		
	}

}
