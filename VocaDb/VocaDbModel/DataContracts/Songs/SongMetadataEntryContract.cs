using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract]
	public class SongMetadataEntryContract {

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public SongMetadataType MetadataType { get; set; }

		[DataMember]
		public string Value { get; set; }

	}

}
