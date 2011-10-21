using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.MikuDb;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.Service {

	public class MikuDbAlbumService : ServiceBase {

		public MikuDbAlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		private Album FindAlbum(ISession session, MikuDbAlbum imported) {

			Album albumMatch = null;
			var webLinkMatch = session.Query<AlbumWebLink>().FirstOrDefault(w => w.Url == imported.SourceUrl);

			if (webLinkMatch == null) {

				var nameMatchDirect = session.Query<Album>()
					.Where(s => !s.Deleted
					&& s.TranslatedName.English.Contains(imported.Title)
						|| s.TranslatedName.Romaji.Contains(imported.Title)
						|| s.TranslatedName.Japanese.Contains(imported.Title))
					.FirstOrDefault();

				if (nameMatchDirect != null) {
					albumMatch = nameMatchDirect;
				} else {

					var nameMatchAdditional = session.Query<AlbumName>()
						.Where(m => !m.Album.Deleted && m.Value.Contains(imported.Title))
						.FirstOrDefault();

					if (nameMatchAdditional != null)
						albumMatch = nameMatchAdditional.Album;

				}

			} else {
				albumMatch = webLinkMatch.Album;
			}

			return albumMatch;

		}

		private Artist FindArtist(ISession session, string artistName) {

			if (string.IsNullOrEmpty(artistName))
				return null;

			var direct = session.Query<Artist>()
				.Where(
					s => !s.Deleted &&
					(s.TranslatedName.English == artistName
						|| s.TranslatedName.Romaji == artistName
						|| s.TranslatedName.Japanese == artistName))
				.FirstOrDefault();

			if (direct != null)
				return direct;

			var additionalNames = session.Query<ArtistName>()
				.Where(m => !m.Artist.Deleted && m.Value == artistName)
				.FirstOrDefault();

			if (additionalNames != null)
				return additionalNames.Artist;

			return null;

		}

		private Song FindSong(ISession session, string songName) {

			var direct = session.Query<Song>()
				.Where(
					s => !s.Deleted &&
					(s.TranslatedName.English == songName
						|| s.TranslatedName.Romaji == songName
						|| s.TranslatedName.Japanese == songName))
				.FirstOrDefault();

			if (direct != null)
				return direct;

			var additionalNames = session.Query<SongName>()
				.Where(m => !m.Song.Deleted && m.Value == songName)
				.FirstOrDefault();

			if (additionalNames != null)
				return additionalNames.Song;

			return null;

		}

		private InspectedAlbum Inspect(ISession session, MikuDbAlbum imported) {

			var albumMatch = FindAlbum(session, imported);

			var importedContract = new MikuDbAlbumContract(imported);
			var data = importedContract.Data;

			var artists = data.ArtistNames.Concat(data.VocalistNames).Concat(new[] { data.CircleName })
				.Select(a => InspectArtist(session, a))
				.ToArray();

			InspectedTrack[] tracks = null;

			if (albumMatch == null || !albumMatch.Songs.Any())
				tracks = data.Tracks.Select(t => InspectTrack(session, t)).ToArray();


			var result = new InspectedAlbum(importedContract);

			if (albumMatch != null)
				result.ExistingAlbum = new AlbumWithAdditionalNamesContract(albumMatch, PermissionContext.LanguagePreference);

			result.Artists = artists;
			result.Tracks = tracks;

			return result;

		}

		private InspectedArtist InspectArtist(ISession session, string name) {

			var inspected = new InspectedArtist(name);

			var matched = FindArtist(session, name);

			if (matched != null)
				inspected.ExistingArtist = new ArtistWithAdditionalNamesContract(matched, PermissionContext.LanguagePreference);

			return inspected;

		}

		private InspectedTrack InspectTrack(ISession session, ImportedAlbumTrack importedTrack) {

			var inspected = new InspectedTrack(importedTrack);
			var existingSong = FindSong(session, importedTrack.Title);

			if (existingSong != null)
				inspected.ExistingSong = new SongWithAdditionalNamesContract(existingSong, PermissionContext.LanguagePreference);

			return inspected;

		}

		public MikuDbAlbumContract[] GetAlbums(AlbumStatus status) {

			return HandleQuery(session => session.Query<MikuDbAlbum>()
				.Where(a => a.Status == status)
				.Select(a => new MikuDbAlbumContract(a))
				.ToArray());

		}

		public int ImportNew() {

			PermissionContext.VerifyPermission(PermissionFlags.MikuDbImport);

			MikuDbAlbumContract[] existing = HandleQuery(session => session.Query<MikuDbAlbum>().Select(a => new MikuDbAlbumContract(a)).ToArray());

			var importer = new AlbumImporter(existing);
			var imported = importer.ImportNew();

			return HandleTransaction(session => {

				//var all = session.Query<MikuDbAlbum>();

				//foreach (var album in all)
				//	session.Delete(album);

				var newAlbums = new List<MikuDbAlbum>();

				foreach (var contract in imported) {

					var newAlbum = new MikuDbAlbum(contract.AlbumContract);

					session.Save(newAlbum);

					newAlbums.Add(newAlbum);

				}

				return newAlbums.Count;

			});

		}

		public InspectedAlbum[] Inspect(int[] importedAlbumIds) {

			return HandleQuery(session => {

				var importedAlbums = session.Query<MikuDbAlbum>().Where(a => importedAlbumIds.Contains(a.Id)).ToArray();

				return importedAlbums.Select(s => Inspect(session, s)).ToArray();

			});

		}

	}
}
