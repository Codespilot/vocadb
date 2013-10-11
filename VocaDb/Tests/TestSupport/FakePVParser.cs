using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.TestSupport {

	public class FakePVParser : IPVParser {

		public VideoUrlParseResult Result { get; set; }

		public VideoUrlParseResult ParseByUrl(string url, bool getTitle) {
			Result.Url = url;
			return Result;
		}

	}

}
