﻿using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using NLog;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using System.Globalization;
using System.Threading;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.Security {

	/// <summary>
	/// Manages login and culture related properties per-request.
	/// </summary>
	public class LoginManager : IUserPermissionContext {

		public const int InvalidId = 0;
		public const string LangParamName = "lang";

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private UserWithPermissionsContract user;

		private bool TryGetLanguagePreferenceFromCookie(ref ContentLanguagePreference languagePreference) {

			if (HttpContext.Current == null)
				return false;

			var cookie = HttpContext.Current.Request.Cookies.Get("languagePreference");

			if (cookie == null || string.IsNullOrEmpty(cookie.Value))
				return false;

			languagePreference = EnumVal<ContentLanguagePreference>.Parse(cookie.Value);

			return true;

		}

		private bool TryGetLanguagePreferenceFromRequest(ref ContentLanguagePreference languagePreference) {

			if (HttpContext.Current == null || string.IsNullOrEmpty(HttpContext.Current.Request.Params[LangParamName]))
				return false;

			return Enum.TryParse(HttpContext.Current.Request.Params[LangParamName], out languagePreference);

		}

		private ContentLanguagePreference OverrideLang {
			get { return (ContentLanguagePreference)HttpContext.Current.Items["overrideLang"]; }
			set { HttpContext.Current.Items["overrideLang"] = value; }
		}

		private bool OverrideUserLang {
			get { return HttpContext.Current != null && HttpContext.Current.Items.Contains("overrideLang"); }
		}

		private void SetCultureSafe(string name, bool culture, bool uiCulture) {

			if (string.IsNullOrEmpty(name))
				return;

			try {

				var c = CultureInfo.GetCultureInfo(name);

				if (culture)
					Thread.CurrentThread.CurrentCulture = c;

				if (uiCulture)
					Thread.CurrentThread.CurrentUICulture = c;

			} catch (ArgumentException x) { 
				log.WarnException("Unable to set culture", x);
			}

		}

		public static string GetHashedPass(string name, string pass, int salt) {

			return CryptoHelper.HashSHA1(name + pass + salt);

		}

		public static string GetHashedAccessKey(string key) {

			var salt = ConfigurationManager.AppSettings["AccessKeySalt"] ?? string.Empty;

			return CryptoHelper.HashSHA1(key + salt);

		}

		public static void SetLoggedUser(UserWithPermissionsContract user) {

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

			if (!IsLoggedIn || !LoggedUser.Active)
				return false;

			if (token == PermissionToken.ManageDatabase && LockdownEnabled)
				return false;

			return (LoggedUser.EffectivePermissions.Contains(token));

		}

		public bool IsLoggedIn {
			get {
				return (HttpContext.Current != null && User != null && User.Identity.IsAuthenticated && User is VocaDbPrincipal);
			}
		}

		public ContentLanguagePreference LanguagePreference {
			get {

				if (OverrideUserLang)
					return OverrideLang;

				var lp = ContentLanguagePreference.Default;

				if (TryGetLanguagePreferenceFromCookie(ref lp)) {
					return lp;
				}

				if (TryGetLanguagePreferenceFromRequest(ref lp)) {
					return lp;
				}

				return (LoggedUser != null ? LoggedUser.DefaultLanguageSelection : ContentLanguagePreference.Default);

			}
		}

		public bool LockdownEnabled {
			get {
				return !string.IsNullOrEmpty(AppConfig.LockdownMessage);
			}
		}

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		public UserWithPermissionsContract LoggedUser {
			get {

				if (user != null)
					return user;

				user = (IsLoggedIn ? ((VocaDbPrincipal)User).User : null);
				return user;

			}
		}

		/// <summary>
		/// Logged user Id or InvalidId if no user is logged in.
		/// </summary>
		public int LoggedUserId {
			get {
				return (LoggedUser != null ? LoggedUser.Id : InvalidId);
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

				var cName = HttpContext.Current.Request.Params["culture"];
				SetCultureSafe(cName, true, true);

			} else if (IsLoggedIn) {
				SetCultureSafe(LoggedUser.Culture, true, false);
				SetCultureSafe(LoggedUser.Language, false, true);
			}

		}

		public void OverrideLanguage(ContentLanguagePreference languagePreference) {
			OverrideLang = languagePreference;
		}

		public void SetLanguagePreferenceCookie(ContentLanguagePreference languagePreference) {


			if (HttpContext.Current != null) {
				var cookie = new HttpCookie("languagePreference", languagePreference.ToString()) { Expires = DateTime.Now + TimeSpan.FromDays(30) };
				//if (HttpContext.Current.Request.Cookies.Get("languagePreference") != null)
				//	HttpContext.Current.Response.Cookies.Set(cookie);				
				//else
					HttpContext.Current.Response.Cookies.Add(cookie);
			}
		}

		public void VerifyLogin() {

			if (!IsLoggedIn)
				throw new NotAllowedException("Must be logged in.");

		}

		public void VerifyPermission(PermissionToken flag) {

			if (!HasPermission(flag)) {
				log.Warn("User '{0}' does not have the requested permission '{1}'", Name, flag);
				throw new NotAllowedException();				
			}

		}

	}

}
