using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagMap : ClassMap<Tag> {

		public TagMap() {

			Id(m => m.Name).Column("[Name]").GeneratedBy.Assigned();

			HasMany(m => m.AlbumTagUsages).Inverse();

		}

	}
}
