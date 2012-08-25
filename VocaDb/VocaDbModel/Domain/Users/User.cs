﻿using System;
using System.Linq;
using System.Net.Mail;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Users {

	public class User : IEntryBase, IEquatable<User> {

		private string accessKey;
		private PermissionCollection additionalPermissions = new PermissionCollection();
		private IList<AlbumForUser> albums = new List<AlbumForUser>();
		private IList<ArtistForUser> artists = new List<ArtistForUser>();
		private IList<UserComment> comments = new List<UserComment>();
		private string culture;
		private string email;
		private IList<FavoriteSongForUser> favoriteSongs = new List<FavoriteSongForUser>();
		private string language;
		private string name;
		private string nameLc;
		private UserOptions options;
		private string password;
		private IList<UserMessage> receivedMessages = new List<UserMessage>();
		private IList<UserMessage> sentMessages = new List<UserMessage>();
		private IList<SongList> songLists = new List<SongList>();

		public User() {

			Active = true;
			AnonymousActivity = false;
			CreateDate = DateTime.Now;
			Culture = string.Empty;
			DefaultLanguageSelection = ContentLanguagePreference.Default;
			Email = string.Empty;
			EmailOptions = UserEmailOptions.PrivateMessagesFromAll;
			GroupId = UserGroupId.Regular;
			Language = string.Empty;
			LastLogin = DateTime.Now;
			Options = new UserOptions(this);
			PreferredVideoService = PVService.Youtube;

		}

		public User(string name, string pass, string email, int salt)
			: this() {

			Name = name;
			NameLC = name.ToLowerInvariant();
			Password = pass;
			Email = email;
			Salt = salt;

			GenerateAccessKey();

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
		public virtual PermissionCollection AdditionalPermissions {
			get {
				return additionalPermissions;
			}
			set {
				additionalPermissions = value ?? new PermissionCollection();
			}
		}

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

		public virtual IEnumerable<ArtistForUser> Artists {
			get {
				return AllArtists.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual IList<ArtistForUser> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual bool AnonymousActivity { get; set; }

		public virtual bool CanBeDisabled {
			get {

				return !EffectivePermissions.Has(PermissionToken.DisableUsers);

			}
		}

		public virtual IList<UserComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual string Culture {
			get { return culture; }
			set { 
				ParamIs.NotNull(() => value);
				culture = value; 
			}
		}

		public virtual ContentLanguagePreference DefaultLanguageSelection { get; set; }

		public virtual string DefaultName {
			get { return Name; }
		}

		public virtual bool Deleted {
			get { return !Active; }
		}

		public virtual PermissionCollection EffectivePermissions {
			get {

				if (!Active)
					return new PermissionCollection();

				return UserGroup.GetPermissions(GroupId).Merge(AdditionalPermissions);

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

		public virtual string Language {
			get { return language; }
			set {
				ParamIs.NotNull(() => value);
				language = value;
			}
		}

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

		public virtual UserOptions Options {
			get { return options; }
			set {
				options = value ?? new UserOptions(this);
			}
		}

		public virtual string Password {
			get { return password; }
			set {
				ParamIs.NotNull(() => value);
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

		public virtual AlbumForUser AddAlbum(Album album, PurchaseStatus status, MediaType mediaType, int rating) {

			ParamIs.NotNull(() => album);

			var link = new AlbumForUser(this, album, status, mediaType, rating);
			AllAlbums.Add(link);
			album.UserCollections.Add(link);
			album.UpdateRatingTotals();

			return link;

		}

		public virtual ArtistForUser AddArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			var link = new ArtistForUser(this, artist);
			AllArtists.Add(link);

			return link;

		}

		public virtual FavoriteSongForUser AddSongToFavorites(Song song) {
			
			ParamIs.NotNull(() => song);

			var link = new FavoriteSongForUser(this, song);
			FavoriteSongs.Add(link);
			song.UserFavorites.Add(link);
			song.FavoritedTimes++;

			return link;

		}

		public virtual UserComment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new UserComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

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

		public virtual void GenerateAccessKey() {

			AccessKey = new AlphaPassGenerator(true, true, true).Generate(20);

		}

		public override int GetHashCode() {
			return NameLC.GetHashCode();
		}

		public virtual bool IsTheSameUser(UserContract contract) {

			if (contract == null)
				return false;

			return (Id == contract.Id);

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

		public virtual void UpdateLastLogin(string host) {
			LastLogin = DateTime.Now;
			Options.LastLoginAddress = host;
		}

	}

}
