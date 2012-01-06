using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace VocaDb.Model.Service.VideoServices {

	public class YoutubeParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			var url = string.Format("youtu.be/{0}", id);

			string videoTitle = null;
			var request = WebRequest.Create(url);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("Youtube (error): " + x.Message);
			}

			try {
				var enc = response.Headers[HttpResponseHeader.ContentEncoding];

				using (var stream = response.GetResponseStream()) {
					videoTitle = GetVideoData(stream, enc);
				}
			} finally {
				response.Close();
			}

			if (!string.IsNullOrEmpty(videoTitle)) {
				return VideoTitleParseResult.CreateSuccess(videoTitle);
			} else {
				return VideoTitleParseResult.CreateError("Title element not found");
			}

		}

		private string GetVideoTitle(HtmlDocument doc) {

			var titleElem = doc.DocumentNode.SelectSingleNode("//span[@id = 'eow-title']");
			string titleText = null;

			if (titleElem != null)
				titleText = titleElem.GetAttributeValue("title", null);
			else {

				var verifyElem = doc.DocumentNode.SelectSingleNode("//meta[@name = 'title']");

				if (verifyElem != null)
					titleText = verifyElem.GetAttributeValue("content", null);

			}

			return HtmlEntity.DeEntitize(titleText);

		}

		private string GetVideoData(Stream htmlStream, string encodingStr) {

			var encoding = (!string.IsNullOrEmpty(encodingStr) ? Encoding.GetEncoding(encodingStr) : Encoding.UTF8);

			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			// Video title element (could use page title as well...)
			var titleText = GetVideoTitle(doc);

			if (string.IsNullOrEmpty(titleText))
				return null;

			var builder = new StringBuilder(titleText);

			var authorElem = doc.DocumentNode.SelectSingleNode("//a[@class = 'author']");
			var authorText = (authorElem != null ? authorElem.InnerText : null);

			if (!string.IsNullOrEmpty(authorText))
				builder.Append(" by " + authorText);

			var dateElem = doc.DocumentNode.SelectSingleNode("//span[@id = 'eow-date']");
			var dateText = (dateElem != null ? dateElem.InnerText : null);

			if (!string.IsNullOrEmpty(dateText))
				builder.Append(" at " + dateText);

			return builder.ToString();

		}


	}

}
