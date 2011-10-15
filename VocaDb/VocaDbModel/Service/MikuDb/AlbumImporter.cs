using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using log4net;
using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Model.Service.MikuDb {

	public class AlbumImporter {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumImporter));

		private const string albumIndexUrl = "http://mikudb.com/album-index/";
		private HashSet<string> existingUrls;

		private AlbumImportResult[] Import(Stream input, string encodingName) {

			var encoding = (!string.IsNullOrEmpty(encodingName) ? Encoding.GetEncoding(encodingName) : Encoding.UTF8);

			var doc = new HtmlDocument();
			doc.Load(input, encoding);

			var listDiv = doc.DocumentNode.SelectSingleNode("//div[@class = 'postcontent2']");
			var albumDivs = listDiv.Descendants("div");
			var list = new List<AlbumImportResult>();

			foreach (var albumDiv in albumDivs.Take(5)) {

				var link = albumDiv.Element("a");

				if (link == null)
					continue;

				var url = link.Attributes["href"].Value;
				var name = link.InnerText;

				list.Add(new AlbumImportResult {AlbumContract = new MikuDbAlbumContract {Title = name, SourceUrl = url, Data = new XDocument(new XElement("root"))}});

			}

			return list.ToArray();

		}

		public AlbumImporter(IEnumerable<MikuDbAlbumContract> existingUrls) {
			
			this.existingUrls = new HashSet<string>(existingUrls.Select(a => a.SourceUrl));

		}

		public AlbumImportResult[] ImportNew() {

			var request = WebRequest.Create(albumIndexUrl);
			WebResponse response;

			try {
				response = request.GetResponse();
			} catch (WebException x) {
				log.Warn("Unable to read albums index", x);
				throw;
			}

			try {
				var enc = response.Headers[HttpResponseHeader.ContentEncoding];

				using (var stream = response.GetResponseStream()) {
					return Import(stream, enc);
				}
			} finally {
				response.Close();
			}

		}

	}
}
