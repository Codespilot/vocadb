﻿using System.Web;
using MarkdownSharp;

namespace VocaDb.Web.Helpers {

	public static class MarkdownHelper {

		/// <summary>
		/// Transforms a block of text with Markdown. The input will be sanitized.
		/// </summary>
		/// <param name="text">Text to be transformed. All HTML will be encoded!</param>
		/// <returns>Markdown-transformed text. This will include HTML.</returns>
		public static string TranformMarkdown(string text) {

			if (string.IsNullOrEmpty(text))
				return text;

			// StrictBoldItalic is needed because otherwise links with underscores won't work (links are more common on VDB).
			return new Markdown(new MarkdownOptions { AutoHyperlink = true, AutoNewLines = true, StrictBoldItalic = true }).Transform(HttpUtility.HtmlEncode(text));

		}

	}

}