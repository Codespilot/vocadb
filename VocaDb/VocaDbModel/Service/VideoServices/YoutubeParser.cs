using System;
using System.IO;
using System.Net;
using System.Text;
using Google.YouTube;
using HtmlAgilityPack;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			var settings = new YouTubeRequestSettings("MikuBot", null);
			var request = new YouTubeRequest(settings);
			var videoEntryUrl = new Uri(string.Format("http://gdata.youtube.com/feeds/api/videos/{0}", id));

			try {
				var video = request.Retrieve<Video>(videoEntryUrl);
				return VideoTitleParseResult.CreateSuccess(video.Title);
			} catch (Exception x) {
				return VideoTitleParseResult.CreateError(x.Message);
			}

		}

	}

}
