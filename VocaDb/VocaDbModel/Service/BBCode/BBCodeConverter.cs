using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.BBCode {

	/// <summary>
	/// Converts BBCode into HTML
	/// </summary>
	public class BBCodeConverter {

		private readonly IBBCodeElementTransformer[] transformers;

		public static void RegexReplace(StringBuilder bbCode, Regex regex, Func<string, string> replacementFunc) {

			var matches = regex.Matches(bbCode.ToString());

			var indexOffset = 0;

			foreach (Match match in matches) {

				var result = replacementFunc(match.Value);

				if (result != match.Value) {
					bbCode.Replace(match.Value, result, match.Index + indexOffset, match.Length);
					indexOffset += (result.Length - match.Value.Length);
				}

			}

		}


		public BBCodeConverter(IEnumerable<IBBCodeElementTransformer> transformers) {

			ParamIs.NotNull(() => transformers);

			this.transformers = transformers.ToArray();

		}

		public string ConvertToHtml(string bbCode) {

			var replaced = new StringBuilder(bbCode);

			foreach (var transformer in transformers)
				transformer.ApplyTransform(replaced);

			return replaced.ToString();

		}
	}

}
