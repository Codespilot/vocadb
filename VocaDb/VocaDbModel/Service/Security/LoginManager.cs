using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using log4net;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.Security {

	public class LoginManager : IUserPermissionContext {

		private static readonly ILog log = LogManager.GetLogger(typeof(LoginManager));

		private UserContract user;

		public static string GetHashedPass(string name, string pass, int salt) {

			return FormsAuthentication.HashPasswordForStoringInConfigFile(name + pass + salt, "sha1");

		}

		public static void SetLoggedUser(UserContract user) {

			ParamIs.NotNull(() => user);

			if (!HttpContext.Current.User.Identity.IsAuthenticated)
				throw new InvalidOperationException("Must be authenticated");

			HttpContext.Current.User = new VocaDbPrincipal(HttpContext.Current.User.Identity, user);

		}

		protected IPrincipal User {
			get {
				return HttpContext.Current.User;
			}
		}

		public bool HasPermission(PermissionFlags flag) {

			if (flag == PermissionFlags.Nothing)
				return true;

			return (IsLoggedIn && LoggedUser.Active && LoggedUser.EffectivePermissions.HasFlag(flag));

		}

		public bool IsLoggedIn {
			get {
				return (HttpContext.Current != null && User.Identity.IsAuthenticated && User is VocaDbPrincipal);
			}
		}

		public ContentLanguagePreference LanguagePreference {
			get {
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

		public void VerifyPermission(PermissionFlags flag) {

			if (!HasPermission(flag)) {
				log.Warn(string.Format("User '{0}' does not have the requested permission '{1}'", Name, flag));
				throw new NotAllowedException();				
			}

		}

	}

}
