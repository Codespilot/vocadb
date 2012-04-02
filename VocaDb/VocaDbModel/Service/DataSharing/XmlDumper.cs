using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Songs;
using System.IO.Packaging;
using System.IO;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.Service.DataSharing {

	public class XmlDumper {

		private void DumpXml<T>(T contract, Package package, string folder, int id) {

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

		}

		public void Create(string path, IEnumerable<Artist> artists, IEnumerable<Album> albums, IEnumerable<Song> songs) {

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

		}

	}

}
