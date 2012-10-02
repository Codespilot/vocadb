using System;
using Google.YouTube;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			var settings = new YouTubeRequestSettings("VocaDB", null);
			var request = new YouTubeRequest(settings);
			var videoEntryUrl = new Uri(string.Format("http://gdata.youtube.com/feeds/api/videos/{0}", id));

			try {
				var video = request.Retrieve<Video>(videoEntryUrl);
				var thumbUrl = video.Thumbnails.Count > 0 ? video.Thumbnails[0].Url : string.Empty;
				return VideoTitleParseResult.CreateSuccess(video.Title, video.Author, thumbUrl);
			} catch (Exception x) {
				return VideoTitleParseResult.CreateError(x.Message);
			}

		}

	}

}
