﻿using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumMap : ClassMap<Album> {

		public AlbumMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Barcode).Length(30).Nullable();
			Map(m => m.CoverPictureMime).Length(32).Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.Description).Not.Nullable();
			Map(m => m.DiscType).Column("[Type]").Not.Nullable();
			Map(m => m.RatingAverageInt).Column("[RatingAverage]").Not.Nullable();
			Map(m => m.RatingCount).Not.Nullable();
			Map(m => m.RatingTotal).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			Component(m => m.ArchivedVersionsManager, 
				c => c.HasMany(m => m.Versions).KeyColumn("[Album]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.ArtistString, c => {
				c.Map(m => m.Japanese, "ArtistString").Length(500).Not.Nullable();
				c.Map(m => m.Romaji, "ArtistStringRomaji").Length(500).Not.Nullable();
				c.Map(m => m.English, "ArtistStringEnglish").Length(500).Not.Nullable();
				c.Map(m => m.Default, "ArtistStringDefault").Length(500).Not.Nullable();
			});

			Component(m => m.CoverPictureData, c => {
				c.Map(m => m.Bytes, "CoverPictureBytes").Length(int.MaxValue);
				c.Component(m => m.Thumb250, c2 => c2.Map(m => m.Bytes, "CoverPictureThumb250Bytes").Length(int.MaxValue));
			}).LazyLoad();

			Component(m => m.OriginalRelease, c => {
				c.Map(m => m.CatNum, "ReleaseCatNum");
				c.Map(m => m.EventName, "[ReleaseEventName]");
				c.Component(m => m.ReleaseDate, c2 => {
					c2.Map(m => m.Year, "ReleaseYear");
					c2.Map(m => m.Month, "ReleaseMonth");
					c2.Map(m => m.Day, "ReleaseDay");
				});
			});

			Component(m => m.Names, c => {
				c.Map(m => m.AdditionalNamesString).Not.Nullable().Length(1024);
				c.HasMany(m => m.Names).Table("AlbumNames").KeyColumn("[Album]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 => {
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
					//c.Map(m => m.Other, "OtherName");
				});
			});

			Component(m => m.Pictures, c => {
				c.HasMany(m => m.Pictures).KeyColumn("[Album]").Inverse().Cascade.All().Cache.ReadWrite();
			});

			Component(m => m.Tags, c => {
				c.HasMany(m => m.Usages).Table("AlbumTagUsages").KeyColumn("[Album]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			HasMany(m => m.AllArtists).Table("ArtistsForAlbums").Inverse().Cascade.All().Cache.ReadWrite();
			HasMany(m => m.AllSongs).Inverse().Cascade.All().OrderBy("DiscNumber, TrackNumber").Cache.ReadWrite();
			HasMany(m => m.Comments).Inverse().Cascade.AllDeleteOrphan().OrderBy("Created");
			HasMany(m => m.Hits).Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.OtherArtists).Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			HasMany(m => m.PVs).Inverse().Cascade.AllDeleteOrphan();
			HasMany(m => m.UserCollections).Inverse().Cache.ReadWrite();
			HasMany(m => m.WebLinks).Table("AlbumWebLinks").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();

		}

	}

	public class ArtistForAlbumMap : ClassMap<ArtistForAlbum> {

		public ArtistForAlbumMap() {

			Schema("dbo");
			Table("ArtistsForAlbums");
			Cache.ReadWrite();

			Id(m => m.Id);

			Map(m => m.IsSupport).Not.Nullable();
			Map(m => m.Name).Nullable();
			Map(m => m.Roles).CustomType(typeof(ArtistRoles)).Not.Nullable();
			References(m => m.Album).Not.Nullable();
			References(m => m.Artist).Nullable();

		}

	}

	public class ArchivedAlbumVersionMap : ClassMap<ArchivedAlbumVersion> {

		public ArchivedAlbumVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.CoverPictureMime).Length(32).Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Reason).Length(30).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Album);
			References(m => m.Author);

			Component(m => m.CoverPicture, c => {
				c.Map(m => m.Bytes, "CoverPictureBytes").Length(int.MaxValue).LazyLoad();
			});

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
				c.Map(m => m.IsSnapshot).Not.Nullable();
			});

		}

	}

}
