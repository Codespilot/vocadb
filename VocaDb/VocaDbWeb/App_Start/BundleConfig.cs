using System.Web.Optimization;

namespace VocaDb.Web.App_Start {

	public static class BundleConfig {

		public static void RegisterBundles(BundleCollection bundles) {

			// jQuery styles don't work if bundled
			/*bundles.Add(new ScriptBundle("~/bundles/commonLibs").Include(
				"~/Scripts/jquery-1.8.2.min.js", "~/Scripts/bootstrap.min.js",
				"~/Scripts/jquery-ui-1.10.1.custom.min.js", "~/Scripts/knockout-2.2.0.js",
				"~/Scripts/underscore.min.js", "~/Scripts/jquery.qtip.min.js"));*/

			// SHARED BUNDLES
			// Legacy common scripts - should be phased out
			bundles.Add(new ScriptBundle("~/bundles/VocaDB").Include("~/Scripts/VocaDB.js"));

			// Included on every page
			bundles.Add(new ScriptBundle("~/bundles/shared/common").Include(
				"~/Scripts/Shared/NamesList.js",
				"~/Scripts/Shared/GlobalSearchBox.js", 
				"~/Scripts/Shared/Messages.js",
				"~/Scripts/Shared/GlobalFunctions.js", 
				"~/Scripts/Shared/EntryUrlMapper.js",
				"~/Scripts/Shared/ReportEntryPopup.js",
				"~/Scripts/KnockoutExtensions/EntryToolTip.js"
			));

			// Included on all entry edit and create pages (album, artist, my settings etc.)
			bundles.Add(new ScriptBundle("~/bundles/shared/edit").Include(
				"~/Scripts/Models/WebLinkCategory.js",
				"~/Scripts/Shared/WebLinkMatcher.js",
				"~/Scripts/ViewModels/WebLinkEditViewModel.js",
				"~/Scripts/ViewModels/WebLinksEditViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/jqxRating").Include(
				"~/Scripts/jqwidgets27/jqxcore.js", "~/Scripts/jqwidgets27/jqxrating.js"));


			// VIEW-SPECIFIC BUNDLES
			bundles.Add(new ScriptBundle("~/bundles/Album/Details").Include("~/Scripts/Album/Details.js"));

			bundles.Add(new ScriptBundle("~/bundles/Album/Edit").Include(
				"~/Scripts/ViewModels/AlbumEditViewModel.js",
				"~/Scripts/Album/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Create").Include(
				"~/Scripts/KnockoutExtensions/FocusOut.js",
				"~/Scripts/Repositories/ArtistRepository.js",
				"~/Scripts/ViewModels/ArtistCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Details").Include("~/Scripts/Artist/Details.js"));

			bundles.Add(new ScriptBundle("~/bundles/Artist/Edit").Include(
				"~/Scripts/ViewModels/ArtistEditViewModel.js",
				"~/Scripts/Artist/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Home/Index").Include("~/Scripts/Home/Index.js"));

			bundles.Add(new ScriptBundle("~/bundles/Song/Create").Include(
				"~/Scripts/KnockoutExtensions/ArtistAutoComplete.js",
				"~/Scripts/KnockoutExtensions/FocusOut.js",
				"~/Scripts/Repositories/SongRepository.js",
				"~/Scripts/ViewModels/SongCreateViewModel.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/Song/Details").Include("~/Scripts/Song/Details.js"));

			bundles.Add(new ScriptBundle("~/bundles/Song/Edit").Include(
				"~/Scripts/ViewModels/SongEditViewModel.js",
				"~/Scripts/Song/Edit.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/User/Details").Include("~/Scripts/User/Details.js"));

			//bundles.Add(new ScriptBundle("~/bundles/User/MySettings").Include(
			//));


#if DEBUG
			bundles.Add(new ScriptBundle("~/bundles/tests")
				.IncludeDirectory("~/Scripts/Models", "*.js")
				.IncludeDirectory("~/Scripts/Repositories", "*.js")
				.IncludeDirectory("~/Scripts/ViewModels", "*.js")
				.Include("~/Scripts/Shared/WebLinkMatcher.js")
				.IncludeDirectory("~/Scripts/Tests", "*.js", true)
			);
#endif


			// Base CSS
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css", 
				"~/Content/Site.css", 
				"~/Content/Styles/ExtLinks.css", 
				"~/Content/Styles/Overrides.css"));

			bundles.Add(new StyleBundle("~/Content/embedSong").Include(
				"~/Content/bootstrap.css", "~/Content/EmbedSong.css"));

			// CSS for jqxRating
			bundles.Add(new StyleBundle("~/Scripts/jqwidgets27/styles/css").Include(
				"~/Scripts/jqwidgets27/styles/jqx.base.css"));

		}

	}

}