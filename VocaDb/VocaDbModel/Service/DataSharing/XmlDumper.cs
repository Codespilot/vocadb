using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using System.IO.Packaging;
using System.IO;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Model.Service.DataSharing {

	public class XmlDumper {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public class Loader<TEntry, TContract> where TEntry : IEntryWithIntId {

			private const int maxEntries = 500;
			private readonly string folder;
			private readonly Func<TEntry, TContract> contractFunc;
			private readonly Func<int, int, TEntry[]> loadFunc;

			private void DumpXml<T>(T[] contract, Package package, int id) {

				var partUri = PackUriHelper.CreatePartUri(new Uri(string.Format("{0}{1}.xml", folder, id), UriKind.Relative));

				if (package.PartExists(partUri)) {
					log.Warn("Duplicate path: {0}", partUri);
					return;
				}

				var packagePart = package.CreatePart(partUri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);

				var data = XmlHelper.SerializeToXml(contract);

				data.Save(packagePart.GetStream());

			}

			public Loader(string folder, Func<int, int, TEntry[]> loadFunc, Func<TEntry, TContract> contractFunc) {
				this.folder = folder;
				this.loadFunc = loadFunc;
				this.contractFunc = contractFunc;
			}

			public void Dump(Package package) {

				bool run = true;
				int start = 0;

				while (run) {

					var entries = loadFunc(start, maxEntries);
					var contracts = entries.Select(e => contractFunc(e)).ToArray();
					DumpXml(contracts, package, start);

					start += contracts.Length;
					run = entries.Any();
					GC.Collect();

				}

			}

		}

		public void Create(string path, ISession session) {

			var artistLoader = new Loader<Artist, ArchivedArtistContract>("/Artists/", 
				(first, max) => session.Query<Artist>().Where(a => !a.Deleted).Skip(first).Take(max).ToArray(), 
				a => new ArchivedArtistContract(a, new ArtistDiff()));

			var albumLoader = new Loader<Album, ArchivedAlbumContract>("/Albums/",
				(first, max) => session.Query<Album>().Where(a => !a.Deleted).Skip(first).Take(max).ToArray(),
				a => new ArchivedAlbumContract(a, new AlbumDiff()));

			var songLoader = new Loader<Song, ArchivedSongContract>("/Songs/",
				(first, max) => { session.Clear(); return session.Query<Song>().Where(a => !a.Deleted).Skip(first).Take(max).ToArray(); },
				a => new ArchivedSongContract(a, new SongDiff()));

			var eventSeriesLoader = new Loader<ReleaseEventSeries, ArchivedEventSeriesContract>("/EventSeries/",
				(first, max) => { session.Clear(); return session.Query<ReleaseEventSeries>().Skip(first).Take(max).ToArray(); },
				a => new ArchivedEventSeriesContract(a));

			var eventLoader = new Loader<ReleaseEvent, ArchivedEventContract>("/Events/",
				(first, max) => { session.Clear(); return session.Query<ReleaseEvent>().Skip(first).Take(max).ToArray(); },
				a => new ArchivedEventContract(a));

			var tagLoader = new Loader<Tag, ArchivedTagContract>("/Tags/",
				(first, max) => { session.Clear(); return session.Query<Tag>().Skip(first).Take(max).ToArray(); },
				a => new ArchivedTagContract(a));

			using (var package = Package.Open(path, FileMode.Create)) {

				artistLoader.Dump(package);
				albumLoader.Dump(package);
				songLoader.Dump(package);
				eventSeriesLoader.Dump(package);
				eventLoader.Dump(package);
				tagLoader.Dump(package);

			}

		}

	}

}
