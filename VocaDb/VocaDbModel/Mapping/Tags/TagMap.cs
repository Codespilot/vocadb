using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagMap : ClassMap<Tag> {

		public TagMap() {

			Cache.NonStrictReadWrite();
			Id(m => m.Name).Column("[Name]").GeneratedBy.Assigned();

			HasMany(m => m.AllAlbumTagUsages).Inverse();

		}

	}
}
