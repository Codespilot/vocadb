using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumRefContract : ObjectRefContract {

		public SongInAlbumRefContract() {
			DiscNumber = 1;
		}

		public SongInAlbumRefContract(SongInAlbum songInAlbum)
			: base(songInAlbum.Song) {

			DiscNumber = songInAlbum.DiscNumber;
			TrackNumber = songInAlbum.TrackNumber;

		}

		[DataMember]
		public int DiscNumber { get; set; }

		[DataMember]
		public int TrackNumber { get; set; }

	}

}
