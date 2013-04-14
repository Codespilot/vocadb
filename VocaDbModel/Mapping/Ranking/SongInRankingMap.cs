using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Ranking;

namespace VocaDb.Model.Mapping.Ranking {

	public class SongInRankingMap : ClassMap<SongInRanking> {

		public SongInRankingMap() {
			
			Table("SongsInRankings");
			Id(m => m.Id);
			Map(m => m.SortIndex).Not.Nullable();
			References(m => m.Ranking);
			References(m => m.Song);

		}

	}

}
