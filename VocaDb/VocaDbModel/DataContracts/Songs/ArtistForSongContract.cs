using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class ArtistForSongContract {

		public ArtistForSongContract() { }

		public ArtistForSongContract(ArtistForSong artistForSong, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => artistForSong);

			Artist = new ArtistWithAdditionalNamesContract(artistForSong.Artist, languagePreference);
			Categories = artistForSong.ArtistCategories;
			Id = artistForSong.Id;
			IsSupport = artistForSong.IsSupport;
			Roles = artistForSong.Roles;

		}

		public ArtistForSongContract(ArtistWithAdditionalNamesContract artistContract) {

			ParamIs.NotNull(() => artistContract);

			Artist = artistContract;

		}


		[DataMember]
		public ArtistWithAdditionalNamesContract Artist { get; set; }

		[DataMember]
		public ArtistCategories Categories { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public ArtistRoles Roles { get; set; }

	}

}
