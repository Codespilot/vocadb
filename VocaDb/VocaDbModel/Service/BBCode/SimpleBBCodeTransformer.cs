using System;

namespace VocaDb.Model.Service.BBCode {

	public class SimpleBBCodeTransformer : IBBCodeElementTransformer {

		public static string ReplaceSimpleTag(string bbCode, string bbTagName, string htmlTagName) {

			return BBCodeConverter.RegexReplace(bbCode, String.Format(@"\[{0}\]([^\]]+)\[\/{0}\]", bbTagName), String.Format("<{0}>$1</{0}>", htmlTagName));

		}

		private readonly string bbTagName;
		private readonly string htmlTagName;

		public SimpleBBCodeTransformer(string bbTagName, string htmlTagName) {
			this.bbTagName = bbTagName;
			this.htmlTagName = htmlTagName;
		}

		public string ApplyTransform(string bbCode) {

			return ReplaceSimpleTag(bbCode, bbTagName, htmlTagName);

		}

	}
}
