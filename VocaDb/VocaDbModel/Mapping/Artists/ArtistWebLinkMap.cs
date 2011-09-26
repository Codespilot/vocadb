using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistWebLinkMap : ClassMap<ArtistWebLink> {

		public ArtistWebLinkMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Not.Nullable();
			Map(m => m.Url).Not.Nullable();

			References(m => m.Artist).Not.Nullable();

		}

	}
}
