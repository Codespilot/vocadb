using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.MikuDb {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ImportedAlbumTrack {

		public ImportedAlbumTrack() {
			DiscNum = 1;
		}

		[DataMember]
		public int DiscNum { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public int TrackNum { get; set; }

	}

}
