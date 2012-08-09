using System;
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

namespace VocaDb.Web {

	public class MvcApplication : HttpApplication {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static ISessionFactory sessionFactory;
		private const string sessionFactoryLock = "lock";

		public static bool IsAjaxRequest(HttpRequest request) {

			ParamIs.NotNull(() => request);

			return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"));

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

			// Generic HTTP exception. Can be 404 or something else.
			var httpException = ex as HttpException;
			if (httpException != null && !(ex is HttpCompileException) && httpException.GetHttpCode() != ErrorLogger.Code_InternalServerError) {
				
				var code = httpException.GetHttpCode();
				HandleHttpError(code);
				return;

			}

			// Generic NHibernate exception. This error has been logged already.
			if (!(ex is HibernateException))
				ErrorLogger.LogException(Request, ex);

#if !DEBUG
			Server.ClearError();
			Response.RedirectToRoute("Default", new { controller = "Error", redirect = true });
#endif
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new HandleErrorAttribute { ExceptionType = typeof(ObjectNotFoundException), View = "NotFound" });
			//filters.Add(new HandleErrorAttribute());
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