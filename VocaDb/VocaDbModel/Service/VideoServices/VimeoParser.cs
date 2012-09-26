using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

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

			return VideoTitleParseResult.CreateSuccess(titleElem.Value);

		}

	}

}
