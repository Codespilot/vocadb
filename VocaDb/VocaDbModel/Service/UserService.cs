using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Web;
using NLog;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Paging;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Model.Service {

	public class UserService : ServiceBase {

		class UserStats {
			
			public int AlbumCollectionCount { get; set;}

			public int ArtistCount { get; set; }

			public int CommentCount { get; set; }

			public int FavoriteSongCount { get; set; }

			public int OwnedAlbumCount { get; set; }

			public int RatedAlbumCount { get; set;}

		}

// ReSharper disable UnusedMember.Local
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
// ReSharper restore UnusedMember.Local

		private UserDetailsContract GetUserDetails(ISession session, User user) {

			var details = new UserDetailsContract(user, PermissionContext);

			var stats = session.Query<User>().Where(u => u.Id == user.Id).Select(u => new UserStats {
				AlbumCollectionCount = u.AllAlbums.Count(a => !a.Album.Deleted),
				ArtistCount = u.AllArtists.Count(a => !a.Artist.Deleted),
				CommentCount = u.Comments.Count,
				FavoriteSongCount = u.FavoriteSongs.Count(c => !c.Song.Deleted),
				OwnedAlbumCount = u.AllAlbums.Count(a => !a.Album.Deleted && a.PurchaseStatus == PurchaseStatus.Owned),
				RatedAlbumCount = u.AllAlbums.Count(a => !a.Album.Deleted && a.Rating != 0),
			}).First();

			details.AlbumCollectionCount = stats.AlbumCollectionCount;
			details.ArtistCount = stats.ArtistCount;
			details.CommentCount = stats.CommentCount;
			details.FavoriteSongCount = stats.FavoriteSongCount;

			/*details.AlbumCollectionCount
				= session.Query<AlbumForUser>().Count(c => c.User == user && !c.Album.Deleted);

			details.ArtistCount
				= session.Query<ArtistForUser>().Count(c => c.User == user && !c.Artist.Deleted);
			 
			details.FavoriteSongCount
				= session.Query<FavoriteSongForUser>().Count(c => c.User == user && !c.Song.Deleted);			 			 
			details.CommentCount
				= session.Query<AlbumComment>().Count(c => c.Author == user && !c.Album.Deleted)
				+ session.Query<ArtistComment>().Count(c => c.Author == user && !c.Artist.Deleted)
				+ session.Query<SongComment>().Count(c => c.Author == user && !c.Song.Deleted);
			 */

			details.FavoriteAlbums = session.Query<AlbumForUser>()
				.Where(c => c.User.Id == user.Id && !c.Album.Deleted && c.Rating > 3)
				.OrderByDescending(c => c.Rating)
				.ThenByDescending(c => c.Id)
				.Select(a => a.Album)
				.Take(5)
				.ToArray()
				.Select(c => new AlbumContract(c, LanguagePreference))
				.ToArray();

			details.EditCount
				= session.Query<ArchivedAlbumVersion>().Count(c => c.Author == user)
				+ session.Query<ArchivedArtistVersion>().Count(c => c.Author == user)
				+ session.Query<ArchivedSongVersion>().Count(c => c.Author == user);

			details.FollowedArtists = session.Query<ArtistForUser>()
				.Where(c => c.User.Id == user.Id && !c.Artist.Deleted)
				.OrderByDescending(a => a.Id)
				.Select(c => c.Artist)
				.Take(6)
				.ToArray()
				.Select(c => new ArtistContract(c, LanguagePreference))
				.ToArray();

			details.LatestComments = session.Query<UserComment>()
				.Where(c => c.User == user).OrderByDescending(c => c.Created).Take(3)
				.ToArray()
				.Select(c => new CommentContract(c)).ToArray();

			details.LatestRatedSongs = session.Query<FavoriteSongForUser>()
				.Where(c => c.User.Id == user.Id && !c.Song.Deleted)
				.OrderByDescending(c => c.Id)
				.Select(c => c.Song)
				.Take(6)
				.ToArray()
				.Select(c => new SongContract(c, LanguagePreference))
				.ToArray();

			details.SubmitCount
				= session.Query<ArchivedAlbumVersion>().Count(c => c.Author == user && c.Version == 0)
				+ session.Query<ArchivedArtistVersion>().Count(c => c.Author == user && c.Version == 0)
				+ session.Query<ArchivedSongVersion>().Count(c => c.Author == user && c.Version == 0);

			details.TagVotes
				= session.Query<TagVote>().Count(t => t.User == user);

			details.Power = UserHelper.GetPower(details, stats.OwnedAlbumCount, stats.RatedAlbumCount);
			details.Level = UserHelper.GetLevel(details.Power);

			return details;

		}

		/*private bool IsPoisoned(ISession session, string lcUserName) {

			return session.Query<UserOptions>().Any(o => o.Poisoned && o.User.NameLC == lcUserName);

		}*/

		private string MakeGeoIpToolLink(string hostname) {

			return string.Format("<a href='http://www.geoiptool.com/?IP={0}'>{0}</a>", hostname);

		}

		public UserService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {

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

		public UserContract CheckAccessWithKey(string name, string accessKey, string hostname) {

			return HandleQuery(session => {

				var lc = name.ToLowerInvariant();
				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.Name == lc);

				if (user == null) {
					AuditLog(string.Format("failed login from {0} - no user.", MakeGeoIpToolLink(hostname)), session, name);
					Thread.Sleep(2000);
					return null;
				}

				var hashed = LoginManager.GetHashedAccessKey(user.AccessKey);

				if (accessKey != hashed) {
					AuditLog(string.Format("failed login from {0} - wrong password.", MakeGeoIpToolLink(hostname)), session, name);
					Thread.Sleep(2000);
					return null;					
				}

				AuditLog(string.Format("logged in from {0} with access key.", MakeGeoIpToolLink(hostname)), session, user);

				return new UserContract(user);

			});

		}

		/*
		public LoginResult CheckAuthentication(string name, string pass, string hostname) {

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();

				if (IsPoisoned(session, lc)) {
					SysLog(string.Format("failed login from {0} - account is poisoned.", MakeGeoIpToolLink(hostname)), name);
					return LoginResult.CreateError(LoginError.AccountPoisoned);
				}

				var user = session.Query<User>().FirstOrDefault(u => u.Active && u.Name == lc);

				if (user == null) {
					AuditLog(string.Format("failed login from {0} - no user.", MakeGeoIpToolLink(hostname)), session, name);
					Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.NotFound);
				}

				var hashed = LoginManager.GetHashedPass(lc, pass, user.Salt);

				if (user.Password != hashed) {
					AuditLog(string.Format("failed login from {0} - wrong password.", MakeGeoIpToolLink(hostname)), session, name);
					Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.InvalidPassword);
				}

				AuditLog(string.Format("logged in from {0}.", MakeGeoIpToolLink(hostname)), session, user);

				user.UpdateLastLogin(hostname);
				session.Update(user);

				return LoginResult.CreateSuccess(new UserContract(user));

			});

		}*/

		public UserContract CheckTwitterAuthentication(string accessToken, string hostname) {

			return HandleTransaction(session => {

				var user = session.Query<UserOptions>().Where(u => u.TwitterOAuthToken == accessToken)
					.Select(a => a.User).FirstOrDefault();

				if (user == null)
					return null;

				AuditLog(string.Format("logged in from {0} with twitter.", MakeGeoIpToolLink(hostname)), session, user);

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

		/*
		/// <summary>
		/// Creates a new user account.
		/// </summary>
		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="pass">Password. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique. Cannot be null.</param>
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <param name="timeSpan">Time in which the user filled the registration form.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		public UserContract Create(string name, string pass, string email, string hostname, TimeSpan timeSpan) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			return HandleTransaction(session => {

				var lc = name.ToLowerInvariant();
				var existing = session.Query<User>().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					existing = session.Query<User>().FirstOrDefault(u => u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();
					
				}

				var salt = new Random().Next();
				var hashed = LoginManager.GetHashedPass(lc, pass, salt);
				var user = new User(name, hashed, email, salt);
				user.UpdateLastLogin(hostname);
				session.Save(user);

				AuditLog(string.Format("registered from {0} in {1}.", MakeGeoIpToolLink(hostname), timeSpan), session, user);

				return new UserContract(user);

			});

		}*/

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

		public UserContract[] FindUsersByName(string term, bool startsWith = false) {

			return HandleQuery(session => {

				User[] users;
				if (startsWith) {
					users = session.Query<User>().Where(u => u.Name.StartsWith(term)).OrderBy(u => u.Name).Take(10).ToArray();										
				} else {
					users = session.Query<User>().Where(u => u.Name.Contains(term)).OrderBy(u => u.Name).Take(10).ToArray();					
				}

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

		/*public UserMessageContract GetMessageDetails(int messageId) {

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

		}*/

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

		public UserWithPermissionsContract GetUserByName(string name, bool skipMessages) {

			return HandleQuery(session => {

				var user = session.Query<User>().FirstOrDefault(u => u.Name.Equals(name));

				if (user == null)
					return null;

				var contract = new UserWithPermissionsContract(user, LanguagePreference);

				if (!skipMessages)
					contract.UnreadMessagesCount = session.Query<UserMessage>().Count(m => !m.Read && m.Receiver.Id == user.Id);

				return contract;

			});

		}

		public UserDetailsContract GetUserByNameNonSensitive(string name) {

			if (string.IsNullOrEmpty(name))
				return null;

			return HandleQuery(session => {

				var user = session
					.Query<User>()
					.FirstOrDefault(u => u.Name == name);

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

				return new PartialFindResult<UserMessageContract>(messages.Select(m => new UserMessageContract(m, null)).ToArray(), count);

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

				return new PartialFindResult<UserMessageContract>(messages.Select(m => new UserMessageContract(m, null)).ToArray(), count);

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

		public UserMessagesContract GetUserMessages(int id, int maxCount, bool unread, IUserIconFactory iconFactory) {

			return HandleQuery(session => new UserMessagesContract(session.Load<User>(id), maxCount, unread, iconFactory));

		}

		[Obsolete("Handled by update")]
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

		public void SendMessage(UserMessageContract contract, string mySettingsUrl, string messagesUrl) {

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
					mailer.SendPrivateMessageNotification(mySettingsUrl, messagesUrl, message);

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

		[Obsolete]
		public void UpdateAlbumForUserMediaType(int albumForUserId, MediaType mediaType) {

			UpdateEntity<AlbumForUser>(albumForUserId, albumForUser => albumForUser.MediaType = mediaType, PermissionToken.EditProfile);

		}

		[Obsolete]
		public void UpdateAlbumForUserRating(int albumForUserId, int rating) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var albumForUser = session.Load<AlbumForUser>(albumForUserId);

				albumForUser.Rating = rating;
				albumForUser.Album.UpdateRatingTotals();
				session.Update(albumForUser.Album);

			});

		}

		public void UpdateContentLanguagePreference(ContentLanguagePreference languagePreference) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var user = GetLoggedUser(session);

				user.DefaultLanguageSelection = languagePreference;
				session.Update(user);

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
				user.Options.Poisoned = contract.Poisoned;

				AuditLog(string.Format("updated user {0}", EntryLinkFactory.CreateEntryLink(user)), session);

			}, PermissionToken.ManageUserPermissions, skipLog: true);

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

	public class UserNameAlreadyExistsException : Exception {

		public UserNameAlreadyExistsException()
			: base("Username is already taken") {}

	}

	public class UserEmailAlreadyExistsException : Exception {

		public UserEmailAlreadyExistsException()
			: base("Email address is already taken") {}

	}

	public enum UserSortRule {

		RegisterDate,

		Name,

		Group

	}

	public enum LoginError {
		
		Nothing,

		NotFound,

		InvalidPassword,

		AccountPoisoned,

	}

	public class LoginResult {

		public static LoginResult CreateError(LoginError error) {
			return new LoginResult {Error = error };
		}

		public static LoginResult CreateSuccess(UserContract user) {
			return new LoginResult {User = user, Error = LoginError.Nothing};
		}

		public LoginError Error { get; set; }

		public bool IsOk {
			get { return Error == LoginError.Nothing; }
		}

		public UserContract User { get; set; }

	}

}
