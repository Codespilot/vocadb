namespace VocaDb.Model.Service.VideoServices {

	public interface IPVParser {

		/// <summary>
		/// Parses PV by URL.
		/// </summary>
		/// <param name="url">URL to be parsed. Cannot be null or empty.</param>
		/// <param name="getTitle">Whether to load metadata such as title and video author.</param>
		/// <returns>Result of PV parsing. Cannot be null.</returns>
		VideoUrlParseResult ParseByUrl(string url, bool getTitle);

	}

	public class PVParser : IPVParser {

		public VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			return VideoServiceHelper.ParseByUrl(url, getTitle);

		}

	}

}
