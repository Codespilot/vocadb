using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumNameMap : ClassMap<AlbumName> {

		public AlbumNameMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Not.Nullable();

			References(m => m.Album).Not.Nullable();

		}

	}

}
