using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumContract {

		public SongInAlbumContract() {}

		public SongInAlbumContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference) {
			
			ParamIs.NotNull(() => songInAlbum);

			Id = songInAlbum.Id;
			Song = new SongWithAdditionalNamesContract(songInAlbum.Song, languagePreference);
			TrackNumber = songInAlbum.TrackNumber;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public SongWithAdditionalNamesContract Song { get; private set; }

		[DataMember]
		public int TrackNumber { get; private set; }

	}

}
