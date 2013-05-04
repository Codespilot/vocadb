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

		/// <summary>
		/// Display name of the user who uploaded the video/song.
		/// This does not need to be unique.
		/// Optional field.
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Identifier of the user who uploaded the video/song.
		/// For example on NND the users are identified by ID number
		/// instead of user name.
		/// This field can be used to uniquely identify the user.
		/// Optional field.
		/// </summary>
		public string AuthorId { get; set; }

		/// <summary>
		/// Error that occurred while parsing metadata.
		/// Null if there was no error.
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		/// Whether the operation was successful.
		/// If false, the error should be specified in the <see cref="Error"/> field.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Parsed title.
		/// Required field, cannot be null if the operation succeeded.
		/// </summary>
		public string Title { get; set; }

		public string ThumbUrl { get; set; }

	}

}
