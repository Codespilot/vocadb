namespace VocaDb.Model.Service.VideoServices {

	public interface IVideoServiceParser {

		VideoTitleParseResult GetTitle(string id);
	
	}

	public class VideoTitleParseResult {

		public static VideoTitleParseResult CreateError(string error) {
			return new VideoTitleParseResult(false, error, null);
		}

		public static VideoTitleParseResult CreateSuccess(string title) {
			return new VideoTitleParseResult(true, null, title);
		}

		public VideoTitleParseResult(bool success, string error, string title) {
			Error = error;
			Success = success;
			Title = title;
		}

		public string Error { get; set; }

		public bool Success { get; set; }

		public string Title { get; set; }

	}

}
