using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagUsageMap : ClassMap<TagUsage> {

		public TagUsageMap() {

			Id(m => m.Id);

			Map(m => m.Count).Not.Nullable();
			References(m => m.Tag).Not.Nullable();

		}

	}

	public class AlbumTagUsageMap : SubclassMap<AlbumTagUsage> {

		public AlbumTagUsageMap() {

			Table("AlbumTagUsages");

			References(m => m.Album).Not.Nullable();

			HasMany(m => m.Votes).Inverse().Cascade.All();

		}

	}

}
