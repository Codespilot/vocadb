using System;
using System.Linq;
using System.Threading;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="User"/>.
	/// </summary>
	public class UserQueries {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IUserPermissionContext permissionContext;
		private readonly IUserRepository repository;

		public IEntryLinkFactory EntryLinkFactory {
			get { return entryLinkFactory; }
		}

		private IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		private bool IsPoisoned(IRepositoryContext<User> ctx, string lcUserName) {

			return ctx.OfType<UserOptions>().Query().Any(o => o.Poisoned && o.User.NameLC == lcUserName);

		}

		private string MakeGeoIpToolLink(string hostname) {

			return string.Format("<a href='http://www.geoiptool.com/?IP={0}'>{0}</a>", hostname);

		}

		public UserQueries(IUserRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) {

			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);
			ParamIs.NotNull(() => entryLinkFactory);

			this.repository = repository;
			this.permissionContext = permissionContext;
			this.entryLinkFactory = entryLinkFactory;

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

		/// <param name="name">User name. Must be unique. Cannot be null or empty.</param>
		/// <param name="pass">Password. Cannot be null or empty.</param>
		/// <param name="email">Email address. Must be unique if specified. Cannot be null.</param>
		/// <param name="hostname">Host name where the registration is from.</param>
		/// <param name="timeSpan">Time in which the user filled the registration form.</param>
		/// <returns>Data contract for the created user. Cannot be null.</returns>
		/// <exception cref="UserNameAlreadyExistsException">If the user name was already taken.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken.</exception>
		public UserContract Create(string name, string pass, string email, string hostname, TimeSpan timeSpan) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNullOrEmpty(() => pass);
			ParamIs.NotNull(() => email);

			return repository.HandleTransaction(ctx => {

				var lc = name.ToLowerInvariant();
				var existing = ctx.Query().FirstOrDefault(u => u.NameLC == lc);

				if (existing != null)
					throw new UserNameAlreadyExistsException();

				if (!string.IsNullOrEmpty(email)) {

					existing = ctx.Query().FirstOrDefault(u => u.Active && u.Email == email);

					if (existing != null)
						throw new UserEmailAlreadyExistsException();

				}

				var salt = new Random().Next();
				var hashed = LoginManager.GetHashedPass(lc, pass, salt);
				var user = new User(name, hashed, email, salt);
				user.UpdateLastLogin(hostname);
				ctx.Save(user);

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} in {1}.", MakeGeoIpToolLink(hostname), timeSpan), user);

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

				ctx.AuditLogger.AuditLog(string.Format("registered from {0} using Twitter.", MakeGeoIpToolLink(hostname)), user);

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

		/// <summary>
		/// Updates user's settings (from my settings page).
		/// </summary>
		/// <param name="contract">New properties. Cannot be null.</param>
		/// <returns>Updated user data. Cannot be null.</returns>
		/// <exception cref="InvalidPasswordException">If password change was attempted and the old password was incorrect.</exception>
		/// <exception cref="UserEmailAlreadyExistsException">If the email address was already taken by another user.</exception>
		public UserContract UpdateUserSettings(UpdateUserSettingsContract contract) {

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
				user.Options.PublicRatings = contract.PublicRatings;
				user.SetEmail(email);

				var validWebLinks = contract.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(user.WebLinks, validWebLinks, user);
				ctx.OfType<UserWebLink>().Sync(webLinkDiff);

				ctx.Update(user);

				ctx.AuditLogger.AuditLog(string.Format("updated settings for {0}", EntryLinkFactory.CreateEntryLink(user)));

				return new UserContract(user);

			});

		}

	}

}