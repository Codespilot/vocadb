using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
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
			
			Album album;
			var diff = new AlbumDiff();

			if (acceptedAlbum.MergedAlbum == null) {

				album = new Album(acceptedAlbum.ImportedAlbum.Title);
				album.DiscType = DiscType.Unknown;
				diff.Names = true;
				session.Save(album);

			} else {
				album = session.Load<Album>(acceptedAlbum.MergedAlbum.Id);
			}

			foreach (var inspectedArtist in acceptedAlbum.Artists) {

				if (inspectedArtist.ExistingArtist != null) {

					var artist = session.Load<Artist>(inspectedArtist.ExistingArtist.Id);

					if (!artist.HasAlbum(album)) {
						session.Save(artist.AddAlbum(album));
						diff.Artists = true;
					}

				} else {
					album.AddArtist(inspectedArtist.Name, false, ArtistRoles.Default);
					diff.Artists = true;
				}

			}

			if (!album.Songs.Any()) {

				foreach (var inspectedTrack in acceptedAlbum.Tracks) {

					Song song;

					if (inspectedTrack.ExistingSong == null || !selectedSongIds.Contains(inspectedTrack.ExistingSong.Id)) {

						song = new Song(inspectedTrack.ImportedTrack.Title);
						session.Save(song);
						album.AddSong(song, inspectedTrack.ImportedTrack.TrackNum, inspectedTrack.ImportedTrack.DiscNum);

						Services.Songs.Archive(session, song, SongArchiveReason.AutoImportedFromMikuDb, 
							"Auto-imported from MikuDB for album '" + album.DefaultName + "'");

						session.Update(song);

					} else {
						song = session.Load<Song>(inspectedTrack.ExistingSong.Id);
						if (!album.HasSong(song))
							session.Save(album.AddSong(song, inspectedTrack.ImportedTrack.TrackNum, inspectedTrack.ImportedTrack.DiscNum));
					}

				}

				diff.Tracks = true;
	
			}

			var importedAlbum = session.Load<MikuDbAlbum>(acceptedAlbum.ImportedAlbum.Id);
			importedAlbum.Status = AlbumStatus.Approved;

			if (importedAlbum.CoverPicture != null && album.CoverPicture == null) {
				album.CoverPicture = importedAlbum.CoverPicture;
				diff.Cover = true;
			}

			if (acceptedAlbum.ImportedAlbum.Data.ReleaseYear != null && album.OriginalReleaseDate.Year == null) {
				album.OriginalReleaseDate.Year = acceptedAlbum.ImportedAlbum.Data.ReleaseYear;
				diff.OriginalRelease = true;
			}

			if (!album.WebLinks.Any(w => w.Url.Contains("mikudb.com"))) {
				album.CreateWebLink("MikuDB", acceptedAlbum.ImportedAlbum.SourceUrl);
				diff.WebLinks = true;
			}

			album.UpdateArtistString();

			AuditLog(string.Format("accepted imported album '{0}'", acceptedAlbum.ImportedAlbum.Title), session);
			AddEntryEditedEntry(session, album, EntryEditEvent.Created);

			Services.Albums.Archive(session, album, diff, AlbumArchiveReason.AutoImportedFromMikuDb);

			session.Update(album);
			session.Update(importedAlbum);

			return new AlbumContract(album, PermissionContext.LanguagePreference);

		}

		private Album[] FindAlbums(ISession session, MikuDbAlbum imported) {

			var webLinkMatches = session.Query<AlbumWebLink>()
				.Where(w => w.Url == imported.SourceUrl)
				.Select(w => w.Album)
				.ToArray();

			var nameMatchDirect = session.Query<Album>()
				.Where(s => !s.Deleted
				&& ((s.Names.SortNames.English == imported.Title)
					|| (s.Names.SortNames.Romaji == imported.Title)
					|| (s.Names.SortNames.Japanese == imported.Title)))
				.ToArray();

			var nameMatchAdditional = session.Query<AlbumName>()
				.Where(m => !m.Album.Deleted && m.Value == imported.Title)
				.Select(a => a.Album)
				.ToArray();

			return webLinkMatches.Union(nameMatchDirect).Union(nameMatchAdditional).ToArray();

		}

		private Artist FindArtist(ISession session, string artistName) {

			if (string.IsNullOrEmpty(artistName))
				return null;

			if (artistName.EndsWith("P"))
				artistName = artistName.Substring(0, artistName.Length - 1);

			var additionalNames = session.Query<ArtistName>()
				.FirstOrDefault(m => !m.Artist.Deleted && (m.Value == artistName || m.Value == artistName + "P"));

			if (additionalNames != null)
				return additionalNames.Artist;

			return null;

		}

		/// <summary>
		/// Finds the best match for a song.
		/// </summary>
		/// <param name="songs"></param>
		/// <param name="artists"></param>
		/// <returns></returns>
		private Song FindMatch(Song[] songs, IEnumerable<Artist> artists) {

			if (songs.Length == 0)
				return null;

			if (songs.Length == 1)
				return songs.First();

			var match = songs.FirstOrDefault(s => s.Artists.Any(a => a.Artist.ArtistType != ArtistType.Vocaloid && artists.Any(a2 => a.Artist.Equals(a2))));
			return match ?? songs.First();

		}

		private Song FindSong(ISession session, string songName, IEnumerable<Artist> artists) {

			var nameMatch = session.Query<SongName>()
				.Where(m => !m.Song.Deleted && m.Value == songName)
				.Select(a => a.Song)
				.ToArray();

			if (nameMatch.Any())
				return FindMatch(nameMatch, artists);

			var direct = session.Query<Song>()
				.Where(
					s => !s.Deleted &&
					(s.Names.SortNames.English == songName
						|| s.Names.SortNames.Romaji == songName
						|| s.Names.SortNames.Japanese == songName))
				.ToArray();

			if (direct.Any())
				return FindMatch(direct, artists);

			return null;

		}

		private InspectedAlbum[] Inspect(ISession session, ImportedAlbumOptions[] importedAlbumIds) {

			var ids = importedAlbumIds.Select(i => i.ImportedDbAlbumId).ToArray();
			var importedAlbums = session.Query<MikuDbAlbum>().Where(a => ids.Contains(a.Id)).ToArray();

			return importedAlbums.Select(s => Inspect(session, s, importedAlbumIds.First(a => a.ImportedDbAlbumId == s.Id).MergedAlbumId)).ToArray();

		}

		private InspectedAlbum Inspect(ISession session, MikuDbAlbum imported, int? mergedAlbumId) {

			Album albumMatch;
			var foundAlbums = FindAlbums(session, imported);
			switch (mergedAlbumId) {
				case null:
					albumMatch = foundAlbums.FirstOrDefault();
					break;
				case 0:
					albumMatch = null;
					break;
				default:
					albumMatch = session.Load<Album>(mergedAlbumId);
					//foundAlbums = foundAlbums.Union(new[] { albumMatch }).ToArray();
					break;
			}

			var importedContract = new MikuDbAlbumContract(imported);
			var data = importedContract.Data;

			var artists = data.ArtistNames.Concat(data.VocalistNames).Concat(!string.IsNullOrEmpty(data.CircleName) ? new[] { data.CircleName } : new string[] {})
				.Select(a => InspectArtist(session, a))
				.ToArray();

			var matchedArtists = artists.Where(a => a.ExistingArtist != null).Select(a => a.ExistingArtist).ToArray();

			InspectedTrack[] tracks = null;

			if (albumMatch == null || !albumMatch.Songs.Any())
				tracks = data.Tracks.Select(t => InspectTrack(session, t, matchedArtists)).ToArray();


			var result = new InspectedAlbum(importedContract);

			if (albumMatch != null) {
				result.MergedAlbum = new AlbumWithAdditionalNamesContract(albumMatch, PermissionContext.LanguagePreference);
				result.MergedAlbumId = albumMatch.Id;
			}

			result.ExistingAlbums = foundAlbums.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
				.Concat(new[] { new AlbumWithAdditionalNamesContract { Name = "Nothing" } }).ToArray();

			result.Artists = artists.Select(a => a.InspectedArtist).ToArray();
			result.Tracks = tracks;

			return result;

		}

		private ArtistInspectResult InspectArtist(ISession session, string name) {

			//var inspected = new InspectedArtist(name);

			var matched = FindArtist(session, name);

			//if (matched != null)
			//	inspected.ExistingArtist = new ArtistWithAdditionalNamesContract(matched, PermissionContext.LanguagePreference);

			return new ArtistInspectResult(name, matched, PermissionContext.LanguagePreference);

		}

		private InspectedTrack InspectTrack(ISession session, ImportedAlbumTrack importedTrack, IEnumerable<Artist> artists) {

			var inspected = new InspectedTrack(importedTrack);
			var existingSong = FindSong(session, importedTrack.Title, artists);

			if (existingSong != null)
				inspected.ExistingSong = new SongWithAdditionalNamesContract(existingSong, PermissionContext.LanguagePreference);

			return inspected;

		}

		public AlbumContract[] AcceptImportedAlbums(ImportedAlbumOptions[] importedAlbumIds, int[] selectedSongIds) {

			PermissionContext.VerifyPermission(PermissionToken.MikuDbImport);

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

			DeleteEntity<MikuDbAlbum>(importedAlbumId, PermissionToken.MikuDbImport);

		}

		public MikuDbAlbumContract[] GetAlbums(AlbumStatus status, int start, int maxEntries) {

			return HandleQuery(session => session
				.Query<MikuDbAlbum>()
				.Where(a => a.Status == status)
				.OrderByDescending(a => a.Created)
				.Skip(start)
				.Take(maxEntries)
				.ToArray()
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

			PermissionContext.VerifyPermission(PermissionToken.MikuDbImport);

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

			PermissionContext.VerifyPermission(PermissionToken.MikuDbImport);

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

		public InspectedAlbum[] Inspect(ImportedAlbumOptions[] importedAlbumIds) {

			ParamIs.NotNull(() => importedAlbumIds);

			return HandleQuery(session => Inspect(session, importedAlbumIds));

		}

		public void SkipAlbum(int importedAlbumId) {

			UpdateEntity<MikuDbAlbum>(importedAlbumId, a => a.Status = AlbumStatus.Skipped, PermissionToken.MikuDbImport);

		}

	}

	public class ArtistInspectResult {

		public ArtistInspectResult(string name, Artist existing, ContentLanguagePreference languagePreference) {

			InspectedArtist = new InspectedArtist(name);

			ExistingArtist = existing;

			if (existing != null)
				InspectedArtist.ExistingArtist = new ArtistWithAdditionalNamesContract(existing, languagePreference);

		}

		public Artist ExistingArtist { get; set; }

		public InspectedArtist InspectedArtist { get; set; }

	}

}
