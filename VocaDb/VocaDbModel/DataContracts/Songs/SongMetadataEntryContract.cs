using System.Runtime.Serialization;
using VocaVoter.Model.Domain.Songs;

namespace VocaVoter.Model.DataContracts.Songs {

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
