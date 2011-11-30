using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
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

		public MikuDbAlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		private AlbumContract AcceptImportedAlbum(ISession session, InspectedAlbum acceptedAlbum, int[] selectedSongIds) {
			
			AuditLog(string.Format("accepting imported album '{0}'", acceptedAlbum.ImportedAlbum.Title), session);

			Album album;

			if (acceptedAlbum.ExistingAlbum == null) {

				album = new Album(acceptedAlbum.ImportedAlbum.Title);
				album.DiscType = DiscType.Unknown;
				session.Save(album);

			} else {
				album = session.Load<Album>(acceptedAlbum.ExistingAlbum.Id);
			}

			foreach (var inspectedArtist in acceptedAlbum.Artists.Where(a => a.ExistingArtist != null)) {

				var artist = session.Load<Artist>(inspectedArtist.ExistingArtist.Id);

				if (!artist.HasAlbum(album))
					session.Save(artist.AddAlbum(album));

			}

			if (!album.Songs.Any()) {

				foreach (var inspectedTrack in acceptedAlbum.Tracks) {

					Song song;

					if (inspectedTrack.ExistingSong == null || !selectedSongIds.Contains(inspectedTrack.ExistingSong.Id)) {

						song = new Song(inspectedTrack.ImportedTrack.Title);
						album.AddSong(song, inspectedTrack.ImportedTrack.TrackNum, inspectedTrack.ImportedTrack.DiscNum);

						Services.Songs.Archive(session, song, SongArchiveReason.AutoImportedFromMikuDb, 
							"Auto-imported from MikuDB for album '" + album.DefaultName + "'");

						session.Save(song);

					} else {
						song = session.Load<Song>(inspectedTrack.ExistingSong.Id);
						if (!album.HasSong(song))
							session.Save(album.AddSong(song, inspectedTrack.ImportedTrack.TrackNum, inspectedTrack.ImportedTrack.DiscNum));
					}

				}
	
			}

			var importedAlbum = session.Load<MikuDbAlbum>(acceptedAlbum.ImportedAlbum.Id);
			importedAlbum.Status = AlbumStatus.Approved;

			if (importedAlbum.CoverPicture != null && album.CoverPicture == null) {
				album.CoverPicture = importedAlbum.CoverPicture;
			}

			if (acceptedAlbum.ImportedAlbum.Data.ReleaseYear != null && album.OriginalReleaseDate.Year == null)
				album.OriginalReleaseDate.Year = acceptedAlbum.ImportedAlbum.Data.ReleaseYear;

			if (!album.WebLinks.Any(w => w.Url.Contains("mikudb.com")))
				album.CreateWebLink("MikuDB", acceptedAlbum.ImportedAlbum.SourceUrl);

			album.UpdateArtistString();
			Services.Albums.Archive(session, album, new AlbumDiff { Cover = true }, AlbumArchiveReason.AutoImportedFromMikuDb);

			session.Update(album);
			session.Update(importedAlbum);

			return new AlbumContract(album, PermissionContext.LanguagePreference);

		}

		private Album FindAlbum(ISession session, MikuDbAlbum imported) {

			Album albumMatch = null;
			var webLinkMatch = session.Query<AlbumWebLink>().FirstOrDefault(w => w.Url == imported.SourceUrl);

			if (webLinkMatch == null) {

				var nameMatchDirect = session.Query<Album>()
					.Where(s => !s.Deleted
					&& ((s.Names.SortNames.English == imported.Title)
						|| (s.Names.SortNames.Romaji == imported.Title)
						|| (s.Names.SortNames.Japanese == imported.Title)))
					.FirstOrDefault();

				if (nameMatchDirect != null) {
					albumMatch = nameMatchDirect;
				} else {

					var nameMatchAdditional = session.Query<AlbumName>()
						.Where(m => !m.Album.Deleted && m.Value == imported.Title)
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
					(s.Names.SortNames.English == artistName
						|| s.Names.SortNames.Romaji == artistName
						|| s.Names.SortNames.Japanese == artistName))
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
					(s.Names.SortNames.English == songName
						|| s.Names.SortNames.Romaji == songName
						|| s.Names.SortNames.Japanese == songName))
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

		private InspectedAlbum[] Inspect(ISession session, int[] importedAlbumIds) {

			var importedAlbums = session.Query<MikuDbAlbum>().Where(a => importedAlbumIds.Contains(a.Id)).ToArray();

			return importedAlbums.Select(s => Inspect(session, s)).ToArray();

		}

		private InspectedAlbum Inspect(ISession session, MikuDbAlbum imported) {

			var albumMatch = FindAlbum(session, imported);

			var importedContract = new MikuDbAlbumContract(imported);
			var data = importedContract.Data;

			var artists = data.ArtistNames.Concat(data.VocalistNames).Concat(!string.IsNullOrEmpty(data.CircleName) ? new[] { data.CircleName } : new string[] {})
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

		public AlbumContract[] AcceptImportedAlbums(int[] importedAlbumIds, int[] selectedSongIds) {

			PermissionContext.VerifyPermission(PermissionFlags.MikuDbImport | PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var importedAlbums = new List<AlbumContract>(importedAlbumIds.Length);
				var inspected = Inspect(session, importedAlbumIds);

				foreach (var acceptedAlbum in inspected) {

					var album = AcceptImportedAlbum(session, acceptedAlbum, selectedSongIds);

					importedAlbums.Add(album);

				}

				return importedAlbums.ToArray();

			});

		}

		public void Delete(int importedAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.MikuDbImport);

			DeleteEntity<MikuDbAlbum>(importedAlbumId);

		}

		public MikuDbAlbumContract[] GetAlbums(AlbumStatus status) {

			return HandleQuery(session => session.Query<MikuDbAlbum>()
				.Where(a => a.Status == status)
				.Select(a => new MikuDbAlbumContract(a))
				.ToArray());

		}

		public PictureContract GetCoverPicture(int id) {

			return HandleQuery(session => {

				var album = session.Load<MikuDbAlbum>(id);

				if (album.CoverPicture != null)
					return new PictureContract(album.CoverPicture, Size.Empty);
				else
					return null;

			});

		}

		public int ImportNew() {

			PermissionContext.VerifyPermission(PermissionFlags.MikuDbImport);

			MikuDbAlbumContract[] existing = HandleQuery(session => session.Query<MikuDbAlbum>().Select(a => new MikuDbAlbumContract(a)).ToArray());

			var importer = new AlbumImporter(existing);
			var imported = importer.ImportNew();

			return HandleTransaction(session => {

				AuditLog("importing new albums from MikuDB", session);

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

		public void ImportOne(string url) {

			PermissionContext.VerifyPermission(PermissionFlags.MikuDbImport);

			MikuDbAlbumContract[] existing = HandleQuery(session => session.Query<MikuDbAlbum>().Select(a => new MikuDbAlbumContract(a)).ToArray());

			var importer = new AlbumImporter(existing);
			var imported = importer.ImportOne(url);

			if (imported.AlbumContract == null)
				return;

			HandleTransaction(session => {

				AuditLog(string.Format("importing album from MikuDB with URL '{0}'", url));

				var newAlbum = new MikuDbAlbum(imported.AlbumContract);
				session.Save(newAlbum);

			});

		} 

		public InspectedAlbum[] Inspect(int[] importedAlbumIds) {

			ParamIs.NotNull(() => importedAlbumIds);

			return HandleQuery(session => Inspect(session, importedAlbumIds));

		}

		public void SkipAlbum(int importedAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.MikuDbImport);

			UpdateEntity<MikuDbAlbum>(importedAlbumId, a => a.Status = AlbumStatus.Skipped);

		}

	}
}
