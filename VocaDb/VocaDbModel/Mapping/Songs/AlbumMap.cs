using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class AlbumMap : ClassMap<Album> {

		public AlbumMap() {

			Id(m => m.Id);
			Map(m => m.ReleaseDate).Nullable();
			Component(m => m.TranslatedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
			});

			HasMany(m => m.Artists).Table("ArtistsForAlbums").Inverse().Cascade.All();
			HasMany(m => m.Songs).Inverse().Cascade.All().OrderBy("TrackNumber");

		}

	}

	public class ArtistForAlbumMap : ClassMap<ArtistForAlbum> {

		public ArtistForAlbumMap() {

			Schema("dbo");
			Table("ArtistsForAlbums");
			Id(m => m.Id);
			References(m => m.Album).Not.Nullable();
			References(m => m.Artist).Not.Nullable();

		}

	}


}
