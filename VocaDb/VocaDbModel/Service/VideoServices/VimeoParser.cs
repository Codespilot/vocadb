using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class VimeoParser : IVideoServiceParser {

		public VideoTitleParseResult GetTitle(string id) {

			var url = string.Format("http://vimeo.com/api/v2/video/{0}.xml", id);

			var request = WebRequest.Create(url);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}

			XDocument doc;

			try {
				doc = XDocument.Load(response.GetResponseStream());
			} catch (XmlException x) {
				return VideoTitleParseResult.CreateError("Vimeo (error): " + x.Message);
			}

			var titleElem = doc.XPathSelectElement("videos/video/title");

			if (titleElem == null) {
				return VideoTitleParseResult.CreateError("Vimeo (error): title element not found");
			}

			var author = XmlHelper.GetNodeTextOrEmpty(doc, "videos/video/user_name");
			var thumbUrl = XmlHelper.GetNodeTextOrEmpty(doc, "videos/video/thumbnail_small");

			return VideoTitleParseResult.CreateSuccess(titleElem.Value, author, thumbUrl);

		}

	}

}
