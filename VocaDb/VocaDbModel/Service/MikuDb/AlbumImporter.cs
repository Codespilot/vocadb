using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using log4net;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.MikuDb {

	public class AlbumImporter {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumImporter));

		private const string albumIndexUrl = "http://mikudb.com/album-index/";
		private readonly HashSet<string> existingUrls;
		
		private HtmlNode FindTracklistRow(HtmlNode row) {

			while (row != null) {

				var cell = row.Element("td");

				if (cell != null) {

					var text = StripHtml(cell.InnerText);

					if (LineMatch(text, "Track list"))
						return row;
					
				}

				row = row.NextSibling;

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

		private void ParseTrackList(ImportedAlbumDataContract data, HtmlNode trackListRow) {

			var cell = trackListRow.Element("td");
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

					tracks.Add(new ImportedAlbumTrack { Title = HtmlEntity.DeEntitize(trackTitle), TrackNum = trackNum });

				}

			}

			data.Tracks = tracks.ToArray();


		}

		private string StripHtml(string html) {

			return Regex.Replace(html, "<.*?>", string.Empty).Trim();

		}

		private ImportedAlbumDataContract GetAlbumData(HtmlDocument doc) {

			var data = new ImportedAlbumDataContract();
			var infoBox = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]/td[2]");

			if (infoBox != null) {

				ParseInfoBox(data, infoBox);

				/*var rows = infoBox.Elements("p").Concat(infoBox.Elements("strong"));

				foreach (var row in rows) {

					if (row.InnerText.Contains("Artist:") || row.InnerText.Contains("Vocals:"))
						ParseInfoBox(data, row);

				}*/

			}

			var trackListRow = FindTracklistRow(infoBox.ParentNode.NextSibling);

			if (trackListRow != null) {
				
				ParseTrackList(data, trackListRow);

			}

			return data;

		}

		private ImportedAlbumDataContract GetAlbumData(string url) {

			HtmlDocument doc;
			
			try {
				doc = HtmlRequestHelper.Download(url);	
			} catch (WebException x) {
				log.Warn("Unable to download album post '" + url + "'", x);
				throw;
			}

			return GetAlbumData(doc);

		}

		private AlbumImportResult[] Import(HtmlDocument doc) {

			var listDiv = doc.DocumentNode.SelectSingleNode("//div[@class = 'postcontent2']");
			var albumDivs = listDiv.Descendants("div");
			var list = new List<AlbumImportResult>();

			foreach (var albumDiv in albumDivs.Take(5)) {

				var link = albumDiv.Element("a");

				if (link == null)
					continue;

				var url = link.Attributes["href"].Value;

				if (existingUrls.Contains(url))
					continue;

				var name = link.InnerText;
				var data = GetAlbumData(url);

				list.Add(new AlbumImportResult {AlbumContract = new MikuDbAlbumContract {Title = name, SourceUrl = url, Data = data}});

			}

			return list.ToArray();

		}

		public AlbumImporter(IEnumerable<MikuDbAlbumContract> existingUrls) {
			
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

	}
}
