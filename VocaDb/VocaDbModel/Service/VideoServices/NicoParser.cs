using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			return NicoHelper.GetTitleAPI(id);

		}

	}

	public static class NicoHelper {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static string GetUserName(Stream htmlStream, Encoding encoding) {

			var doc = new HtmlDocument();
			try {
				doc.Load(htmlStream, encoding);
			} catch (IOException x) {
				log.WarnException("Unable to load document for user name", x);
			}

			var titleElem = doc.DocumentNode.SelectSingleNode("//html/body/div/p[2]/a/strong");

			var titleText = (titleElem != null ? titleElem.InnerText : null);

			return (titleText != null ? HtmlEntity.DeEntitize(titleText) : null);

		}

		public static string GetUserName(string userId) {

			var url = string.Format("http://ext.nicovideo.jp/thumb_user/{0}", userId);

			var request = WebRequest.Create(url);
			request.Timeout = 10000;
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				log.WarnException("Unable to get response for user name", x);
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
			request.Timeout = 10000;

			XDocument doc;

			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {
					doc = XDocument.Load(stream);
				}
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("NicoVideo (error): " + x.Message);
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

			var title = HtmlEntity.DeEntitize(titleElem.Value);
			var thumbUrl = XmlHelper.GetNodeTextOrEmpty(doc, "//nicovideo_thumb_response/thumb/thumbnail_url");
			var userId = XmlHelper.GetNodeTextOrEmpty(doc, "//nicovideo_thumb_response/thumb/user_id");
			var author = GetUserName(userId);

			var result = VideoTitleParseResult.CreateSuccess(title, author, thumbUrl);
			result.AuthorId = userId;
			return result;

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

		/// <summary>
		/// Parses song title, artists and song type from NicoNico video title.
		/// </summary>
		/// <param name="title">NicoNico video title. Can be null or empty, in which case that value is returned.</param>
		/// <returns>Parse result. Cannot be null.</returns>
		/// <remarks>This works with titles that follow the common format, for example 【重音テト】 ハイゲインワンダーランド 【オリジナル】.</remarks>
		public static NicoTitleParseResult ParseTitle(string title, Func<string, Artist> artistFunc) {

			if (string.IsNullOrEmpty(title))
				return new NicoTitleParseResult(title);

			var elemRegex = new Regex(@"【\s?([\w･]+)\s?】");
			var matches = elemRegex.Matches(title);
			Artist artist = null;
			var songType = SongType.Unspecified;
			int offset = 0;

			if (matches.Count == 0)
				return new NicoTitleParseResult(title);

			foreach (Match match in matches) {

				var content = match.Groups[1].Value;
				if (content == "オリジナル")
					songType = SongType.Original;
				else {
					var a = artistFunc(content.Trim());
					artist = artist ?? a;
				}

				title = title.Remove(match.Index - offset, match.Value.Length);
				offset += match.Length;

			}

			return new NicoTitleParseResult(title.Trim(), new List<Artist> { artist }, songType);

		}

	}

}
