using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumTagUsageMap : ClassMap<AlbumTagUsage> {

		public AlbumTagUsageMap() {

			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			References(m => m.Album).Not.Nullable();
			HasMany(m => m.Votes).Inverse().Cascade.All();

		}

	}

}
