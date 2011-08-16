using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistMetadataEntryMap : ClassMap<ArtistMetadataEntry> {

		public ArtistMetadataEntryMap() {

			Schema("dbo");
			Table("ArtistMetadata");
			Id(m => m.Id);
			Map(m => m.MetadataType).Column("[Type]").Not.Nullable();
			Map(m => m.Value).Not.Nullable();
			References(m => m.Artist).Not.Nullable();

		}

	}

}
