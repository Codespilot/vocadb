using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users {

	public class UserMap : ClassMap<User> {

		public UserMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Active).Not.Nullable();
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.DefaultLanguageSelection).Not.Nullable();
			Map(m => m.Email).Length(50).Not.Nullable();
			Map(m => m.EmailOptions).CustomType(typeof(UserEmailOptions)).Not.Nullable();
			Map(m => m.GroupId).Column("[UserGroup]").Not.Nullable();
			Map(m => m.LastLogin).Not.Nullable();
			Map(m => m.Name).Length(100).Not.Nullable();
			Map(m => m.NameLC).Length(100).Not.Nullable();
			Map(m => m.Password).Not.Nullable();
			Map(m => m.AdditionalPermissions).Column("[PermissionFlags]").CustomType(typeof(PermissionFlags)).Not.Nullable();
			Map(m => m.PreferredVideoService).Not.Nullable();
			Map(m => m.Salt).Not.Nullable();

			HasMany(m => m.AllAlbums).Inverse().Cascade.All();
			HasMany(m => m.FavoriteSongs).Inverse().Cascade.All();
			HasMany(m => m.ReceivedMessages).KeyColumn("[Receiver]").OrderBy("Created DESC").Inverse().Cascade.All();
			HasMany(m => m.SentMessages).KeyColumn("[Sender]").OrderBy("Created DESC").Inverse().Cascade.All();

		}

	}

	public class AlbumForUserMap : ClassMap<AlbumForUser> {

		public AlbumForUserMap() {

			Table("AlbumsForUsers");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.MediaType).Not.Nullable();

			References(m => m.Album).Not.Nullable();
			References(m => m.User).Not.Nullable();

		}

	}

	public class FavoriteSongForUserMap : ClassMap<FavoriteSongForUser> {
		
		public FavoriteSongForUserMap() {
			
			Table("FavoriteSongsForUsers");
			Cache.ReadWrite();
			Id(m => m.Id);

			References(m => m.Song).Not.Nullable();
			References(m => m.User).Not.Nullable();

		}

	}

}
