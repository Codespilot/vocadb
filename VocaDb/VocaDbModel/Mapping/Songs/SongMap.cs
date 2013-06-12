﻿using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongMap : ClassMap<Song> {

		public SongMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.FavoritedTimes).Not.Nullable();
			Map(m => m.NicoId).Nullable();
			Map(m => m.Notes).Length(800).Not.Nullable();
			Map(m => m.PVServices).CustomType(typeof(PVServices)).Not.Nullable();
			Map(m => m.RatingScore).Not.Nullable();
			Map(m => m.SongType).Not.Nullable();
			Map(m => m.Status).CustomType(typeof(EntryStatus)).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.OriginalVersion).Nullable();

			Component(m => m.ArchivedVersionsManager,
				c => c.HasMany(m => m.Versions).KeyColumn("[Song]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Names, c => {
				c.Map(m => m.AdditionalNamesString).Not.Nullable().Length(1024);
				c.HasMany(m => m.Names).Table("SongNames").KeyColumn("[Song]").Inverse().Cascade.All().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 => {
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
				});
			});

			Component(m => m.PVs, c => {
				c.HasMany(m => m.PVs).KeyColumn("[Song]").Inverse().Cascade.All().Cache.ReadWrite();
			});

			Component(m => m.Tags, c => {
				c.HasMany(m => m.Usages).KeyColumn("[Song]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			Component(m => m.ArtistString, c => {
				c.Map(m => m.Japanese, "ArtistString").Length(500).Not.Nullable();
				c.Map(m => m.Romaji, "ArtistStringRomaji").Length(500).Not.Nullable();
				c.Map(m => m.English, "ArtistStringEnglish").Length(500).Not.Nullable();
				c.Map(m => m.Default, "ArtistStringDefault").Length(500).Not.Nullable();
			});

			HasMany(m => m.AllAlbums).Table("SongsInAlbums").Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.AllAlternateVersions).KeyColumn("[OriginalVersion]").Inverse();
			HasMany(m => m.AllArtists).Table("ArtistsForSongs").Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.Comments).Inverse().Cascade.AllDeleteOrphan().OrderBy("Created");
			HasMany(m => m.ListLinks).Inverse();
			HasMany(m => m.Lyrics).Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.UserFavorites).Inverse();
			HasMany(m => m.WebLinks).Table("SongWebLinks").Inverse().Cascade.All().Cache.ReadWrite();

		}

	}

	public class ArchivedSongVersionMap : ClassMap<ArchivedSongVersion> {

		public ArchivedSongVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Length(100).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Reason).Length(30).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author);
			References(m => m.Song);

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
				c.Map(m => m.IsSnapshot).Not.Nullable();
			});

		}

	}

	public class SongNameMap : ClassMap<SongName> {

		public SongNameMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Length(255).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class ArtistForSongMap : ClassMap<ArtistForSong> {

		public ArtistForSongMap() {

			Schema("dbo");
			Table("ArtistsForSongs");
			Cache.ReadWrite();

			Id(m => m.Id);
			Map(m => m.Name).Nullable();
			Map(m => m.IsSupport).Not.Nullable();
			Map(m => m.Roles).CustomType(typeof(ArtistRoles)).Not.Nullable();
			References(m => m.Artist).Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongInAlbumMap : ClassMap<SongInAlbum> {

		public SongInAlbumMap() {

			Schema("dbo");
			Table("SongsInAlbums");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.DiscNumber).Not.Nullable();
			Map(m => m.TrackNumber).Not.Nullable();
			References(m => m.Album).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongWebLinkMap : ClassMap<SongWebLink> {

		public SongWebLinkMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Category).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			Map(m => m.Url).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class PVForSongMap : ClassMap<PVForSong> {
		
		public PVForSongMap() {
			
			Table("PVsForSongs");
			Id(m => m.Id);

			Map(m => m.Author).Length(50).Not.Nullable();
			Map(m => m.Length).Not.Nullable();
			Map(m => m.Name).Length(200).Not.Nullable();
			Map(m => m.PVId).Length(255).Not.Nullable();
			Map(m => m.PVType).Not.Nullable();
			Map(m => m.Service).Not.Nullable();
			Map(m => m.ThumbUrl).Length(255).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

}
