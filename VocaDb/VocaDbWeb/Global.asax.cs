using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using NLog;
using NHibernate;
using VocaDb.Model;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Security;

namespace VocaDb.Web {

	public class MvcApplication : HttpApplication {

		private static readonly BlockedIPManager blockedIpManager = new BlockedIPManager(LoadBlockedIPs);
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static ISessionFactory sessionFactory;
		private const string sessionFactoryLock = "lock";

		private static string[] LoadBlockedIPs() {

			return Services.Other.GetIPRules().Select(i => i.Address).ToArray();

		}

		public static bool IsAjaxRequest(HttpRequest request) {

			ParamIs.NotNull(() => request);

			return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"));

		}

		public static BlockedIPManager IPRules {
			get {
				return blockedIpManager;
			}
		}

		public static LoginManager LoginManager {
			get {
				return new LoginManager();
			}
		}

		public static ServiceModel Services {
			get {
				return new ServiceModel(SessionFactory, LoginManager, new EntryAnchorFactory());
			}
		}

		public static ISessionFactory SessionFactory {
			get {

				lock (sessionFactoryLock) {
					if (sessionFactory == null)
						sessionFactory = DatabaseConfiguration.BuildSessionFactory();
				}

				return sessionFactory;

			}
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {

			try {

				// Get user roles from cookie and assign correct principal
				if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated) {
					if (HttpContext.Current.User.Identity is FormsIdentity && !(HttpContext.Current.User is VocaDbPrincipal)) {
						var id = (FormsIdentity)HttpContext.Current.User.Identity;
						var user = Services.Users.GetUserByName(id.Name, IsAjaxRequest(Request));
						LoginManager.SetLoggedUser(user);
					}
				}

				LoginManager.InitLanguage();

			} catch (Exception x) {
				ErrorLogger.LogException(Request, x, LogLevel.Fatal);
			}

		}

		/*protected void Application_BeginRequest(object sender, EventArgs e) {

		}*/

		private void HandleHttpError(int code, string description = null) {

			ErrorLogger.LogHttpError(Request, code);

			Server.ClearError();
			Response.StatusCode = code;

			if (!string.IsNullOrEmpty(description))
				Response.StatusDescription = description;

			Response.RedirectToRoute("Default",
				new { controller = "Error", code, redirect = true });

		}

		protected void Application_Error(object sender, EventArgs e) {

			var ex = HttpContext.Current.Server.GetLastError();

			if (ex == null)
				return;

			// NHibernate missing entity exceptions. Usually caused by an invalid or deleted Id. This error has been logged already.
			if (ex is ObjectNotFoundException) {
				HandleHttpError(ErrorLogger.Code_NotFound, "Entity not found");
				return;
			}

			// Insufficient privileges. This error has been logged already.
			if (ex is NotAllowedException) {
				HandleHttpError(ErrorLogger.Code_Forbidden, ex.Message);
				return;
			}

			// Not found (usually by the controller). We get these a lot.
			var httpException = ex as HttpException;
			var code = (httpException != null ? httpException.GetHttpCode() : 0);
			if (httpException != null && code == ErrorLogger.Code_NotFound) {				
				HandleHttpError(code);
				return;
			}

			// Generic NHibernate exception. This error has been logged already.
			if (!(ex is HibernateException))
				ErrorLogger.LogException(Request, ex);

			// Redirect user to generic error page in release mode or display detailed message in debug mode
#if !DEBUG
			Server.ClearError();
			Response.RedirectToRoute("Default", new { controller = "Error", redirect = true });
#endif
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new HandleErrorAttribute { ExceptionType = typeof(ObjectNotFoundException), View = "NotFound" });
			//filters.Add(new HandleErrorAttribute());
			filters.Add(new RestrictBlockedIPAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {

			// Ignored files
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("favicon.ico");

			// Invalid routes
			routes.MapRoute("AlbumDetailsError", "Album/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
			routes.MapRoute("ArtistDetailsError", "Artist/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
			routes.MapRoute("SongDetailsError", "Song/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });

			// Special routes
			//routes.MapRoute("Search", "Search/{filter}", 
			//	new { controller = "Home", action = "Search", filter = UrlParameter.Optional });	// Doesn't work well with certain chars
			routes.MapRoute("Song", "S/{id}", new { controller = "Song", action = "Details" }, new { id = "[0-9]+" });
			routes.MapRoute("User", "Profile/{id}", new { controller = "User", action = "Profile" });

			// Default mapping
			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

		}
	}
}