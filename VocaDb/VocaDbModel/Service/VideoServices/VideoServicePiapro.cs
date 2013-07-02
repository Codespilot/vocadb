using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServicePiapro : VideoService {

		private int? GetLength(HtmlNode dataElem) {

			if (dataElem == null)
				return null;

			var lengthMatch = Regex.Match(dataElem.InnerHtml, @"タイム／サイズ.+(\d\d:\d\d)");

			if (!lengthMatch.Success)
				return null;

			return NicoHelper.ParseLength(lengthMatch.Groups[1].Value);

		}

		public VideoServicePiapro(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		private static Encoding GetEncoding(string encodingStr) {

			// New piapro uses UTF-8
			return HtmlParsingHelper.GetEncoding(encodingStr, Encoding.UTF8);

			/*var shift_jis = Encoding.GetEncoding(932);

			if (string.IsNullOrEmpty(encodingStr))
				return shift_jis;

			try {
				return Encoding.GetEncoding(encodingStr);
			} catch (ArgumentException) {
				return shift_jis;
			}*/

		}

		/*public override string GetUrlById(string id) {

			var compositeId = new CompositeId(id);
			var matcher = linkMatchers.First();
			return "http://" + matcher.MakeLinkFromId(compositeId.SoundCloudUrl);

		}*/

		private VideoUrlParseResult ParseByHtmlStream(Stream htmlStream, Encoding encoding, string url) {

			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			var dataElem = doc.DocumentNode.SelectSingleNode("//div[@class = 'dtl_data']");

			if (dataElem == null || !dataElem.InnerHtml.Contains("/music/"))
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Content type indicates this isn't an audio file.");

			var length = GetLength(dataElem);

			var idElem = doc.DocumentNode.SelectSingleNode("//input[@name = 'id']");

			if (idElem == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Could not find id element on page.");

			var contentId = idElem.GetAttributeValue("value", string.Empty);

			var titleElem = doc.DocumentNode.SelectSingleNode("//h1[@class = 'dtl_title']");

			if (titleElem == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Could not find title element on page.");

			var title = HtmlEntity.DeEntitize(titleElem.InnerText).Trim();

			var authorElem = doc.DocumentNode.SelectSingleNode("//div[@class = 'dtl_by_name']/a");
			var author = (authorElem != null ? authorElem.InnerText : string.Empty);

			return VideoUrlParseResult.CreateOk(url, PVService.Piapro, contentId, VideoTitleParseResult.CreateSuccess(title, author, string.Empty, length));

		}

		// Old Piapro
		/*private VideoUrlParseResult ParseByHtmlStream(Stream htmlStream, Encoding encoding, string url) {

			//var commentRegex = new Regex(@"<!-- (\w+) -->");
			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			var imageMeta = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");

			if (imageMeta != null && imageMeta.Attributes["content"] != null && imageMeta.Attributes["content"].Value != "/modpub/images/img_snd_list.gif") {
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Content type indicates this isn't an audio file.");
			}

			var comment = doc.DocumentNode.SelectSingleNode("//div[@id = 'content']/comment()[1]");

			if (comment == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Could not find content ID on page (no comment element).");

			var idMatch = commentRegex.Match(comment.InnerText);

			if (!idMatch.Success)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Could not find content ID on page (comment element didn't match).");

			var contentId = idMatch.Groups[1].Value;

			var nameSpan = doc.DocumentNode.SelectSingleNode("//div[@id = 'content']/div[1]/div[@class='worktitlebox']/div[@class='title']/span[2]");

			if (nameSpan == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Could not find title element on page.");

			var title = nameSpan.InnerText;

			var authorElem = doc.DocumentNode.SelectSingleNode("//div[@id = 'localmenu']/div[@class='info']/div[1]/div[1]/a");
			var author = (authorElem != null ? authorElem.InnerText : string.Empty);
			//var pageId = linkMatchers[0].GetId(url);
			//var compositeId = new CompositeId(id, pageId);

			if (author.EndsWith("さん"))
				author = author.Remove(author.Length - 2);

			return VideoUrlParseResult.CreateOk(url, PVService.Piapro, contentId, VideoTitleParseResult.CreateSuccess(title, author, string.Empty));

		}*/

		private VideoUrlParseResult ParseByPiaproUrl(string piaproUrl) {

			WebRequest request;
			try {
				request = WebRequest.Create(UrlHelper.MakeLink(piaproUrl));
			} catch (UriFormatException x) {
				return VideoUrlParseResult.CreateError(piaproUrl, VideoUrlParseResultType.LoadError, x.Message);				
			}

			request.Timeout = 10000;
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				return VideoUrlParseResult.CreateError(piaproUrl, VideoUrlParseResultType.LoadError, x.Message);
			}

			var enc = GetEncoding(response.Headers[HttpResponseHeader.ContentEncoding]);

			try {
				using (var stream = response.GetResponseStream()) {
					return ParseByHtmlStream(stream, enc, piaproUrl);
				}
			} finally {
				response.Close();
			}


		}

		public override VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			return ParseByPiaproUrl(url);

			//var piaproUrl = linkMatchers[0].GetId(url);

			//return ParseByPiaproUrl(piaproUrl);

		}

	}
}
