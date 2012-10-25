﻿using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
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

		private AlbumContract AcceptImportedAlbum(ISession session, ContentLanguageSelection languageSelection, 
			InspectedAlbum acceptedAlbum, int[] selectedSongIds) {
			
			Album album;
			var diff = new AlbumDiff(false);

			if (acceptedAlbum.MergedAlbum == null) {

				album = new Album(new LocalizedString(acceptedAlbum.ImportedAlbum.Title, languageSelection));
				album.DiscType = DiscType.Unknown;
				diff.Names = true;
				session.Save(album);

			} else {

				album = session.Load<Album>(acceptedAlbum.MergedAlbum.Id);

				if (!album.Names.HasName(acceptedAlbum.ImportedAlbum.Title)) {
					album.CreateName(acceptedAlbum.ImportedAlbum.Title, languageSelection);
					diff.Names = true;
				}

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

			if (acceptedAlbum.MergedAlbum == null || acceptedAlbum.MergeTracks) {

				foreach (var inspectedTrack in acceptedAlbum.Tracks) {
					if (AcceptImportedSong(session, album, inspectedTrack, languageSelection, selectedSongIds))
						diff.Tracks = true;
				}

			}

			var importedAlbum = session.Load<MikuDbAlbum>(acceptedAlbum.ImportedAlbum.Id);
			importedAlbum.Status = AlbumStatus.Approved;

			if (importedAlbum.CoverPicture != null && album.CoverPictureData == null) {
				album.CoverPictureData = importedAlbum.CoverPicture;
				diff.Cover = true;
			}

			if (acceptedAlbum.ImportedAlbum.Data.ReleaseYear != null && album.OriginalReleaseDate.Year == null) {
				album.OriginalReleaseDate.Year = acceptedAlbum.ImportedAlbum.Data.ReleaseYear;
				diff.OriginalRelease = true;
			}

			if (!album.WebLinks.Any(w => w.Url.Contains("mikudb.com"))) {
				album.CreateWebLink("MikuDB", acceptedAlbum.ImportedAlbum.SourceUrl, WebLinkCategory.Reference);
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

		private bool AcceptImportedSong(ISession session, Album album, InspectedTrack inspectedTrack, 
			ContentLanguageSelection languageSelection, int[] selectedSongIds) {

			Song song = null;
			var diff = new SongDiff(false);

			if (NewTrack(inspectedTrack, selectedSongIds, album)) {

				song = new Song(new LocalizedString(inspectedTrack.ImportedTrack.Title, languageSelection));
				session.Save(song);
				album.AddSong(song, inspectedTrack.ImportedTrack.TrackNum, inspectedTrack.ImportedTrack.DiscNum);
				diff.Names = true;

			} else if (selectedSongIds.Contains(inspectedTrack.ExistingSong.Id)) {

				song = session.Load<Song>(inspectedTrack.ExistingSong.Id);

				if (!album.HasSong(song))
					album.AddSong(song, inspectedTrack.ImportedTrack.TrackNum, inspectedTrack.ImportedTrack.DiscNum);

				var newName = inspectedTrack.ImportedTrack.Title;

				if (!song.Names.HasName(newName) && !song.Names.HasNameForLanguage(languageSelection)) {
					song.CreateName(new LocalizedString(newName, languageSelection));
					diff.Names = true;
				}

			}

			if (song != null) {

				if (inspectedTrack.ImportedTrack != null) {
					foreach (var artistName in inspectedTrack.ImportedTrack.ArtistNames) {
						if (CreateArtist(session, song, artistName, ArtistRoles.Composer))
							diff.Artists = true;
					}

					foreach (var artistName in inspectedTrack.ImportedTrack.VocalistNames) {
						if (CreateArtist(session, song, artistName, ArtistRoles.Vocalist))
							diff.Artists = true;
					}

					song.UpdateArtistString();

				}

				Services.Songs.Archive(session, song, diff, SongArchiveReason.AutoImportedFromMikuDb,
					string.Format("Auto-imported from MikuDB for album '{0}'", album.DefaultName));

				session.Update(song);
				return true;

			}

			return false;

		}

		private bool CreateArtist(ISession session, Song song, string name, ArtistRoles roles) {

			var artist = FindArtist(session, name);
			var link = (artist != null ? new ArtistForSong(song, artist, false, roles) : new ArtistForSong(song, name, false, roles));

			if (!song.HasArtistLink(link)) {
				song.AllArtists.Add(link);
				return true;
			}

			return false;

		}

		private List<Album> FindAlbums(ISession session, MikuDbAlbum imported) {

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

			return webLinkMatches.Union(nameMatchDirect).Union(nameMatchAdditional).ToList();

		}

		private Artist FindArtist(ISession session, string artistName) {

			if (string.IsNullOrEmpty(artistName))
				return null;

			var artistMatch = QueryArtist(session, artistName);

			if (artistMatch == null) {

				var normalized = artistName.Normalize(NormalizationForm.FormKC);
				var compositeRegex = new Regex(@"\S+\s?\((.+)\)");	// catch name(name)
				var match = compositeRegex.Match(normalized);

				if (!match.Success) {
					compositeRegex = new Regex(@"\S+\s?[/@](.+)");	// catch name/name and name@name
					match = compositeRegex.Match(normalized);
				}

				if (match.Success) {

					var resolvedName = match.Groups[1].Value;
					artistMatch = QueryArtist(session, resolvedName);

				}

			}

			return artistMatch;

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

			return importedAlbumIds.Select(a => Inspect(session, a)).ToArray();

		}

		private InspectedAlbum Inspect(ISession session, ImportedAlbumOptions options) {

			var imported = session.Load<MikuDbAlbum>(options.ImportedDbAlbumId);

			return Inspect(session, imported, options);

		}

		private InspectedAlbum Inspect(ISession session, MikuDbAlbum imported, ImportedAlbumOptions options) {

			Album albumMatch;
			var foundAlbums = FindAlbums(session, imported);
			switch (options.MergedAlbumId) {
				case null:
					albumMatch = foundAlbums.FirstOrDefault();
					break;
				case 0:
					albumMatch = null;
					break;
				default:
					albumMatch = session.Load<Album>(options.MergedAlbumId);
					if (!foundAlbums.Contains(albumMatch))
						foundAlbums.Add(albumMatch);
					break;
			}

			var importedContract = new MikuDbAlbumContract(imported);
			var data = importedContract.Data;

			var artists = data.ArtistNames.Concat(data.VocalistNames).Concat(!string.IsNullOrEmpty(data.CircleName) ? new[] { data.CircleName } : new string[] {})
				.Select(a => InspectArtist(session, a))
				.ToArray();

			var matchedArtists = artists.Where(a => a.ExistingArtist != null).Select(a => a.ExistingArtist).ToArray();

			var tracks = data.Tracks.Select(t => InspectTrack(session, t, matchedArtists, albumMatch)).ToArray();

			var result = new InspectedAlbum(importedContract);
			result.MergeTracks = options.MergeTracks ?? true;

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

		private InspectedTrack InspectTrack(ISession session, ImportedAlbumTrack importedTrack, IEnumerable<Artist> artists, Album album) {

			var inspected = new InspectedTrack(importedTrack);
			var existingTrack = album != null ? album.GetSongByTrackNum(importedTrack.DiscNum, importedTrack.TrackNum) : null;

			var existingSong = existingTrack != null ? existingTrack.Song
				: FindSong(session, importedTrack.Title, artists);

			if (existingSong != null)
				inspected.ExistingSong = new SongWithAdditionalNamesContract(existingSong, PermissionContext.LanguagePreference);

			inspected.Selected = existingTrack != null;

			return inspected;

		}

		private bool NewTrack(InspectedTrack inspectedTrack, int[] selectedSongIds, Album album) {

			if (inspectedTrack.ExistingSong == null)
				return true;

			var albumTrack = album.GetSongByTrackNum(inspectedTrack.ImportedTrack.DiscNum, inspectedTrack.ImportedTrack.TrackNum);
			if (albumTrack != null)
				return false;

			return !selectedSongIds.Contains(inspectedTrack.ExistingSong.Id);

		}

		private Artist QueryArtist(ISession session, string artistName) {

			artistName = ArtistHelper.GetCanonizedName(artistName);

			var artist = session.Query<ArtistName>()
				.Where(m => !m.Artist.Deleted)
				.FilterByArtistName(artistName, artistName, NameMatchMode.Exact)
				.Select(an => an.Artist)
				.FirstOrDefault();

			if (artist == null)
				artist = session.Query<ArtistName>()
					.Where(m => !m.Artist.Deleted)
					.FilterByArtistName(artistName, artistName, NameMatchMode.Words)
					.Select(an => an.Artist)
					.FirstOrDefault();

			return artist;

		}

		public AlbumContract[] AcceptImportedAlbums(ImportedAlbumOptions[] importedAlbumIds, int[] selectedSongIds) {

			PermissionContext.VerifyPermission(PermissionToken.MikuDbImport);

			return HandleTransaction(session => {

				var importedAlbums = new List<AlbumContract>(importedAlbumIds.Length);

				foreach (var importedAlbum in importedAlbumIds) {

					var inspected = Inspect(session, importedAlbum);
					var album = AcceptImportedAlbum(session, importedAlbum.SelectedLanguage, inspected, selectedSongIds);

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

		public int ImportFromFile(Stream stream) {

			PermissionContext.VerifyPermission(PermissionToken.MikuDbImport);

			var parser = new AlbumFileParser();
			var imported = parser.Parse(stream);

			return HandleTransaction(session => {

				AuditLog(string.Format("importing album from file with name '{0}'", imported.Title));

				var newAlbum = new MikuDbAlbum(imported);
				session.Save(newAlbum);

				return newAlbum.Id;

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
