using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongWithPVPlayerAndVoteContract : SongContract {

		[DataMember]
		public string PlayerHtml { get; set; }

		[DataMember]
		public SongWithPVAndVoteContract Song { get; set; }

	}

}
