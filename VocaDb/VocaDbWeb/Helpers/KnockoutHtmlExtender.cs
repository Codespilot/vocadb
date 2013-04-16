using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using VocaDb.Model;

namespace VocaDb.Web.Helpers {

	/// <summary>
	/// Extends <see cref="HtmlHelper"/> with Knockout-specific methods.
	/// </summary>
	public static class KnockoutHtmlExtender {

		/// <summary>
		/// Text box bound to knockout model.
		/// </summary>
		/// <typeparam name="TModel">Model type.</typeparam>
		/// <typeparam name="TProperty">Property expression type.</typeparam>
		/// <param name="htmlHelper">HTML helper. Cannot be null.</param>
		/// <param name="expression">Property access expression. Cannot be null.</param>
		/// <param name="binding">Knockout data-bind attribute. Cannot be null.</param>
		/// <param name="cssClass">CSS class attribute. Can be null or empty, in which case this attribute is not specified.</param>
		/// <param name="id">ID attribute. Can be null or empty, in which case this attribute is not specified.</param>
		/// <param name="maxLength">Max length attribute. Can be null, in which case this attribute is not specified.</param>
		/// <param name="size">Size attribute. Can be null, in which case this attribute is not specified.</param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxForKnockout<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, 
			string binding, string cssClass = null, string id = null, int? maxLength = null, int? size = null) {

			ParamIs.NotNull(() => htmlHelper);
			ParamIs.NotNull(() => expression);
			ParamIs.NotNull(() => binding);

			var htmlAttributes = new Dictionary<string, object> { { "data-bind", binding } };

			if (!string.IsNullOrEmpty(cssClass))
				htmlAttributes.Add("class", cssClass);

			if (!string.IsNullOrEmpty(id))
				htmlAttributes.Add("id", id);

			if (maxLength.HasValue)
				htmlAttributes.Add("maxlength", maxLength.Value);

			if (size.HasValue)
				htmlAttributes.Add("size", size.Value);

			return htmlHelper.TextBoxFor(expression, htmlAttributes);

		}

	}

}