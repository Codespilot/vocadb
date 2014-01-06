using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumContract {

		public SongInAlbumContract() {}

		public SongInAlbumContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference, bool getThumbUrl = true) {
			
			ParamIs.NotNull(() => songInAlbum);

			DiscNumber = songInAlbum.DiscNumber;
			Id = songInAlbum.Id;
			Song = new SongContract(songInAlbum.Song, languagePreference, getThumbUrl);
			TrackNumber = songInAlbum.TrackNumber;

		}

		[DataMember]
		public int DiscNumber { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public SongContract Song { get; set; }

		[DataMember]
		public int TrackNumber { get; set; }

		public override string ToString() {
			return string.Format("({0}.{1}) {2} in album", DiscNumber, TrackNumber, Song);
		}

	}

}
