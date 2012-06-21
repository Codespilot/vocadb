using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
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

		public class Loader<TEntry, TContract> where TEntry : IEntryWithIntId {

			private const int maxEntries = 2000;
			private readonly string folder;
			private readonly Func<TEntry, TContract> contractFunc;
			private readonly Func<int, int, TEntry[]> loadFunc;

			private void DumpXml<T>(T contract, Package package, int id) {

				var partUri = PackUriHelper.CreatePartUri(new Uri(folder + id + ".xml", UriKind.Relative));

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

					foreach (var entry in entries) {

						var contract = contractFunc(entry);

						DumpXml(contract, package, entry.Id);

					}

					start += entries.Length;
					run = entries.Any();

				}

			}

		}

		/*private void DumpXml<T>(T contract, Package package, string folder, int id) {

			var partUri = PackUriHelper.CreatePartUri(new Uri(folder + id + ".xml", UriKind.Relative));

			var packagePart = package.CreatePart(partUri, System.Net.Mime.MediaTypeNames.Text.Xml, CompressionOption.Normal);

			var data = XmlHelper.SerializeToXml(contract);

			data.Save(packagePart.GetStream());

		}

		private void Dump(Album album, Package package) {

			var contract = new ArchivedAlbumContract(album, new AlbumDiff());
			DumpXml(contract, package, "/Albums/", album.Id);

		}

		private void Dump(Artist artist, Package package) {

			var contract = new ArchivedArtistContract(artist, new ArtistDiff());
			DumpXml(contract, package, "/Artists/", artist.Id);

		}

		private void Dump(Song song, Package package) {

			var contract = new ArchivedSongContract(song, new SongDiff());
			DumpXml(contract, package, "/Songs/", song.Id);

		}*/

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

		/*public void Create(string path, IEnumerable<Artist> artists, IEnumerable<Album> albums, IEnumerable<Song> songs) {

			using (var package = Package.Open(path, FileMode.Create)) {

				foreach (var artist in artists) {
					Dump(artist, package);
				}

				foreach (var album in albums) {
					Dump(album, package);
				}

				foreach (var song in songs) {
					Dump(song, package);
				}

			}

		}*/

	}

}
