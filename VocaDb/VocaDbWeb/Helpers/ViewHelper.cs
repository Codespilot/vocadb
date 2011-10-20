using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using VocaDb.Model;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using System.Web;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Helpers {

	public static class ViewHelper {

		private static Dictionary<ContentLanguageSelection, string> LanguageSelections {
			get {

				return EnumVal<ContentLanguageSelection>.Values
					.ToDictionary(l => l, Translate.ContentLanguageSelectionName);

			}

		}

		private static Dictionary<ContentLanguagePreference, string> LanguagePreferences {
			get {

				return EnumVal<ContentLanguagePreference>.Values
					.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);

			}
		}

		public static SelectList LanguagePreferenceList {
			get {
				return new SelectList(LanguagePreferences, "Key", "Value");
			}
		}

		public static SelectList LanguageSelectionList {
			get {
				return new SelectList(LanguageSelections, "Key", "Value");
			}
		}

		public static MvcHtmlString LanguagePreferenceDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguagePreference>> expression) {

			return htmlHelper.DropDownListFor(expression, LanguagePreferenceList);

		}

		public static MvcHtmlString LanguageSelectionDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguageSelection>> expression) {

			return htmlHelper.DropDownListFor(expression, LanguageSelectionList);

		}

		public static MvcHtmlString LanguageSelectionDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguageSelection>> expression, object htmlAttributes) {

			return htmlHelper.DropDownListFor(expression, LanguageSelectionList, htmlAttributes);

		}

		public static MvcHtmlString LanguageSelectionDropDownList(this HtmlHelper htmlHelper, string name, object htmlAttributes) {

			return htmlHelper.DropDownList(name, LanguageSelectionList, htmlAttributes);

		}

		public static MvcHtmlString LinkList<T>(this HtmlHelper htmlHelper, IEnumerable<T> list, Func<T, MvcHtmlString> linkFunc) {

			return StringHelper.Join(", ", list.Select(linkFunc));

		}

		public static string VideoServiceLinkUrl(this HtmlHelper htmlHelper, PVService service) {

			switch (service) {
				case PVService.Youtube:
					return UrlHelper.GenerateContentUrl("~/Content/youtube.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.NicoNicoDouga:
					return UrlHelper.GenerateContentUrl("~/Content/nico.png", new HttpContextWrapper(HttpContext.Current));
				default:
					return string.Empty;
			}

		}

		/*public static MvcHtmlString ValidationSymmaryPanel(this HtmlHelper htmlHelper, string message) {

			if (!HttpContext.Current.ViewData.ModelState.IsValid) {
			<div class="ui-widget">
				<div class="ui-state-error ui-corner-all" style="padding: 0 .7em;">
					@Html.ValidationSummary(false, "Unable to save properties.")
				</div>
			</div>
			} else {
				return null;
			}

		}*/

	}

}