namespace VocaDb.Model.Service.VideoServices {

	public interface IPVParser {

		VideoUrlParseResult ParseByUrl(string url, bool getTitle);

	}

	public class PVParser : IPVParser {

		public VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			return VideoServiceHelper.ParseByUrl(url, getTitle);

		}

	}

}
