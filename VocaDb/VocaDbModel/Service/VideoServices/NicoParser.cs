using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			return NicoHelper.GetTitleAPI(id);

		}

	}

	public static class NicoHelper {

		private static string GetUserName(Stream htmlStream, Encoding encoding) {

			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			var titleElem = doc.DocumentNode.SelectSingleNode("//html/body/div/p[2]/a/strong");

			var titleText = (titleElem != null ? titleElem.InnerText : null);

			return (titleText != null ? HtmlEntity.DeEntitize(titleText) : null);

		}

		public static string GetUserName(string userId) {

			var url = string.Format("http://ext.nicovideo.jp/thumb_user/{0}", userId);

			var request = WebRequest.Create(url);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				return null;
			}

			var enc = GetEncoding(response.Headers[HttpResponseHeader.ContentEncoding]);

			try {
				using (var stream = response.GetResponseStream()) {
					return GetUserName(stream, enc);
				}
			} finally {
				response.Close();
			}

		}

		public static VideoTitleParseResult GetTitleAPI(string id) {

			var url = string.Format("http://ext.nicovideo.jp/api/getthumbinfo/{0}", id);

			var request = WebRequest.Create(url);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
			}

			XDocument doc;

			try {
				doc = XDocument.Load(response.GetResponseStream());
			} catch (XmlException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
			}

			var res = doc.Element("nicovideo_thumb_response");

			if (res == null || res.Attribute("status") == null || res.Attribute("status").Value == "fail") {
				var err = (res != null ? res.XPathSelectElement("//nicovideo_thumb_response/error/description").Value : "empty response");
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + err);
			}

			var titleElem = doc.XPathSelectElement("//nicovideo_thumb_response/thumb/title");

			if (titleElem == null) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): title element not found");
			}

			var thumbUrl = XmlHelper.GetNodeTextOrEmpty(doc, "//nicovideo_thumb_response/thumb/thumbnail_url");
			var userId = XmlHelper.GetNodeTextOrEmpty(doc, "//nicovideo_thumb_response/thumb/user_id");
			var author = GetUserName(userId);

			return VideoTitleParseResult.CreateSuccess(titleElem.Value, author, thumbUrl);

		}

		public static VideoTitleParseResult GetTitleHtml(string id) {

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
				return VideoTitleParseResult.CreateSuccess(videoTitle, null, null);
			} else {
				return VideoTitleParseResult.CreateError("Title element not found");
			}

		}

		private static Encoding GetEncoding(string encodingStr) {

			if (string.IsNullOrEmpty(encodingStr))
				return Encoding.UTF8;

			try {
				return Encoding.GetEncoding(encodingStr);
			} catch (ArgumentException) {
				return Encoding.UTF8;
			}

		}

		private static string GetVideoTitle(Stream htmlStream, Encoding encoding) {

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
