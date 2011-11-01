using System;
using System.Linq;
using System.Net.Mail;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Domain.Users {

	public class User {

		private IList<AlbumForUser> albums = new List<AlbumForUser>();
		private string email;
		private IList<FavoriteSongForUser> favoriteSongs = new List<FavoriteSongForUser>();
		private string name;
		private string nameLc;
		private string password;

		public User() {

			Active = true;
			CreateDate = DateTime.Now;
			DefaultLanguageSelection = ContentLanguagePreference.Default;
			Email = string.Empty;
			LastLogin = DateTime.Now;
			PermissionFlags = PermissionFlags.Default;
			PreferredVideoService = PVService.Youtube;

		}

		public User(string name, string pass, int salt)
			: this() {

			Name = name;
			NameLC = name.ToLowerInvariant();
			Password = pass;
			Salt = salt;

		}

		public virtual bool Active { get; set; }

		public virtual IEnumerable<AlbumForUser> Albums {
			get {
				return AllAlbums.Where(a => !a.Album.Deleted);
			}
		}

		public virtual IList<AlbumForUser> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		public virtual bool CanBeDisabled {
			get {

				return (!PermissionFlags.HasFlag(PermissionFlags.Admin) 
					&& !PermissionFlags.HasFlag(PermissionFlags.ManageUserBlocks)
					&& !PermissionFlags.HasFlag(PermissionFlags.ManageUsers));

			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual ContentLanguagePreference DefaultLanguageSelection { get; set; }

		public virtual string Email {
			get { return email; }
			set {
				ParamIs.NotNull(() => value);
				email = value;
			}
		}

		public virtual IList<FavoriteSongForUser> FavoriteSongs {
			get { return favoriteSongs; }
			set {
				ParamIs.NotNull(() => value);
				favoriteSongs = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual DateTime LastLogin { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value;
			}
		}

		public virtual string NameLC {
			get { return nameLc; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				nameLc = value;
			}
		}

		public virtual string Password {
			get { return password; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				password = value;
			}
		}

		public virtual PermissionFlags PermissionFlags { get; set; }

		public virtual PVService PreferredVideoService { get; set; }

		public virtual RoleTypes Roles { get; set; }

		public virtual int Salt { get; set; }

		public virtual AlbumForUser AddAlbum(Album album) {

			ParamIs.NotNull(() => album);

			var link = new AlbumForUser(this, album);
			AllAlbums.Add(link);

			return link;

		}

		public virtual FavoriteSongForUser AddSongToFavorites(Song song) {
			
			ParamIs.NotNull(() => song);

			var link = new FavoriteSongForUser(this, song);
			FavoriteSongs.Add(link);

			return link;

		}

		public virtual bool Equals(User another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.NameLC == another.NameLC;

		}

		public override bool Equals(object obj) {
			return Equals(obj as User);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void SetEmail(string newEmail) {
			
			ParamIs.NotNull(() => newEmail);

			if (newEmail != string.Empty)
				new MailAddress(newEmail);

			Email = newEmail;

		}

		public override string ToString() {
			return string.Format("user '{0}' [{1}]", Name, Id);
		}

		public virtual void UpdateLastLogin() {
			LastLogin = DateTime.Now;
		}

	}

}
