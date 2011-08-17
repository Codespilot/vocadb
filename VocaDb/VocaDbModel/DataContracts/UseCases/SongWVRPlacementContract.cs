using VocaDb.Model.Domain.Ranking;

namespace VocaDb.Model.DataContracts.UseCases {

	public class SongWVRPlacementContract {

		public SongWVRPlacementContract(SongInRanking songInRanking) {
			PollId = songInRanking.Ranking.Id;
			PollName = songInRanking.Ranking.Name;
			SortIndex = songInRanking.SortIndex;
		}

		public int PollId { get; private set; }

		public string PollName { get; private set; }

		public int SortIndex { get; private set; }

	}

}
