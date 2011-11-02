using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.MikuDb {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ImportedAlbumTrack {

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public int TrackNum { get; set; }

	}

}
