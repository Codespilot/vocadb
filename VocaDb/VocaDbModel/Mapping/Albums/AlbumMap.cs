using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumMap : ClassMap<Album> {

		public AlbumMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.ArtistString).Not.Nullable().Length(500);
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.DiscType).Column("[Type]").Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			Component(m => m.CoverPicture, c => {
				c.Map(m => m.Bytes, "CoverPictureBytes").Length(int.MaxValue);
				c.Map(m => m.Mime, "CoverPictureMime");
			}).LazyLoad();

			Component(m => m.OriginalRelease, c => {
				c.Map(m => m.CatNum, "ReleaseCatNum");
				c.Map(m => m.EventName, "ReleaseEventName");
				//c.References(m => m.Label, "ReleaseLabel");
				c.Component(m => m.ReleaseDate, c2 => {
					c2.Map(m => m.Year, "ReleaseYear");
					c2.Map(m => m.Month, "ReleaseMonth");
					c2.Map(m => m.Day, "ReleaseDay");
				});
			});

			Component(m => m.TranslatedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
			});

			HasMany(m => m.AllArtists).Table("ArtistsForAlbums").Inverse().Cascade.All();
			HasMany(m => m.AllSongs).Inverse().Cascade.All().OrderBy("TrackNumber");
			HasMany(m => m.Names).Table("AlbumNames").Inverse().Cascade.All();
			HasMany(m => m.WebLinks).Table("AlbumWebLinks").Inverse().Cascade.All();

		}

	}

	public class ArtistForAlbumMap : ClassMap<ArtistForAlbum> {

		public ArtistForAlbumMap() {

			Schema("dbo");
			Table("ArtistsForAlbums");
			Cache.ReadWrite();
			Id(m => m.Id);

			References(m => m.Album).Not.Nullable();
			References(m => m.Artist).Not.Nullable();

		}

	}

	public class ArchivedAlbumVersionMap : ClassMap<ArchivedAlbumVersion> {

		public ArchivedAlbumVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Album);
			References(m => m.Author);

		}

	}

}
