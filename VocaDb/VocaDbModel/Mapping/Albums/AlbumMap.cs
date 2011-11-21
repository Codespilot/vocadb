using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumMap : ClassMap<Album> {

		public AlbumMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			Map(m => m.DiscType).Column("[Type]").Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			Component(m => m.ArtistString, c => {
				c.Map(m => m.Japanese, "ArtistString").Length(500).Not.Nullable();
				c.Map(m => m.Romaji, "ArtistStringRomaji").Length(500).Not.Nullable();
				c.Map(m => m.English, "ArtistStringEnglish").Length(500).Not.Nullable();
			});

			Component(m => m.CoverPicture, c => {
				c.Map(m => m.Bytes, "CoverPictureBytes").Length(int.MaxValue).LazyLoad();
				c.Map(m => m.Mime, "CoverPictureMime");
				c.Component(m => m.Thumb250, c2 => c2.Map(m => m.Bytes, "CoverPictureThumb250Bytes").Length(int.MaxValue).LazyLoad());
			});

			Component(m => m.OriginalRelease, c => {
				c.Map(m => m.CatNum, "ReleaseCatNum");
				c.Map(m => m.EventName, "ReleaseEventName");
				c.Component(m => m.ReleaseDate, c2 => {
					c2.Map(m => m.Year, "ReleaseYear");
					c2.Map(m => m.Month, "ReleaseMonth");
					c2.Map(m => m.Day, "ReleaseDay");
				});
			});

			Component(m => m.Names, c => {
				c.HasMany(m => m.Names).Table("AlbumNames").KeyColumn("[Album]").Inverse().Cascade.All().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 => {
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
					//c.Map(m => m.Other, "OtherName");
				});
			});

			Component(m => m.Tags, c => {
				c.HasMany(m => m.Usages).Table("AlbumTagUsages").KeyColumn("[Album]").Inverse().Cascade.AllDeleteOrphan();
			});

			HasMany(m => m.AllArtists).Table("ArtistsForAlbums").Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.AllSongs).Inverse().Cascade.All().OrderBy("TrackNumber").Cache.ReadWrite();
			HasMany(m => m.ArchivedVersions).Inverse().Cascade.All().OrderBy("Created DESC");
			HasMany(m => m.Comments).Inverse().Cascade.AllDeleteOrphan().OrderBy("Created");
			HasMany(m => m.PVs).Inverse().Cascade.All();
			HasMany(m => m.UserCollections).Inverse();
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
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Reason).Length(30).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Album);
			References(m => m.Author);

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
				c.Map(m => m.IsSnapshot).Not.Nullable();
			});

		}

	}

}
