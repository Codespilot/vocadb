using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using NLog;
using NHibernate;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code;

namespace VocaDb.Web {

	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static ISessionFactory sessionFactory;
		private const string sessionFactoryLock = "lock";

		public static bool IsAjaxRequest(HttpRequest request) {

			if (request == null) {
				throw new ArgumentNullException("request");
			}

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
				log.FatalException("Error during authentication", x);
			}

		}

		protected void Application_Error(object sender, EventArgs e) {

			var ex = HttpContext.Current.Server.GetLastError();

			if (ex == null)
				return;

			if (ex is ObjectNotFoundException && HttpContext.Current != null) {
				Response.StatusCode = 404;
				Response.StatusDescription = "Entity not found";
				Response.TrySkipIisCustomErrors = true;
				Server.ClearError();
				HttpContext.Current.Response.RedirectToRoute("Default", new { controller = "Error", action = "NotFound" });
				return;
			}

			var httpException = ex as HttpException;
			if (httpException != null) {
				Server.ClearError();
				HttpContext.Current.Response.RedirectToRoute("Default", new { controller = "Error", code = httpException.GetHttpCode() });
				return;
			}

			var request = (HttpContext.Current.Request != null ? " (" + HttpContext.Current.Request.RawUrl + " from " + HttpContext.Current.Request.UserHostAddress + ")" : string.Empty);

			log.ErrorException("Unhandled exception" + request, ex);

			Server.ClearError();
			HttpContext.Current.Response.RedirectToRoute("Default", new { controller = "Error" });

		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new HandleErrorAttribute { ExceptionType = typeof(ObjectNotFoundException), View = "NotFound" });
			//filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("favicon.ico");

			routes.MapRoute("Search", "Search/{filter}", new { controller = "Home", action = "Search", filter = UrlParameter.Optional });
			routes.MapRoute("Song", "S/{id}", new { controller = "Song", action = "Details" });
			routes.MapRoute("User", "Profile/{id}", new { controller = "User", action = "Profile" });

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