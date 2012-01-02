using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using log4net;
using VocaDb.Model.Domain.Tags;
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

		/// <summary>
		/// Finds songs based on criteria.
		/// </summary>
		/// <param name="session">Open session. Canot be null.</param>
		/// <param name="query">Query search string. Can be null or empty, in which case no filtering by name is done.</param>
		/// <param name="start">0-based order number of the first item to be returned.</param>
		/// <param name="maxResults">Maximum number of results to be returned.</param>
		/// <param name="draftsOnly">Whether to return only entries with a draft status.</param>
		/// <param name="getTotalCount">Whether to return the total number of entries matching the criteria.</param>
		/// <param name="nameMatchMode">Mode for name maching. Ignored when query string is null or empty.</param>
		/// <param name="onlyByName">Whether to search items only by name, and not for example NicoId. Ignored when query string is null or empty.</param>
		/// <param name="moveExactToTop">Whether to move exact match to the top of search results.</param>
		/// <param name="ignoreIds">List of entries to be ignored. Can be null in which case no filtering is done.</param>
		/// <returns></returns>
		private PartialFindResult<SongWithAdditionalNamesContract> Find(ISession session, string query, int start, int maxResults,
			bool draftsOnly, bool getTotalCount, NameMatchMode nameMatchMode, bool onlyByName, bool moveExactToTop, int[] ignoreIds) {

			var originalQuery = query;

			SongWithAdditionalNamesContract[] contracts;
			bool foundExactMatch = false;
			ignoreIds = ignoreIds ?? new int[] { };

			if (string.IsNullOrWhiteSpace(query)) {

				var q = session.Query<Song>()
					.Where(s => !s.Deleted 
						&& !ignoreIds.Contains(s.Id));

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				contracts = q
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Skip(start)
					.Take(maxResults)
					.ToArray()
					.Select(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

			} else {

				query = query.Trim();

				var directQ = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

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

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Song.Status == EntryStatus.Draft);

				additionalNamesQ = FindHelpers.AddEntryNameFilter(additionalNamesQ, query, nameMatchMode);

				var additionalNames = additionalNamesQ
					.Select(m => m.Song)
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Distinct()
					//.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				var entries = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults);

				if (moveExactToTop) {
					
					var exactMatch = entries.FirstOrDefault(
						e => e.Names.Any(n => n.Value.Equals(query, StringComparison.InvariantCultureIgnoreCase)));

					if (exactMatch != null) {
						entries = CollectionHelper.MoveToTop(entries, exactMatch);
						foundExactMatch = true;
					}

				}

				contracts = entries
					.Where(e => !ignoreIds.Contains(e.Id))
					.Select(a => new SongWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
					.ToArray();

			}

			int count = (getTotalCount ? GetSongCount(session, query, onlyByName, draftsOnly, nameMatchMode) : 0);

			return new PartialFindResult<SongWithAdditionalNamesContract>(contracts, count, originalQuery, foundExactMatch);

		}

		private int GetSongCount(ISession session, string query, bool onlyByName, bool draftsOnly, NameMatchMode nameMatchMode) {

			if (string.IsNullOrWhiteSpace(query)) {

				var q = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				return q.Count();

			} else {

				var directQ = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

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

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Song.Status == EntryStatus.Draft);

				additionalNamesQ = FindHelpers.AddEntryNameFilter(additionalNamesQ, query, nameMatchMode);

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

		public SongService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {

		}

		[Obsolete("Integrated to properties saving")]
		public ArtistForSongContract AddArtist(int songId, int artistId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("linking {0} to {1}", 
					EntryLinkFactory.CreateEntryLink(artist), EntryLinkFactory.CreateEntryLink(song)), session);

				var artistForSong = artist.AddSong(song);
				session.Save(artistForSong);

				song.UpdateArtistString();
				session.Update(song);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		[Obsolete("Disabled")]
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

		/*public SongContract AddAlternateVersion(int songId, int addedSongId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				var addedSong = session.Load<Song>(addedSongId);

				AuditLog("adding " + addedSong + " to " + song);

				song.AddAlternateVersion(addedSong);

				session.Update(addedSong);

				return new SongContract(addedSong, PermissionContext.LanguagePreference);

			});

		}*/

		public void Archive(ISession session, Song song, SongDiff diff, SongArchiveReason reason, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Song song, SongArchiveReason reason, string notes = "") {

			Archive(session, song, new SongDiff(), reason, notes);

		}

		public SongContract Create(string name) {

			ParamIs.NotNullOrWhiteSpace(() => name);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			name = name.Trim();

			return HandleTransaction(session => {

				AuditLog("creating a new song with name " + name, session);

				var song = new Song(name);

				session.Save(song);

				Archive(session, song, SongArchiveReason.Created);
				session.Update(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		[Obsolete("Replaced by updating properties")]
		public SongInAlbumContract CreateForAlbum(int albumId, string newSongName) {

			ParamIs.NotNullOrWhiteSpace(() => newSongName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			newSongName = newSongName.Trim();

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("creating a new song '{0}' to {1}", newSongName, album), session);

				var song = new Song(newSongName);
				session.Save(song);

				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				Archive(session, song, SongArchiveReason.Created, string.Format("Created for album '{0}'", album.DefaultName));

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public PVContract CreatePVForSong(int songId, PVService service, string pvId, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvId);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				AuditLog(string.Format("creating a PV for {0}", EntryLinkFactory.CreateEntryLink(song)), session);

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

				AuditLog(string.Format("creating a new song with name '{0}'", contract.Names.First().Value));

				var song = new Song { 
					SongType = contract.SongType, 
					Status = contract.Draft ? EntryStatus.Draft : EntryStatus.Finished 
				};

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
				Archive(session, song, SongArchiveReason.Created);
				session.Update(song);

				AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(song)), session);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.DeleteEntries);

			UpdateEntity<Song>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", EntryLinkFactory.CreateEntryLink(a)), session);

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

		public SongWithAdditionalNamesContract FindFirst(string[] query) {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session, q, 0, 1, false, false, NameMatchMode.Exact, true, false, null);

					if (result.Items.Any())
						return result.Items.First();

				}

				return null;

			});

		}

		public PartialFindResult<SongWithAdditionalNamesContract> Find(string query, int start, int maxResults, 
			bool draftOnly, bool getTotalCount, NameMatchMode nameMatchMode, bool onlyByName, int[] ignoredIds) {

			return HandleQuery(session => 
				Find(session, query, start, maxResults, draftOnly, getTotalCount, nameMatchMode, onlyByName, true, ignoredIds));

		}

		[Obsolete]
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

		public SongWithAdditionalNamesContract GetSongWithAdditionalNames(int id) {

			return HandleQuery(
				session => new SongWithAdditionalNamesContract(session.Load<Song>(id), PermissionContext.LanguagePreference));

		}

		public SongWithArchivedVersionsContract GetSongWithArchivedVersions(int songId) {

			return HandleQuery(session => new SongWithArchivedVersionsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public SongWithAdditionalNamesContract GetSongWithPV(PVService service, string pvId) {

			return HandleQuery(session => {

				var pv = session.Query<PVForSong>().FirstOrDefault(p => p.Service == service && p.PVId == pvId);

				return (pv != null ? new SongWithAdditionalNamesContract(pv.Song, PermissionContext.LanguagePreference) : null);

			});

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

		public TagSelectionContract[] GetTagSelections(int albumId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<SongTagUsage>().Where(a => a.Song.Id == albumId).ToArray();
				var tagVotes = session.Query<SongTagVote>().Where(a => a.User.Id == userId && a.Usage.Song.Id == albumId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag.Name, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public ArchivedSongVersionDetailsContract GetVersionDetails(int id) {

			return HandleQuery(session =>
				new ArchivedSongVersionDetailsContract(session.Load<ArchivedSongVersion>(id), PermissionContext.LanguagePreference));

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionFlags.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Song>(sourceId);
				var target = session.Load<Song>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", 
					EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)), session);

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

				if (target.OriginalVersion == null)
					target.OriginalVersion = source.OriginalVersion;

				var alternateVersions = source.AlternateVersions.ToArray();
				foreach (var alternate in alternateVersions) {
					alternate.OriginalVersion = target;
					session.Update(alternate);
				}

				source.Deleted = true;

				target.UpdateArtistString();
				target.UpdateNicoId();

				Archive(session, target, SongArchiveReason.Merged, "Merged from " + source);

				session.Update(source);
				session.Update(target);

			});

		}

		public void Restore(int songId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				song.Deleted = false;

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(song), session);

			});

		}

		public EntryRevertedContract RevertToVersion(int archivedSongVersionId) {

			PermissionContext.VerifyPermission(PermissionFlags.RestoreEntries);

			return HandleTransaction(session => {

				var archivedVersion = session.Load<ArchivedSongVersion>(archivedSongVersionId);
				var song = archivedVersion.Song;

				AuditLog("reverting " + song + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedSongContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				song.NicoId = fullProperties.NicoId;
				song.Notes = fullProperties.Notes;
				song.SongType = fullProperties.SongType;
				song.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;

				// Artists
				SessionHelper.RestoreObjectRefs<ArtistForSong, Artist>(
					session, warnings, song.AllArtists, fullProperties.Artists, (a1, a2) => (a1.Artist.Id == a2.Id),
					artist => (!song.HasArtist(artist) ? song.AddArtist(artist) : null),
					artistForSong => artistForSong.Delete());

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = song.Names.SyncByContent(fullProperties.Names, song);
					SessionHelper.Sync(session, nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(song.WebLinks, fullProperties.WebLinks, song);
					SessionHelper.Sync(session, webLinkDiff);
				}

				// Lyrics
				if (fullProperties.Lyrics != null) {

					var lyricsDiff = CollectionHelper.Diff(song.Lyrics, fullProperties.Lyrics, (p1, p2) => (p1.Id == p2.Id));

					foreach (var lyrics in lyricsDiff.Added) {
						session.Save(song.CreateLyrics(lyrics.Language, lyrics.Value, lyrics.Source));
					}

					foreach (var lyrics in lyricsDiff.Removed) {
						song.Lyrics.Remove(lyrics);
						session.Delete(lyrics);
					}

					foreach (var lyrics in lyricsDiff.Unchanged) {

						var newLyrics = fullProperties.Lyrics.First(l => l.Id == lyrics.Id);

						lyrics.Language = newLyrics.Language;
						lyrics.Source = newLyrics.Source;
						lyrics.Value = newLyrics.Value;
						session.Update(lyrics);

					}

				}

				// PVs
				if (fullProperties.PVs != null) {

					var pvDiff = CollectionHelper.Diff(song.PVs, fullProperties.PVs, (p1, p2) => (p1.PVId == p2.PVId && p1.Service == p2.Service));

					foreach (var pv in pvDiff.Added) {
						session.Save(song.CreatePV(pv.Service, pv.PVId, pv.PVType));
					}

					foreach (var pv in pvDiff.Removed) {
						pv.OnDelete();
						session.Delete(pv);
					}

				}

				Archive(session, song, SongArchiveReason.Reverted);
				AuditLog("reverted " + EntryLinkFactory.CreateEntryLink(song) + " to revision " + archivedVersion.Version, session);

				return new EntryRevertedContract(song, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int songId, string[] tags) {

			ParamIs.NotNull(() => tags);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				tags = tags.Distinct(new CaseInsensitiveStringComparer()).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("tagging {0} with {1}",
					EntryLinkFactory.CreateEntryLink(song), string.Join(", ", tags)), session, user);

				var existingTags = session.Query<Tag>().ToDictionary(t => t.Name, new CaseInsensitiveStringComparer());

				song.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session), new SongTagUsageFactory(session, song));

				return song.Tags.Usages.OrderByDescending(u => u.Count).Take(3).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public string UpdateArtists(int songId, int[] artistIds) {

			ParamIs.NotNull(() => artistIds);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog("updating artists for " + EntryLinkFactory.CreateEntryLink(song), session);

				var oldArtists = song.Artists.Select(a => a.Artist).ToArray();
				var artists = session.Query<Artist>().Where(a => artistIds.Contains(a.Id)).ToArray();

				var artistDiff = CollectionHelper.Diff(oldArtists, artists, (a, a2) => a.Id == a2.Id);

				foreach (var added in artistDiff.Added)
					session.Save(song.AddArtist(added));

				foreach (var removed in artistDiff.Removed) {
					var link = song.RemoveArtist(removed);
					if (link != null)
						session.Delete(link);
				}

				if (artistDiff.Changed) {

					var diff = new SongDiff(DoSnapshot(song.GetLatestVersion())) { Artists = true };

					song.UpdateArtistString();
					Archive(session, song, diff, SongArchiveReason.PropertiesUpdated);
					session.Update(song);
				}

				return song.ArtistString[PermissionContext.LanguagePreference];

			});

		}

		public KeyValuePair<int, string>[] UpdateArtistsForMultipleTracks(int[] songIds, int[] artistIds, bool add) {

			ParamIs.NotNull(() => songIds);
			ParamIs.NotNull(() => artistIds);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songs = session.Query<Song>().Where(s => songIds.Contains(s.Id)).ToArray();
				var artists = session.Query<Artist>().Where(s => artistIds.Contains(s.Id)).ToArray();
				var artistStrings = new List<KeyValuePair<int, string>>(songIds.Length);

				foreach (var song in songs) {

					AuditLog("updating artists for " + EntryLinkFactory.CreateEntryLink(song), session);

					var changed = false;

					foreach (var artist in artists) {

						if (add && !song.HasArtist(artist)) {
							session.Save(song.AddArtist(artist));
							changed = true;
						} else if (!add && song.HasArtist(artist)) {
							var link = song.RemoveArtist(artist);
							if (link != null) {
								session.Delete(link);
								changed = true;
							}
						}

					}

					if (changed) {
						song.UpdateArtistString();
						session.Update(song);
						artistStrings.Add(new KeyValuePair<int, string>(song.Id, song.ArtistString[PermissionContext.LanguagePreference]));
					}
				}

				return artistStrings.ToArray();

			});

		}

		public SongForEditContract UpdateBasicProperties(SongForEditContract properties) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var song = session.Load<Song>(properties.Song.Id);
				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion()));

				AuditLog(string.Format("updating properties for {0}", song));

				if (song.Notes != properties.Notes) {
					diff.Notes = true;
					song.Notes = properties.Notes;
				}

				var newOriginalVersion = (properties.OriginalVersion != null && properties.OriginalVersion.Id != 0 ? session.Load<Song>(properties.OriginalVersion.Id) : null);

				if (!Equals(song.OriginalVersion, newOriginalVersion)) {
					song.OriginalVersion = newOriginalVersion;
					diff.OriginalVersion = true;
				}

				if (song.SongType != properties.Song.SongType) {
					diff.SongType = true;
					song.SongType = properties.Song.SongType;
				}

				if (song.TranslatedName.DefaultLanguage != properties.TranslatedName.DefaultLanguage) {
					song.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;
					diff.OriginalName = true;
				}

				var nameDiff = song.Names.Sync(properties.Names, song);
				SessionHelper.Sync(session, nameDiff);

				if (nameDiff.Changed)
					diff.Names = true;

				var webLinkDiff = WebLink.Sync(song.WebLinks, properties.WebLinks, song);
				SessionHelper.Sync(session, webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks = true;

				if (song.Status != properties.Song.Status) {
					song.Status = properties.Song.Status;
					diff.Status = true;
				}

				var artistGetter = new Func<ArtistForSongContract, Artist>(artistForSong => 
					session.Load<Artist>(artistForSong.Artist.Id));

				var artistsDiff = song.SyncArtists(properties.Artists, artistGetter);
				SessionHelper.Sync(session, artistsDiff);

				if (artistsDiff.Changed)
					diff.Artists = true;

				var lyricsDiff = song.SyncLyrics(properties.Lyrics);
				SessionHelper.Sync(session, lyricsDiff);

				if (lyricsDiff.Changed)
					diff.Lyrics = true;

				AuditLog(string.Format("updated properties for {0} ({1})", EntryLinkFactory.CreateEntryLink(song), diff.ChangedFieldsString), session);

				Archive(session, song, diff, SongArchiveReason.PropertiesUpdated);

				session.Update(song);
				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

		[Obsolete("Integrated to saving properties")]
		public SongForEditContract UpdateLyrics(int songId, IEnumerable<LyricsForSongContract> lyrics) {
			
			ParamIs.NotNull(() => lyrics);

			var validLyrics = lyrics.Where(l => !string.IsNullOrEmpty(l.Value));

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion())) { Lyrics = true };	// TODO: actually check if they changed

				AuditLog("updating lyrics for " + song);

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

				AuditLog(string.Format("updated properties for {0} ({1})", EntryLinkFactory.CreateEntryLink(song), diff.ChangedFieldsString), session);

				Archive(session, song, diff, SongArchiveReason.PropertiesUpdated);
				session.Update(song);

				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

	}

	public class SongTagUsageFactory : ITagUsageFactory<SongTagUsage> {

		private readonly Song song;
		private readonly ISession session;

		public SongTagUsageFactory(ISession session, Song song) {
			this.session = session;
			this.song = song;
		}

		public SongTagUsage CreateTagUsage(Tag tag) {

			var usage = new SongTagUsage(song, tag);
			session.Save(usage);

			return usage;

		}

	}

}
