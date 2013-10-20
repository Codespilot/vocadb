﻿using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Web.Code;

namespace VocaDb.Web.App_Start {

	public static class RouteConfig {

		public static void RegisterRoutes(RouteCollection routes) {

			// Ignored files
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("favicon.ico");

			// Invalid routes - redirects to 404
			routes.MapRoute("AlbumDetailsError", "Album/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
			routes.MapRoute("ArtistDetailsError", "Artist/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
			routes.MapRoute("SongDetailsError", "Song/Details/{id}",
				new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });

			// Action routes
			routes.MapRoute("Album", "Al/{id}", new { controller = "Album", action = "Details" }, new { id = "[0-9]+" });
			routes.MapRoute("Artist", "Ar/{id}", new { controller = "Artist", action = "Details" }, new { id = "[0-9]+" });

			// Song shortcut, for example /S/393939
			routes.MapRoute("Song", "S/{id}", new { controller = "Song", action = "Details" }, new { id = "[0-9]+" });

			// User profile route, for example /Profile/riipah
			routes.MapRoute("User", "Profile/{id}", new { controller = "User", action = "Profile" });


			// Default route
			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

	}

}