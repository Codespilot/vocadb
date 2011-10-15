using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.MikuDb;

namespace VocaDb.Model.Mapping.MikuDb {

	public class MikuDbAlbumMap :ClassMap<MikuDbAlbum> {

		public MikuDbAlbumMap() {

			Schema("mikudb");
			Table("ImportedAlbums");

			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.SourceUrl).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Title).Not.Nullable();

		}

	}

}
