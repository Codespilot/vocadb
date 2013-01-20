using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Song;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

#pragma warning disable 169
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
#pragma warning restore 169

		private static IQueryable<Song> AddNameFilter(IQueryable<Song> directQ, string query, NameMatchMode nameMatchMode, bool onlyByName) {

			var matchMode = FindHelpers.GetMatchMode(query, nameMatchMode);

			if (matchMode == NameMatchMode.Exact) {

				return directQ.Where(s =>
					s.Names.SortNames.English == query
						|| s.Names.SortNames.Romaji == query
						|| s.Names.SortNames.Japanese == query);

			} else if (matchMode == NameMatchMode.StartsWith) {

				return directQ.Where(s =>
					s.Names.SortNames.English.StartsWith(query)
						|| s.Names.SortNames.Romaji.StartsWith(query)
						|| s.Names.SortNames.Japanese.StartsWith(query));

			} else {

				return directQ.Where(s =>
					s.Names.SortNames.English.Contains(query)
						|| s.Names.SortNames.Romaji.Contains(query)
						|| s.Names.SortNames.Japanese.Contains(query)
						|| (!onlyByName &&
							(s.ArtistString.Japanese.Contains(query)
								|| s.ArtistString.Romaji.Contains(query)
								|| s.ArtistString.English.Contains(query)))
						|| (s.NicoId != null && s.NicoId == query));

			}

		}

		private IQueryable<Song> AddPVFilter(IQueryable<Song> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<ArtistForSong> AddPVFilter(IQueryable<ArtistForSong> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.Song.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<SongName> AddPVFilter(IQueryable<SongName> criteria, bool onlyWithPVs) {

			if (onlyWithPVs)
				return criteria.Where(t => t.Song.PVServices != PVServices.Nothing);
			else
				return criteria;

		}

		private IQueryable<Song> AddTimeFilter(IQueryable<Song> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.CreateDate >= since);

		}

		private IQueryable<ArtistForSong> AddTimeFilter(IQueryable<ArtistForSong> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.Song.CreateDate >= since);

		}

		private IQueryable<SongName> AddTimeFilter(IQueryable<SongName> criteria, TimeSpan timeFilter) {

			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.Song.CreateDate >= since);

		}

		/// <summary>
		/// Finds songs based on criteria.
		/// </summary>
		/// <param name="session">Open session. Canot be null.</param>
		/// <param name="queryParams">Query parameters. Cannot be null.</param>
		/// <returns>List song search results. Cannot be null.</returns>
		private PartialFindResult<Song> Find(ISession session, SongQueryParams queryParams) {

			ParamIs.NotNull(() => queryParams);

			var draftsOnly = queryParams.Common.DraftOnly;
			var getTotalCount = queryParams.Paging.GetTotalCount;
			var ignoreIds = queryParams.IgnoredIds ?? new int[] { };
			var moveExactToTop = queryParams.Common.MoveExactToTop;
			var nameMatchMode = queryParams.Common.NameMatchMode;
			var onlyByName = queryParams.Common.OnlyByName;
			var query = queryParams.Common.Query;
			var songTypes = queryParams.SongTypes;
			var sortRule = queryParams.SortRule;
			var start = queryParams.Paging.Start;
			var maxResults = queryParams.Paging.MaxEntries;

			bool filterByType = songTypes.Any();
			Song[] songs;
			bool foundExactMatch = false;

			if (queryParams.ArtistId == 0 && string.IsNullOrWhiteSpace(query)) {

				var q = session.Query<Song>()
					.Where(s => !s.Deleted 
						&& !ignoreIds.Contains(s.Id));

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.SongType));

				q = AddTimeFilter(q, queryParams.TimeFilter);
				q = AddPVFilter(q, queryParams.OnlyWithPVs);

				q = q.AddOrder(sortRule, LanguagePreference);

				songs = q
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			} else if (queryParams.ArtistId != 0) {

				int artistId = queryParams.ArtistId;

				var q = session.Query<ArtistForSong>()
					.Where(m => !m.Song.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					q = q.Where(a => a.Song.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.Song.SongType));

				q = AddTimeFilter(q, queryParams.TimeFilter);
				q = AddPVFilter(q, queryParams.OnlyWithPVs);

				songs = q
					.Select(m => m.Song)
					.AddOrder(sortRule, PermissionContext.LanguagePreference)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			} else {

				query = query.Trim();

				// Searching by SortNames can be disabled in the future because all names should be included in the Names list anyway.
				var directQ = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					directQ = directQ.Where(s => songTypes.Contains(s.SongType));

				directQ = AddTimeFilter(directQ, queryParams.TimeFilter);
				directQ = AddPVFilter(directQ, queryParams.OnlyWithPVs);

				directQ = AddNameFilter(directQ, query, nameMatchMode, onlyByName);

				directQ = directQ.AddOrder(sortRule, LanguagePreference);

				var direct = directQ.ToArray();

				var additionalNamesQ = session.Query<SongName>()
					.Where(m => !m.Song.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Song.Status == EntryStatus.Draft);

				additionalNamesQ = AddTimeFilter(additionalNamesQ, queryParams.TimeFilter);
				additionalNamesQ = AddPVFilter(additionalNamesQ, queryParams.OnlyWithPVs);

				additionalNamesQ = FindHelpers.AddEntryNameFilter(additionalNamesQ, query, nameMatchMode);

				if (filterByType)
					additionalNamesQ = additionalNamesQ.Where(m => songTypes.Contains(m.Song.SongType));

				var additionalNames = additionalNamesQ
					.Select(m => m.Song)
					.AddOrder(sortRule, PermissionContext.LanguagePreference)
					.Distinct()
					//.Take(maxResults)
					.ToArray()
					.Where(a => !direct.Contains(a));

				var entries = direct.Concat(additionalNames)
					.Where(e => !ignoreIds.Contains(e.Id))
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				if (moveExactToTop) {
					
					var exactMatch = entries
						.Where(e => e.Names.Any(n => n.Value.Equals(query, StringComparison.InvariantCultureIgnoreCase)))
						.ToArray();

					if (exactMatch.Any()) {
						entries = CollectionHelper.MoveToTop(entries, exactMatch).ToArray();
						foundExactMatch = true;
					}

				}

				songs = entries;

			}

			int count = (getTotalCount 
				? GetSongCount(session, query, songTypes, onlyByName, draftsOnly, nameMatchMode, queryParams.TimeFilter, queryParams.OnlyWithPVs, queryParams) 
				: 0);

			return new PartialFindResult<Song>(songs, count, queryParams.Common.Query, foundExactMatch);

		}

		private int GetSongCount(ISession session, string query, SongType[] songTypes, bool onlyByName, bool draftsOnly, NameMatchMode nameMatchMode, 
			TimeSpan timeFilter, bool onlyWithPVs, SongQueryParams queryParams) {

			bool filterByType = songTypes.Any();

			if (queryParams.ArtistId == 0 && string.IsNullOrWhiteSpace(query)) {

				var q = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					q = q.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.SongType));

				q = AddTimeFilter(q, timeFilter);
				q = AddPVFilter(q, onlyWithPVs);

				return q.Count();

			} else if (queryParams.ArtistId != 0) {

				int artistId = queryParams.ArtistId;

				var q = session.Query<ArtistForSong>()
					.Where(m => !m.Song.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					q = q.Where(a => a.Song.Status == EntryStatus.Draft);

				if (filterByType)
					q = q.Where(s => songTypes.Contains(s.Song.SongType));

				q = AddTimeFilter(q, timeFilter);
				q = AddPVFilter(q, onlyWithPVs);

				return q.Count();

			} else {

				var directQ = session.Query<Song>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				if (filterByType)
					directQ = directQ.Where(s => songTypes.Contains(s.SongType));

				directQ = AddTimeFilter(directQ, timeFilter);
				directQ = AddPVFilter(directQ, onlyWithPVs);

				directQ = AddNameFilter(directQ, query, nameMatchMode, onlyByName);

				var direct = directQ.ToArray();

				var additionalNamesQ = session.Query<SongName>()
					.Where(m => !m.Song.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Song.Status == EntryStatus.Draft);

				if (filterByType)
					additionalNamesQ = additionalNamesQ.Where(s => songTypes.Contains(s.Song.SongType));

				additionalNamesQ = AddTimeFilter(additionalNamesQ, timeFilter);
				additionalNamesQ = AddPVFilter(additionalNamesQ, onlyWithPVs);

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

		private PartialFindResult<SongInListContract> GetSongsInList(ISession session, int listId, int start, int maxItems, bool getTotalCount) {

			var q = session.Query<SongInList>().Where(a => !a.Song.Deleted && a.List.Id == listId);

			IQueryable<SongInList> resultQ = q.OrderBy(s => s.Order);
			resultQ = resultQ.Skip(start).Take(maxItems);

			var contracts = resultQ.ToArray().Select(a => new SongInListContract(a, PermissionContext.LanguagePreference)).ToArray();
			var totalCount = (getTotalCount ? q.Count() : 0);

			return new PartialFindResult<SongInListContract>(contracts, totalCount);

		}

		private VideoUrlParseResult ParsePV(ISession session, string url) {

			if (string.IsNullOrEmpty(url))
				return null;

			var pvResult = VideoServiceHelper.ParseByUrl(url, true);

			if (!pvResult.IsOk)
				throw pvResult.Exception;

			var existing = session.Query<PVForSong>().FirstOrDefault(
				s => s.Service == pvResult.Service && s.PVId == pvResult.Id && !s.Song.Deleted);

			if (existing != null) {
				throw new VideoParseException(string.Format("Song '{0}' already contains this PV",
					existing.Song.TranslatedName[PermissionContext.LanguagePreference]));
			}

			return pvResult;

		}

		private ArtistForSong RestoreArtistRef(Song song, Artist artist, ArchivedArtistForSongContract albumRef) {

			if (artist != null) {

				return (!artist.HasSong(song) ? artist.AddSong(song, albumRef.IsSupport, albumRef.Roles) : null);

			} else {

				return song.AddArtist(albumRef.NameHint, albumRef.IsSupport, albumRef.Roles);

			}

		}

		public SongService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {

		}

		[Obsolete("Integrated to properties saving")]
		public ArtistForSongContract AddArtist(int songId, int artistId) {

			VerifyManageDatabase();

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

		[Obsolete("Integrated to saving properties")]
		public ArtistForSongContract AddArtist(int songId, string newArtistName) {

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				AuditLog(string.Format("creating custom artist {0} to {1}", newArtistName, song), session);

				var artistForSong = song.AddArtist(newArtistName, false, ArtistRoles.Default);
				session.Save(artistForSong);

				song.UpdateArtistString();
				session.Update(song);

				return new ArtistForSongContract(artistForSong, PermissionContext.LanguagePreference);

			});

		}

		public void AddSongToList(int listId, int songId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var list = session.Load<SongList>(listId);
				var items = session.Query<SongInList>().Where(s => s.List.Id == listId);
				int order = 1;

				if (items.Any())
					order = items.Max(s => s.Order) + 1;

				VerifyResourceAccess(list.Author);

				var song = session.Load<Song>(songId);

				var link = list.AddSong(song, order, string.Empty);
				session.Save(link);

			});

		}

		public void Archive(ISession session, Song song, SongDiff diff, SongArchiveReason reason, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Song song, SongArchiveReason reason, string notes = "") {

			Archive(session, song, new SongDiff(), reason, notes);

		}

		/*
		[Obsolete("Disabled")]
		public SongContract Create(string name) {

			ParamIs.NotNullOrWhiteSpace(() => name);

			VerifyManageDatabase();

			name = name.Trim();

			return HandleTransaction(session => {

				AuditLog("creating a new song with name " + name, session);

				var song = new Song(name);

				session.Save(song);

				Archive(session, song, SongArchiveReason.Created);
				session.Update(song);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}*/

		public CommentContract CreateComment(int songId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				var agent = SessionHelper.CreateAgentLoginData(session, PermissionContext);

				AuditLog(string.Format("creating comment for {0}: '{1}'",
					EntryLinkFactory.CreateEntryLink(song),
					HttpUtility.HtmlEncode(message.Truncate(60))), session, agent.User);

				var comment = song.CreateComment(message, agent);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		/*
		[Obsolete("Replaced by updating properties")]
		public SongInAlbumContract CreateForAlbum(int albumId, string newSongName) {

			ParamIs.NotNullOrWhiteSpace(() => newSongName);

			VerifyManageDatabase();

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

		}*/

		/*
		[Obsolete("Integrated to saving properties")]
		public PVContract CreatePVForSong(int songId, PVService service, string pvId, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvId);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				AuditLog(string.Format("creating a PV for {0}", EntryLinkFactory.CreateEntryLink(song)), session);

				var pv = song.CreatePV(service, pvId, pvType, string.Empty);
				session.Save(pv);

				if (service == PVService.NicoNicoDouga && pvType == PVType.Original) {
					song.UpdateNicoId();
					session.Update(song);
				}

				return new PVContract(pv);

			});

		}*/

		/*
		[Obsolete("Integrated to saving properties")]
		public PVContract CreatePVForSong(int songId, string pvUrl, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			var result = VideoServiceHelper.ParseByUrl(pvUrl, true);

			return CreatePVForSong(songId, result.Service, result.Id, pvType);

		}*/

		public bool CreateReport(int songId, SongReportType reportType, string hostname, string notes) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(session => {

				var loggedUserId = PermissionContext.LoggedUserId;
				var existing = session.Query<SongReport>()
					.FirstOrDefault(r => r.Song.Id == songId && ((loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname));

				if (existing != null)
					return false;

				var song = session.Load<Song>(songId);
				var report = new SongReport(song, reportType, GetLoggedUserOrDefault(session), hostname, notes.Truncate(200));

				var msg =  string.Format("reported {0} as {1} ({2})", EntryLinkFactory.CreateEntryLink(song), reportType, HttpUtility.HtmlEncode(notes));
				AuditLog(msg.Truncate(200), session, new AgentLoginData(GetLoggedUserOrDefault(session), hostname));

				session.Save(report);
				return true;

			}, IsolationLevel.ReadUncommitted);

		}

		public SongContract Create(CreateSongContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			VerifyManageDatabase();

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
					session.Save(song.CreatePV(new PVContract(pvResult, PVType.Original)));
				}

				if (reprintPvResult != null) {
					session.Save(song.CreatePV(new PVContract(reprintPvResult, PVType.Reprint)));
				}

				song.UpdateArtistString();
				Archive(session, song, SongArchiveReason.Created);
				session.Update(song);

				AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(song)), session);
				AddEntryEditedEntry(session, song, EntryEditEvent.Created);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		private SongList CreateSongList(ISession session, SongListForEditContract contract) {

			var user = GetLoggedUser(session);
			var newList = new SongList(contract.Name, user);
			newList.Description = contract.Description;

			if (EntryPermissionManager.CanManageFeaturedLists(PermissionContext))
				newList.FeaturedCategory = contract.FeaturedCategory;

			session.Save(newList);

			var songDiff = newList.SyncSongs(contract.SongLinks, c => session.Load<Song>(c.SongId));
			SessionHelper.Sync(session, songDiff);

			session.Update(newList);

			AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(newList)), session, user);

			return newList;

		}

		public void Delete(int id) {

			UpdateEntity<Song>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				a.Delete();

			}, PermissionToken.DeleteEntries, skipLog: true);

		}

		[Obsolete("Integrating to saving properties")]
		public void DeleteArtistForSong(int artistForSongId) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var artistForSong = session.Load<ArtistForSong>(artistForSongId);

				AuditLog("deleting " + artistForSong, session);

				artistForSong.Song.DeleteArtistForSong(artistForSong);
				session.Delete(artistForSong);
				session.Update(artistForSong.Song);

			});

		}

		public void DeleteComment(int commentId) {

			HandleTransaction(session => {

				var comment = session.Load<SongComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				comment.Song.Comments.Remove(comment);
				session.Delete(comment);

			});

		}

		[Obsolete("Integrated to saving properties")]
		public void DeletePvForSong(int pvForSongId) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var pvForSong = session.Load<PVForSong>(pvForSongId);

				AuditLog("deleting " + pvForSong, session);

				pvForSong.OnDelete();

				session.Delete(pvForSong);
				session.Update(pvForSong.Song);

			});

		}

		public void DeleteSongList(int listId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var user = GetLoggedUser(session);
				var list = session.Load<SongList>(listId);

				VerifyResourceAccess(list.Author);

				session.Delete(list);

				AuditLog(string.Format("deleted {0}", list.ToString()), session, user);

			});

		}

		public T FindFirst<T>(Func<Song, T> fac, string[] query, NameMatchMode nameMatchMode)
			where T : class {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session,
						new SongQueryParams {
							Common = new CommonSearchParams {
								Query = q, NameMatchMode = nameMatchMode, OnlyByName = true, MoveExactToTop = true
							},
							Paging = new PagingProperties(0, 10, false)
						});

					if (result.Items.Any())
						return fac(result.Items.First());

				}

				return null;

			});

		}

		public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName, string[] anyPv) {

			var names = anyName.Select(n => n.Trim()).Where(n => n != string.Empty).ToArray();
			var pvs = anyPv.Select(p => VideoServiceHelper.ParseByUrl(p, false)).Where(p => p.IsOk).ToArray();

			if (!names.Any() && !pvs.Any())
				return new EntryRefWithCommonPropertiesContract[] { };

			return HandleQuery(session => {

				var nameMatches = (names.Any() ? session.Query<SongName>()
					.Where(n => names.Contains(n.Value))
					.Select(n => n.Song)
					.Where(n => !n.Deleted)
					.Distinct()
					.Take(10)
					.ToArray() : new Song[] { });

				var pvMatches = pvs.Select(pv => session.Query<PVForSong>()
					.Where(p => p.PVId == pv.Id && p.Service == pv.Service)
					.Select(n => n.Song)
					.FirstOrDefault(n => !n.Deleted))
					.Where(p => p != null);

				return nameMatches.Union(pvMatches)
					.Select(s => new EntryRefWithCommonPropertiesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public SongWithAdditionalNamesContract FindFirst(string[] query, NameMatchMode nameMatchMode) {

			return FindFirst(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference), query, nameMatchMode);

		}

		public SongDetailsContract FindFirstDetails(string query, NameMatchMode nameMatchMode) {

			return FindFirst(s => new SongDetailsContract(s, PermissionContext.LanguagePreference), new[]{ query }, nameMatchMode);

		}

		public PartialFindResult<T> Find<T>(Func<Song, T> fac, SongQueryParams queryParams)
			where T : class {

			return HandleQuery(session => {

				var result = Find(session, queryParams);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(),
					result.TotalCount, result.Term, result.FoundExactMatch);

			});

		}

		public PartialFindResult<SongContract> Find(SongQueryParams queryParams) {

			return Find(s => new SongContract(s, PermissionContext.LanguagePreference), queryParams);

		}

		public PartialFindResult<SongWithAlbumAndPVsContract> FindWithAlbum(SongQueryParams queryParams, bool getPVs) {

			return Find(s => new SongWithAlbumAndPVsContract(s, PermissionContext.LanguagePreference, getPVs), queryParams);

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

		public string[] FindNames(string query, int maxResults) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] { };

			query = query.Trim();

			return HandleQuery(session => {

				var names = session.Query<SongName>()
					.Where(a => !a.Song.Deleted)
					.AddEntryNameFilter(query, NameMatchMode.Auto)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				return names;

			});

		}

		public CommentContract[] GetComments(int songId) {

			return HandleQuery(session => {

				return session.Query<SongComment>()
					.Where(c => c.Song.Id == songId)
					.OrderByDescending(c => c.Created)
					.Select(c => new CommentContract(c)).ToArray();

			});

		}

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int songId) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				return new EntryWithTagUsagesContract(song, song.Tags.Usages);

			});

		}

		// Used for RSS feed. TODO: integrate with Find.
		public SongContract[] GetNewSongsWithVideos() {

			return Find(new SongQueryParams { OnlyWithPVs = true, Paging = new PagingProperties(0, 20, false) }).Items;

		}

		public LyricsForSongContract GetRandomLyricsForSong(string query) {

			return HandleQuery(session => {

				var songContract = Find(session, new SongQueryParams(query, new SongType[] {}, 0, 10, false, false, 
					NameMatchMode.Auto, SongSortRule.Name, false, true, null)).Items;

				if (!songContract.Any())
					return null;
				
				var songIds = songContract.Select(s => s.Id).ToArray();

				var songs = session.Query<Song>().Where(s => songIds.Contains(s.Id)).ToArray();
				var allLyrics = songs.SelectMany(s => s.Lyrics).ToArray();

				//var song = session.Query<Song>().Where(s => s.Id) session.Load<Song>(songContract.Id);)

				if (!allLyrics.Any())
					return null;

				var lyrics = allLyrics[new Random().Next(allLyrics.Length)];

				return new LyricsForSongContract(lyrics);

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

		public SongDetailsContract GetSongDetails(int songId, string hostname) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				var contract = new SongDetailsContract(song, PermissionContext.LanguagePreference);
				int agentNum = 0;
				var user = PermissionContext.LoggedUser;

				if (user != null) {

					var rating = session.Query<FavoriteSongForUser>()
						.FirstOrDefault(s => s.Song.Id == songId && s.User.Id == user.Id);

					contract.UserRating = (rating != null ? rating.Rating : SongVoteRating.Nothing);

					agentNum = user.Id;

				} else if (!string.IsNullOrEmpty(hostname)) {
					agentNum = hostname.GetHashCode();
				}

				contract.CommentCount = session.Query<SongComment>().Count(c => c.Song.Id == songId);
				contract.LatestComments = session.Query<SongComment>()
					.Where(c => c.Song.Id == songId)
					.OrderByDescending(c => c.Created).Take(3).ToArray()
					.Select(c => new CommentContract(c)).ToArray();
				contract.Hits = session.Query<SongHit>().Count(h => h.Song.Id == songId);

				if (agentNum != 0) {
					using (var tx = session.BeginTransaction(IsolationLevel.ReadUncommitted)) {

						var isHit = session.Query<SongHit>().Any(h => h.Song.Id == songId && h.Agent == agentNum);

						if (!isHit) {
							var hit = new SongHit(song, agentNum);
							session.Save(hit);
						}

						tx.Commit();

					}
				}

				return contract;

			});

		}

		public SongForEditContract GetSongForEdit(int songId) {

			return HandleQuery(session => new SongForEditContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public SongListContract GetSongList(int listId) {

			return HandleQuery(session => new SongListContract(session.Load<SongList>(listId), PermissionContext));

		}

		public SongListsByCategoryContract[] GetSongListsByCategory() {

			return HandleQuery(session => {

				var lists = session.Query<SongList>()
					.Where(l => l.FeaturedCategory != SongListFeaturedCategory.Nothing)
					.OrderBy(l => l.Name)
					.GroupBy(l => l.FeaturedCategory)
					.ToArray()
					.OrderBy(c => c.Key);

				return lists.Select(l => new SongListsByCategoryContract(l.Key, l, PermissionContext)).ToArray();

			});

		}

		public SongListDetailsContract GetSongListDetails(int listId) {

			return HandleQuery(session => new SongListDetailsContract(
				session.Load<SongList>(listId), GetSongsInList(session, listId, 0, 50, true), PermissionContext));
		
		}

		public SongListForEditContract GetSongListForEdit(int listId, bool loadSongs = true) {

			return HandleQuery(session => new SongListForEditContract(session.Load<SongList>(listId), PermissionContext, loadSongs));

		}

		public SongListBaseContract[] GetSongListsForCurrentUser(int ignoreSongId) {

			PermissionContext.VerifyLogin();

			var canEditPools = PermissionContext.HasPermission(PermissionToken.EditFeaturedLists);

			return HandleQuery(session => {

				var ignoredSong = session.Load<Song>(ignoreSongId);

				return session.Query<SongList>()
					.Where(l => (l.Author.Id == PermissionContext.LoggedUser.Id && l.FeaturedCategory == SongListFeaturedCategory.Nothing) 
						|| (canEditPools && l.FeaturedCategory == SongListFeaturedCategory.Pools))
					.OrderBy(l => l.Name).ToArray()
					.Where(l => !ignoredSong.ListLinks.Any(i => i.List.Equals(l)))
					.Select(l => new SongListBaseContract(l)).ToArray();

			});

		}

		public PartialFindResult<SongInListContract> GetSongsInList(int listId, int start, int maxItems, bool getTotalCount) {

			return HandleQuery(session => GetSongsInList(session, listId, start, maxItems, getTotalCount));

		}

		public SongWithAdditionalNamesContract GetSongWithAdditionalNames(int id) {

			return HandleQuery(
				session => new SongWithAdditionalNamesContract(session.Load<Song>(id), PermissionContext.LanguagePreference));

		}

		public SongWithArchivedVersionsContract GetSongWithArchivedVersions(int songId) {

			return HandleQuery(session => new SongWithArchivedVersionsContract(session.Load<Song>(songId), PermissionContext.LanguagePreference));

		}

		public T GetSongWithPV<T>(Func<Song, T> fac, PVService service, string pvId) 
			where T : class {

			return HandleQuery(session => {

				var pv = session.Query<PVForSong>()
					.FirstOrDefault(p => p.Service == service && p.PVId == pvId && !p.Song.Deleted);

				return (pv != null ? fac(pv.Song) : null);

			});

		}
		public SongWithAdditionalNamesContract GetSongWithPV(PVService service, string pvId) {

			return GetSongWithPV(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference), service, pvId);

		}

		public SongWithPVAndVoteContract GetSongWithPVAndVote(int songId) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				var userId = PermissionContext.LoggedUserId;
				var vote = session.Query<FavoriteSongForUser>().FirstOrDefault(s => s.Song.Id == songId && s.User.Id == userId);

				return new SongWithPVAndVoteContract(song, vote != null ? vote.Rating : SongVoteRating.Nothing, PermissionContext.LanguagePreference);

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

		public TagSelectionContract[] GetTagSelections(int songId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<SongTagUsage>().Where(a => a.Song.Id == songId).ToArray();
				var tagVotes = session.Query<SongTagVote>().Where(a => a.User.Id == userId && a.Usage.Song.Id == songId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag.Name, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		[Obsolete("Not used anymore")]
		public SongWithAdditionalNamesContract[] GetTopFavoritedSongs(int maxResults) {

			return HandleQuery(session => {

				var songs = session.Query<Song>()
					.Where(s => s.FavoritedTimes > 0)
					.OrderByDescending(s => s.FavoritedTimes)
					.Take(maxResults).ToArray();

				return songs.Select(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		public ArchivedSongVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedSongVersionDetailsContract(session.Load<ArchivedSongVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedSongVersion>(comparedVersionId) : null, 
					PermissionContext.LanguagePreference));

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Song>(sourceId);
				var target = session.Load<Song>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", 
					EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)), session);

				// Names
				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				// Weblinks
				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					session.Save(link);
				}

				// PVs
				var pvs = source.PVs.Where(a => !target.HasPV(a.Service, a.PVId));
				foreach (var p in pvs) {
					var pv = target.CreatePV(new PVContract(p));
					session.Save(pv);
				}

				// Artist links
				var artists = source.Artists.Where(a => !target.HasArtistLink(a)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				// Album links
				var albums = source.Albums.Where(s => !target.IsOnAlbum(s.Album)).ToArray();
				foreach (var s in albums) {
					s.Move(target);
					session.Update(s);
				}

				// Favorites
				var userFavorites = source.UserFavorites.Where(a => !target.IsFavoritedBy(a.User)).ToArray();
				foreach (var u in userFavorites) {
					u.Move(target);
					session.Update(u);
				}

				// Custom lists
				var songLists = source.ListLinks.ToArray();
				foreach (var s in songLists) {
					s.ChangeSong(target);
					session.Update(s);
				}

				// Other properties
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

		public PVContract PVForSong(int pvId) {

			return HandleQuery(session => new PVContract(session.Load<PVForSong>(pvId)));

		}

		public int RemoveTagUsage(long tagUsageId) {

			return RemoveTagUsage<SongTagUsage>(tagUsageId);

		}

		public void Restore(int songId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				song.Deleted = false;

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(song), session);

			});

		}

		public EntryRevertedContract RevertToVersion(int archivedSongVersionId) {

			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

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
				SessionHelper.RestoreObjectRefs<ArtistForSong, Artist, ArchivedArtistForSongContract>(
					session, warnings, song.AllArtists, fullProperties.Artists,
					(a1, a2) => (a1.Artist != null && a1.Artist.Id == a2.Id) || (a1.Artist == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(artist, artistRef) => RestoreArtistRef(song, artist, artistRef),
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
						session.Save(song.CreatePV(new PVContract(pv)));
					}

					foreach (var pv in pvDiff.Removed) {
						pv.OnDelete();
						session.Delete(pv);
					}

				}

				song.UpdateFavoritedTimes();

				Archive(session, song, SongArchiveReason.Reverted);
				AuditLog("reverted " + EntryLinkFactory.CreateEntryLink(song) + " to revision " + archivedVersion.Version, session);

				return new EntryRevertedContract(song, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int songId, string[] tags) {

			ParamIs.NotNull(() => tags);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				tags = tags.Distinct(new CaseInsensitiveStringComparer()).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("tagging {0} with {1}",
					EntryLinkFactory.CreateEntryLink(song), string.Join(", ", tags)), session, user);

				var existingTags = session.Query<Tag>().ToDictionary(t => t.Name, new CaseInsensitiveStringComparer());

				song.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session, new AgentLoginData(user)), new SongTagUsageFactory(session, song));

				return song.Tags.Usages.OrderByDescending(u => u.Count).Take(Tag.MaxDisplayedTags).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public string UpdateArtists(int songId, int[] artistIds) {

			ParamIs.NotNull(() => artistIds);

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);

				VerifyEntryEdit(song);

				var oldArtists = song.ArtistList.ToArray();
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

					var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), GetLoggedUser(session))) { Artists = true };

					song.UpdateArtistString();
					Archive(session, song, diff, SongArchiveReason.PropertiesUpdated);
					session.Update(song);

					AuditLog("updated artists for " + EntryLinkFactory.CreateEntryLink(song), session);
					AddEntryEditedEntry(session, song, EntryEditEvent.Updated);

				}

				return song.ArtistString[PermissionContext.LanguagePreference];

			});

		}

		public KeyValuePair<int, string>[] UpdateArtistsForMultipleTracks(int[] songIds, int[] artistIds, bool add) {

			ParamIs.NotNull(() => songIds);
			ParamIs.NotNull(() => artistIds);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var songs = session.Query<Song>().Where(s => songIds.Contains(s.Id)).ToArray();
				var artists = session.Query<Artist>().Where(s => artistIds.Contains(s.Id)).ToArray();
				var artistStrings = new List<KeyValuePair<int, string>>(songIds.Length);

				foreach (var song in songs) {

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

						var diff = new SongDiff { Artists = true };
						song.UpdateArtistString();
						Archive(session, song, diff, SongArchiveReason.PropertiesUpdated);

						session.Update(song);
						artistStrings.Add(new KeyValuePair<int, string>(song.Id, song.ArtistString[PermissionContext.LanguagePreference]));

						AuditLog("updated artists for " + EntryLinkFactory.CreateEntryLink(song), session);
						AddEntryEditedEntry(session, song, EntryEditEvent.Updated);

					}
				}

				return artistStrings.ToArray();

			});

		}

		public SongForEditContract UpdateBasicProperties(SongForEditContract properties) {

			ParamIs.NotNull(() => properties);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var song = session.Load<Song>(properties.Song.Id);

				VerifyEntryEdit(song);

				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), GetLoggedUser(session)));

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

				var nameDiff = song.Names.Sync(properties.Names.AllNames, song);
				SessionHelper.Sync(session, nameDiff);

				if (nameDiff.Changed)
					diff.Names = true;

				var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(song.WebLinks, validWebLinks, song);
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

				var pvDiff = song.SyncPVs(properties.PVs);
				SessionHelper.Sync(session, pvDiff);

				if (pvDiff.Changed)
					diff.PVs = true;

				var lyricsDiff = song.SyncLyrics(properties.Lyrics);
				SessionHelper.Sync(session, lyricsDiff);

				if (lyricsDiff.Changed)
					diff.Lyrics = true;

				var logStr = string.Format("updated properties for {0} ({1})", EntryLinkFactory.CreateEntryLink(song), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				AuditLog(logStr, session);
				AddEntryEditedEntry(session, song, EntryEditEvent.Updated);

				Archive(session, song, diff, SongArchiveReason.PropertiesUpdated, properties.UpdateNotes);

				session.Update(song);
				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

		/*
		[Obsolete("Integrated to saving properties")]
		public SongForEditContract UpdateLyrics(int songId, IEnumerable<LyricsForSongContract> lyrics) {
			
			ParamIs.NotNull(() => lyrics);

			var validLyrics = lyrics.Where(l => !string.IsNullOrEmpty(l.Value));

			return HandleTransaction(session => {

				var song = session.Load<Song>(songId);
				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), GetLoggedUser(session))) { Lyrics = true };	// TODO: actually check if they changed

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

		}*/

		public int UpdateSongList(SongListForEditContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return HandleTransaction(session => {

				var user = GetLoggedUser(session);
				SongList list;

				if (contract.Id == 0) {

					list = CreateSongList(session, contract);
					
				} else {

					list = session.Load<SongList>(contract.Id);

					EntryPermissionManager.VerifyEdit(PermissionContext, list);

					list.Description = contract.Description;
					list.Name = contract.Name;

					if (EntryPermissionManager.CanManageFeaturedLists(PermissionContext))
						list.FeaturedCategory = contract.FeaturedCategory;

					var songDiff = list.SyncSongs(contract.SongLinks, c => session.Load<Song>(c.SongId));
					SessionHelper.Sync(session, songDiff);

					session.Update(list);

					AuditLog(string.Format("updated {0}", EntryLinkFactory.CreateEntryLink(list)), session, user);

				}

				return list.Id;

			});

		}

		public SongDetailsContract XGetSongByNameArtistAndAlbum(string name, string artist, string album) {

			return HandleQuery(session => {

				var matches = session.Query<SongName>().Where(n => n.Value == name)
					.Select(n => n.Song)
					.ToArray();

				Artist[] artists = null;

				if (!string.IsNullOrEmpty(artist)) {

					artists = session.Query<ArtistName>()
						.FilterByArtistName(artist)
						.Select(n => n.Artist)
						.Take(10)
						.ToArray();

				}

				if (artists != null && artists.Any())
					matches = matches.Where(s => s.ArtistList.Any(a => artists.Contains(a))).ToArray();

				Album[] albums = null;

				if (!string.IsNullOrEmpty(album)) {

					albums = session.Query<AlbumName>()
						.AddEntryNameFilter(album, NameMatchMode.Auto)
						.Select(n => n.Album)
						.Take(10)
						.ToArray();

				}

				if (albums != null && albums.Any())
					matches = matches.Where(s => s.Albums.Any(a => albums.Contains(a.Album))).ToArray();

				if (matches.Length == 1)
					return new SongDetailsContract(matches.First(), PermissionContext.LanguagePreference);

				if (matches.Length == 0)
					return null;

				matches = session.Query<SongName>()
					.AddEntryNameFilter(name, NameMatchMode.Auto)
					.Select(n => n.Song)
					.ToArray();

				if (artists != null && artists.Any())
					matches = matches.Where(s => s.ArtistList.Any(a => artists.Contains(a))).ToArray();

				if (albums != null && albums.Any())
					matches = matches.Where(s => s.Albums.Any(a => albums.Contains(a.Album))).ToArray();

				if (matches.Length == 1)
					return new SongDetailsContract(matches.First(), PermissionContext.LanguagePreference);

				return null;

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

	public enum SongSortRule {

		None,

		Name,

		AdditionDate,

		FavoritedTimes,

		RatingScore

	}

}
