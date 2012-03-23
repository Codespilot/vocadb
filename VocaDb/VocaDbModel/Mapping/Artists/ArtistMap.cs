﻿using FluentNHibernate.Mapping;
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
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			HasMany(m => m.AllAlbums).Table("ArtistsForAlbums").Inverse().Cascade.All();
			HasMany(m => m.AllGroups).Inverse().KeyColumn("[Member]").Cascade.All().Cache.ReadWrite();
			HasMany(m => m.AllSongs).Table("ArtistsForSongs").Inverse().Cascade.All();
			HasMany(m => m.AllMembers).Inverse().KeyColumn("[Group]").Cache.ReadWrite();
			HasMany(m => m.Comments).Inverse().Cascade.AllDeleteOrphan().OrderBy("Created");
			HasMany(m => m.WebLinks).Table("ArtistWebLinks").Inverse().Cascade.All().Cache.ReadWrite();

			Component(m => m.ArchivedVersionsManager, 
				c => c.HasMany(m => m.Versions).KeyColumn("[Artist]").Inverse().Cascade.All().OrderBy("Created DESC"));

			Component(m => m.Names, c => {
				c.HasMany(m => m.Names).Table("ArtistNames").KeyColumn("[Artist]").Inverse().Cascade.All().Cache.ReadWrite();
				c.Component(m => m.SortNames, c2 => {
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
				});
			});

			Component(m => m.Picture, c => {
				c.Map(m => m.Bytes, "PictureBytes").Length(int.MaxValue).LazyLoad();
				c.Map(m => m.Mime, "PictureMime");
				c.Component(m => m.Thumb250, c2 => c2.Map(m => m.Bytes, "PictureThumb250Bytes").Length(int.MaxValue).LazyLoad());
			});

			Component(m => m.Tags, c => {
				c.HasMany(m => m.Usages).Table("ArtistTagUsages").KeyColumn("[Artist]").Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
			});

			HasMany(m => m.Users).Inverse();

		}

	}

	public class ArchivedArtistVersionMap : ClassMap<ArchivedArtistVersion> {
		
		public ArchivedArtistVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Reason).Length(30).Not.Nullable();
			Map(m => m.Status).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Artist);
			References(m => m.Author);

			Component(m => m.Diff, c => {
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
				c.Map(m => m.IsSnapshot).Not.Nullable();
			});

			Component(m => m.Picture, c => {
				c.Map(m => m.Bytes, "PictureBytes").Length(int.MaxValue).LazyLoad();
				c.Map(m => m.Mime, "PictureMime");
			});

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
