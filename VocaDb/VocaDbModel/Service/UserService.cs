﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using VocaDb.Model.Service.Paging;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.Service {

	public class UserService : ServiceBase {

// ReSharper disable UnusedMember.Local
		private static readonly ILog log = LogManager.GetLogger(typeof(UserService));
// ReSharper restore UnusedMember.Local

		private IQueryable<User> AddOrder(IQueryable<User> criteria, UserSortRule sortRule) {

			switch (sortRule) {
				case UserSortRule.Name:
					return criteria.OrderBy(u => u.Name);
				case UserSortRule.RegisterDate:
					return criteria.OrderBy(u => u.CreateDate);
				case UserSortRule.Group:
					return criteria
						.OrderBy(u => u.GroupId)
						.ThenBy(u => u.Name);
			}

			return criteria;

		}

		private UserDetailsContract GetUserDetails(ISession session, User user) {

			var details = new UserDetailsContract(user, PermissionContext);

			details.AlbumCollectionCount
				= session.Query<AlbumForUser>().Count(c => c.User == user && !c.Album.Deleted);

			details.ArtistCount
				= session.Query<ArtistForUser>().Count(c => c.User == user && !c.Artist.Deleted);

			details.FavoriteSongCount
				= session.Query<FavoriteSongForUser>().Count(c => c.User == user && !c.Song.Deleted);

			details.CommentCount
				= session.Query<AlbumComment>().Count(c => c.Author == user && !c.Album.Deleted)
				+ session.Query<ArtistComment>().Count(c => c.Author == user && !c.Artist.Deleted);

			details.EditCount
				= session.Query<ArchivedAlbumVersion>().Count(c => c.Author == user && !c.Album.Deleted)
				+ session.Query<ArchivedArtistVersion>().Count(c => c.Author == user && !c.Artist.Deleted)
				+ session.Query<ArchivedSongVersion>().Count(c => c.Author == user && !c.Song.Deleted);

			details.SubmitCount
				= session.Query<ArchivedAlbumVersion>().Count(c => c.Author == user && c.Version == 0 && !c.Album.Deleted)
				+ session.Query<ArchivedArtistVersion>().Count(c => c.Author == user && c.Version == 0 && !c.Artist.Deleted)
				+ session.Query<ArchivedSongVersion>().Count(c => c.Author == user && c.Version == 0 && !c.Song.Deleted);

			details.TagVotes
				= session.Query<TagVote>().Count(t => t.User == user);

			return details;

		}

		public UserService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {

		}

		// For quick-adding the album on user page
		public AlbumForUserContract AddAlbum(int userId, int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return HandleTransaction(session => {

				var user = session.Load<User>(userId);
				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("adding {0} for {1}", album, user), session);

				var albumForUser = user.AddAlbum(album, PurchaseStatus.Owned, MediaType.PhysicalDisc, AlbumForUser.NotRated);
				session.Save(albumForUser);

				return new AlbumForUserContract(albumForUser, PermissionContext.LanguagePreference);

			});

		}

		public void AddArtist(int userId, int artistId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var exists = session.Query<ArtistForUser>().Any(u => u.User.Id == userId && u.Artist.Id == artistId);

				if (exists)
					return;

				var user = session.Load<User>(userId);
				var artist = session.Load<Artist>(artistId);

				user.AddArtist(artist);

				session.Update(user);

				AuditLog(string.Format("added {0} for {1}", artist, user), session, user);

			});

		}

		public void AddSongToFavorites(int userId, int songId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var user = session.Load<User>(userId);
				var song = session.Load<Song>(songId);

				var link = user.AddSongToFavorites(song);

				session.Save(link);
				session.Update(song);

				AuditLog(string.Format("added {0} to favorites", EntryLinkFactory.CreateEntryLink(song)), 
					session, user);

			});

		}

		public UserContract CheckAccessWithKey(string name, string accessKey) {

			return HandleQuery(session => {

				var lc = name.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.Name == lc);

				if (user == null)
					return null;

				var hashed = LoginManager.GetHashedAccessKey(user.AccessKey);

				if (accessKey != hashed)
					return null;

				return new UserContract(user);

			});

		}

		public UserContract CheckAuthentication(string name, string pass, string hostname) {

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.Name == lc);

				if (user == null) {
					AuditLog(string.Format("failed login from {0}.", hostname), session, name);
					Thread.Sleep(2000);
					return null;
				}

				var hashed = LoginManager.GetHashedPass(lc, pass, user.Salt);

				if (user.Password != hashed) {
					AuditLog(string.Format("failed login from {0}.", hostname), session, name);
					Thread.Sleep(2000);
					return null;
				}

				AuditLog(string.Format("logged in from {0}.", hostname), session, user);

				user.UpdateLastLogin();
				session.Update(user);

				return new UserContract(user);

			});

		}

		public bool CheckPasswordResetRequest(Guid requestId) {

			return HandleQuery(session => session.Query<PasswordResetRequest>().Any(r => r.Id == requestId));

		}

		public UserContract Create(string name, string pass, string email, string hostname) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();
				var existing = session.Query<User>().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					return null;

				var salt = new Random().Next();
				var hashed = LoginManager.GetHashedPass(lc, pass, salt);
				var user = new User(name, hashed, email, salt);
				session.Save(user);

				AuditLog("registered from " + hostname, session, user);

				return new UserContract(user);

			});

		}

		public void DeleteAlbumForUser(int albumForUserId) {

			DeleteEntity<AlbumForUser>(albumForUserId, PermissionToken.EditProfile);

		}

		public void DisableUser(int userId) {

			UpdateEntity<User>(userId, user => {

				if (!user.CanBeDisabled)
					throw new InvalidOperationException("This user account cannot be disabled");

				user.Active = false;

			}, PermissionToken.DisableUsers);

		}

		public UserContract[] FindUsersByName(string term) {

			return HandleQuery(session => {

				var users = session.Query<User>().Where(u => u.Name.Contains(term)).OrderBy(u => u.Name).Take(10).ToArray();

				return users.Select(u => new UserContract(u)).ToArray();

			});

		}

		public PartialFindResult<AlbumForUserContract> GetAlbumCollection(int userId, PurchaseStatus status, PagingProperties paging) {

			return HandleQuery(session => {

				var albums = session.Query<AlbumForUser>()
					.Where(a => a.User.Id == userId && !a.Album.Deleted && status == PurchaseStatus.Nothing || (a.PurchaseStatus == status))
					.AddNameOrder(LanguagePreference)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(a => new AlbumForUserContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = paging.GetTotalCount ? session.Query<AlbumForUser>()
					.Count(a => a.User.Id == userId && !a.Album.Deleted && status == PurchaseStatus.Nothing || (a.PurchaseStatus == status)) : 0;

				return new PartialFindResult<AlbumForUserContract>(albums, count);

			});

		}

		public ArtistWithAdditionalNamesContract[] GetArtists(int userId) {

			return HandleQuery(session =>
				session.Load<User>(userId)
					.Artists
					.Select(a => new ArtistWithAdditionalNamesContract(a.Artist, PermissionContext.LanguagePreference))
					.OrderBy(s => s.Name)
					.ToArray());

		}

		public CommentContract[] GetComments(int userId) {

			return HandleQuery(session => {

				var user = session.Load<User>(userId);

				var comments = session.Query<AlbumComment>()
					.Where(c => c.Author == user && !c.Album.Deleted).OrderByDescending(c => c.Created).ToArray().Cast<Comment>()
					.Concat(session.Query<ArtistComment>()
						.Where(c => c.Author == user && !c.Artist.Deleted)).OrderByDescending(c => c.Created).ToArray();

				return comments.Select(c => new CommentContract(c)).ToArray();

			});

		}

		[Obsolete]
		public SongWithAdditionalNamesContract[] GetFavoriteSongs(int userId) {

			return HandleQuery(session =>
				session.Load<User>(userId)
					.FavoriteSongs
					.Select(a => new SongWithAdditionalNamesContract(a.Song, PermissionContext.LanguagePreference))
					.OrderBy(s => s.Name)
					.ToArray());

		}

		public PartialFindResult<SongWithAdditionalNamesContract> GetFavoriteSongs(int userId, int start, int maxItems) {

			return HandleQuery(session => {

				var q = session.Query<FavoriteSongForUser>().Where(a => !a.Song.Deleted && a.User.Id == userId);

				var resultQ = FindHelpers.AddNameOrder(q.Select(a => a.Song), PermissionContext.LanguagePreference);
				resultQ = resultQ.Skip(start).Take(maxItems);

				var contracts = resultQ.ToArray().Select(a => new SongWithAdditionalNamesContract(a, PermissionContext.LanguagePreference)).ToArray();
				var totalCount = q.Count();

				return new PartialFindResult<SongWithAdditionalNamesContract>(contracts, totalCount);

			});
		}

		public UserMessageContract GetMessageDetails(int messageId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return HandleTransaction(session => {

				var msg = session.Load<UserMessage>(messageId);

				VerifyResourceAccess(msg.Sender, msg.Receiver);

				if (!msg.Read && PermissionContext.LoggedUser.Id == msg.Receiver.Id) {
					msg.Read = true;
					session.Update(msg);
				}

				return new UserMessageContract(msg);

			});

		}

		public PartialFindResult<UserContract> GetUsers(UserGroupId groupId, UserSortRule sortRule, PagingProperties paging) {

			return HandleQuery(session => {

				var users = AddOrder(session.Query<User>()
					.Where(u => (groupId == UserGroupId.Nothing || u.GroupId == groupId)), sortRule)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(u => new UserContract(u))
					.ToArray();

				var count = paging.GetTotalCount ? session.Query<User>()
					.Count(u => groupId == UserGroupId.Nothing || u.GroupId == groupId) : 0;

				return new PartialFindResult<UserContract>(users, count);

			});

		}

		public UserContract GetUser(int id) {

			return HandleQuery(session => new UserContract(session.Load<User>(id)));

		}

		public UserDetailsContract GetUserDetails(int id) {

			return HandleQuery(session => GetUserDetails(session, session.Load<User>(id)));

		}

		public UserForMySettingsContract GetUserForMySettings(int id) {

			return HandleQuery(session => new UserForMySettingsContract(session.Load<User>(id), PermissionContext.LanguagePreference));

		}

		public UserContract GetUserByName(string name, bool skipMessages) {

			return HandleQuery(session => {

				var user = session.Query<User>().First(u => u.Name.Equals(name));
				var contract = new UserContract(user);

				contract.HasUnreadMessages = (!skipMessages && session.Query<UserMessage>().Any(m => !m.Read && m.Receiver.Id == user.Id));

				return contract;

			});

		}

		public UserDetailsContract GetUserByNameNonSensitive(string name) {

			return HandleQuery(session => 
				GetUserDetails(session, session.Query<User>().ToArray()
				.First(u => u.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))));

		}

		private IQueryable<T> AddFilter<T>(IQueryable<T> query, int userId, int maxCount, bool onlySubmissions) where T : ArchivedObjectVersion {

			query = query.Where(q => q.Author.Id == userId);

			if (onlySubmissions)
				query = query.Where(q => q.Version == 0);

			query = query.OrderByDescending(q => q.Created);

			query = query.Take(maxCount);

			return query;

		}

		public UserWithActivityEntriesContract GetUserWithActivityEntries(int id, int maxCount, bool onlySubmissions) {

			return HandleQuery(session => {

				var user = session.Load<User>(id);
				var activity = 
					AddFilter(session.Query<ArchivedAlbumVersion>(), id, maxCount, onlySubmissions).ToArray().Cast<ArchivedObjectVersion>().Concat(
					AddFilter(session.Query<ArchivedArtistVersion>(), id, maxCount, onlySubmissions).ToArray()).Concat(
					AddFilter(session.Query<ArchivedSongVersion>(), id, maxCount, onlySubmissions).ToArray());

				var activityContracts = activity
					.Where(a => !a.EntryBase.Deleted)
					.OrderByDescending(a => a.Created).Take(maxCount)
					.Select(a => new ActivityEntryContract(a, PermissionContext.LanguagePreference)).ToArray();

				return new UserWithActivityEntriesContract(user, activityContracts, PermissionContext.LanguagePreference);

			});

		}

		public UserWithMessagesContract GetUserWithMessages(int id) {

			return HandleQuery(session => new UserWithMessagesContract(session.Load<User>(id)));

		}

		public void RemoveAlbumFromUser(int userId, int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var link = session.Query<AlbumForUser>().FirstOrDefault(a => a.Album.Id == albumId && a.User.Id == userId);

				AuditLog("deleting " + link, session);

				if (link != null)
					session.Delete(link);

			});

		}

		public void RemoveArtistFromUser(int userId, int artistId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var link = session.Query<ArtistForUser>().FirstOrDefault(a => a.Artist.Id == artistId && a.User.Id == userId);

				AuditLog("removing " + link, session);

				if (link != null)
					session.Delete(link);

			});

		}

		public void RemoveSongFromFavorites(int userId, int songId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var link = session.Query<FavoriteSongForUser>().FirstOrDefault(a => a.Song.Id == songId && a.User.Id == userId);

				AuditLog("deleting " + link, session);

				if (link != null) {

					link.Delete();
					session.Delete(link);
					session.Update(link.Song);

				}

			});

		}

		public void RequestPasswordReset(string username, string email, string resetUrl) {

			ParamIs.NotNullOrEmpty(() => username);
			ParamIs.NotNullOrEmpty(() => email);

			var lc = username.ToLowerInvariant();

			HandleTransaction(session => {

				var user = session.Query<User>().Where(u => u.NameLC.Equals(lc) && email.Equals(u.Email)).FirstOrDefault();

				if (user == null)
					throw new UserNotFoundException();

				var request = new PasswordResetRequest(user);
				session.Save(request);

				var mailer = new PasswordResetRequestMailer();
				mailer.Send(resetUrl, request);

			});

		}

		public UserContract ResetPassword(Guid requestId, string password) {

			ParamIs.NotNullOrEmpty(() => password);

			return HandleTransaction(session => {

				var request = session.Load<PasswordResetRequest>(requestId);
				var user = request.User;

				AuditLog("resetting password", session, user);

				var newHashed = LoginManager.GetHashedPass(user.NameLC, password, user.Salt);
				user.Password = newHashed;

				session.Update(user);

				session.Delete(request);

				return new UserContract(user);

			});

		}

		public void SendMessage(UserMessageContract contract, string messagesUrl) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var receiver = session.Query<User>().FirstOrDefault(u => u.Name.Equals(contract.Receiver.Name));

				if (receiver == null)
					throw new UserNotFoundException();

				var sender = session.Load<User>(contract.Sender.Id);

				VerifyResourceAccess(sender);

				AuditLog("sending message from " + sender + " to " + receiver);

				var message = sender.SendMessage(receiver, contract.Subject, contract.Body, contract.HighPriority);

				if (receiver.EmailOptions == UserEmailOptions.PrivateMessagesFromAll 
					|| (receiver.EmailOptions == UserEmailOptions.PrivateMessagesFromAdmins 
						&& sender.EffectivePermissions.Has(PermissionToken.DesignatedStaff))) {

					var mailer = new UserMessageMailer();
					mailer.Send(messagesUrl, message);

				}

				session.Save(message);

			});

		}

		public void UpdateAlbumForUser(int userId, int albumId, PurchaseStatus status, 
			MediaType mediaType, int rating) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var albumForUser = session.Query<AlbumForUser>()
					.FirstOrDefault(a => a.Album.Id == albumId && a.User.Id == userId);

				// Delete
				if (albumForUser != null && status == PurchaseStatus.Nothing && rating == 0) {

					AuditLog(string.Format("deleting {0} for {1}", 
						CreateEntryLink(albumForUser.Album), albumForUser.User), session);

					albumForUser.Delete();
					session.Delete(albumForUser);
					session.Update(albumForUser.Album);

				// Add
				} else if (albumForUser == null && (status != PurchaseStatus.Nothing || rating != 0)) {

					var user = session.Load<User>(userId);
					var album = session.Load<Album>(albumId);

					albumForUser = user.AddAlbum(album, status, mediaType, rating);
					session.Save(albumForUser);
					session.Update(album);

					AuditLog(string.Format("added {0} for {1}", CreateEntryLink(album), user), session);

				// Update
				} else if (albumForUser != null) {

					albumForUser.MediaType = mediaType;
					albumForUser.PurchaseStatus = status;
					session.Update(albumForUser);

					if (albumForUser.Rating != rating) {
						albumForUser.Rating = rating;
						albumForUser.Album.UpdateRatingTotals();
						session.Update(albumForUser.Album);
					}

					AuditLog(string.Format("updated {0} for {1}", 
						CreateEntryLink(albumForUser.Album), albumForUser.User), session);

				}

			});

		}

		public void UpdateAlbumForUserMediaType(int albumForUserId, MediaType mediaType) {

			UpdateEntity<AlbumForUser>(albumForUserId, albumForUser => albumForUser.MediaType = mediaType, PermissionToken.EditProfile);

		}

		public void UpdateAlbumForUserRating(int albumForUserId, int rating) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var albumForUser = session.Load<AlbumForUser>(albumForUserId);

				albumForUser.Rating = rating;
				albumForUser.Album.UpdateRatingTotals();
				session.Update(albumForUser.Album);

			});

		}

		public void UpdateUser(UserContract contract) {

			ParamIs.NotNull(() => contract);

			UpdateEntity<User>(contract.Id, (session, user) => {

				user.Active = contract.Active;
				user.AdditionalPermissions = new PermissionCollection(contract.AdditionalPermissions.Select(p => PermissionToken.GetById(p.Id)));
				user.GroupId = contract.GroupId;

				AuditLog(string.Format("updated {0}", EntryLinkFactory.CreateEntryLink(user)), session);

			}, PermissionToken.ManageUserPermissions, skipLog: true);

		}

		public UserContract UpdateUserSettings(UpdateUserSettingsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return HandleTransaction(session => {

				var user = session.Load<User>(contract.Id);

				AuditLog("Updating settings for " + user);

				VerifyResourceAccess(user);

				if (!string.IsNullOrEmpty(contract.NewPass)) {

					var oldHashed = LoginManager.GetHashedPass(user.NameLC, contract.OldPass, user.Salt);

					if (user.Password != oldHashed)
						throw new InvalidPasswordException();

					var newHashed = LoginManager.GetHashedPass(user.NameLC, contract.NewPass, user.Salt);
					user.Password = newHashed;

				}

				user.AnonymousActivity = contract.AnonymousActivity;
				user.DefaultLanguageSelection = contract.DefaultLanguageSelection;
				user.EmailOptions = contract.EmailOptions;
				user.Language = contract.Language;
				user.PreferredVideoService = contract.PreferredVideoService;
				user.SetEmail(contract.Email);
				session.Update(user);

				AuditLog(string.Format("updated settings for {0}", EntryLinkFactory.CreateEntryLink(user)), session);

				return new UserContract(user);

			});

		}
	}

	public class InvalidPasswordException : Exception {
		
		public InvalidPasswordException()
			: base("Invalid password") {}

		protected InvalidPasswordException(SerializationInfo info, StreamingContext context) 
			: base(info, context) {}

	}

	public class UserNotFoundException : Exception {

		public UserNotFoundException()
			: base("User not found") {}

		protected UserNotFoundException(SerializationInfo info, StreamingContext context) 
			: base(info, context) {}

	}

	public enum UserSortRule {

		RegisterDate,

		Name,

		Group

	}

}
