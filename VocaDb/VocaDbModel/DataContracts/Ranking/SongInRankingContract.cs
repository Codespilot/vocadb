using System.Runtime.Serialization;
using VocaDb.Model.Domain.Ranking;

namespace VocaDb.Model.DataContracts.Ranking {

	[DataContract]
	public class SongInRankingContract {

		public SongInRankingContract() {}

		public SongInRankingContract(SongInRanking songInPoll) {

			Id = songInPoll.Id;
			Name = songInPoll.Song.DefaultName;
			NicoId = songInPoll.Song.NicoId;
			SongId = songInPoll.Song.Id;
			SortIndex = songInPoll.SortIndex;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public int SongId { get; set; }

		[DataMember]
		public int SortIndex { get; set; }

	}
}
