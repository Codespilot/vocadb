using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistMergeRecordMap : ClassMap<ArtistMergeRecord> {

		public ArtistMergeRecordMap() {

			Id(m => m.Id);

			References(m => m.Source).Not.Nullable();
			References(m => m.Target).Not.Nullable();

		}

	}
}
