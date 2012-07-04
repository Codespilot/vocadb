using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumPictureFileMap : ClassMap<AlbumPictureFile> {

		public AlbumPictureFileMap() {

			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Mime).Not.Nullable();
			Map(m => m.Name).Not.Nullable();

			References(m => m.Album).Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
