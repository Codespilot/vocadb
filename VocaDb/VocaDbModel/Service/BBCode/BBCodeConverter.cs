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

		public static string RegexReplace(string bbCode, string pattern, string replacement) {

			return Regex.Replace(bbCode, pattern, replacement, RegexOptions.Multiline | RegexOptions.CultureInvariant);

		}

		public static string ReplaceSimpleTag(string bbCode, string bbTagName, string htmlTagName) {

			return RegexReplace(bbCode, string.Format(@"\[{0}\]([^\]]+)\[\/{0}\]", bbTagName), string.Format("<{0}>$1</{0}>", htmlTagName));

		}

		public BBCodeConverter(IEnumerable<IBBCodeElementTransformer> transformers) {

			this.transformers = transformers.ToArray();

		}

		public string ConvertToHtml(string bbCode) {

			foreach (var transformer in transformers)
				bbCode = transformer.ApplyTransform(bbCode);

			return bbCode;

		}

	}

}
