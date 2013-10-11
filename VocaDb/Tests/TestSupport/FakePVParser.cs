using System;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.TestSupport {

	public class FakePVParser : IPVParser {

		public Func<string, VideoUrlParseResult> ResultFunc { get; set; }

		public VideoUrlParseResult ParseByUrl(string url, bool getTitle) {
			return ResultFunc(url);
		}

	}

}
