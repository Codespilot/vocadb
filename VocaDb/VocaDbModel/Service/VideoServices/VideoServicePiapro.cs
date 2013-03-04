using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServicePiapro : VideoService {

		public VideoServicePiapro(PVService service, IVideoServiceParser parser, RegexLinkMatcher[] linkMatchers) 
			: base(service, parser, linkMatchers) {}

		private static Encoding GetEncoding(string encodingStr) {

			var shift_jis = Encoding.GetEncoding(932);

			if (string.IsNullOrEmpty(encodingStr))
				return shift_jis;

			try {
				return Encoding.GetEncoding(encodingStr);
			} catch (ArgumentException) {
				return shift_jis;
			}

		}

		/*public override string GetUrlById(string id) {

			var compositeId = new CompositeId(id);
			var matcher = linkMatchers.First();
			return "http://" + matcher.MakeLinkFromId(compositeId.SoundCloudUrl);

		}*/

		private VideoUrlParseResult ParseByHtmlStream(Stream htmlStream, Encoding encoding, string url) {

			var commentRegex = new Regex(@"<!-- (\w+) -->");
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

			return VideoUrlParseResult.CreateOk(url, PVService.Piapro, contentId, VideoTitleParseResult.CreateSuccess(title, author, string.Empty));

		}

		private VideoUrlParseResult ParseByPiaproUrl(string piaproUrl) {

			var request = WebRequest.Create(piaproUrl);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException) {
				return null;
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
