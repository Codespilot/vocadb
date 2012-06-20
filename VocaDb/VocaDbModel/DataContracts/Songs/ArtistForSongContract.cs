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

			Artist = (artistForSong.Artist != null ? new ArtistWithAdditionalNamesContract(artistForSong.Artist, languagePreference) : null);
			Categories = artistForSong.ArtistCategories;
			Id = artistForSong.Id;
			IsSupport = artistForSong.IsSupport;
			Name = artistForSong.Name;
			Roles = artistForSong.Roles;

		}

		public ArtistForSongContract(ArtistWithAdditionalNamesContract artistContract) {

			ParamIs.NotNull(() => artistContract);

			Artist = artistContract;

		}

		public ArtistForSongContract(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			Name = name;

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
		public string Name { get; set; }

		[DataMember]
		public ArtistRoles Roles { get; set; }

	}

}
