using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using NLog;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Artists;
using System.Drawing;
using System;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

// ReSharper disable UnusedMember.Local
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
// ReSharper restore UnusedMember.Local

		private IQueryable<Album> AddDiscTypeRestriction(IQueryable<Album> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.DiscType == discType) : query);
		}

		private IQueryable<AlbumName> AddDiscTypeRestriction(IQueryable<AlbumName> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.Album.DiscType == discType) : query);
		}

		private IQueryable<AlbumTagUsage> AddDiscTypeRestriction(IQueryable<AlbumTagUsage> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.Album.DiscType == discType) : query);
		}

		private IQueryable<ArtistForAlbum> AddDiscTypeRestriction(IQueryable<ArtistForAlbum> query, DiscType discType) {
			return (discType != DiscType.Unknown ? query.Where(a => a.Album.DiscType == discType) : query);
		}

		private IQueryable<Album> AddNameMatchFilter(IQueryable<Album> criteria, string name, NameMatchMode matchMode) {

			var mode = FindHelpers.GetMatchMode(name, matchMode);

			if (mode == NameMatchMode.Exact) {

				return criteria.Where(s =>
					s.Names.SortNames.English == name
						|| s.Names.SortNames.Romaji == name
						|| s.Names.SortNames.Japanese == name);

			} else if (mode == NameMatchMode.StartsWith) {

				return criteria.Where(s =>
					s.Names.SortNames.English.StartsWith(name)
						|| s.Names.SortNames.Romaji.StartsWith(name)
						|| s.Names.SortNames.Japanese.StartsWith(name));

			} else {

				return criteria.Where(s =>
					s.Names.SortNames.English.Contains(name)
						|| s.Names.SortNames.Romaji.Contains(name)
						|| s.Names.SortNames.Japanese.Contains(name)
						|| (s.OriginalRelease.CatNum != null && s.OriginalRelease.CatNum.Contains(name)));

			}

		}

		private IQueryable<Album> AddOrder(IQueryable<Album> criteria, AlbumSortRule sortRule, ContentLanguagePreference languagePreference) {

			switch (sortRule) {
				case AlbumSortRule.Name:
					return FindHelpers.AddNameOrder(criteria, languagePreference);
				case AlbumSortRule.ReleaseDate:
					return AddReleaseRestriction(criteria)
						.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day);
				case AlbumSortRule.AdditionDate:
					return criteria.OrderByDescending(a => a.CreateDate);
				case AlbumSortRule.RatingAverage:
					return criteria.OrderByDescending(a => a.RatingAverageInt)
						.ThenByDescending(a => a.RatingCount);
				case AlbumSortRule.NameThenReleaseDate:
					return FindHelpers.AddNameOrder(criteria, languagePreference)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Year)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
						.ThenBy(a => a.OriginalRelease.ReleaseDate.Day);
			}

			return criteria;

		}

		private IQueryable<Album> AddReleaseRestriction(IQueryable<Album> criteria) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null);

		}

		private PartialFindResult<Album> FindAdvanced(
			ISession session, string query, PagingProperties paging, AlbumSortRule sortRule) {

			var queryPlan = new AlbumQueryBuilder().BuildPlan(query);
			return FindAdvanced(session, queryPlan, paging, sortRule);

		}

		private PartialFindResult<Album> FindAdvanced(
			ISession session, QueryPlan<Album> queryPlan, PagingProperties paging, AlbumSortRule sortRule) {

			if (!queryPlan.Any())
				return new PartialFindResult<Album>(new Album[] {}, 0);

			IQueryable<Album> albums = null;

			foreach (var filter in queryPlan) {

				if (albums == null)
					albums = filter.Query(session);
				else
					albums = filter.Filter(albums, session);

			}

			var result = albums
				.Skip(paging.Start)
				.Take(paging.MaxEntries)
				.ToArray();

			return new PartialFindResult<Album>(result, albums.Count());


		}

		private PartialFindResult<Album> Find(
			ISession session, string query, DiscType discType, int start, int maxResults, bool draftsOnly,
			bool getTotalCount, NameMatchMode nameMatchMode, AlbumSortRule sortRule, bool moveExactToTop) {

			Album[] entries;
			string originalQuery = query;
			bool foundExactMatch = false;

			if (string.IsNullOrWhiteSpace(query)) {

				var albumsQ = session.Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					albumsQ = albumsQ.Where(a => a.Status == EntryStatus.Draft);

				albumsQ = AddDiscTypeRestriction(albumsQ, discType);

				albumsQ = AddOrder(albumsQ, sortRule, LanguagePreference);

				entries = albumsQ
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			// TODO: refactor using advanced search parser
			} else if (query.StartsWith("tag:")) {

				var tagName = query.Substring(4);

				var tagQ = session.Query<AlbumTagUsage>()
					.Where(m => !m.Album.Deleted && m.Tag.Name == tagName);

				if (draftsOnly)
					tagQ = tagQ.Where(a => a.Album.Status == EntryStatus.Draft);

				tagQ = AddDiscTypeRestriction(tagQ, discType);
				entries = AddOrder(tagQ.Select(m => m.Album), sortRule, PermissionContext.LanguagePreference)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			// TODO: refactor using advanced search parser
			} else if (query.StartsWith("artist:")) {

				int artistId;
				int.TryParse(query.Substring(7), out artistId);

				var albumQ = session.Query<ArtistForAlbum>()
					.Where(m => !m.Album.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Album.Status == EntryStatus.Draft);

				albumQ = AddDiscTypeRestriction(albumQ, discType);
				entries = AddOrder(albumQ.Select(m => m.Album), sortRule, PermissionContext.LanguagePreference)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

			} else {

				query = query.Trim();

				// Searching by SortNames can be disabled in the future because all names should be included in the Names list anyway.
				var directQ = session.Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

				directQ = AddDiscTypeRestriction(directQ, discType);
				directQ = AddNameMatchFilter(directQ, query, nameMatchMode);

				var direct = AddOrder(directQ, sortRule, LanguagePreference)
					.ToArray();

				var additionalNamesQ = session.Query<AlbumName>()
					.Where(m => !m.Album.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Album.Status == EntryStatus.Draft);

				additionalNamesQ = AddDiscTypeRestriction(additionalNamesQ, discType);

				additionalNamesQ = additionalNamesQ.AddEntryNameFilter(query, nameMatchMode);

				var additionalNames = 
					AddOrder(additionalNamesQ.Select(m => m.Album), sortRule, PermissionContext.LanguagePreference)
					.Distinct()
					.Take(maxResults)	// TODO: verify this
					.ToArray()
					.Where(a => !direct.Contains(a));

				entries = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				if (moveExactToTop) {

					var exactMatch = entries.FirstOrDefault(
						e => e.Names.Any(n => n.Value.Equals(query, StringComparison.InvariantCultureIgnoreCase)));

					if (exactMatch != null) {
						entries = CollectionHelper.MoveToTop(entries, exactMatch).ToArray();
						foundExactMatch = true;
					}

				}

			}

			var count = (getTotalCount ? GetAlbumCount(session, query, discType, draftsOnly, nameMatchMode, sortRule) : 0);

			return new PartialFindResult<Album>(entries, count, originalQuery, foundExactMatch);


		}

		private int GetAlbumCount(
			ISession session, string query, DiscType discType, bool draftsOnly, NameMatchMode nameMatchMode, AlbumSortRule sortRule) {

			if (string.IsNullOrWhiteSpace(query)) {

				var albumQ = session.Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Status == EntryStatus.Draft);

				if (sortRule == AlbumSortRule.ReleaseDate)
					albumQ = AddReleaseRestriction(albumQ);

				albumQ = AddDiscTypeRestriction(albumQ, discType);

				return albumQ.Count();

			}

			if (query.StartsWith("tag:")) {

				var tagName = query.Substring(4);

				var tagQ = session.Query<AlbumTagUsage>()
					.Where(m => !m.Album.Deleted && m.Tag.Name == tagName);

				if (draftsOnly)
					tagQ = tagQ.Where(a => a.Album.Status == EntryStatus.Draft);

				tagQ = AddDiscTypeRestriction(tagQ, discType);

				return tagQ.Count();

			}

			if (query.StartsWith("artist:")) {

				int artistId;
				int.TryParse(query.Substring(7), out artistId);

				var albumQ = session.Query<ArtistForAlbum>()
					.Where(m => !m.Album.Deleted && m.Artist.Id == artistId);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Album.Status == EntryStatus.Draft);

				albumQ = AddDiscTypeRestriction(albumQ, discType);
				return albumQ.Count();

			}

			var directQ = session.Query<Album>()
				.Where(s => !s.Deleted);

			if (draftsOnly)
				directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

			if (sortRule == AlbumSortRule.ReleaseDate)
				directQ = AddReleaseRestriction(directQ);

			directQ = AddDiscTypeRestriction(directQ, discType);

			directQ = AddNameMatchFilter(directQ, query, nameMatchMode);

			var direct = directQ.ToArray();

			var additionalNamesQ = session.Query<AlbumName>()
				.Where(m => !m.Album.Deleted);

			if (draftsOnly)
				additionalNamesQ = additionalNamesQ.Where(a => a.Album.Status == EntryStatus.Draft);

			additionalNamesQ = AddDiscTypeRestriction(additionalNamesQ, discType);

			additionalNamesQ = additionalNamesQ.AddEntryNameFilter(query, nameMatchMode);

			var additionalNamesAlbumQ = additionalNamesQ.Select(a => a.Album);

			if (sortRule == AlbumSortRule.ReleaseDate)
				additionalNamesAlbumQ = AddReleaseRestriction(additionalNamesAlbumQ);

			var additionalNames = additionalNamesAlbumQ
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

		private ArtistForAlbum RestoreArtistRef(Album album, Artist artist, ArchivedArtistForAlbumContract albumRef) {

			if (artist != null) {

				return (!artist.HasAlbum(album) ? artist.AddAlbum(album, albumRef.IsSupport, albumRef.Roles) : null);

			} else {

				return album.AddArtist(albumRef.NameHint, albumRef.IsSupport, albumRef.Roles);

			}

		}

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext,entryLinkFactory) {}

		public ArtistForAlbumContract AddArtist(int albumId, string newArtistName) {

			ParamIs.NotNullOrEmpty(() => newArtistName);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("adding custom artist '{0}' to {1}", newArtistName, CreateEntryLink(album)), session);

				var artistForAlbum = album.AddArtist(newArtistName, false, ArtistRoles.Default);
				session.Save(artistForAlbum);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		[Obsolete("Replaced by updating properties")]
		public SongInAlbumContract AddSong(int albumId, int songId) {

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var song = session.Load<Song>(songId);

				AuditLog(string.Format("adding {0} to {1}", song, album), session);

				var songInAlbum = album.AddSong(song);
				session.Save(songInAlbum);

				return new SongInAlbumContract(songInAlbum, PermissionContext.LanguagePreference);

			});

		}

		public void Archive(ISession session, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Album album, AlbumArchiveReason reason, string notes = "") {

			Archive(session, album, new AlbumDiff(), reason, notes);

		}

		/*
		[Obsolete("Disabled")]
		public AlbumContract Create(string name) {

			ParamIs.NotNullOrWhiteSpace(() => name);

			VerifyManageDatabase();

			name = name.Trim();

			return HandleTransaction(session => {

				AuditLog(string.Format("creating a new artist with name '{0}'", name), session);

				var album = new Album(name);

				session.Save(album);

				Archive(session, album, AlbumArchiveReason.Created);
				session.Update(album);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}*/

		public AlbumContract Create(CreateAlbumContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			VerifyManageDatabase();

			return HandleTransaction(session => {

				AuditLog(string.Format("creating a new album with name '{0}'", contract.Names.First().Value));

				var album = new Album { DiscType = contract.DiscType };

				album.Names.Init(contract.Names, album);

				session.Save(album);

				foreach (var artist in contract.Artists) {
					session.Save(session.Load<Artist>(artist.Id).AddAlbum(album));
				}

				album.UpdateArtistString();
				Archive(session, album, AlbumArchiveReason.Created);
				session.Update(album);

				AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(album)), session);
				AddEntryEditedEntry(session, album, EntryEditEvent.Created);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		/*
		[Obsolete]
		public ArtistForAlbumContract CreateForArtist(int artistId, string newAlbumName) {

			ParamIs.NotNullOrWhiteSpace(() => newAlbumName);

			VerifyManageDatabase();

			newAlbumName = newAlbumName.Trim();

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				AuditLog(string.Format("creating a new album '{0}' for {1}", 
					newAlbumName, EntryLinkFactory.CreateEntryLink(artist)), session);

				var album = new Album(newAlbumName);

				session.Save(album);
				var artistForAlbum = artist.AddAlbum(album);
				session.Update(artist);
				session.Save(artistForAlbum);

				album.UpdateArtistString();
				Archive(session, album, AlbumArchiveReason.Created);
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}*/

		public CommentContract CreateComment(int albumId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var agent = SessionHelper.CreateAgentLoginData(session, PermissionContext);

				AuditLog(string.Format("creating comment for {0}: '{1}'", 
					EntryLinkFactory.CreateEntryLink(album), 
					HttpUtility.HtmlEncode(message)), session, agent.User);

				var comment = album.CreateComment(message, agent);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		/*
		[Obsolete("Integrated to saving properties")]
		public PVContract CreatePV(int albumId, string pvUrl, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			VerifyManageDatabase();

			var result = VideoServiceHelper.ParseByUrl(pvUrl);

			if (!result.IsOk)
				throw result.Exception;

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				AuditLog(string.Format("creating a PV for {0}", EntryLinkFactory.CreateEntryLink(album)), session);

				var pv = album.CreatePV(result.Service, result.Id, pvType, string.Empty);
				session.Save(pv);

				return new PVContract(pv);

			});
			
		}*/

		public bool CreateReport(int albumId, AlbumReportType reportType, string hostname, string notes) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(session => {

				var loggedUserId = PermissionContext.LoggedUserId;
				var existing = session.Query<AlbumReport>()
					.FirstOrDefault(r => r.Album.Id == albumId 
						&& ((loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname));

				if (existing != null)
					return false;

				var album = session.Load<Album>(albumId);
				var report = new AlbumReport(album, reportType, GetLoggedUserOrDefault(session), hostname, notes.Truncate(200));

				var msg = string.Format("reported {0} as {1} ({2})", EntryLinkFactory.CreateEntryLink(album), reportType, HttpUtility.HtmlEncode(notes));
				AuditLog(msg.Truncate(200), session, new AgentLoginData(GetLoggedUserOrDefault(session), hostname));

				session.Save(report);
				return true;

			}, IsolationLevel.ReadUncommitted);

		}

		public void Delete(int id) {

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			}, PermissionToken.DeleteEntries, skipLog: true);

		}

		public void DeleteArtistForAlbum(int artistForAlbumId) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);

				AuditLog(string.Format("deleting {0}", artistForAlbum), session);

				artistForAlbum.Album.DeleteArtistForAlbum(artistForAlbum);
				session.Delete(artistForAlbum);
				session.Update(artistForAlbum.Album);

			});

		}

		public void DeleteComment(int commentId) {

			HandleTransaction(session => {

				var comment = session.Load<AlbumComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				comment.Album.Comments.Remove(comment);
				session.Delete(comment);

			});

		}

		public SongInAlbumContract[] DeleteSongInAlbum(int songInAlbumId) {

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("removing " + songInAlbum, session);

				songInAlbum.OnDeleting();
				session.Delete(songInAlbum);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.Select(s => new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		[Obsolete("Integrated to saving properties")]
		public void DeletePv(int pvForAlbumId) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var pvForAlbum = session.Load<PVForAlbum>(pvForAlbumId);

				AuditLog("deleting " + pvForAlbum, session);

				pvForAlbum.OnDelete();

				session.Delete(pvForAlbum);

			});

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> Find(
			string query, DiscType discType, int start, int maxResults, bool draftsOnly, bool getTotalCount, 
			NameMatchMode nameMatchMode = NameMatchMode.Auto, AlbumSortRule sortRule = AlbumSortRule.Name, bool moveExactToTop = false) {

			return HandleQuery(session => {
				
				var results = Find(session, query, discType, start, maxResults, draftsOnly, getTotalCount,
					nameMatchMode, sortRule, moveExactToTop);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(
					results.Items.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference)).ToArray(),
					results.TotalCount, results.Term, results.FoundExactMatch);

			});

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> FindAdvanced(
			string query, PagingProperties paging, AlbumSortRule sortRule) {

			return HandleQuery(session => {

				var results = FindAdvanced(session, query, paging, sortRule);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(
					results.Items.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference)).ToArray(),
					results.TotalCount, results.Term, results.FoundExactMatch);

			});

		}

		public int? FindByMikuDbId(int mikuDbId) {

			return HandleQuery(session => {

				var link = session.Query<AlbumWebLink>()
					.FirstOrDefault(w => !w.Album.Deleted && w.Url.Contains("mikudb.com/" + mikuDbId + "/"));

				return (link != null ? (int?)link.Album.Id : null);

			});

		}

		public AlbumWithAdditionalNamesContract FindByNames(string[] query) {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session, q, DiscType.Unknown, 0, 1, false, false, NameMatchMode.Exact, AlbumSortRule.Name, false);

					if (result.Items.Any())
						return new AlbumWithAdditionalNamesContract(result.Items.First(), PermissionContext.LanguagePreference);

				}

				return null;

			});

		}

		public EntryRefWithNameContract[] FindDuplicates(string[] anyName) {

			var names = anyName.Select(n => n.Trim()).Where(n => n != string.Empty).ToArray();

			if (!names.Any())
				return new EntryRefWithNameContract[] { };

			return HandleQuery(session => {

				return session.Query<AlbumName>()
					.Where(n => names.Contains(n.Value))
					.Select(n => n.Album)
					.Where(n => !n.Deleted)
					.Distinct()
					.Take(10)
					.ToArray()
					.Select(n => new EntryRefWithNameContract(n, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public AlbumDetailsContract FindFirstDetails(string query) {

			return HandleQuery(session => {

				var result = Find(session, query, DiscType.Unknown, 0, 1, false, false, NameMatchMode.Auto, AlbumSortRule.Name, true);

				if (result.Items.Any())
					return new AlbumDetailsContract(result.Items.First(), PermissionContext.LanguagePreference);

				return null;

			});

		}

		public string[] FindNames(string query, int maxResults) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] { };

			query = query.Trim();

			return HandleQuery(session => {

				var names = session.Query<AlbumName>()
					.Where(a => !a.Album.Deleted)
					.AddEntryNameFilter(query, NameMatchMode.Auto)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				return names;

			});

		}

		public string[] FindReleaseEvents(string query) {

			return HandleQuery(session => {

				return session.Query<Album>()
					.Where(a => !a.Deleted)
					.Select(a => a.OriginalRelease.EventName)
					.Where(e => e.StartsWith(query))
					.OrderBy(e => e)
					.Distinct()
					.Take(10).ToArray();

			});

		}

		public AlbumContract GetAlbum(int id) {

			return HandleQuery(session => new AlbumContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumWithAdditionalNamesContract GetAlbumByLink(string link) {

			return HandleQuery(session => {

				var webLink = session.Query<AlbumWebLink>().FirstOrDefault(p => p.Url.Contains(link));

				return (webLink != null ? new AlbumWithAdditionalNamesContract(webLink.Album, PermissionContext.LanguagePreference) : null);

			});

		}

		public AlbumDetailsContract GetAlbumDetails(int id, string hostname) {

			return HandleQuery(session => {

				var album = session.Load<Album>(id);
				var contract = new AlbumDetailsContract(album, PermissionContext.LanguagePreference);

				var user = PermissionContext.LoggedUser;

				if (user != null) {

					var albumForUser = session.Query<AlbumForUser>()
						.FirstOrDefault(a => a.Album.Id == id && a.User.Id == user.Id);

					contract.AlbumForUser = (albumForUser != null ? new AlbumForUserContract(albumForUser, PermissionContext.LanguagePreference) : null);

				}

				contract.CommentCount = session.Query<AlbumComment>().Count(c => c.Album.Id == id);
				contract.LatestComments = session.Query<AlbumComment>()
					.Where(c => c.Album.Id == id)
					.OrderByDescending(c => c.Created).Take(3).ToArray()
					.Select(c => new CommentContract(c)).ToArray();
				contract.Hits = session.Query<AlbumHit>().Count(h => h.Album.Id == id);

				if (user != null || !string.IsNullOrEmpty(hostname)) {

					var agentNum = (user != null ? user.Id : hostname.GetHashCode());

					using (var tx = session.BeginTransaction(IsolationLevel.ReadUncommitted)) {

						var isHit = session.Query<AlbumHit>().Any(h => h.Album.Id == id && h.Agent == agentNum);

						if (!isHit) {
							var hit = new AlbumHit(album, agentNum);
							session.Save(hit);
						}

						tx.Commit();

					}

				}


				return contract;

			});

		}

		public AlbumForEditContract GetAlbumForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumWithAdditionalNamesContract GetAlbumWithAdditionalNames(int id) {

			return HandleQuery(session => new AlbumWithAdditionalNamesContract(
				session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumWithArchivedVersionsContract GetAlbumWithArchivedVersions(int albumId) {

			return HandleQuery(session => 
				new AlbumWithArchivedVersionsContract(session.Load<Album>(albumId), PermissionContext.LanguagePreference));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.Where(a => !a.Deleted)
				.ToArray()
				.OrderBy(a => a.DefaultName)
				.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public EntryForPictureDisplayContract GetArchivedAlbumPicture(int archivedVersionId) {

			return HandleQuery(session =>
				EntryForPictureDisplayContract.Create(
				session.Load<ArchivedAlbumVersion>(archivedVersionId), PermissionContext.LanguagePreference));

		}

		public ArtistContract[] GetArtists(int albumId, ArtistType[] types) {

			return HandleQuery(session => session.Load<Album>(albumId).Artists.Where(a => a.Artist != null && types.Contains(a.Artist.ArtistType))
				.Select(a => new ArtistContract(a.Artist, PermissionContext.LanguagePreference)).ToArray());

		}

		public CommentContract[] GetComments(int albumId) {

			return HandleQuery(session => {

				return session.Query<AlbumComment>()
					.Where(c => c.Album.Id == albumId)
					.OrderByDescending(c => c.Created)
					.Select(c => new CommentContract(c)).ToArray();

			});

		}

		/// <summary>
		/// Gets the cover picture for a <see cref="Album"/>.
		/// </summary>
		/// <param name="id">Album Id.</param>
		/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
		/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
		public EntryForPictureDisplayContract GetCoverPicture(int id, Size requestedSize) {

			return HandleQuery(session =>
				EntryForPictureDisplayContract.Create(session.Load<Album>(id), PermissionContext.LanguagePreference, requestedSize));

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> GetDeleted(int start, int maxEntries) {

			return HandleQuery(session => {

				var albums = session
					.Query<Album>()
					.Where(a => a.Deleted)
					.Skip(start)
					.Take(maxEntries)
					.ToArray()
					.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = session
					.Query<Album>().Count(a => a.Deleted);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(albums, count);

			});

		}

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int albumId) {

			return HandleQuery(session => { 
				
				var album = session.Load<Album>(albumId);
				return new EntryWithTagUsagesContract(album, album.Tags.Usages);

			});

		}

		public TagSelectionContract[] GetTagSelections(int albumId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<AlbumTagUsage>().Where(a => a.Album.Id == albumId).ToArray();
				var tagVotes = session.Query<AlbumTagVote>().Where(a => a.User.Id == userId && a.Usage.Album.Id == albumId).ToArray();

				var tagSelections = tagsInUse.Select(t => 
					new TagSelectionContract(t.Tag.Name, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public TrackPropertiesContract GetTrackProperties(int albumId, int songId) {

			return HandleQuery(session => {

				var artists = session.Query<ArtistForAlbum>()
					.Where(a => a.Album.Id == albumId && a.Artist != null && !a.Artist.Deleted 
						&& ArtistHelper.SongArtistTypes.Contains(a.Artist.ArtistType))
					.Select(a => a.Artist)
					.ToArray();
				var song = session.Load<Song>(songId);

				return new TrackPropertiesContract(song, 
					artists, PermissionContext.LanguagePreference);

			});

		}

		public ArchivedAlbumVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedAlbumVersionDetailsContract(session.Load<ArchivedAlbumVersion>(id), 
					(comparedVersionId != 0 ? session.Load<ArchivedAlbumVersion>(comparedVersionId) : null), PermissionContext.LanguagePreference));

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target albums can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Album>(sourceId);
				var target = session.Load<Album>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)), session);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					session.Save(link);
				}

				var artists = source.Artists.Where(a => !target.HasArtistForAlbum(a)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				var songs = source.Songs.Where(s => !target.HasSong(s.Song)).ToArray();
				foreach (var s in songs) {
					s.Move(target);
					session.Update(s);
				}

				var pictures = source.Pictures.ToArray();
				foreach (var p in pictures) {
					p.Move(target);
					session.Update(p);
				}

				var userCollections = source.UserCollections.Where(a => !target.IsInUserCollection(a.User)).ToArray();
				foreach (var u in userCollections) {
					u.Move(target);
					session.Update(u);
				}

				if (target.Description == string.Empty)
					target.Description = source.Description;

				if (target.OriginalRelease == null)
					target.OriginalRelease = new AlbumRelease();

				if (string.IsNullOrEmpty(target.OriginalRelease.CatNum) && source.OriginalRelease != null)
					target.OriginalRelease.CatNum = source.OriginalRelease.CatNum;

				if (string.IsNullOrEmpty(target.OriginalRelease.EventName) && source.OriginalRelease != null)
					target.OriginalRelease.EventName = source.OriginalRelease.EventName;

				if (target.OriginalRelease.ReleaseDate == null)
					target.OriginalRelease.ReleaseDate = new OptionalDateTime();

				if (target.OriginalReleaseDate.Year == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Year = source.OriginalReleaseDate.Year;

				if (target.OriginalReleaseDate.Month == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Month = source.OriginalReleaseDate.Month;

				if (target.OriginalReleaseDate.Day == null && source.OriginalRelease != null)
					target.OriginalReleaseDate.Day = source.OriginalReleaseDate.Day;

				source.Deleted = true;

				target.UpdateArtistString();
				target.Names.UpdateSortNames();

				Archive(session, target, AlbumArchiveReason.Merged, string.Format("Merged from {0}", source));

				session.Update(source);
				session.Update(target);

			});

		}

		public int MoveToTrash(int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("moving {0} to trash", album), session);

				var archived = new ArchivedAlbumContract(album, new AlbumDiff(true));
				var data = XmlHelper.SerializeToXml(archived);
				var trashed = new TrashedEntry(album, data, GetLoggedUser(session));

				session.Save(trashed);

				album.DeleteLinks();
				session.Delete(album);

				return trashed.Id;

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return RemoveTagUsage<AlbumTagUsage>(tagUsageId);

		}

		public void Restore(int albumId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				album.Deleted = false;

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(album), session);

			});

		}

		public EntryRevertedContract RevertToVersion(int archivedAlbumVersionId) {

			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

			return HandleTransaction(session => {

				var archivedVersion = session.Load<ArchivedAlbumVersion>(archivedAlbumVersionId);
				var album = archivedVersion.Album;

				AuditLog("reverting " + album + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedAlbumContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				album.Description = fullProperties.Description;
				album.DiscType = fullProperties.DiscType;
				album.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;

				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(AlbumEditableFields.Cover);

				if (versionWithPic != null)
					album.CoverPictureData = versionWithPic.CoverPicture;

				// Original release
				album.OriginalRelease = (fullProperties.OriginalRelease != null ? new AlbumRelease(fullProperties.OriginalRelease) : null);

				// Artists
				SessionHelper.RestoreObjectRefs<ArtistForAlbum, Artist, ArchivedArtistForAlbumContract>(
					session, warnings, album.AllArtists, fullProperties.Artists, 
					(a1, a2) => (a1.Artist != null && a1.Artist.Id == a2.Id) || (a1.Artist == null && a2.Id == 0 && a1.Name == a2.NameHint),
					(artist, albumRef) => RestoreArtistRef(album, artist, albumRef),
					albumForArtist => albumForArtist.Delete());

				// Songs
				SessionHelper.RestoreObjectRefs<SongInAlbum, Song, SongInAlbumRefContract>(
					session, warnings, album.AllSongs, fullProperties.Songs, (a1, a2) => (a1.Song.Id == a2.Id),
					(song, songRef) => (!album.HasSong(song) ? album.AddSong(song, songRef.TrackNumber, Math.Min(songRef.DiscNumber, 1)) : null),
					songInAlbum => songInAlbum.Delete());

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = album.Names.SyncByContent(fullProperties.Names, album);
					SessionHelper.Sync(session, nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(album.WebLinks, fullProperties.WebLinks, album);
					SessionHelper.Sync(session, webLinkDiff);
				}

				// PVs
				if (fullProperties.PVs != null) {

					var pvDiff = CollectionHelper.Diff(album.PVs, fullProperties.PVs, (p1, p2) => (p1.PVId == p2.PVId && p1.Service == p2.Service));

					foreach (var pv in pvDiff.Added) {
						session.Save(album.CreatePV(new PVContract(pv)));
					}

					foreach (var pv in pvDiff.Removed) {
						pv.OnDelete();
						session.Delete(pv);
					}

				}

				album.UpdateArtistString();
				album.UpdateRatingTotals();

				Archive(session, album, AlbumArchiveReason.Reverted);
				AuditLog("reverted " + EntryLinkFactory.CreateEntryLink(album) + " to revision " + archivedVersion.Version, session);

				return new EntryRevertedContract(album, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int albumId, string[] tags) {

			ParamIs.NotNull(() => tags);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				tags = tags.Distinct(new CaseInsensitiveStringComparer()).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("tagging {0} with {1}", 
					EntryLinkFactory.CreateEntryLink(album), string.Join(", ", tags)), session, user);

				var existingTags = session.Query<Tag>().ToDictionary(t => t.Name, new CaseInsensitiveStringComparer());

				album.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session, new AgentLoginData(user)), new AlbumTagUsageFactory(session, album));

				return album.Tags.Usages.OrderByDescending(u => u.Count).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public void UpdateAllReleaseEventNames(string old, string newName) {

			ParamIs.NotNullOrWhiteSpace(() => old);
			ParamIs.NotNullOrWhiteSpace(() => newName);

			old = old.Trim();
			newName = newName.Trim();

			if (old.Equals(newName))
				return;

			VerifyManageDatabase();

			HandleTransaction(session => {

				AuditLog("replacing release event name '" + old + "' with '" + newName + "'", session);

				var albums = session.Query<Album>().Where(a => a.OriginalRelease.EventName == old).ToArray();

				foreach (var a in albums) {
					a.OriginalRelease.EventName = newName;
					session.Update(a);
				}

			});

		}

		public void UpdateArtistForAlbumIsSupport(int artistForAlbumId, bool isSupport) {

			VerifyManageDatabase();

			HandleTransaction(session => {
				
				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);
				var album = artistForAlbum.Album;

				artistForAlbum.IsSupport = isSupport;
				album.UpdateArtistString();

				AuditLog(string.Format("updated IsSupport for {0} on {1}", artistForAlbum.ArtistToStringOrName, CreateEntryLink(album)), session);

				session.Update(album);

			});

		}

		public void UpdateArtistForAlbumRoles(int artistForAlbumId, ArtistRoles roles) {

			VerifyManageDatabase();

			HandleTransaction(session => {

				var artistForAlbum = session.Load<ArtistForAlbum>(artistForAlbumId);
				var album = artistForAlbum.Album;

				artistForAlbum.Roles = roles;
				album.UpdateArtistString();

				AuditLog(string.Format("updated roles for {0} on {1}", artistForAlbum.ArtistToStringOrName, CreateEntryLink(album)), session);

				session.Update(album);

			});

		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, PictureDataContract pictureData) {

			ParamIs.NotNull(() => properties);

			return HandleQuery(session => {
				using (var tx = session.BeginTransaction()) {

					var album = session.Load<Album>(properties.Id);

					VerifyEntryEdit(album);

					var diff = new AlbumDiff(DoSnapshot(album.ArchivedVersionsManager.GetLatestVersion(), GetLoggedUser(session)));

					AuditLog(string.Format("updating properties for {0}", album));

					if (album.DiscType != properties.DiscType) {
						album.DiscType = properties.DiscType;
						album.UpdateArtistString();
						diff.DiscType = true;
					}

					if (album.Description != properties.Description) {
						album.Description = properties.Description;
						diff.Description = true;
					}

					if (album.TranslatedName.DefaultLanguage != properties.TranslatedName.DefaultLanguage) {
						album.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;
						diff.OriginalName = true;
					}

					var validNames = properties.Names.AllNames;
					var nameDiff = album.Names.Sync(validNames, album);
					SessionHelper.Sync(session, nameDiff);

					album.Names.UpdateSortNames();

					if (nameDiff.Changed)
						diff.Names = true;

					var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
					var webLinkDiff = WebLink.Sync(album.WebLinks, validWebLinks, album);
					SessionHelper.Sync(session, webLinkDiff);

					if (webLinkDiff.Changed)
						diff.WebLinks = true;

					var newOriginalRelease = (properties.OriginalRelease != null ? new AlbumRelease(properties.OriginalRelease) : new AlbumRelease());

					if (album.OriginalRelease == null)
						album.OriginalRelease = new AlbumRelease();

					if (!album.OriginalRelease.Equals(newOriginalRelease)) {
						album.OriginalRelease = newOriginalRelease;
						diff.OriginalRelease = true;
					}

					if (pictureData != null) {
						album.CoverPictureData = new PictureData(pictureData);
						diff.Cover = true;
					}

					if (album.Status != properties.Status) {
						album.Status = properties.Status;
						diff.Status = true;
					}

					var songGetter = new Func<SongInAlbumEditContract, Song>(contract => {

						if (contract.SongId != 0)
							return session.Load<Song>(contract.SongId);
						else {

							AuditLog(string.Format("creating a new song '{0}' to {1}", contract.SongName, album));

							var song = new Song(new LocalizedString(contract.SongName, ContentLanguageSelection.Unspecified));
							session.Save(song);

							Services.Songs.Archive(session, song, SongArchiveReason.Created,
								string.Format("Created for album '{0}'", album.DefaultName));

							AuditLog(string.Format("created {0} for {1}",
								EntryLinkFactory.CreateEntryLink(song), EntryLinkFactory.CreateEntryLink(album)), session);
							AddEntryEditedEntry(session, song, EntryEditEvent.Created);

							return song;

						}

					});

					var tracksDiff = album.SyncSongs(properties.Songs, songGetter);

					SessionHelper.Sync(session, tracksDiff);

					if (tracksDiff.Changed) {

						var add = string.Join(", ", tracksDiff.Added.Select(i => i.Song.ToString()));
						var rem = string.Join(", ", tracksDiff.Removed.Select(i => i.Song.ToString()));
						var edit = string.Join(", ", tracksDiff.Edited.Select(i => i.Song.ToString()));

						var str = string.Format("edited tracks (added: {0}, removed: {1}, reordered: {2})", add, rem, edit)
							.Truncate(300);

						AuditLog(str, session);

						diff.Tracks = true;

					}

					var picsDiff = album.Pictures.SyncPictures(properties.Pictures, GetLoggedUser(session), album.CreatePicture);
					SessionHelper.Sync(session, picsDiff);
					ImageHelper.GenerateThumbsAndMoveImages(picsDiff.Added);

					if (picsDiff.Changed)
						diff.Pictures = true;

					var pvDiff = album.SyncPVs(properties.PVs);
					SessionHelper.Sync(session, pvDiff);

					if (pvDiff.Changed)
						diff.PVs = true;

					var logStr = string.Format("updated properties for {0} ({1})", 
						EntryLinkFactory.CreateEntryLink(album), diff.ChangedFieldsString)
						+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
						.Truncate(400);

					AuditLog(logStr, session);

					AddEntryEditedEntry(session, album, EntryEditEvent.Updated);

					Archive(session, album, diff, AlbumArchiveReason.PropertiesUpdated, properties.UpdateNotes);
					session.Update(album);
					tx.Commit();
					return new AlbumForEditContract(album, PermissionContext.LanguagePreference);
				}
			});

		}

	}

	public class AlbumTagUsageFactory : ITagUsageFactory<AlbumTagUsage> {

		private readonly Album album;
		private readonly ISession session;

		public AlbumTagUsageFactory(ISession session, Album album) {
			this.session = session;
			this.album = album;
		}

		public AlbumTagUsage CreateTagUsage(Tag tag) {

			var usage = new AlbumTagUsage(album, tag);
			session.Save(usage);

			return usage;

		}

	}

	public enum AlbumSortRule {

		None,

		Name,

		ReleaseDate,

		AdditionDate,

		RatingAverage,

		NameThenReleaseDate

	}

}
