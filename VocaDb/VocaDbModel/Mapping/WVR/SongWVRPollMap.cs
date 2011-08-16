using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Ranking;

namespace VocaDb.Model.Mapping.WVR {

	public class SongWVRPollMap : ClassMap<SongInRanking> {

		public SongWVRPollMap() {
			
			Table("SongsInPoll");
			Id(m => m.Id);
			Map(m => m.SortIndex).Not.Nullable();
			References(m => m.List);
			References(m => m.Song);

		}

	}

}
