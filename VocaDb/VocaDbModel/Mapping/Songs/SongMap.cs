using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongMap : ClassMap<Song> {

		public SongMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.ArtistString).Not.Nullable().Length(500);
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.NicoId).Nullable();
			Map(m => m.OriginalName).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			//HasMany(m => m.Metadata).Inverse().Cascade.AllDeleteOrphan();
			Component(m => m.TranslatedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
			});

			HasMany(m => m.AllAlbums).Table("SongsInAlbums").Inverse().Cascade.All();
			HasMany(m => m.AllArtists).Table("ArtistsForSongs").Inverse().Cascade.All();
			HasMany(m => m.ArchivedVersions).Inverse().Cascade.All();
			HasMany(m => m.Names).Table("SongNames").Inverse().Cascade.All();
			HasMany(m => m.Lyrics).Inverse().Cascade.All();
			HasMany(m => m.PVs).Inverse().Cascade.All();
			HasMany(m => m.WebLinks).Table("SongWebLinks").Inverse().Cascade.All();

		}

	}

	public class ArchivedSongVersionMap : ClassMap<ArchivedSongVersion> {

		public ArchivedSongVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author);
			References(m => m.Song);

		}

	}

	public class SongNameMap : ClassMap<SongName> {

		public SongNameMap() {

			Cache.ReadWrite();
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
			Cache.ReadWrite();
			Id(m => m.Id);
			References(m => m.Artist).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongInAlbumMap : ClassMap<SongInAlbum> {

		public SongInAlbumMap() {

			Schema("dbo");
			Table("SongsInAlbums");
			Cache.ReadWrite();
			Id(m => m.Id);
			Map(m => m.TrackNumber).Not.Nullable();
			References(m => m.Album).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongWebLinkMap : ClassMap<SongWebLink> {

		public SongWebLinkMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Not.Nullable();
			Map(m => m.Url).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class PVForSongMap : ClassMap<PVForSong> {
		
		public PVForSongMap() {
			
			Table("PVsForSongs");
			Id(m => m.Id);

			Map(m => m.Notes).Not.Nullable();
			Map(m => m.PVId).Not.Nullable();
			Map(m => m.PVType).Not.Nullable();
			Map(m => m.Service).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

}
