using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumContract {

		public SongInAlbumContract() {}

		public SongInAlbumContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => songInAlbum);

			DiscNumber = songInAlbum.DiscNumber;
			Id = songInAlbum.Id;
			Song = new SongWithAdditionalNamesContract(songInAlbum.Song, languagePreference);
			TrackNumber = songInAlbum.TrackNumber;

		}

		[DataMember]
		public int DiscNumber { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract Song { get; set; }

		[DataMember]
		public int TrackNumber { get; set; }

	}

}
