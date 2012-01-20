using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Rss;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.Rankings {

	public class NNDWVRParser {

		private static readonly Regex wvrIdRegex = new Regex(@"#(\d{3})");
		private static readonly Regex numRegex = new Regex(@"\d+");

		/*private RankingContract GetSongs(Stream htmlStream, string encodingStr) {

			var encoding = (!string.IsNullOrEmpty(encodingStr) ? Encoding.GetEncoding(encodingStr) : Encoding.UTF8);

			var result = new RankingContract();
			var songs = new List<SongInRankingContract>();

			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);

			var titleElem = doc.DocumentNode.SelectSingleNode("//div[@id='SYS_box_mylist_header']/div[1]/h1[1]");

			if (titleElem != null) {

				result.Name = titleElem.InnerText;

				var match = numRegex.Match(result.Name);

				if (match.Success)
					result.WVRId = int.Parse(match.Groups[1].Value);

			}

			var links = doc.DocumentNode.SelectNodes("//a[@class='watch']");

			int order = 1;

			foreach (var link in links) {

				var att = link.Attributes["href"];
				var text = link.InnerText;
				var url = att != null ? att.Value : null;
				var id = url != null ? VideoService.NicoNicoDouga.GetIdByUrl(url) : null;

				if (id != null) {

					songs.Add(new SongInRankingContract { NicoId = id, SortIndex = order, Name = text, Url = url });
					++order;

				}

			}

			result.Songs = songs.ToArray();
			return result;

		}*/

		public RankingContract GetSongs(string url) {

			var feed = RssFeed.Read(url);

			var result = new RankingContract();
			var channel = feed.Channels[0];
			result.Name = channel.Title;
			var wvrIdMatch = wvrIdRegex.Match(result.Name);

			if (wvrIdMatch.Success)
				result.WVRId = int.Parse(wvrIdMatch.Groups[1].Value);

			var songs = new List<SongInRankingContract>();
			var order = 1;

			foreach (var item in channel.Items.Cast<RssItem>()) {

				var node = HtmlNode.CreateNode(item.Description);

				if (char.IsDigit(node.InnerText, 0)) {

					var nicoId = VideoService.NicoNicoDouga.GetIdByUrl(item.Link.ToString());
					songs.Add(new SongInRankingContract { NicoId = nicoId, SortIndex = order, Name = item.Title, Url = item.Link.ToString() });
					++order;

				}

			}

			result.Songs = songs.ToArray();
			return result;

			/*var request = WebRequest.Create(url);
			using (var response = request.GetResponse()) {
				var enc = response.Headers[HttpResponseHeader.ContentEncoding];
				return GetSongs(response.GetResponseStream(), enc);
			}*/

		}

	}

}
