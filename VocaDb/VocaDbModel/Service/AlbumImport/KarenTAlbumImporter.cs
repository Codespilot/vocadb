using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.AlbumImport {

	public class KarenTAlbumImporter : IAlbumImporter {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly RegexLinkMatcher matcher = new RegexLinkMatcher("http://karent.jp/album/{0}", @"http://karent.jp/album/(\d+)");
		private readonly IPictureDownloader pictureDownloader;

		public KarenTAlbumImporter(IPictureDownloader pictureDownloader) {
			this.pictureDownloader = pictureDownloader;
		}

		private PictureDataContract DownloadCoverPicture(string url) {

			if (url.Contains("_0280_0280.jpg")) {

				var fullUrl = url.Replace("_0280_0280", string.Empty);

				var pic = pictureDownloader.Create(fullUrl);

				if (pic != null)
					return pic;

			}

			return pictureDownloader.Create(url);

		}

		public MikuDbAlbumContract GetAlbumData(HtmlDocument doc, string url) {

			var data = new ImportedAlbumDataContract();

			var titleElem = doc.DocumentNode.SelectSingleNode("//div[@class = 'pgtitle_in']/h1/span");

			if (titleElem != null)
				data.Title = titleElem.InnerText;

			var mainPanel = doc.GetElementbyId("main_ref");

			if (mainPanel != null) {

				var descBox = mainPanel.SelectSingleNode("p[@class = 'overview']");

				if (descBox != null)
					data.Description = descBox.InnerText;

				var infoBox = mainPanel.SelectSingleNode("div[1]");

				if (infoBox != null)
					ParseInfoBox(data, infoBox);

				var tracklistElem = mainPanel.SelectSingleNode("div[@class = 'songlistbox']");

				if (tracklistElem != null)
					ParseTracklist(data, tracklistElem);

			}

			var coverElem = doc.DocumentNode.SelectSingleNode("//div[@id = 'sub_ref']/div[@class = 'artwork']/div/a/img");
			PictureDataContract coverPic = null;

			if (coverElem != null) {
				coverPic = DownloadCoverPicture("http://karent.jp" + coverElem.Attributes["src"].Value);
			}

			return new MikuDbAlbumContract(data) { CoverPicture = coverPic, SourceUrl = url };

		}

		private string GetVocalistName(string portraitImg) {
			
			switch (portraitImg) {
				case "/modpub/images/ico/ico_cv_1.png":
					return "Hatsune Miku";
				default:
					return null;
			}

		}

		private HtmlNode GetInfoElem(HtmlNodeCollection nodes, string title) {
			return nodes.FirstOrDefault(n => n.SelectNodes("span[text() = '" + title + "&nbsp;:']") != null);
		}

		private void ParseInfoBox(ImportedAlbumDataContract data, HtmlNode infoBox) {

			var statusRows = infoBox.SelectNodes("//p[@class='albumstatus']");

			var artistRow = GetInfoElem(statusRows, "Artist");

			if (artistRow != null) {

				var links = artistRow.SelectNodes("a");
				data.ArtistNames = links.Select(l => l.InnerText).ToArray();

			}

			var charaRow = GetInfoElem(statusRows, "Characters");
			
			if (charaRow != null) {

				var charaImgs = charaRow.SelectNodes("a/img");
				data.VocalistNames = charaImgs.Select(l => GetVocalistName(l.Attributes["src"].Value)).Where(l => l != null).ToArray();

			}

		}

		public ImportedAlbumTrack ParseTrackRow(int trackNum, string songTitle) {

			var trackRegex = new Regex(@"\d\d\.\&nbsp\;(.+) \(feat\. (.+)\)"); // 01.&nbsp;Cloud Science (feat. Hatsune Miku)

			var match = trackRegex.Match(songTitle);

			if (!match.Success)
				return null;

			var title = match.Groups[1].Value;
			var vocalists = match.Groups[2].Value;

			return new ImportedAlbumTrack { TrackNum = trackNum, DiscNum = 1, Title = title, VocalistNames = new[] { vocalists } };

		}

		private void ParseTracklist(ImportedAlbumDataContract data, HtmlNode tracklistElem) {

			var songElems = tracklistElem.SelectNodes("//div[@class = 'song']");

			var tracks = new List<ImportedAlbumTrack>();
			for (int i = 1; i <= songElems.Count; ++i) {

				var songLink = songElems[i-1].Element("a");
				var track = ParseTrackRow(i, songLink.InnerText);

				if (track != null)
					tracks.Add(track);

			}

			data.Tracks = tracks.ToArray();

		}

		public AlbumImportResult ImportOne(string url) {

			HtmlDocument doc;

			try {
				doc = HtmlRequestHelper.Download(url, "en-US");
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
