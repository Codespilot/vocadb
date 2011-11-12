using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class PVForAlbumMap : ClassMap<PVForAlbum> {

		public PVForAlbumMap() {

			Table("PVsForAlbums");
			Cache.ReadWrite();
			Id(m => m.Id);

			//Map(m => m.Name).Not.Nullable();
			Map(m => m.PVId).Not.Nullable();
			Map(m => m.PVType).Not.Nullable();
			Map(m => m.Service).Not.Nullable();

			References(m => m.Album).Not.Nullable();

		}

	}

}
