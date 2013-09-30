using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
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
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service {

	public class SongService : ServiceBase {

		class SongTupleEqualityComparer<T> : IEqualityComparer<System.Tuple<Song, T>> {
			public bool Equals(System.Tuple<Song, T> x, System.Tuple<Song, T> y) {
				return Equals(x.Item1, y.Item1);
			}

			public int GetHashCode(System.Tuple<Song, T> obj) {
				return obj.Item1.GetHashCode();
			}
		}

#pragma warning disable 169
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
#pragma warning restore 169

		private PartialFindResult<Song> Find(ISession session, SongQueryParams queryParams) {
			return new SongSearch(new QuerySourceSession(session), LanguagePreference).Find(queryParams);
		}

		private SongMergeRecord GetMergeRecord(ISession session, int sourceId) {
			return session.Query<SongMergeRecord>().FirstOrDefault(s => s.Source == sourceId);			
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

		public void AddSongToList(int listId, int songId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			HandleTransaction(session => {

				var list = session.Load<SongList>(listId);
				var items = session.Query<SongInList>().Where(s => s.List.Id == listId);
				int order = 1;

				if (items.Any())
					order = items.Max(s => s.Order) + 1;

				EntryPermissionManager.VerifyEdit(PermissionContext, list);

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
				throw new ArgumentException("Song needs at least one name", "contract");

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var pvResult = ParsePV(session, contract.PVUrl);
				var reprintPvResult = ParsePV(session, contract.ReprintPVUrl);

				SysLog(string.Format("creating a new song with name '{0}'", contract.Names.First().Value));

				var song = new Song { 
					SongType = contract.SongType, 
					Status = contract.Draft ? EntryStatus.Draft : EntryStatus.Finished 
				};

				song.Names.Init(contract.Names, song);

				session.Save(song);

				foreach (var artistContract in contract.Artists) {
					var artist = session.Load<Artist>(artistContract.Id);
					if (!song.HasArtist(artist))
						session.Save(song.AddArtist(artist));
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

				AuditLog(string.Format("created song {0} ({1})", EntryLinkFactory.CreateEntryLink(song), song.SongType), session);
				AddEntryEditedEntry(session, song, EntryEditEvent.Created);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public void Delete(int id) {

			UpdateEntity<Song>(id, (session, a) => {

				AuditLog(string.Format("deleting song {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				a.Delete();

			}, PermissionToken.DeleteEntries, skipLog: true);

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
							Paging = new PagingProperties(0, 30, false)
						});

					if (result.Items.Any())
						return fac(result.Items.First());

				}

				return null;

			});

		}

		private NicoTitleParseResult ParseNicoPV(ISession session, string url) {

			if (string.IsNullOrEmpty(url))
				return null;

			var res = VideoService.NicoNicoDouga.ParseByUrl(url, true);

			if (!res.IsOk)
				return null;

			var titleParseResult = NicoHelper.ParseTitle(res.Title, a => Services.Artists.Find(session, new ArtistQueryParams {
				Common = new CommonSearchParams { Query = a, NameMatchMode = NameMatchMode.Exact },
				Paging = new PagingProperties(0, 1, false), 
				SortRule = ArtistSortRule.AdditionDateAsc
			}).Items.FirstOrDefault());

			if (!string.IsNullOrEmpty(res.AuthorId)) {

				var author = session
					.Query<ArtistWebLink>()
					.Where(w => w.Url == "http://www.nicovideo.jp/user/" + res.AuthorId)
					.Select(w => w.Artist)
					.FirstOrDefault();

				if (author != null)
					titleParseResult.Artists.Add(author);

			}

			return titleParseResult;

		}

		public NewSongCheckResultContract FindDuplicates(string[] anyName, string[] anyPv, bool getPVInfo) {

			var names = anyName.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToArray();
			var pvs = anyPv.Select(p => VideoServiceHelper.ParseByUrl(p, false)).Where(p => p.IsOk).ToArray();

			if (!names.Any() && !pvs.Any())
				return new NewSongCheckResultContract();

			return HandleQuery(session => {

				NicoTitleParseResult titleParseResult = null;
				if (getPVInfo) {

					var nicoPV = anyPv.FirstOrDefault(p => VideoService.NicoNicoDouga.IsValidFor(p));

					titleParseResult = ParseNicoPV(session, nicoPV);

					if (titleParseResult != null && !string.IsNullOrEmpty(titleParseResult.Title))
						names = names.Concat(new[] { titleParseResult.Title }).ToArray();

				}

				var nameMatches = (names.Any() ? session.Query<SongName>()
					.Where(n => names.Contains(n.Value))
					.Select(n => n.Song)
					.Where(n => !n.Deleted)
					.Distinct()
					.Take(10)
					.ToArray()
					.Select(d => new System.Tuple<Song, SongMatchProperty>(d, SongMatchProperty.Title)) : new System.Tuple<Song, SongMatchProperty>[] { });

				var pvMatches = pvs.Select(pv => session.Query<PVForSong>()
					.Where(p => p.PVId == pv.Id && p.Service == pv.Service)
					.Select(n => n.Song)
					.FirstOrDefault(n => !n.Deleted))
					.Where(p => p != null)
					.Select(d => new System.Tuple<Song, SongMatchProperty>(d, SongMatchProperty.PV));


				var matches = pvMatches.Union(nameMatches, new SongTupleEqualityComparer<SongMatchProperty>())
					.Select(s => new DuplicateEntryResultContract<SongMatchProperty>(new EntryRefWithCommonPropertiesContract(s.Item1, PermissionContext.LanguagePreference), s.Item2))
					.ToArray();

				return new NewSongCheckResultContract(matches, titleParseResult, LanguagePreference);

			});

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

		public PartialFindResult<SongContract> FindWithThumbPreferNotNico(SongQueryParams queryParams) {

			return Find(s => new SongContract(s, PermissionContext.LanguagePreference, VideoServiceHelper.GetThumbUrlPreferNotNico(s.PVs.PVs)), queryParams);

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

				return NameHelper.MoveExactNamesToTop(names, query);

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

		public T GetSong<T>(int id, Func<Song, T> fac) {

			return HandleQuery(session => fac(session.Load<Song>(id)));

		}

		public T GetSongWithMergeRecord<T>(int id, Func<Song, SongMergeRecord, T> fac) {

			return HandleQuery(session => {
				var song = session.Load<Song>(id);
				return fac(song, (song.Deleted ? GetMergeRecord(session, id) : null));
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

				if (song.Deleted) {
					var mergeEntry = GetMergeRecord(session, songId);
					contract.MergedTo = (mergeEntry != null ? new SongContract(mergeEntry.Target, LanguagePreference) : null);
				}

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

		public SongListContract[] GetPublicSongListsForSong(int songId) {

			return HandleQuery(session => {

				var song = session.Load<Song>(songId);
				var userId = PermissionContext.LoggedUserId;
				return song.ListLinks
					.Where(l => l.List.FeaturedCategory != SongListFeaturedCategory.Nothing || l.List.Author.Id == userId || l.List.Author.Options.PublicRatings)
					.OrderBy(l => l.List.Name)
					.Select(l => l.List)
					.Distinct()
					.Select(l => new SongListContract(l, PermissionContext))
					.ToArray();

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
		public SongContract GetSongWithPV(PVService service, string pvId) {

			return GetSongWithPV(s => new SongContract(s, PermissionContext.LanguagePreference), service, pvId);

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

		public FavoriteSongForUserContract[] GetUsersWithSongRating(int songId) {

			return HandleQuery(session =>

				session.Load<Song>(songId)
					.UserFavorites
					.Where(a => a.User.Options.PublicRatings)
					.OrderBy(u => u.User.Name)
					.Select(u => new FavoriteSongForUserContract(u, LanguagePreference)).ToArray());

		}

		public ArchivedSongVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedSongVersionDetailsContract(session.Load<ArchivedSongVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedSongVersion>(comparedVersionId) : null, 
					PermissionContext.LanguagePreference));

		}

		public XDocument GetVersionXml(int id) {
			return HandleQuery(session => session.Load<ArchivedSongVersion>(id).Data);
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

				// Create merge record
				var mergeEntry = new SongMergeRecord(source, target);
				session.Save(mergeEntry);

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

				SysLog("reverting " + song + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedSongContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				song.LengthSeconds = fullProperties.LengthSeconds;
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

				var existingTags = TagHelpers.GetTags(session, tags);

				song.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session, new AgentLoginData(user)), new SongTagUsageFactory(session, song));

				return song.Tags.Usages.OrderByDescending(u => u.Count).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		// Not in use currently - done while saving album properties
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

		// Not in use currently - done while saving album properties
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

				SysLog(string.Format("updating properties for {0}", song));

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

				if (song.LengthSeconds != properties.Song.LengthSeconds) {
					diff.Length = true;
					song.LengthSeconds = properties.Song.LengthSeconds;
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

				var logStr = string.Format("updated properties for song {0} ({1})", EntryLinkFactory.CreateEntryLink(song), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				AuditLog(logStr, session);
				AddEntryEditedEntry(session, song, EntryEditEvent.Updated);

				Archive(session, song, diff, SongArchiveReason.PropertiesUpdated, properties.UpdateNotes);

				session.Update(song);
				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

		/*private void SetThumb(SongList list, UploadedFileContract uploadedFile) {

			if (uploadedFile != null) {

				var thumb = new EntryThumb(list, uploadedFile.Mime);
				list.Thumb = thumb;
				var thumbGenerator = new ImageThumbGenerator(new ServerImagePathMapper());
				thumbGenerator.GenerateThumbsAndMoveImage(uploadedFile.Stream, thumb, ImageSizes.Thumb | ImageSizes.SmallThumb);

			}

		}

		public int UpdateSongList(SongListForEditContract contract, UploadedFileContract uploadedFile) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return HandleTransaction(session => {

				var user = GetLoggedUser(session);
				SongList list;

				if (contract.Id == 0) {

					list = CreateSongList(session, contract, uploadedFile);
					
				} else {

					list = session.Load<SongList>(contract.Id);

					EntryPermissionManager.VerifyEdit(PermissionContext, list);

					list.Description = contract.Description;
					list.Name = contract.Name;

					if (EntryPermissionManager.CanManageFeaturedLists(PermissionContext))
						list.FeaturedCategory = contract.FeaturedCategory;

					var songDiff = list.SyncSongs(contract.SongLinks, c => session.Load<Song>(c.SongId));
					SessionHelper.Sync(session, songDiff);
					SetThumb(list, uploadedFile);

					session.Update(list);

					AuditLog(string.Format("updated song list {0}", EntryLinkFactory.CreateEntryLink(list)), session, user);

				}

				return list.Id;

			});

		}*/

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
