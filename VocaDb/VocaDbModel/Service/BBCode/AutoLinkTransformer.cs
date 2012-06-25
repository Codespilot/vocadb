using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.BBCode {

	public class AutoLinkTransformer : IBBCodeElementTransformer {

		private static readonly Regex regex = new Regex(@"http[s]?\:[a-zA-Z0-9_\#\-\.\:\/\%\?\&\=\+\(\)]+");

		public static string ReplaceUrisWithLinks(string text) {

			var parsed = new StringBuilder(text);

			var matches = regex.Matches(text);

			var indexOffset = 0;

			foreach (Match match in matches) {

				var link = GetLink(match.Value);

				if (link != match.Value) {

					parsed.Replace(match.Value, link, match.Index + indexOffset, match.Length);

					indexOffset += (link.Length - match.Value.Length);

				}
			}

			return parsed.ToString();

		}

		public static string GetLink(string text) {

			Uri uri;

			if (Uri.TryCreate(text, UriKind.Absolute, out uri)) {
				return "<a href='" + text + "'>" + text + "</a>";
			} else {
				return text;
			}


		}

		public void ApplyTransform(StringBuilder bbCode) {
			
			BBCodeConverter.RegexReplace(bbCode, regex, GetLink);

		}

	}

}
