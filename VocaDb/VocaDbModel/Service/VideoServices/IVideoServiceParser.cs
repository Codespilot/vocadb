namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoServiceParser {

		VideoTitleParseResult GetTitle(string id);
	
	}

	public class VideoTitleParseResult {

		public static VideoTitleParseResult Empty {
			get {
				return new VideoTitleParseResult(true, null, null, null, null);
			}
		}

		public static VideoTitleParseResult CreateError(string error) {
			return new VideoTitleParseResult(false, error, null, null, null);
		}

		public static VideoTitleParseResult CreateSuccess(string title, string author, string thumbUrl) {
			return new VideoTitleParseResult(true, null, title, author, thumbUrl);
		}

		public VideoTitleParseResult(bool success, string error, string title, string author, string thumbUrl) {
			Error = error;
			Success = success;
			Title = title ?? string.Empty;
			Author = author ?? string.Empty;
			ThumbUrl = thumbUrl ?? string.Empty;
		}

		public string Author { get; set; }

		public string Error { get; set; }

		public bool Success { get; set; }

		public string Title { get; set; }

		public string ThumbUrl { get; set; }

	}

}
