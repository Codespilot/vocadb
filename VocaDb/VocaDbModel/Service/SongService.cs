using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using log4net;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

#pragma warning disable 169
		private static readonly ILog log = LogManager.GetLogger(typeof(SongService));
#pragma warning restore 169

		private int GetSongCount(ISession session, string query, bool onlyByName = false) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Song>()
					.Where(s => !s.Deleted)
					.Count();

			} else {

				var directQ = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (query.Length < 3) {

					directQ = directQ.Where(s =>
						s.Names.SortNames.English == query
							|| s.Names.SortNames.Romaji == query
							|| s.Names.SortNames.Japanese == query);

				} else {

					directQ = directQ.Where(s =>
						s.Names.SortNames.English.Contains(query)
							|| s.Names.SortNames.Romaji.Contains(query)
							|| s.Names.SortNames.Japanese.Contains(query)
							|| (!onlyByName && 
								(s.ArtistString.Japanese.Contains(query)
								|| s.ArtistString.Romaji.Contains(query)
								|| s.ArtistString.English.Contains(query)))
							|| (s.NicoId != null && s.NicoId == query));

				}

				var direct = directQ.ToArray();

				var additionalNamesQ = session.Query<SongName>()
					.Where(m => !m.Song.Deleted);
					
				if (query.Length < 3) {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

				} else {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

				}

				var additionalNames = additionalNamesQ
					.Select(m => m.Song)
					.Distinct()
					.ToArray()
					.Where(a => !direct.Contains(a))
					.ToArray();

				return direct.Count() + additionalNames.Count();
			}

		}

		private VideoUrlParseResult ParsePV(ISession session, string url) {

			if (string.IsNullOrEmpty(url))
				return null;

			var pvResult = VideoServiceHelper.ParseByUrl(url);

			var existing = session.Query<PVForSong>().FirstOrDefault(
				s => s.Service == pvResult.Service && s.PVId == pvResult.Id && !s.Song.Deleted);

			if (existing != null) {
				throw new VideoParseException(string.Format("Song '{0}' already contains this PV",
					existing.Song.TranslatedName[PermissionContext.LanguagePreference]));
			}

			return pvResult;

		}

		public SongService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) {

		}

		public ArtistForSongContract AddArtist(int songId, int artistId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("linking {0} to {1}", artist, song), session);

				var artistForSong = artist.AddSong(song);
				session.Save(artistForSong);

				song.UpdateArtistString();
				session.Update(song);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public ArtistForSongContract AddArtist(int songId, string newArtistName) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog(string.Format("creating new artist {0} to {1}", newArtistName, song), session);

				var artist = new Artist(newArtistName);

				var artistForSong = artist.AddSong(song);
				session.Save(artist);

				song.UpdateArtistString();
				session.Update(song);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public void Archive(ISession session, Song song, SongDiff diff, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Song song, string notes = "") {

			Archive(session, song, new SongDiff(), notes);

		}

		public SongContract Create(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				AuditLog("creating a new song with name " + name, session);

				var song = new Song(name);

				session.Save(song);

				Archive(session, song, "Created");
				session.Update(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public SongInAlbumContract CreateForAlbum(int albumId, string newSongName) {

			ParamIs.NotNullOrEmpty(() => newSongName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("creating a new song '{0}' to {1}", newSongName, album), session);

				var song = new Song(newSongName);
				session.Save(song);

				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				Archive(session, song, string.Format("Created for album '{0}'", album.Name));

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public PVContract CreatePVForSong(int songId, PVService service, string pvId, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvId);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				AuditLog("creating a PV for " + song, session);

				var pv = song.CreatePV(service, pvId, pvType);
				session.Save(pv);

				if (service == PVService.NicoNicoDouga && pvType == PVType.Original) {
					song.UpdateNicoId();
					session.Update(song);
				}

				return new PVContract(pv);

			});

		}

		public PVContract CreatePVForSong(int songId, string pvUrl, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			var result = VideoServiceHelper.ParseByUrl(pvUrl);

			return CreatePVForSong(songId, result.Service, result.Id, pvType);

		}

		public SongContract Create(CreateSongContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var pvResult = ParsePV(session, contract.PVUrl);
				var reprintPvResult = ParsePV(session, contract.ReprintPVUrl);

				AuditLog(string.Format("creating a new song with name '{0}'", contract.Names.First().Value), session);

				var song = new Song { SongType = contract.SongType };

				song.Names.Init(contract.Names, song);

				session.Save(song);

				foreach (var artist in contract.Artists) {
					session.Save(song.AddArtist(session.Load<Artist>(artist.Id)));
				}

				if (pvResult != null) {
					session.Save(song.CreatePV(pvResult.Service, pvResult.Id, PVType.Original));
				}

				if (reprintPvResult != null) {
					session.Save(song.CreatePV(reprintPvResult.Service, reprintPvResult.Id, PVType.Reprint));
				}

				song.UpdateArtistString();
				Archive(session, song, "Created");
				session.Update(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.DeleteEntries);

			UpdateEntity<Song>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", a), session);

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			}, skipLog: true);

		}

		public void DeleteArtistForSong(int artistForSongId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var artistForSong = session.Load<ArtistForSong>(artistForSongId);

				AuditLog("deleting " + artistForSong, session);

				artistForSong.Song.DeleteArtistForSong(artistForSong);
				session.Delete(artistForSong);
				session.Update(artistForSong.Song);

			});

		}

		public void DeletePvForSong(int pvForSongId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var pvForSong = session.Load<PVForSong>(pvForSongId);

				AuditLog("deleting " + pvForSong, session);

				pvForSong.OnDelete();

				session.Delete(pvForSong);
				session.Update(pvForSong.Song);

			});

		}

		public PartialFindResult<SongWithAdditionalNamesContract> Find(string query, int start, int maxResults, 
			bool getTotalCount = false, NameMatchMode nameMatchMode = NameMatchMode.Auto, bool onlyByName = false) {

			return HandleQuery(session => {

				SongWithAdditionalNamesContract[] contracts;

				if (string.IsNullOrEmpty(query)) {

					contracts = session.Query<Song>()
						.Where(s => !s.Deleted)
						.OrderBy(s => s.Names.SortNames.Romaji)
						.Skip(start)
						.Take(maxResults)
						.ToArray()
						.Select(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
						.ToArray();

				} else {

					var directQ = session.Query<Song>()
						.Where(s => !s.Deleted);

					if (nameMatchMode == NameMatchMode.Exact || (nameMatchMode == NameMatchMode.Auto && query.Length < 3)) {

						directQ = directQ.Where(s =>
							s.Names.SortNames.English == query
								|| s.Names.SortNames.Romaji == query
								|| s.Names.SortNames.Japanese == query);

					} else {

						directQ = directQ.Where(s =>
							s.Names.SortNames.English.Contains(query)
								|| s.Names.SortNames.Romaji.Contains(query)
								|| s.Names.SortNames.Japanese.Contains(query)
							|| (!onlyByName &&
								(s.ArtistString.Japanese.Contains(query)
								|| s.ArtistString.Romaji.Contains(query)
								|| s.ArtistString.English.Contains(query)))
							|| (s.NicoId != null && s.NicoId == query));

					}

					var direct = directQ
						.OrderBy(s => s.Names.SortNames.Romaji)
						.ToArray();

					var additionalNamesQ = session.Query<SongName>()
						.Where(m => !m.Song.Deleted);

					if (!string.IsNullOrEmpty(query) && query.Length < 3) {

						additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

					} else if (!string.IsNullOrEmpty(query)) {

						additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

					}

					var additionalNames = additionalNamesQ
						.Select(m => m.Song)
						.OrderBy(s => s.Names.SortNames.Romaji)
						.Distinct()
						//.Take(maxResults)
						.ToArray()
						.Where(a => !direct.Contains(a));

					contracts = direct.Concat(additionalNames)
						.Skip(start)
						.Take(maxResults)
						.Select(a => new SongWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
						.ToArray();

				}

				int count = (getTotalCount ? GetSongCount(session, query, onlyByName) : 0);

				return new PartialFindResult<SongWithAdditionalNamesContract>(contracts, count);

			});

		}

		public SongContract[] FindByName(string term, int maxResults) {

			return HandleQuery(session => {

				var direct = session.Query<Song>()
					.Where(s =>
						!s.Deleted &&
						(string.IsNullOrEmpty(term)
							|| s.Names.SortNames.English.Contains(term)
							|| s.Names.SortNames.Romaji.Contains(term)
							|| s.Names.SortNames.Japanese.Contains(term)
						|| (s.NicoId != null && s.NicoId == term)))
					.Take(maxResults)
					.ToArray();

				var additionalNames = session.Query<SongName>()
					.Where(m => m.Value.Contains(term) && !m.Song.Deleted)
					.Select(m => m.Song)
					.Distinct()
					.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				return direct.Concat(additionalNames)
					.Take(maxResults)
					.Select(a => new SongContract(a, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public LyricsForSongContract GetRandomSongWithLyricsDetails() {

			return HandleQuery(session => {

				var ids = session.Query<LyricsForSong>().Select(s => s.Id).ToArray();
				var id = ids[new Random().Next(ids.Length)];

				return new LyricsForSongContract(session.Load<LyricsForSong>(id));

			});

		}

		public SongContract GetSong(int id) {

			return HandleQuery(
				session => new SongContract(session.Load<Song>(id), PermissionContext.LanguagePreference));

		}

		public int GetSongCount(string query) {

			return HandleQuery(session => {

				return GetSongCount(session, query);

				/*if (string.IsNullOrWhiteSpace(query)) {

					return session.Query<Song>()
						.Where(s => !s.Deleted)
						.Count();

				}

				//var artistForSong = (artistId != null ? session.Query<ArtistForSong>()
				//	.Where(a => a.Artist.Id == artistId).Select(a => a.Song).ToArray() : null);

				var direct = session.Query<Song>()
					.Where(s => 
						!s.Deleted &&
						(string.IsNullOrEmpty(query)
							|| s.Names.SortNames.English.Contains(query)
							|| s.Names.SortNames.Romaji.Contains(query)
							|| s.Names.SortNames.Japanese.Contains(query)
						|| (s.ArtistString.Contains(query))
						|| (s.NicoId == null || s.NicoId == query)))
					.ToArray();

				var additionalNames = session.Query<SongName>()
					.Where(m => m.Value.Contains(query) && !m.Song.Deleted)
					.Select(m => m.Song)
					.Distinct()
					.ToArray()
					.Where(a => !direct.Contains(a))
					.ToArray();

				return direct.Count() + additionalNames.Count();*/

			});

		}

		public SongDetailsContract GetSongDetails(int songId) {

			return HandleQuery(session => {

				var contract = new SongDetailsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference);

				if (PermissionContext.LoggedUser != null)
					contract.IsFavorited = session.Query<FavoriteSongForUser>()
						.Any(s => s.Song.Id == songId && s.User.Id == PermissionContext.LoggedUser.Id);

				return contract;

			});

		}

		public SongForEditContract GetSongForEdit(int songId) {

			return HandleQuery(session => new SongForEditContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public SongWithArchivedVersionsContract GetSongWithArchivedVersions(int songId) {

			return HandleQuery(session => new SongWithArchivedVersionsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public SongContract[] GetSongs(string filter, int start, int count) {

			return HandleQuery(session => session.Query<Song>()
				.Where(s => string.IsNullOrEmpty(filter)
					|| s.Names.SortNames.Japanese.Contains(filter) 
					|| s.NicoId == filter)
				.OrderBy(s => s.Names.SortNames.Japanese)
				.Skip(start)
				.Take(count)
				.ToArray()
				.Select(s => new SongContract(s, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionFlags.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Song>(sourceId);
				var target = session.Load<Song>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", source, target), session);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url);
					session.Save(link);
				}

				var pvs = source.PVs.Where(a => !target.HasPV(a.Service, a.PVId));
				foreach (var p in pvs) {
					var pv = target.CreatePV(p.Service, p.PVId, p.PVType);
					session.Save(pv);
				}

				var artists = source.Artists.Where(a => !target.HasArtist(a.Artist)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				var albums = source.Albums.Where(s => !target.IsOnAlbum(s.Album)).ToArray();
				foreach (var s in albums) {
					s.Move(target);
					session.Update(s);
				}

				var userFavorites = source.UserFavorites.Where(a => !target.IsFavoritedBy(a.User)).ToArray();
				foreach (var u in userFavorites) {
					u.Move(target);
					session.Update(u);
				}

				source.Deleted = true;

				target.UpdateArtistString();
				target.UpdateNicoId();

				Archive(session, source, "Merged to '" + target.DefaultName + "'");
				Archive(session, target, "Merged from '" + source.DefaultName + "'");

				session.Update(source);
				session.Update(target);

			});

		}

		public void UpdateArtists(int songId, int[] artistIds) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog("updating artists for " + song, session);

				var oldArtists = song.Artists.Select(a => a.Artist).ToArray();
				var artists = session.Query<Artist>().Where(a => artistIds.Contains(a.Id)).ToArray();

				var diff = CollectionHelper.Diff(oldArtists, artists, (a, a2) => a.Id == a2.Id);

				foreach (var added in diff.Added)
					session.Save(song.AddArtist(added));

				foreach (var removed in diff.Removed) {
					var link = song.RemoveArtist(removed);
					if (link != null)
						session.Delete(link);
				}

				if (diff.Added.Any() || diff.Removed.Any()) {
					song.UpdateArtistString();
					Archive(session, song, "Updated artists");
					session.Update(song);
				}

			});

		}

		public SongForEditContract UpdateBasicProperties(SongForEditContract properties) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(properties.Song.Id);

				AuditLog(string.Format("updating properties for {0}", song), session);

				song.SongType = properties.Song.SongType;
				song.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;

				var nameDiff = song.Names.Sync(properties.Names, song);
				SessionHelper.Sync(session, nameDiff);

				var webLinkDiff = WebLink.Sync(song.WebLinks, properties.WebLinks, song);
				SessionHelper.Sync(session, webLinkDiff);

				Archive(session, song, new SongDiff { IncludeLyrics = false }, "Updated properties");

				session.Update(song);
				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

		public SongForEditContract UpdateLyrics(int songId, IEnumerable<LyricsForSongContract> lyrics) {
			
			ParamIs.NotNull(() => lyrics);

			var validLyrics = lyrics.Where(l => !string.IsNullOrEmpty(l.Value));

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog("updating lyrics for " + song, session);

				var deleted = song.Lyrics.Where(l => !validLyrics.Any(l2 => l.Id == l2.Id)).ToArray();

				foreach (var l in deleted) {
					song.Lyrics.Remove(l);
					session.Delete(l);
				}

				foreach (var entry in validLyrics) {

					var entry1 = entry;
					var old = (entry1.Id != 0 ? song.Lyrics.FirstOrDefault(l => l.Id == entry1.Id) : null);

					if (old != null) {

						old.Language = entry.Language;
						old.Source = entry.Source;
						old.Value = entry.Value;
						session.Update(old);

					} else {

						var l = song.CreateLyrics(entry.Language, entry.Value, entry.Source);
						session.Save(l);

					}

				}

				Archive(session, song, "Updated lyrics");
				session.Update(song);

				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

	}

}
