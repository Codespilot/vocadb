using System;
using System.Net; 
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.AlbumImport {

	public class KarenTAlbumImporter : IAlbumImporter {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly RegexLinkMatcher matcher = new RegexLinkMatcher("http://karent.jp/album/{0}", @"http://karent.jp/album/(\d+)");

		public MikuDbAlbumContract GetAlbumData(HtmlDocument doc, string url) {

			var data = new ImportedAlbumDataContract();

			var titleElem = doc.DocumentNode.SelectSingleNode("//div[@class = 'pgtitle_in']/h1/span");

			if (titleElem != null)
				data.Title = titleElem.InnerText;

			var mainPanel = doc.GetElementbyId("main_ref");

			if (mainPanel != null) {

				var descBox = mainPanel.SelectSingleNode("/p[@class = 'overview']");

				if (descBox != null)
					data.Description = descBox.InnerText;

				var infoBox = mainPanel.SelectSingleNode("/div[1]");

				if (infoBox != null)
					ParseInfoBox(data, infoBox);

				var tracklistElem = mainPanel.SelectSingleNode("/div[@class = 'songlistbox']");

				if (tracklistElem != null)
					ParseTracklist(data, tracklistElem);

			}

			return new MikuDbAlbumContract(data) { SourceUrl = url };

		}

		private void ParseInfoBox(ImportedAlbumDataContract data, HtmlNode infoBox) {
			


		}

		private void ParseTracklist(ImportedAlbumDataContract data, HtmlNode tracklistElem) {
			

		}

		public AlbumImportResult ImportOne(string url) {

			HtmlDocument doc;

			try {
				doc = HtmlRequestHelper.Download(url);
			} catch (WebException x) {
				log.WarnException("Unable to download album post '" + url + "'", x);
				throw;
			}

			var data = GetAlbumData(doc, url);

			return new AlbumImportResult {AlbumContract = data};

		}

		public bool IsValidFor(string url) {
			return matcher.IsMatch(url);
		}

		public string ServiceName {
			get { return "KarenT"; }
		}
	}

}
