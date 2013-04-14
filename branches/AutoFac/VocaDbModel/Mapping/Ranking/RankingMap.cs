using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Ranking;

namespace VocaDb.Model.Mapping.Ranking {

	public class RankingMap : ClassMap<RankingList> {

		public RankingMap() {

			Table("Rankings");
			Id(m => m.Id);
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			Map(m => m.Name).Not.Nullable();
			Map(m => m.NicoId).Not.Nullable();
			Map(m => m.WVRId).Not.Nullable();

			HasMany(m => m.Songs).Inverse().Cascade.All().OrderBy("SortIndex");

		}

	}

}
