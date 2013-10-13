using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;

namespace VocaDb.Web.Helpers {

	public static class SearchHelpers {

		public static string GetMatchModeFromQuery(string query, ref NameMatchMode matchMode) {

			if (matchMode == NameMatchMode.Auto && query != null && query.Length <= 2) {
				matchMode = NameMatchMode.StartsWith;
				return query;
			}

			if (matchMode == NameMatchMode.Auto && query != null && query.Length > 1 && query.EndsWith("*")) {
				matchMode = NameMatchMode.StartsWith;
				return query.Substring(0, query.Length - 1);
			}

			return query;

		}

		public static EntryType GlobalSearchObjectType<TModel>(this HtmlHelper<TModel> htmlHelper) {

			if (htmlHelper.ViewData.ContainsKey("GlobalSearchObjectType")) {
				return (EntryType)htmlHelper.ViewData["GlobalSearchObjectType"];
			}

			return EntryType.Undefined;

		}

		public static string GlobalSearchTerm<TModel>(this HtmlHelper<TModel> htmlHelper) {

			if (htmlHelper.ViewData.ContainsKey("GlobalSearchTerm")) {
				return (string)htmlHelper.ViewData["GlobalSearchTerm"];
			}

			return string.Empty;

		}

	}
}