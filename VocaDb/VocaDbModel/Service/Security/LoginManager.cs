using System;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using NLog;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using System.Globalization;
using System.Threading;

namespace VocaDb.Model.Service.Security {

	public class LoginManager : IUserPermissionContext {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private UserContract user;

		public static string GetHashedPass(string name, string pass, int salt) {

			return FormsAuthentication.HashPasswordForStoringInConfigFile(name + pass + salt, "sha1");

		}

		public static string GetHashedAccessKey(string key) {

			var salt = ConfigurationManager.AppSettings["AccessKeySalt"] ?? string.Empty;

			return FormsAuthentication.HashPasswordForStoringInConfigFile(key + salt, "sha1");

		}

		public static void SetLoggedUser(UserContract user) {

			ParamIs.NotNull(() => user);

			if (!HttpContext.Current.User.Identity.IsAuthenticated)
				throw new InvalidOperationException("Must be authenticated");

			HttpContext.Current.User = new VocaDbPrincipal(HttpContext.Current.User.Identity, user);

		}

		protected IPrincipal User {
			get {
				return (HttpContext.Current != null ? HttpContext.Current.User : null);
			}
		}

		public bool HasPermission(PermissionToken token) {

			if (token == PermissionToken.Nothing)
				return true;

			return (IsLoggedIn && LoggedUser.Active && LoggedUser.EffectivePermissions.Contains(token));

		}

		public bool IsLoggedIn {
			get {
				return (HttpContext.Current != null && User != null && User.Identity.IsAuthenticated && User is VocaDbPrincipal);
			}
		}

		public ContentLanguagePreference LanguagePreference {
			get {

				ContentLanguagePreference lp;

				if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Params["lang"]) 
					&& Enum.TryParse(HttpContext.Current.Request.Params["lang"], out lp))
					return lp;

				return (LoggedUser != null ? LoggedUser.DefaultLanguageSelection : ContentLanguagePreference.Default);

			}
		}

		public UserContract LoggedUser {
			get {

				if (user != null)
					return user;

				user = (IsLoggedIn ? ((VocaDbPrincipal)User).User : null);
				return user;

			}
		}

		public string Name {
			get {
				return User.Identity.Name;
			}
		}

		public UserGroupId UserGroupId {
			get {

				if (LoggedUser == null)
					return UserGroupId.Nothing;

				return LoggedUser.GroupId;

			}
		}

		public void InitLanguage() {

			if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Params["culture"])) {

				try {
					var culture = CultureInfo.GetCultureInfo(HttpContext.Current.Request.Params["culture"]);
					Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
				} catch (ArgumentException) { }

			} else if (IsLoggedIn && !string.IsNullOrEmpty(LoggedUser.Language)) {
				try {
					var culture = CultureInfo.GetCultureInfo(user.Language);
					Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
				} catch (ArgumentException) { }
			}

		}

		public void VerifyLogin() {

			if (!IsLoggedIn)
				throw new NotAllowedException("Must be logged in.");

		}

		public void VerifyPermission(PermissionToken flag) {

			if (!HasPermission(flag)) {
				log.Warn(string.Format("User '{0}' does not have the requested permission '{1}'", Name, flag));
				throw new NotAllowedException();				
			}

		}

	}

}
