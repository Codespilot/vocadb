using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongMap : ClassMap<Song> {

		public SongMap() {

			Id(m => m.Id);

			Map(m => m.ArtistString).Not.Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.NicoId);
			Map(m => m.OriginalName).Not.Nullable();

			HasMany(m => m.Metadata).Inverse().Cascade.AllDeleteOrphan();
			Component(m => m.TranslatedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
			});

			HasMany(m => m.Albums).Table("SongsInAlbums").Inverse().Cascade.All();
			HasMany(m => m.Artists).Table("ArtistsForSongs").Inverse().Cascade.All();
			HasMany(m => m.Lyrics).Inverse().Cascade.All();

		}

	}

	public class SongNameMap : ClassMap<SongName> {

		public SongNameMap() {

			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class ArtistForSongMap : ClassMap<ArtistForSong> {

		public ArtistForSongMap() {

			Schema("dbo");
			Table("ArtistsForSongs");
			Id(m => m.Id);
			References(m => m.Artist).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongInAlbumMap : ClassMap<SongInAlbum> {

		public SongInAlbumMap() {

			Schema("dbo");
			Table("SongsInAlbums");
			Id(m => m.Id);
			Map(m => m.TrackNumber).Not.Nullable();
			References(m => m.Album).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

}
