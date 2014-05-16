using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using NLog;
using VocaDb.SiteMapGenerator.Sitemap;
using VocaDb.SiteMapGenerator.VocaDb;

namespace VocaDb.SiteMapGenerator {
	class Program {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static async Task GenerateSitemap() {
			
			var config = new Config();
			var client = new VocaDbClient(config.SiteRootUrl);

			log.Info("Getting entries from VocaDB");

			var artists = await client.GetArtists();
			var albums = await client.GetAlbums();
			var songs = await client.GetSongs();

			log.Info("Generating sitemap");

			var generator = new SitemapGenerator(config.SiteRootUrl, config.SitemapRootUrl);
			generator.Generate(config.OutFolder, new Dictionary<EntryType, int[]> {
				{ EntryType.Artist, artists },
				{ EntryType.Album, albums },
				{ EntryType.Song, songs },
			});

		}

		static void Main(string[] args) {

			Task.WaitAll(GenerateSitemap());

		}
	}
}
