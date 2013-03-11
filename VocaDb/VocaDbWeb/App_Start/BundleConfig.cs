using System.Web.Optimization;

namespace VocaDb.Web.App_Start {

	public static class BundleConfig {

		public static void RegisterBundles(BundleCollection bundles) {

			// jQuery styles don't work if bundled
			/*bundles.Add(new ScriptBundle("~/bundles/commonLibs").Include(
				"~/Scripts/jquery-1.8.2.min.js", "~/Scripts/bootstrap.min.js",
				"~/Scripts/jquery-ui-1.10.1.custom.min.js", "~/Scripts/knockout-2.2.0.js",
				"~/Scripts/underscore.min.js", "~/Scripts/jquery.qtip.min.js"));*/

			bundles.Add(new ScriptBundle("~/bundles/VocaDB").Include("~/Scripts/VocaDB.js"));

			bundles.Add(new ScriptBundle("~/bundles/shared").Include(
				"~/Scripts/Shared/NamesList.js", "~/Scripts/Shared/WebLinksList.js", 
				"~/Scripts/Shared/GlobalSearchBox.js", "~/Scripts/Shared/Messages.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqxRating").Include(
				"~/Scripts/jqwidgets27/jqxcore.js", "~/Scripts/jqwidgets27/jqxrating.js"));

			bundles.Add(new ScriptBundle("~/bundles/Shared/ReportEntryPopup").Include("~/Scripts/Shared/ReportEntryPopup.js"));
			bundles.Add(new ScriptBundle("~/bundles/Album/Details").Include("~/Scripts/Album/Details.js"));
			bundles.Add(new ScriptBundle("~/bundles/Artist/Details").Include("~/Scripts/Artist/Details.js"));
			bundles.Add(new ScriptBundle("~/bundles/Song/Details").Include("~/Scripts/Song/Details.js"));
			bundles.Add(new ScriptBundle("~/bundles/User/Details").Include("~/Scripts/User/Details.js"));


			// Base CSS
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/bootstrap.css", "~/Content/Site.css"));

			// CSS for jqxRating
			bundles.Add(new StyleBundle("~/Scripts/jqwidgets27/styles/css").Include(
				"~/Scripts/jqwidgets27/styles/jqx.base.css"));

		}

	}

}