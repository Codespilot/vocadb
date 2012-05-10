using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Helpers {
	public static class SearchHelpers {

		public static EntryType GlobalSearchObjectType<TModel>(this HtmlHelper<TModel> htmlHelper) {

			if (htmlHelper.ViewData.ContainsKey("GlobalSearchObjectType")) {
				return (EntryType)htmlHelper.ViewData["GlobalSearchObjectType"];
			}

			return EntryType.Artist;

		}

		public static string GlobalSearchTerm<TModel>(this HtmlHelper<TModel> htmlHelper) {

			if (htmlHelper.ViewData.ContainsKey("GlobalSearchTerm")) {
				return (string)htmlHelper.ViewData["GlobalSearchTerm"];
			}

			return string.Empty;

		}
	}
}