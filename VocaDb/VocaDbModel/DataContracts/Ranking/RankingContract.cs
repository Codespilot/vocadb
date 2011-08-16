using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Ranking;
using VocaVoter.Model;

namespace VocaDb.Model.DataContracts.Ranking {

	[DataContract]
	public class RankingContract {

		public RankingContract() {
			Description = string.Empty;
			NicoId = string.Empty;
		}

		public RankingContract(RankingList poll) {

			ParamIs.NotNull(() => poll);

			CreateDate = poll.CreateDate;
			Description = poll.Description;
			Id = poll.Id;
			Name = poll.Name;
			NicoId = poll.NicoId;
			Songs = poll.Songs.Select(m => new SongInRankingContract(m)).ToArray();

		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public SongInRankingContract[] Songs { get; set; }

		[DataMember]
		public int WVRId { get; set; }

	}

}
