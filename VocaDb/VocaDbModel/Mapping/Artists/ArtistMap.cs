using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistMap : ClassMap<Artist> {

		public ArtistMap() {

			Cache.ReadWrite();

			Id(m => m.Id);
			Map(m => m.ArtistType).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			Map(m => m.StartDate).Nullable();
			Map(m => m.Version).Not.Nullable();

			HasMany(m => m.AllAlbums).Table("ArtistsForAlbums").Inverse().Cascade.All();
			HasMany(m => m.AllGroups).Inverse().KeyColumn("[Member]").Cascade.All();
			HasMany(m => m.AllSongs).Table("ArtistsForSongs").Inverse().Cascade.All();
			HasMany(m => m.ArchivedVersions).Inverse().Cascade.All();
			HasMany(m => m.AllMembers).Inverse().KeyColumn("[Group]");
			HasMany(m => m.Names).Table("ArtistNames").Inverse().Cascade.All();
			HasMany(m => m.WebLinks).Table("ArtistWebLinks").Inverse().Cascade.All();

			Component(m => m.TranslatedName, c => {
				c.Map(m => m.DefaultLanguage, "DefaultNameLanguage").Not.Nullable();
				c.Map(m => m.Japanese, "JapaneseName");
				c.Map(m => m.English, "EnglishName");
				c.Map(m => m.Romaji, "RomajiName");
				c.Map(m => m.Other, "OtherName");
			});

			Component(m => m.Picture, c => {
				c.Map(m => m.Bytes, "PictureBytes").Length(int.MaxValue).LazyLoad();
				c.Map(m => m.Mime, "PictureMime");
				c.Component(m => m.Thumb250, c2 => c2.Map(m => m.Bytes, "PictureThumb250Bytes").Length(int.MaxValue).LazyLoad());
			});

		}

	}

	public class ArchivedArtistVersionMap : ClassMap<ArchivedArtistVersion> {
		
		public ArchivedArtistVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Artist);
			References(m => m.Author);

		}

	}

	public class GroupForArtistMap : ClassMap<GroupForArtist> {
		
		public GroupForArtistMap() {
			
			Table("GroupsForArtists");
			Id(m => m.Id);

			References(m => m.Group).Not.Nullable();
			References(m => m.Member).Not.Nullable();

		}

	}

}
