using System;
using System.Linq;
using System.Net.Mail;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Users {

	public class User : IEntryBase, IEquatable<User> {

		private string accessKey;
		private IList<AlbumForUser> albums = new List<AlbumForUser>();
		private string email;
		private IList<FavoriteSongForUser> favoriteSongs = new List<FavoriteSongForUser>();
		private string name;
		private string nameLc;
		private string password;
		private IList<UserMessage> receivedMessages = new List<UserMessage>();
		private IList<UserMessage> sentMessages = new List<UserMessage>();
		private IList<SongList> songLists = new List<SongList>();

		public User() {

			Active = true;
			CreateDate = DateTime.Now;
			DefaultLanguageSelection = ContentLanguagePreference.Default;
			Email = string.Empty;
			EmailOptions = UserEmailOptions.PrivateMessagesFromAll;
			LastLogin = DateTime.Now;
			PreferredVideoService = PVService.Youtube;
			GroupId = UserGroupId.Regular;

		}

		public User(string name, string pass, int salt)
			: this() {

			Name = name;
			NameLC = name.ToLowerInvariant();
			Password = pass;
			Salt = salt;

			AccessKey = new AlphaPassGenerator(true, true, true).Generate(20);

		}

		public virtual string AccessKey {
			get { return accessKey; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				accessKey = value; 
			}
		}

		public virtual bool Active { get; set; }

		/// <summary>
		/// Additional user permissions. Base permissions are assigned through the user group.
		/// </summary>
		public virtual PermissionFlags AdditionalPermissions { get; set; }

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

				return !EffectivePermissions.HasFlag(PermissionFlags.ManageUserBlocks);

			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual ContentLanguagePreference DefaultLanguageSelection { get; set; }

		public virtual string DefaultName {
			get { return Name; }
		}

		public virtual PermissionFlags EffectivePermissions {
			get {

				if (!Active)
					return PermissionFlags.Nothing;

				return UserGroup.GetPermissions(GroupId) | AdditionalPermissions;

			}
		}

		public virtual string Email {
			get { return email; }
			set {
				ParamIs.NotNull(() => value);
				email = value;
			}
		}

		public virtual UserEmailOptions EmailOptions { get; set; }

		public virtual EntryType EntryType {
			get { return EntryType.User; }
		}

		public virtual IList<FavoriteSongForUser> FavoriteSongs {
			get { return favoriteSongs; }
			set {
				ParamIs.NotNull(() => value);
				favoriteSongs = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual UserGroupId GroupId { get; set; }

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

		public virtual PVService PreferredVideoService { get; set; }

		public virtual IList<UserMessage> ReceivedMessages {
			get { return receivedMessages; }
			set {
				ParamIs.NotNull(() => value);
				receivedMessages = value;
			}
		}

		public virtual RoleTypes Roles { get; set; }

		public virtual int Salt { get; set; }

		public virtual IList<UserMessage> SentMessages {
			get { return sentMessages; }
			set {
				ParamIs.NotNull(() => value);
				sentMessages = value;
			}
		}

		public virtual IList<SongList> SongLists {
			get { return songLists; }
			set {
				ParamIs.NotNull(() => value);
				songLists = value; 
			}
		}

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
			return NameLC.GetHashCode();
		}

		public virtual UserMessage SendMessage(User to, string subject, string body, bool highPriority) {

			ParamIs.NotNull(() => to);

			var msg = new UserMessage(this, to, subject, body, highPriority);
			SentMessages.Add(msg);
			to.ReceivedMessages.Add(msg);

			return msg;

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
