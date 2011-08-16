using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Ranking;
using VocaVoter.Model.Domain;

namespace VocaVoter.Model.DataContracts.UseCases {

	public class SongWVRPlacementContract {

		public SongWVRPlacementContract(SongInRanking songInRanking) {
			PollId = songInRanking.List.Id;
			PollName = songInRanking.List.Name;
			SortIndex = songInRanking.SortIndex;
		}

		public int PollId { get; private set; }

		public string PollName { get; private set; }

		public int SortIndex { get; private set; }

	}

}
