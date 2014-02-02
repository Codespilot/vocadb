﻿using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code.Security;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="User"/>.
	/// </summary>
	public class UserQueries : QueriesBase<IUserRepository, User> {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IUserMessageMailer mailer;
		private readonly IStopForumSpamClient sfsClient;

		public IEntryLinkFactory EntryLinkFactory {
			get { return entryLinkFactory; }
		}

		private IQueryable<User> AddOrder(IQueryable<User> query, UserSortRule sortRule) {

			switch (sortRule) {
				case UserSortRule.Name:
					return query.OrderBy(u => u.Name);
				case UserSortRule.RegisterDate:
					return query.OrderBy(u => u.CreateDate);
				case UserSortRule.Group:
					return query
						.OrderBy(u => u.GroupId)
						.ThenBy(u => u.Name);
			}

			return query;

		}

		private bool IsPoisoned(IRepositoryContext<User> ctx, string lcUserName) {

			return ctx.OfType<UserOptions>().Query().Any(o => o.Poisoned && o.User.NameLC == lcUserName);

		}

		private string MakeGeoIpToolLink(string hostname) {

			return string.Format("<a href='http://www.geoiptool.com/?IP={0}'>{0}</a>", hostname);

		}

		private void SendEmailVerificationRequest(IRepositoryContext<User> ctx, User user, string resetUrl, string subject) {
			
			var request = new PasswordResetRequest(user);
			ctx.Save(request);

			var body = string.Format("Please click the link below to verify your email at VocaDB.\n{0}?token={1}", resetUrl, request.Id);

			mailer.SendEmail(request.User.Email, request.User.Name, subject, body);

		}

		private void ValidateEmail(string email) {
			
			try {
				new MailAddress(email);
			} catch (FormatException x) {
				throw new InvalidEmailFormatException("Email format is invalid", x);
			}

		}

		public UserQueries(IUserRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IStopForumSpamClient sfsClient,
			IUserMessageMailer mailer)
			: base(repository, permissionContext) {

			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);
			ParamIs.NotNull(() => entryLinkFactory);
			ParamIs.NotNull(() => sfsClient);
			ParamIs.NotNull(() => mailer);

			this.entryLinkFactory = entryLinkFactory;
			this.sfsClient = sfsClient;
			this.mailer = mailer;

		}

		/// <summary>
		/// Attempts to log in a user.
		/// </summary>
		/// <param name="name">Username. Cannot be null.</param>
		/// <param name="pass">Password. Cannot be null.</param>
		/// <param name="hostname">Host name where the user is logging in from. Cannot be null.</param>
		/// <param name="delayFailedLogin">Whether failed login should cause artificial delay.</param>
		/// <returns>Login attempt result. Cannot be null.</returns>
		public LoginResult CheckAuthentication(string name, string pass, string hostname, bool delayFailedLogin) {

			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(pass))
				return LoginResult.CreateError(LoginError.InvalidPassword);

			var lc = name.ToLowerInvariant();

			return repository.HandleTransaction(ctx => {

				if (IsPoisoned(ctx, lc)) {
					ctx.AuditLogger.SysLog(string.Format("failed login from {0} - account is poisoned.", MakeGeoIpToolLink(hostname)), name);
					return LoginResult.CreateError(LoginError.AccountPoisoned);
				}

				// Attempt to find user by either lowercase username.
				var user = ctx.Query().FirstOrDefault(u => u.Active && u.NameLC == lc);

				if (user == null) {
					ctx.AuditLogger.AuditLog(string.Format("failed login from {0} - no user.", MakeGeoIpToolLink(hostname)), name);
					if (delayFailedLogin)
						Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.NotFound);
				}

				// Attempt to verify password.
				var hashed = LoginManager.GetHashedPass(user.NameLC, pass, user.Salt);

				if (user.Password != hashed) {
					ctx.AuditLogger.AuditLog(string.Format("failed login from {0} - wrong password.", MakeGeoIpToolLink(hostname)), name);
					if (delayFailedLogin)
						Thread.Sleep(2000);
					return LoginResult.CreateError(LoginError.InvalidPassword);
				}

				// Login attempt successful.
				ctx.AuditLogger.AuditLog(string.Format("logged in from {0} with '{1}'.", MakeGeoIpToolLink(hostname), name), user);

				user.UpdateLastLogin(hostname);
				ctx.Update(user);

				return LoginResult.CreateSuccess(new UserContract(user));

			});

		}

		public bool CheckPasswordResetRequest(Guid requestId) {

			var cutoff = DateTime.Now - PasswordResetRequest.ExpirationTime;

			return repository.HandleQuery(ctx => ctx.OfType<PasswordResetRequest>().Query().Any(r => r.Id == requestId && r.Created >= cutoff));

		}

		public CommentContract CreateComment(int userId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var user = ctx.Load(userId);
				var agent = ctx.CreateAgentLoginData(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					EntryLinkFactory.CreateEntryLink(user),
					HttpUtility.HtmlEncode(message)), agent.User);

				var comment = user.CreateComment(message, agent);
				ctx.OfType<UserComment>().Save(comment);

				var commentMsg = comment.Message.Truncate(200);
				var notificationMsg = string.Format("{0} posted a comment on your profile.\n\n{1}", agent.Name, commentMsg);
				var notification = new UserMessage(user, "Comment posted on your profile", notificationMsg, false);
				ctx.OfType<UserMessage>().Save(notification);

				return new CommentContract(comment);

			});

		}

		private string GetSFSCheckStr(SFSResponseContract result) {

			if (result == null)
				return "error";

			switch (result.Conclusion) {
				case SFSCheckResultType.Malicious:
					return string.Format("Malicious ({0} % confidence)", result.Confidence);
				case SFSCheckResultType.Uncertain:
					return string.Format("Uncertain ({0} % confidence)", result.Confidence);
				default:
					return "Ok";
			}

		}

		/// <summary>
		/// Clears all rated albums and songs by a user.
		/// Also updates rating totals.
		/// 
		/// Staff members cannot be cleared.
		/// </summary>
		/// <param name="id">User Id.</param>
		public void ClearRatings(int id) {
			
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			repository.HandleTransaction(ctx => {
				
				var user = ctx.Load(id);

				if (!user.CanBeDisabled)
					throw new NotAllowedException("This user account cannot be cleared.");

				ctx.AuditLogger.AuditLog(string.Format("clearing ratings by {0}", user));

				while (user.AllAlbums.Any()) {
					var albumLink = user.AllAlbums[0];
					albumLink.Delete();		
					ctx.Delete(albumLink);
					ctx.Update(albumLink.Album); // Update album ratings
				}

				while (user.FavoriteSongs.Any()) {
					var songLink = user.FavoriteSongs[0];
					songLink.Delete();
					ctx.Delete(songLink);
					ctx.Update(songLink.Song); // Update song ratings
				}

				while (user.AllArtists.Any()) {
					var artistLink = user.AllArtists[0];
					ctx.Delete(artistLink);
					artistLink.Delete();
				}

				ctx.Update(user);

			});

		}

		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="pass">Password. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique if specified. Cannot be null.</param>
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <param name="timeSpan">Time in which the user filled the registration form.</param>
		/// <param name="softbannedIPs">List of application's soft-banned IPs. Soft-banned IPs are cleared when the application restarts.</param>
		/// <param name="verifyEmailUrl">Email verification URL. Cannot be null.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		/// <exception cref="TooFastRegistrationException">If the user registered too fast.</exception>
		public UserContract Create(string name, string pass, string email, string hostname, TimeSpan timeSpan,
			HostCollection softbannedIPs, string verifyEmailUrl) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			if (timeSpan < TimeSpan.FromSeconds(5)) {

				log.Warn(string.Format("Suspicious registration form fill time ({0}) from {1}.", timeSpan, hostname));

				if (timeSpan < TimeSpan.FromSeconds(2)) {
					softbannedIPs.Add(hostname);
				}

				throw new TooFastRegistrationException();

			}

			return repository.HandleTransaction(ctx => {

				// Verification
				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					ValidateEmail(email);

					existing = ctx.Query().FirstOrDefault(u => u.Active && u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				// All ok, create user
				var sfsCheckResult = sfsClient.CallApi(hostname);
				var sfsStr = GetSFSCheckStr(sfsCheckResult);

				var salt = new Random().Next();
				var hashed = LoginManager.GetHashedPass(lc, pass, salt);
				var user = new User(name, hashed, email, salt);
				user.UpdateLastLogin(hostname);
				ctx.Save(user);

				if (sfsCheckResult != null && sfsCheckResult.Conclusion == SFSCheckResultType.Malicious) {

					var report = new UserReport(user, UserReportType.MaliciousIP, null, hostname, 
						string.Format("Confidence {0} %, Frequency {1}, Last seen {2}.", 
						sfsCheckResult.Confidence, sfsCheckResult.Frequency, sfsCheckResult.LastSeen.ToShortDateString()));

					ctx.OfType<UserReport>().Save(report);

					user.GroupId = UserGroupId.Limited;
					ctx.Update(user);

				}

				if (!string.IsNullOrEmpty(user.Email)) {
					var subject = "Welcome to VocaDB, please verify your email.";
					SendEmailVerificationRequest(ctx, user, verifyEmailUrl, subject);					
				}

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} in {1} (SFS check {2}).", MakeGeoIpToolLink(hostname), timeSpan, sfsStr), user);

				return new UserContract(user);

			});

		}

		/// <summary>
		/// Creates a new user account using Twitter authentication token.
		/// </summary>
		/// <param name="authToken">Twitter OAuth token. Cannot be null or empty.</param>
		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique. Cannot be null.</param>
		/// <param name="twitterId">Twitter user Id. Cannot be null or empty.</param>
		/// <param name="twitterName">Twitter user name. Cannot be null.</param>
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		public UserContract CreateTwitter(string authToken, string name, string email, int twitterId, string twitterName, string hostname) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => email);

			return repository.HandleTransaction(ctx => {

				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					ValidateEmail(email);

					existing = ctx.Query().FirstOrDefault(u => u.Active && u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				var salt = new Random().Next();
				var user = new User(name, string.Empty, email, salt);
				user.Options.TwitterId = twitterId;
				user.Options.TwitterName = twitterName;
				user.Options.TwitterOAuthToken = authToken;
				user.UpdateLastLogin(hostname);
				ctx.Save(user);

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} using Twitter name '{1}'.", MakeGeoIpToolLink(hostname), twitterName), user);

				return new UserContract(user);

			});

		}

		public void DisableUser(int userId) {
			
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			repository.HandleTransaction(ctx => {

				var user = ctx.Load(userId);

				if (!user.CanBeDisabled)
					throw new NotAllowedException("This user account cannot be disabled.");

				user.Active = false;

				ctx.AuditLogger.AuditLog(string.Format("disabled {0}.", EntryLinkFactory.CreateEntryLink(user)));

				ctx.Update(user);

			});

		}

		public PartialFindResult<UserContract> GetUsers(UserGroupId groupId, string name, bool disabled, bool verifiedArtists, UserSortRule sortRule, PagingProperties paging) {

			return repository.HandleQuery(ctx => {

				var usersQuery = ctx.Query();

				if (groupId != UserGroupId.Nothing) {
					usersQuery = usersQuery.Where(u => u.GroupId == groupId);
				}

				if (!string.IsNullOrWhiteSpace(name)) {
					usersQuery = usersQuery.Where(u => u.Name.Contains(name));
				}

				if (!disabled) {
					usersQuery = usersQuery.Where(u => u.Active);
				}

				if (verifiedArtists) {
					usersQuery = usersQuery.Where(u => u.AllOwnedArtists.Any());
				}

				var users = AddOrder(usersQuery, sortRule)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray()
					.Select(u => new UserContract(u))
					.ToArray();

				var count = paging.GetTotalCount ? usersQuery.Count() : 0;

				return new PartialFindResult<UserContract>(users, count);

			});

		}

		public void RequestEmailVerification(int userId, string resetUrl) {

			repository.HandleTransaction(ctx => {

				var user = ctx.Load(userId);
				ctx.AuditLogger.SysLog("requesting email verification", user.Name);

				var subject = "Verify your email at VocaDB.";

				SendEmailVerificationRequest(ctx, user, resetUrl, subject);

			});

		}

		public void RequestPasswordReset(string username, string email, string resetUrl) {

			ParamIs.NotNullOrEmpty(() => username);
			ParamIs.NotNullOrEmpty(() => email);

			var lc = username.ToLowerInvariant();

			repository.HandleTransaction(ctx => {

				var user = ctx.Query().FirstOrDefault(u => u.NameLC.Equals(lc) && email.Equals(u.Email));

				if (user == null)
					throw new UserNotFoundException();

				var request = new PasswordResetRequest(user);
				ctx.Save(request);

				var subject = "Password reset requested.";

				var body = 
					"You (or someone who knows your email address) has requested to reset your password on VocaDB.\n" +
					"You can perform this action at " + resetUrl + "/" + request.Id + ". If you did not request this action, you can ignore this message.";

				mailer.SendEmail(request.User.Email, request.User.Name, subject, body);

			});

		}

		public UserContract ResetPassword(Guid requestId, string password) {

			ParamIs.NotNullOrEmpty(() => password);

			return repository.HandleTransaction(ctx => {

				var request = ctx.OfType<PasswordResetRequest>().Load(requestId);

				if (!request.IsValid)
					throw new RequestNotValidException("Request has expired");

				var user = request.User;

				ctx.AuditLogger.AuditLog("resetting password", user);

				var newHashed = LoginManager.GetHashedPass(user.NameLC, password, user.Salt);
				user.Password = newHashed;

				ctx.Update(user);

				ctx.Delete(request);

				return new UserContract(user);

			});

		}

		public void SetAlbumFormatString(string formatString) {

			if (!PermissionContext.IsLoggedIn)
				return;

			repository.HandleTransaction(ctx => {

				var user = ctx.GetLoggedUser(PermissionContext);

				user.Options.AlbumFormatString = formatString;
				ctx.Update(user);

			});

		}

		public void UpdateArtistSubscriptionForCurrentUser(int artistId, bool emailNotifications) {
			
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			var userId = PermissionContext.LoggedUserId;

			repository.HandleTransaction(ctx => {
				
				var subscription = ctx.OfType<ArtistForUser>().Query().FirstOrDefault(u => u.User.Id == userId && u.Artist.Id == artistId);

				// No subscription found. Shouldn't happen, but could also be handled so that a new subscription is added.
				if (subscription == null)
					return;

				subscription.EmailNotifications = emailNotifications;
				ctx.Update(subscription);

				ctx.AuditLogger.SysLog(string.Format("updated artist subscription for {0}.", subscription.Artist));

			});

		}

		/// <summary>
		/// Updates user's settings (from my settings page).
		/// </summary>
		/// <param name="contract">New properties. Cannot be null.</param>
		/// <returns>Updated user data. Cannot be null.</returns>
		/// <exception cref="InvalidEmailFormatException">If the email format was invalid.</exception>
		/// <exception cref="InvalidPasswordException">If password change was attempted and the old password was incorrect.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken by another user.</exception>
		public UserWithPermissionsContract UpdateUserSettings(UpdateUserSettingsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {

				var user = ctx.Load(contract.Id);

				ctx.AuditLogger.SysLog(string.Format("Updating settings for {0}", user));

				PermissionContext.VerifyResourceAccess(user);

				if (!string.IsNullOrEmpty(contract.NewPass)) {

					var oldHashed = (!string.IsNullOrEmpty(user.Password) ? LoginManager.GetHashedPass(user.NameLC, contract.OldPass, user.Salt) : string.Empty);

					if (user.Password != oldHashed)
						throw new InvalidPasswordException();

					var newHashed = LoginManager.GetHashedPass(user.NameLC, contract.NewPass, user.Salt);
					user.Password = newHashed;

				}

				var email = contract.Email;

				if (!string.IsNullOrEmpty(email)) {

					ValidateEmail(email);

					var existing = ctx.Query().FirstOrDefault(u => u.Active && u.Id != user.Id && u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				user.Options.AboutMe = contract.AboutMe;
				user.AnonymousActivity = contract.AnonymousActivity;
				user.Culture = contract.Culture;
				user.DefaultLanguageSelection = contract.DefaultLanguageSelection;
				user.EmailOptions = contract.EmailOptions;
				user.Language = contract.Language;
				user.Options.Location = contract.Location;
				user.PreferredVideoService = contract.PreferredVideoService;
				user.Options.PublicAlbumCollection = contract.PublicAlbumCollection;
				user.Options.PublicRatings = contract.PublicRatings;
				user.SetEmail(email);

				var validWebLinks = contract.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(user.WebLinks, validWebLinks, user);
				ctx.OfType<UserWebLink>().Sync(webLinkDiff);

				ctx.Update(user);

				ctx.AuditLogger.AuditLog(string.Format("updated settings for {0}", EntryLinkFactory.CreateEntryLink(user)));

				return new UserWithPermissionsContract(user, PermissionContext.LanguagePreference);

			});

		}

		/// <summary>
		/// Verifies user email.
		/// Logged user must be the same as the user being verified.
		/// </summary>
		/// <param name="requestId">ID of the verification request.</param>
		/// <returns>True if the request was found and was processed. False if the request was not found (already used).</returns>
		public bool VerifyEmail(Guid requestId) {
			
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {
				
				var request = ctx.OfType<PasswordResetRequest>().Get(requestId);

				if (request == null || !request.IsValid)
					return false;

				var user = request.User;
				if (!user.Equals(PermissionContext.LoggedUser))
					throw new RequestNotValidException("Email verification request not valid for this user");

				if (!user.Email.Equals(request.Email, StringComparison.InvariantCultureIgnoreCase)) {
					throw new RequestNotValidException("Email verification request not valid for this user");					
					/*
					// Update email from request in case the user hasn't saved the new email yet.
					user.Email = request.Email;
					*/
				}

				user.Options.EmailVerified = true;
				ctx.Update(user);

				ctx.Delete(request);

				ctx.AuditLogger.AuditLog("verified email");

				return true;

			});

		}

	}

}