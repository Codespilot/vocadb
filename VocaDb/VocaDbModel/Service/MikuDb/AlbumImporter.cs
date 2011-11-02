using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using log4net;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.MikuDb {

	public class AlbumImporter {

		private const string albumIndexUrl = "http://mikudb.com/album-index/";
		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumImporter));
		private const int maxResults = 5;

		private readonly HashSet<string> existingUrls;

		private bool ContainsTracklist(HtmlNode node) {

			var text = StripHtml(node.InnerText);

			return (LineMatch(text, "Track list") || LineMatch(text, "Tracks list"));

		}

		private HtmlNode FindTracklistRow(HtmlDocument doc, HtmlNode row) {

			// Find the first table row on the page
			if (row == null)
				row = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]");

			if (row != null) {

				while (row != null) {

					var cell = row.Element("td");

					if (cell != null && ContainsTracklist(cell))
						return row;

					row = row.NextSibling;

				}

			} else {

				// Legacy pages don't have a <table>, but <p> elements instead
				row = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/p[2]");

				while (row != null) {

					if (ContainsTracklist(row))
						return row;

					row = row.NextSibling;

				}

			}

			return null;

		}

		private bool LineMatch(string line, string field) {

			return line.StartsWith(field + ":") || line.StartsWith(field + " :");

		}

		private void ParseInfoBox(ImportedAlbumDataContract data, HtmlNode infoBox) {

			var text = infoBox.InnerHtml;
			var rows = text.Split(new[] { "<br>", "<br />", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var row in rows) {

				var stripped = HtmlEntity.DeEntitize(StripHtml(row));
				if (stripped.StartsWith("\"") && stripped.EndsWith("\""))
					stripped = stripped.Substring(1, stripped.Length - 2).Trim();

				if (LineMatch(stripped, "Artist") || LineMatch(stripped, "Artists")) {

					var artists = stripped.Substring(8).Split(',').Select(s => s.Trim()).ToArray();
					data.ArtistNames = artists;

				} else if (LineMatch(stripped, "Vocals")) {

					var vocals = stripped.Substring(8).Split(',').Select(s => s.Trim()).ToArray();
					data.VocalistNames = vocals;

				} else if (LineMatch(stripped, "Circle")) {

					var artists = stripped.Substring(8).Trim();
					data.CircleName = artists;

				} else if (LineMatch(stripped, "Year")) {

					int year;
					if (int.TryParse(stripped.Substring(6), out year))
						data.ReleaseYear = year;

				}

			}

		}

		private void ParseTrackList(ImportedAlbumDataContract data, HtmlNode cell) {

			var lines = cell.InnerText.Split(new[] { "<br>", "<br />", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			var tracks = new List<ImportedAlbumTrack>();
			foreach (var line in lines) {

				var dotPos = line.IndexOf('.');

				if (dotPos <= 0)
					continue;

				var trackText = line.Substring(0, dotPos);

				int trackNum;

				if (int.TryParse(trackText, out trackNum)) {

					var trackTitle = line.Substring(dotPos + 1, line.Length - dotPos - 1).Trim();
					trackTitle = trackTitle.Replace("(lyrics)", string.Empty);

					tracks.Add(new ImportedAlbumTrack { Title = HtmlEntity.DeEntitize(trackTitle), TrackNum = trackNum });

				}

			}

			data.Tracks = tracks.ToArray();


		}

		private string StripHtml(string html) {

			return Regex.Replace(html, "<.*?>", string.Empty).Trim();

		}

		private MikuDbAlbumContract GetAlbumData(HtmlDocument doc, string url) {

			var data = new ImportedAlbumDataContract();

			string title = string.Empty;
			var titleElem = doc.DocumentNode.SelectSingleNode(".//h2[@class='posttitle']/a");

			if (titleElem != null)
				title = HtmlEntity.DeEntitize(titleElem.InnerText);

			var coverPicLink = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]/td[1]/a/img");
			PictureDataContract coverPicture = null;

			if (coverPicLink != null) {

				var address = coverPicLink.Attributes["src"].Value;

				var request = WebRequest.Create(address);
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {

					var buf = StreamHelper.ReadStream(stream, response.ContentLength);

					coverPicture = new PictureDataContract(buf, response.ContentType);

				}

			}

			var infoBox = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]/td[2]");

			if (infoBox != null) {
				ParseInfoBox(data, infoBox);
			}

			var trackListRow = FindTracklistRow(doc, (infoBox != null ? infoBox.ParentNode.NextSibling : null));
	
			if (trackListRow != null) {
				
				ParseTrackList(data, trackListRow);

			}

			return new MikuDbAlbumContract { Title = title, Data = data, CoverPicture = coverPicture, SourceUrl = url };

		}

		private MikuDbAlbumContract GetAlbumData(string url) {

			HtmlDocument doc;
			
			try {
				doc = HtmlRequestHelper.Download(url);	
			} catch (WebException x) {
				log.Warn("Unable to download album post '" + url + "'", x);
				throw;
			}

			return GetAlbumData(doc, url);

		}

		private AlbumImportResult[] Import(HtmlDocument doc) {

			var listDiv = doc.DocumentNode.SelectSingleNode("//div[@class = 'postcontent2']");
			var albumDivs = listDiv.Descendants("div");
			var list = new List<AlbumImportResult>();

			foreach (var albumDiv in albumDivs) {

				var link = albumDiv.Element("a");

				if (link == null)
					continue;

				var url = link.Attributes["href"].Value;

				if (existingUrls.Contains(url))
					continue;

				//var name = HtmlEntity.DeEntitize(link.InnerText);
				var data = GetAlbumData(url);

				list.Add(new AlbumImportResult {AlbumContract = data});

				/*list.Add(new AlbumImportResult {
					AlbumContract = new MikuDbAlbumContract {
						Title = name, SourceUrl = url, CoverPicture = data.CoverPicture, Data = data.AlbumData
					}
				});*/

				if (list.Count >= maxResults)
					break;

				Thread.Sleep(300);

			}

			return list.ToArray();

		}

		public AlbumImporter(IEnumerable<MikuDbAlbumContract> existingUrls) {

			ParamIs.NotNull(() => existingUrls);

			this.existingUrls = new HashSet<string>(existingUrls.Select(a => a.SourceUrl));

		}

		public AlbumImportResult[] ImportNew() {

			HtmlDocument albumIndex;

			try {
				albumIndex = HtmlRequestHelper.Download(albumIndexUrl);
			} catch (WebException x) {
				log.Warn("Unable to read albums index", x);
				throw;
			}

			return Import(albumIndex);

		}

		public AlbumImportResult ImportOne(string url) {
			
			if (existingUrls.Contains(url))
				return new AlbumImportResult { Message = "Album already imported" };

			var data = GetAlbumData(url);

			return new AlbumImportResult {AlbumContract = data};

		}

	}
}
