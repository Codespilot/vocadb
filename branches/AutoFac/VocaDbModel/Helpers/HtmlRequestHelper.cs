using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace VocaDb.Model.Helpers {

	public static class HtmlRequestHelper {

		public static HtmlDocument Download(string url) {

			var request = WebRequest.Create(url);
			WebResponse response = request.GetResponse();

			try {
				var enc = response.Headers[HttpResponseHeader.ContentEncoding];

				using (var stream = response.GetResponseStream()) {
					var encoding = (!string.IsNullOrEmpty(enc) ? Encoding.GetEncoding(enc) : Encoding.UTF8);

					var doc = new HtmlDocument();
					doc.Load(stream, encoding);
					return doc;
				}
			} finally {
				response.Close();
			}

		}

	}
}
