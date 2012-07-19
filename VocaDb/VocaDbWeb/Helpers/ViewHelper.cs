using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using Microsoft.Web.Helpers;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using System.Web;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Service.BBCode;
using VocaDb.Web.Code.BBCode;
using VocaDb.Web.Helpers.Support;

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

		public static SelectList CreateEnumList<T>(object selectedValue, TranslateableEnum<T> enumType) where T : struct, IConvertible {
			return CreateEnumList(selectedValue, enumType.ValuesAndNames);
		}

		public static SelectList CreateEnumList<T>(object selectedValue, IEnumerable<KeyValuePair<T, string>> vals) where T : struct, IConvertible {
			return new SelectList(vals, "Key", "Value", selectedValue);
		}

		public static SelectList CreateLanguageSelectionList(object selectedValue) {
			return new SelectList(LanguageSelections, "Key", "Value", selectedValue);
		}

		public static SelectList CreateLanguageSelectionListWithoutUnspecified(object selectedValue) {
			return new SelectList(LanguageSelectionsWithoutUnspecified, "Key", "Value", selectedValue);
		}

		public static SelectList CreateSongTypesList(object selectedValue) {
			return new SelectList(EnumVal<SongType>.Values.ToDictionary(s => s, Translate.SongTypeNames.GetName), "Key", "Value", selectedValue);
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

		public static MvcHtmlString Encode(this HtmlHelper htmlHelper, string str) {
			return new MvcHtmlString(HttpUtility.HtmlEncode(str));
		}

		public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name,
			TranslateableEnum<TEnum> enumType, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownList(name, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, object htmlAttributes = null, object selectedValue = null) 
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownListForDic<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			TranslateableEnum<TEnum> enumType, IDictionary<string, object> htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, enumType), htmlAttributes);

		}

		public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TEnum>> expression,
			IEnumerable<KeyValuePair<TEnum, string>> values, object htmlAttributes = null, object selectedValue = null)
			where TEnum : struct, IConvertible {

			return htmlHelper.DropDownListFor(expression, CreateEnumList(selectedValue, values), htmlAttributes);

		}

		public static int GetComparedEntryId(ArchivedObjectVersionContract archivedVersion, int comparedEntryId, 
			IEnumerable<ArchivedObjectVersionContract> allVersions) {

			if (comparedEntryId != 0)
				return comparedEntryId;

			var nextVersion = allVersions.FirstOrDefault(v => v.Version == archivedVersion.Version - 1);

			return (nextVersion ?? archivedVersion).Id;

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

		public static string ParseBBCode(string bbCode) {

			return new BBCodeCache(BBCodeConverters.Default()).GetHtml(bbCode);

		}

		[Obsolete]
		public static HtmlString ProfileIcon(this HtmlHelper htmlHelper, UserBaseContract user, int size = 80) {

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

		public static MvcHtmlString UserLink(this HtmlHelper htmlHelper, UserBaseContract user, string name) {

			return (user != null ? htmlHelper.ActionLink(user.Name, "Profile", "User", new { id = user.Name }, null) 
				: Encode(htmlHelper, name));

		}

		public static string VideoServiceLinkUrl(this HtmlHelper htmlHelper, PVService service) {

			switch (service) {
				case PVService.NicoNicoDouga:
					return UrlHelper.GenerateContentUrl("~/Content/nico.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.SoundCloud:
					return UrlHelper.GenerateContentUrl("~/Content/Icons/soundcloud.png", new HttpContextWrapper(HttpContext.Current));
				case PVService.Youtube:
					return UrlHelper.GenerateContentUrl("~/Content/youtube.png", new HttpContextWrapper(HttpContext.Current));
				default:
					return string.Empty;
			}

		}

		public static void RenderPartialTyped<T>(this HtmlHelper htmlHelper, string partialViewName, T model) {

			htmlHelper.RenderPartial(partialViewName, model);

		}

		public static string ReplaceUrisWithLinks(string text) {

			return AutoLinkTransformer.ReplaceUrisWithLinks(text);

		}

	}

}