using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class ArtistForSongContract {

		public ArtistForSongContract() { }

		public ArtistForSongContract(ArtistForSong artistForSong, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => artistForSong);

			Artist = new ArtistWithAdditionalNamesContract(artistForSong.Artist, languagePreference);
			Id = artistForSong.Id;

		}

		public ArtistForSongContract(ArtistWithAdditionalNamesContract artistContract) {

			ParamIs.NotNull(() => artistContract);

			Artist = artistContract;

		}


		[DataMember]
		public ArtistWithAdditionalNamesContract Artist { get; set; }

		[DataMember]
		public int Id { get; set; }

	}

}
