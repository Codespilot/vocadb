﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Web;
using NLog;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Paging;
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
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.Service {

	public class UserService : ServiceBase {

// ReSharper disable UnusedMember.Local
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
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
				+ session.Query<ArtistComment>().Count(c => c.Author == user && !c.Artist.Deleted)
				+ session.Query<SongComment>().Count(c => c.Author == user && !c.Song.Deleted);

			details.EditCount
				= session.Query<ArchivedAlbumVersion>().Count(c => c.Author == user && !c.Album.Deleted)
				+ session.Query<ArchivedArtistVersion>().Count(c => c.Author == user && !c.Artist.Deleted)
				+ session.Query<ArchivedSongVersion>().Count(c => c.Author == user && !c.Song.Deleted);

			details.LatestComments = session.Query<UserComment>()
				.Where(c => c.User == user).OrderByDescending(c => c.Created).Take(3)
				.ToArray()
				.Select(c => new CommentContract(c)).ToArray();

			details.SubmitCount
				= session.Query<ArchivedAlbumVersion>().Count(c => c.Author == user && c.Version == 0 && !c.Album.Deleted)
				+ session.Query<ArchivedArtistVersion>().Count(c => c.Author == user && c.Version == 0 && !c.Artist.Deleted)
				+ session.Query<ArchivedSongVersion>().Count(c => c.Author == user && c.Version == 0 && !c.Song.Deleted);

			details.TagVotes
				= session.Query<TagVote>().Count(t => t.User == user);

			details.Power = UserHelper.GetPower(details, user);
			details.Level = UserHelper.GetLevel(details.Power);

			return details;

		}

		private string MakeGeoIpToolLink(string hostname) {

			return string.Format("<a href='http://www.geoiptool.com/?IP={0}'>{0}</a>", hostname);

		}

		public UserService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {

		}

		// For quick-adding the album on user page
		[Obsolete]
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
					AuditLog(string.Format("failed login from {0} - no user.", MakeGeoIpToolLink(hostname)), session, name);
					Thread.Sleep(2000);
					return null;
				}

				var hashed = LoginManager.GetHashedPass(lc, pass, user.Salt);

				if (user.Password != hashed) {
					AuditLog(string.Format("failed login from {0} - wrong password.", MakeGeoIpToolLink(hostname)), session, name);
					Thread.Sleep(2000);
					return null;
				}

				AuditLog(string.Format("logged in from {0}.", MakeGeoIpToolLink(hostname)), session, user);

				user.UpdateLastLogin(hostname);
				session.Update(user);

				return new UserContract(user);

			});

		}

		public UserContract CheckTwitterAuthentication(string accessToken, string hostname) {

			return HandleTransaction(session => {

				var user = session.Query<UserOptions>().Where(u => u.TwitterOAuthToken == accessToken)
					.Select(a => a.User).FirstOrDefault();

				if (user == null)
					return null;

				AuditLog(string.Format("logged in from {0}.", MakeGeoIpToolLink(hostname)), session, user);

				user.UpdateLastLogin(hostname);
				session.Update(user);

				return new UserContract(user);

			});

		}

		public bool CheckPasswordResetRequest(Guid requestId) {

			return HandleQuery(session => session.Query<PasswordResetRequest>().Any(r => r.Id == requestId));

		}

		public bool ConnectTwitter(string authToken, int twitterId, string twitterName, string hostname) {

			ParamIs.NotNullOrEmpty(() => authToken);
			ParamIs.NotNullOrEmpty(() => hostname);

			return HandleTransaction(session => {

				var user = session.Query<UserOptions>().Where(u => u.TwitterOAuthToken == authToken)
					.Select(a => a.User).FirstOrDefault();

				if (user != null)
					return false;

				user = GetLoggedUser(session);

				user.Options.TwitterId = twitterId;
				user.Options.TwitterName = twitterName;
				user.Options.TwitterOAuthToken = authToken;
				session.Update(user);

				AuditLog(string.Format("connected to twitter from {0}.", MakeGeoIpToolLink(hostname)), session, user);

				return true;

			});

		}

		public UserContract Create(string name, string pass, string email, string hostname, TimeSpan timeSpan) {

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
				user.UpdateLastLogin(hostname);
				session.Save(user);

				AuditLog(string.Format("registered from {0} in {1}.", MakeGeoIpToolLink(hostname), timeSpan), session, user);

				return new UserContract(user);

			});

		}

		public CommentContract CreateComment(int userId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return HandleTransaction(session => {

				var user = session.Load<User>(userId);
				var agent = SessionHelper.CreateAgentLoginData(session, PermissionContext);

				AuditLog(string.Format("creating comment for {0}: '{1}'",
					EntryLinkFactory.CreateEntryLink(user),
					HttpUtility.HtmlEncode(message)), session, agent.User);

				var comment = user.CreateComment(message, agent);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		public UserContract CreateTwitter(string authToken, string name, string email, int twitterId, string twitterName, string hostname) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => email);

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();
				var existing = session.Query<User>().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					return null;

				var salt = new Random().Next();
				var user = new User(name, string.Empty, email, salt);
				user.Options.TwitterId = twitterId;
				user.Options.TwitterName = twitterName;
				user.Options.TwitterOAuthToken = authToken;
				user.UpdateLastLogin(hostname);
				session.Save(user);

				AuditLog(string.Format("registered from {0} using Twitter.", MakeGeoIpToolLink(hostname)), session, user);

				return new UserContract(user);

			});

		}

		public void DeleteAlbumForUser(int albumForUserId) {

			DeleteEntity<AlbumForUser>(albumForUserId, PermissionToken.EditProfile);

		}

		public void DeleteComment(int commentId) {

			HandleTransaction(session => {

				var comment = session.Load<UserComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author) && !user.Equals(comment.User))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				comment.User.Comments.Remove(comment);
				session.Delete(comment);

			});

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

		public PartialFindResult<AlbumForUserContract> GetAlbumCollection(AlbumCollectionQueryParams queryParams) {

			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session => {

				var status = queryParams.FilterByStatus;
				var paging = queryParams.Paging;

				var albums = session.Query<AlbumForUser>()
					.Where(a => a.User.Id == queryParams.UserId && !a.Album.Deleted && (status == PurchaseStatus.Nothing || a.PurchaseStatus == status))
					.AddNameOrder(LanguagePreference)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(a => new AlbumForUserContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = paging.GetTotalCount ? session.Query<AlbumForUser>()
					.Count(a => a.User.Id == queryParams.UserId && !a.Album.Deleted && (status == PurchaseStatus.Nothing || a.PurchaseStatus == status)) : 0;

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

		public PartialFindResult<FavoriteSongForUserContract> GetFavoriteSongs(RatedSongQueryParams queryParams) {

			ParamIs.NotNull(() => queryParams);

			return HandleQuery(session => {

				// Apply initial filter
				var q = session.Query<FavoriteSongForUser>()
					.Where(a => !a.Song.Deleted && a.User.Id == queryParams.UserId);

				if (queryParams.FilterByRating != SongVoteRating.Nothing)
					q = q.Where(s => s.Rating == queryParams.FilterByRating);

				// Group by rating if needed
				if (queryParams.GroupByRating)
					q = q.OrderByDescending(r => r.Rating);

				// Add custom order
				q = q.AddSongOrder(queryParams.SortRule, LanguagePreference);

				// Apply paging
				var resultQ = q.Skip(queryParams.Paging.Start).Take(queryParams.Paging.MaxEntries);

				var contracts = resultQ.ToArray().Select(a => new FavoriteSongForUserContract(a, PermissionContext.LanguagePreference)).ToArray();
				var totalCount = (queryParams.Paging.GetTotalCount ? q.Count() : 0);

				return new PartialFindResult<FavoriteSongForUserContract>(contracts, totalCount);

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

		public PartialFindResult<UserContract> GetUsers(UserGroupId groupId, string name, UserSortRule sortRule, PagingProperties paging) {

			return HandleQuery(session => {

				var users = AddOrder(session.Query<User>()
					.Where(u => (groupId == UserGroupId.Nothing || u.GroupId == groupId) && string.IsNullOrWhiteSpace(name) || u.Name.Contains(name)), sortRule)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(u => new UserContract(u))
					.ToArray();

				var count = paging.GetTotalCount ? session.Query<User>()
					.Count(u => (groupId == UserGroupId.Nothing || u.GroupId == groupId) && string.IsNullOrWhiteSpace(name) || u.Name.Contains(name)) : 0;

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

			return HandleQuery(session => new UserForMySettingsContract(session.Load<User>(id)));

		}

		public UserWithPermissionsContract GetUserWithPermissions(int id) {

			return HandleQuery(session => new UserWithPermissionsContract(session.Load<User>(id), LanguagePreference));

		}

		public UserContract GetUserByName(string name, bool skipMessages) {

			return HandleQuery(session => {

				var user = session.Query<User>().FirstOrDefault(u => u.Name.Equals(name));

				if (user == null)
					return null;

				var contract = new UserContract(user);

				contract.HasUnreadMessages = (!skipMessages && session.Query<UserMessage>().Any(m => !m.Read && m.Receiver.Id == user.Id));

				return contract;

			});

		}

		public UserDetailsContract GetUserByNameNonSensitive(string name) {

			if (string.IsNullOrEmpty(name))
				return null;

			return HandleQuery(session => {

				var user = session.Query<User>().ToArray()
					.FirstOrDefault(u => u.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

				if (user == null)
					return null;

				return GetUserDetails(session, user);
				
			});

		}

		private IQueryable<T> AddFilter<T>(IQueryable<T> query, int userId, int maxCount, bool onlySubmissions) where T : ArchivedObjectVersion {

			query = query.Where(q => q.Author.Id == userId);

			if (onlySubmissions)
				query = query.Where(q => q.Version == 0);

			query = query.OrderByDescending(q => q.Created);

			query = query.Take(maxCount);

			return query;

		}

		public PartialFindResult<UserMessageContract> GetReceivedMessages(int userId, PagingProperties paging) {

			return HandleQuery(session => {

				var query = session.Query<UserMessage>()
					.Where(m => m.Receiver.Id == userId);

				var messages = query
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var count = (paging.GetTotalCount ? query.Count() : 0);

				return new PartialFindResult<UserMessageContract>(messages.Select(m => new UserMessageContract(m)).ToArray(), count);

			});

		}

		public PartialFindResult<UserMessageContract> GetSentMessages(int userId, PagingProperties paging) {

			return HandleQuery(session => {

				var query = session.Query<UserMessage>()
					.Where(m => m.Sender.Id == userId);

				var messages = query
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var count = (paging.GetTotalCount ? query.Count() : 0);

				return new PartialFindResult<UserMessageContract>(messages.Select(m => new UserMessageContract(m)).ToArray(), count);

			});

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

				if (link != null) {
					AuditLog("deleting " + link, session);
					session.Delete(link);
				}

			});

		}

		public void RemoveArtistFromUser(int userId, int artistId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var link = session.Query<ArtistForUser>()
					.FirstOrDefault(a => a.Artist.Id == artistId && a.User.Id == userId);

				AuditLog(string.Format("removing {0}", link), session);

				if (link != null) {
					link.Delete();
					session.Delete(link);
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

		public void ResetAccessKey() {

			PermissionContext.VerifyLogin();

			HandleTransaction(session => {

				var user = GetLoggedUser(session);
				user.GenerateAccessKey();

				session.Update(user);

				AuditLog("reset access key", session);

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

				SysLog("sending message from " + sender + " to " + receiver);

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

					NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);
					albumForUser.Delete();
					session.Delete(albumForUser);
					session.Update(albumForUser.Album);

				// Add
				} else if (albumForUser == null && (status != PurchaseStatus.Nothing || rating != 0)) {

					var user = session.Load<User>(userId);
					var album = session.Load<Album>(albumId);

					NHibernateUtil.Initialize(album.CoverPictureData);
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
						NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);
						session.Update(albumForUser.Album);
					}

					AuditLog(string.Format("updated {0} for {1}", 
						CreateEntryLink(albumForUser.Album), albumForUser.User), session);

				}

			});

		}

		[Obsolete]
		public void UpdateAlbumForUserMediaType(int albumForUserId, MediaType mediaType) {

			UpdateEntity<AlbumForUser>(albumForUserId, albumForUser => albumForUser.MediaType = mediaType, PermissionToken.EditProfile);

		}

		[Obsolete]
		public void UpdateAlbumForUserRating(int albumForUserId, int rating) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var albumForUser = session.Load<AlbumForUser>(albumForUserId);
				NHibernateUtil.Initialize(albumForUser.Album.CoverPictureData);

				albumForUser.Rating = rating;
				albumForUser.Album.UpdateRatingTotals();
				session.Update(albumForUser.Album);

			});

		}

		public void UpdateSongRating(int userId, int songId, SongVoteRating rating) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var existing = session.Query<FavoriteSongForUser>().FirstOrDefault(f => f.User.Id == userId && f.Song.Id == songId);
				var user = session.Load<User>(userId);
				var song = session.Load<Song>(songId);
				var agent = new AgentLoginData(user);

				if (existing != null) {

					if (rating != SongVoteRating.Nothing) {
						existing.SetRating(rating);
						session.Update(existing);
					} else {
						existing.Delete();
						session.Delete(existing);
					}

				} else if (rating != SongVoteRating.Nothing) {

					var link = user.AddSongToFavorites(song, rating);
					session.Save(link);

				}

				session.Update(song);

				AuditLog(string.Format("rating {0} as '{1}'.", EntryLinkFactory.CreateEntryLink(song), rating),
					session, agent);

			}, string.Format("Unable to rate song with ID '{0}'.", songId));

		}

		public void UpdateUser(UserWithPermissionsContract contract) {

			ParamIs.NotNull(() => contract);

			UpdateEntity<User>(contract.Id, (session, user) => {

				if (!EntryPermissionManager.CanEditUser(PermissionContext, user.GroupId)) {
					var loggedUser = GetLoggedUser(session);
					var msg = string.Format("{0} (level {1}) not allowed to edit {2}", loggedUser, loggedUser.GroupId, user);
					log.Error(msg);
					throw new NotAllowedException(msg);
				}

				if (EntryPermissionManager.CanEditGroupTo(PermissionContext, contract.GroupId)) {
					user.GroupId = contract.GroupId;
				}

				if (EntryPermissionManager.CanEditAdditionalPermissions(PermissionContext)) {
					user.AdditionalPermissions = new PermissionCollection(contract.AdditionalPermissions.Select(p => PermissionToken.GetById(p.Id)));
				}

				var diff = OwnedArtistForUser.Sync(user.AllOwnedArtists, contract.OwnedArtistEntries, a => user.AddOwnedArtist(session.Load<Artist>(a.Artist.Id)));
				SessionHelper.Sync(session, diff);

				user.Active = contract.Active;

				AuditLog(string.Format("updated {0}", EntryLinkFactory.CreateEntryLink(user)), session);

			}, PermissionToken.ManageUserPermissions, skipLog: true);

		}

		public UserContract UpdateUserSettings(UpdateUserSettingsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return HandleTransaction(session => {

				var user = session.Load<User>(contract.Id);

				SysLog(string.Format("Updating settings for {0}", user));

				VerifyResourceAccess(user);

				if (!string.IsNullOrEmpty(contract.NewPass)) {

					var oldHashed = (!string.IsNullOrEmpty(user.Password) ? LoginManager.GetHashedPass(user.NameLC, contract.OldPass, user.Salt) : string.Empty);

					if (user.Password != oldHashed)
						throw new InvalidPasswordException();

					var newHashed = LoginManager.GetHashedPass(user.NameLC, contract.NewPass, user.Salt);
					user.Password = newHashed;

				}

				user.Options.AboutMe = contract.AboutMe;
				user.AnonymousActivity = contract.AnonymousActivity;
				user.Culture = contract.Culture;
				user.DefaultLanguageSelection = contract.DefaultLanguageSelection;
				user.EmailOptions = contract.EmailOptions;
				user.Language = contract.Language;
				user.Options.Location = contract.Location;
				user.PreferredVideoService = contract.PreferredVideoService;
				user.Options.PublicRatings = contract.PublicRatings;
				user.SetEmail(contract.Email);

				var webLinkDiff = WebLink.Sync(user.WebLinks, contract.WebLinks, user);
				SessionHelper.Sync(session, webLinkDiff);

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
