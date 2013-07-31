using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Web.Code;

namespace VocaDb.Web.App_Start {

	public static class RouteConfig {

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

	}

}