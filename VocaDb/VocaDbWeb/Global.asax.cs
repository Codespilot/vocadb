using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using log4net;
using NHibernate;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web {

	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {

		private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));
		private static ISessionFactory sessionFactory;

		public static LoginManager LoginManager {
			get {
				return new LoginManager();
			}
		}

		public static ServiceModel Services {
			get {

				if (sessionFactory == null)
					sessionFactory = DatabaseConfiguration.BuildSessionFactory();

				return new ServiceModel(sessionFactory, LoginManager);

			}
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {

			try {

				// Get user roles from cookie and assign correct principal
				if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated) {
					if (HttpContext.Current.User.Identity is FormsIdentity && !(HttpContext.Current.User is VocaDbPrincipal)) {
						var id = (FormsIdentity)HttpContext.Current.User.Identity;
						var user = Services.Users.GetUserByName(id.Name);
						LoginManager.SetLoggedUser(user);
					}
				}
			} catch (Exception x) {
				log.Fatal("Error during authentication", x);
			}

		}

		protected void Application_Error(object sender, EventArgs e) {

			var ex = HttpContext.Current.Server.GetLastError();

			if (ex == null)
				return;

			var request = (HttpContext.Current.Request != null ? " (" + HttpContext.Current.Request.RawUrl + " from " + HttpContext.Current.Request.UserHostAddress + ")" : string.Empty);

			log.Error("Unhandled exception" + request, ex);

		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

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

			log4net.Config.XmlConfigurator.Configure();

		}
	}
}