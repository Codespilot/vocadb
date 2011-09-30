using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForAlbumContract {

		public ArtistForAlbumContract(ArtistForAlbum artistForAlbum, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => artistForAlbum);

			Album = new AlbumContract(artistForAlbum.Album, languagePreference);
			Artist = new ArtistContract(artistForAlbum.Artist, languagePreference);
			Id = artistForAlbum.Id;

		}

		[DataMember]
		public AlbumContract Album { get; set; }

		[DataMember]
		public ArtistContract Artist { get; set; }

		[DataMember]
		public int Id { get; set; }

	}

}
