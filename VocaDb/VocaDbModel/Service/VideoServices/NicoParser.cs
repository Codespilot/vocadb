using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoParser : IVideoServiceParser {

		private static Encoding GetEncoding(string encodingStr) {

			if (string.IsNullOrEmpty(encodingStr))
				return Encoding.UTF8;

			try {
				return Encoding.GetEncoding(encodingStr);
			} catch (ArgumentException) {
				return Encoding.UTF8;
			}

		}

		public VideoTitleParseResult GetTitle(string id) {

			var url = string.Format("http://nicovideo.jp/watch/{0}", id);

			string videoTitle = null;
			var request = WebRequest.Create(url);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
			}

			var enc = GetEncoding(response.Headers[HttpResponseHeader.ContentEncoding]);

			try {
				using (var stream = response.GetResponseStream()) {
					videoTitle = GetVideoTitle(stream, enc);
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

		private string GetVideoTitle(Stream htmlStream, Encoding encoding) {

			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			// Video title element (could use page title as well but this is more reliable)
			var titleElem = doc.DocumentNode.SelectSingleNode("//div[@id = 'PAGEBODY']/div/div/div/div/h1");
			//var titleElem = doc.DocumentNode.SelectSingleNode("//p[@class = 'video_title']");

			var titleText = (titleElem != null ? titleElem.InnerText : null);

			return (titleText != null ? HtmlEntity.DeEntitize(titleText) : null);

		}

	}
}
