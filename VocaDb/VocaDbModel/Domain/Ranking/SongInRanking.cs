using VocaDb.Model.Domain.Songs;
using VocaVoter.Model;

namespace VocaDb.Model.Domain.Ranking {

	public class SongInRanking {

		public SongInRanking() {}

		public SongInRanking(RankingList poll, Song song, int sortIndex) {

			ParamIs.NotNull(() => poll);
			ParamIs.NotNull(() => song);

			List = poll;
			Song = song;
			SortIndex = sortIndex;
		}

		public virtual int Id { get; set; }

		public virtual RankingList List { get; set; }

		public virtual Song Song { get; set; }

		public virtual int SortIndex { get; set; }

	}

}
