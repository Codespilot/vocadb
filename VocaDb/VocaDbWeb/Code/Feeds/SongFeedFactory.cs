using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code.Feeds {

	public class SongFeedFactory {

		private static readonly XNamespace mediaNs = XNamespace.Get(@"http://search.yahoo.com/mrss/");

		private SyndicationItem CreateFeedItem(SongContract song, Func<SongContract, string> contentFac, Func<SongContract, string> urlFac) {

			var item = new SyndicationItem(song.Name, new TextSyndicationContent(contentFac(song), TextSyndicationContentKind.Html),
					VocaUriBuilder.CreateAbsolute(urlFac(song)), song.Id.ToString(), song.CreateDate);

			//item.Summary = new TextSyndicationContent(string.Format("<img src='{0}' alt='{1}' title='{1}' />", song.ThumbUrl, song.Name), TextSyndicationContentKind.Html);
			item.Summary = new TextSyndicationContent(contentFac(song), TextSyndicationContentKind.Html);
			item.ElementExtensions.Add(new XElement(mediaNs + "thumbnail", new XAttribute("url", song.ThumbUrl)));

			return item;

		}

		public SyndicationFeed Create(IEnumerable<SongContract> songs, Uri uri, Func<SongContract, string> contentFac, Func<SongContract, string> urlFac) {

			var items = songs.Select(s => CreateFeedItem(s, contentFac, urlFac));

			var feed = new SyndicationFeed("Latest songs with videos", string.Empty, uri, items);
			feed.AttributeExtensions.Add(new XmlQualifiedName("media", XNamespace.Xmlns.ToString()), mediaNs.ToString());

			return feed;

		}

	}

}