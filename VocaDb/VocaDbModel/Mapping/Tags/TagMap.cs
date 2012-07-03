using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Mapping.Tags {

	public class TagMap : ClassMap<Tag> {

		public TagMap() {

			Cache.NonStrictReadWrite();
			Id(m => m.Name).Column("[Name]").Length(30).GeneratedBy.Assigned();

			Map(m => m.CategoryName).Length(30).Not.Nullable();
			Map(m => m.Description).Length(400).Not.Nullable();
			Map(m => m.TagName).Column("[Name]").ReadOnly().Not.Insert();

			HasMany(m => m.AllAlbumTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllArtistTagUsages).Cascade.AllDeleteOrphan().Inverse();
			HasMany(m => m.AllSongTagUsages).Cascade.AllDeleteOrphan().Inverse();

		}

	}
}
