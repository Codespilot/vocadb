using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using Microsoft.Web.Helpers;
using VocaDb.Model;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using System.Web;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Helpers {

	public static class ViewHelper {

		private static Dictionary<ContentLanguageSelection, string> LanguageSelections {
			get {

				return EnumVal<ContentLanguageSelection>.Values
					.ToDictionary(l => l, Translate.ContentLanguageSelectionName);

			}

		}

		private static Dictionary<ContentLanguageSelection, string> LanguageSelectionsWithoutUnspecified {
			get {

				return EnumVal<ContentLanguageSelection>.Values.Where(l => l != ContentLanguageSelection.Unspecified)
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

		public static SelectList LanguageSelectionListWithoutUnspecified {
			get {
				return new SelectList(LanguageSelectionsWithoutUnspecified, "Key", "Value");
			}
		}

		public static SelectList CreateArtistTypesList(object selectedValue) {
			return new SelectList(EnumVal<ArtistType>.Values.ToDictionary(s => s, Translate.ArtistTypeName), "Key", "Value", selectedValue);
		}

		public static SelectList CreateDiscTypesList(object selectedValue) {
			return new SelectList(EnumVal<DiscType>.Values.ToDictionary(s => s, Translate.DiscTypeName), "Key", "Value", selectedValue);
		}

		public static SelectList CreateEmailOptionsList(object selectedValue) {
			return new SelectList(EnumVal<UserEmailOptions>.Values.ToDictionary(s => s, Translate.EmailOptions), "Key", "Value", selectedValue);
		}

		public static SelectList CreateLanguageSelectionList(object selectedValue) {
			return new SelectList(LanguageSelections, "Key", "Value", selectedValue);
		}

		public static SelectList CreateLanguageSelectionListWithoutUnspecified(object selectedValue) {
			return new SelectList(LanguageSelectionsWithoutUnspecified, "Key", "Value", selectedValue);
		}

		public static SelectList CreateSongTypesList(object selectedValue) {
			return new SelectList(EnumVal<SongType>.Values.ToDictionary(s => s, s => s.ToString()), "Key", "Value", selectedValue);
		}

		public static MvcHtmlString ArtistTypeDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ArtistType>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateArtistTypesList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString DiscTypeDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, DiscType>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateDiscTypesList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString EmailOptionsDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, UserEmailOptions>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateEmailOptionsList(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString Image(this HtmlHelper htmlHelper, string src, string alt) {

			return new MvcHtmlString(string.Format("<img src='{0}' alt='{1}' />", src, alt));

		}

		public static MvcHtmlString LanguagePreferenceDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguagePreference>> expression) {

			return htmlHelper.DropDownListFor(expression, LanguagePreferenceList);

		}

		public static MvcHtmlString LanguageSelectionDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, ContentLanguageSelection>> expression, object htmlAttributes = null, bool allowUnspecified = false, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, allowUnspecified ? CreateLanguageSelectionList(selectedValue) : CreateLanguageSelectionListWithoutUnspecified(selectedValue), htmlAttributes);

		}

		public static MvcHtmlString LanguageSelectionDropDownList(this HtmlHelper htmlHelper, string name, object htmlAttributes, bool allowUnspecified) {

			return htmlHelper.DropDownList(name, allowUnspecified ? LanguageSelectionList : LanguageSelectionListWithoutUnspecified, htmlAttributes);

		}

		public static MvcHtmlString LinkList<T>(this HtmlHelper htmlHelper, IEnumerable<T> list, Func<T, MvcHtmlString> linkFunc) {

			return StringHelper.Join(", ", list.Select(linkFunc));

		}

		public static MvcHtmlString SongTypeDropDownListFor<TModel>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, SongType>> expression, object htmlAttributes = null, object selectedValue = null) {

			return htmlHelper.DropDownListFor(expression, CreateSongTypesList(selectedValue), htmlAttributes);

		}

		public static HtmlString ProfileIcon(this HtmlHelper htmlHelper, UserContract user, int size = 80) {

			if (user != null && !string.IsNullOrEmpty(user.Email))
				return Gravatar.GetHtml(user.Email, size);
			else
				return new HtmlString(string.Empty);

		}

		public static MvcHtmlString PVServiceIcons(this HtmlHelper htmlHelper, PVServices services) {

			var sb = new StringBuilder();

			if (services.HasFlag(PVServices.NicoNicoDouga))
				sb.Append(Image(htmlHelper, VideoServiceLinkUrl(htmlHelper, PVService.NicoNicoDouga), "NicoNicoDouga"));

			if (services.HasFlag(PVServices.Youtube))
				sb.Append(Image(htmlHelper, VideoServiceLinkUrl(htmlHelper, PVService.Youtube), "Youtube"));

			return new MvcHtmlString(sb.ToString());

		}

		public static MvcHtmlString UserLink(this HtmlHelper htmlHelper, UserContract user, string name) {

			return (user != null ? htmlHelper.ActionLink(user.Name, "Profile", "User", new { id = user.Name }, null) : MvcHtmlString.Create(name));

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

		public static void SetStatusMessage(this TempDataDictionary temp, string val) {
			temp["StatusMessage"] = val;
		}

		public static string StatusMessage(this TempDataDictionary temp) {

			var msg = temp["StatusMessage"];
			return (msg != null ? msg.ToString() : null);

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