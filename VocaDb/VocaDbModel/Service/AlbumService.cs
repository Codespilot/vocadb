using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Artists;
using System.Drawing;
using System;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumService));

		private PartialFindResult<AlbumWithAdditionalNamesContract> Find(
			ISession session, string query, int start, int maxResults, bool draftsOnly,
			bool getTotalCount, NameMatchMode nameMatchMode, bool moveExactToTop) {

			if (string.IsNullOrWhiteSpace(query)) {

				var albumsQ = session.Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					albumsQ = albumsQ.Where(a => a.Status == EntryStatus.Draft);

				var albums = albumsQ
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				var contracts = albums.Select(s => new AlbumWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetAlbumCount(session, query, draftsOnly, nameMatchMode) : 0);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(contracts, count, null, false);

			} else {

				var originalQuery = query;
				bool foundExactMatch = false;

				query = query.Trim();

				var directQ = session.Query<Album>()
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
						|| (s.OriginalRelease.CatNum != null && s.OriginalRelease.CatNum.Contains(query)));

				}

				var direct = directQ
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Take(maxResults)
					.ToArray();

				var additionalNamesQ = session.Query<AlbumName>()
					.Where(m => !m.Album.Deleted);

				if (draftsOnly)
					additionalNamesQ = additionalNamesQ.Where(a => a.Album.Status == EntryStatus.Draft);

				if (nameMatchMode == NameMatchMode.Exact || (nameMatchMode == NameMatchMode.Auto && query.Length < 3)) {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

				} else {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

				}

				var additionalNames = additionalNamesQ
					.Select(m => m.Album)
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Distinct()
					.Take(maxResults)
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

				var contracts = entries
					.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetAlbumCount(session, query, draftsOnly, nameMatchMode) : 0);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(contracts, count, originalQuery, foundExactMatch);

			}

		}

		private int GetAlbumCount(
			ISession session, string query, bool draftsOnly, NameMatchMode nameMatchMode) {

			if (string.IsNullOrWhiteSpace(query)) {

				var albumQ = session.Query<Album>()
					.Where(s => !s.Deleted);

				if (draftsOnly)
					albumQ = albumQ.Where(a => a.Status == EntryStatus.Draft);

				return albumQ.Count();

			}

			var directQ = session.Query<Album>()
				.Where(s => !s.Deleted);

			if (draftsOnly)
				directQ = directQ.Where(a => a.Status == EntryStatus.Draft);

			directQ = AddNameMatchFilter(directQ, query, nameMatchMode);

			var direct = directQ.ToArray();

			var additionalNamesQ = session.Query<AlbumName>()
				.Where(m => !m.Album.Deleted);

			if (draftsOnly)
				additionalNamesQ = additionalNamesQ.Where(a => a.Album.Status == EntryStatus.Draft);

			additionalNamesQ = FindHelpers.AddEntryNameFilter(additionalNamesQ, query, nameMatchMode);

			var additionalNames = additionalNamesQ
				.Select(m => m.Album)
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

		private IEnumerable<Artist> GetAllLabels(ISession session) {

			return session.Query<Artist>().Where(a => a.ArtistType == ArtistType.Label);

		}

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext,entryLinkFactory) {}

		// Not in use currently. Might be re-enabled in the future.
		public ArtistForAlbumContract AddArtist(int albumId, string newArtistName) {

			ParamIs.NotNullOrEmpty(() => newArtistName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var artist = new Artist(newArtistName);

				AuditLog(string.Format("creating a new artist '{0}' to {1}", newArtistName, album), session);

				var artistForAlbum = artist.AddAlbum(album);
				Services.Artists.Archive(session, artist, ArtistArchiveReason.Created, "Created for album '" + album.DefaultName + "'");
				session.Save(artist);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		[Obsolete("Replaced by updating properties")]
		public SongInAlbumContract AddSong(int albumId, int songId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

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

		[Obsolete("Disabled")]
		public AlbumContract Create(string name) {

			ParamIs.NotNullOrWhiteSpace(() => name);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			name = name.Trim();

			return HandleTransaction(session => {

				AuditLog(string.Format("creating a new artist with name '{0}'", name), session);

				var album = new Album(name);

				session.Save(album);

				Archive(session, album, AlbumArchiveReason.Created);
				session.Update(album);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public AlbumContract Create(CreateAlbumContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

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

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public ArtistForAlbumContract CreateForArtist(int artistId, string newAlbumName) {

			ParamIs.NotNullOrWhiteSpace(() => newAlbumName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

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

		}

		public CommentContract CreateComment(int albumId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			message = message.Trim();

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var agent = SessionHelper.CreateAgentLoginData(session, PermissionContext);

				AuditLog(string.Format("creating comment for {0}: '{1}'", 
					EntryLinkFactory.CreateEntryLink(album), 
					message.Truncate(60)), session, agent.User);

				var comment = album.CreateComment(message, agent);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		[Obsolete("Integrated to saving properties")]
		public PVContract CreatePV(int albumId, string pvUrl, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			var result = VideoServiceHelper.ParseByUrl(pvUrl);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				AuditLog(string.Format("creating a PV for {0}", EntryLinkFactory.CreateEntryLink(album)), session);

				var pv = album.CreatePV(result.Service, result.Id, pvType, string.Empty);
				session.Save(pv);

				return new PVContract(pv);

			});
			
		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.DeleteEntries);

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();

			}, skipLog: true);

		}

		public void DeleteArtistForAlbum(int artistForAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

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
					PermissionContext.VerifyPermission(PermissionFlags.ManageUserBlocks);

				comment.Album.Comments.Remove(comment);
				session.Delete(comment);

			});

		}

		public SongInAlbumContract[] DeleteSongInAlbum(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

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

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var pvForAlbum = session.Load<PVForAlbum>(pvForAlbumId);

				AuditLog("deleting " + pvForAlbum, session);

				pvForAlbum.OnDelete();

				session.Delete(pvForAlbum);

			});

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> Find(
			string query, int start, int maxResults, bool draftsOnly, bool getTotalCount, bool moveExactToTop) {

			return HandleQuery(session => Find(session, query, start, maxResults, draftsOnly, getTotalCount, 
				NameMatchMode.Auto, moveExactToTop));

		}

		public int? FindByMikuDbId(int mikuDbId) {

			return HandleQuery(session => {

				var link = session.Query<AlbumWebLink>().FirstOrDefault(w => w.Url.Contains("mikudb.com/" + mikuDbId + "/"));

				return (link != null ? (int?)link.Album.Id : null);

			});

		}

		public AlbumWithAdditionalNamesContract FindByNames(string[] query) {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session, q, 0, 1, false, false, NameMatchMode.Exact, false);

					if (result.Items.Any())
						return result.Items.First();

				}

				return null;

			});

		}

		public AlbumContract GetAlbum(int id) {

			return HandleQuery(session => new AlbumContract(session.Load<Album>(id), PermissionContext.LanguagePreference));

		}

		public AlbumDetailsContract GetAlbumDetails(int id) {

			return HandleQuery(session => {

				var contract = new AlbumDetailsContract(session.Load<Album>(id), PermissionContext.LanguagePreference);
				if (PermissionContext.LoggedUser != null)
					contract.UserHasAlbum = session.Query<AlbumForUser>()
						.Any(a => a.Album.Id == id && a.User.Id == PermissionContext.LoggedUser.Id);
				contract.CommentCount = session.Query<AlbumComment>().Where(c => c.Album.Id == id).Count();
				contract.LatestComments = session.Query<AlbumComment>()
					.Where(c => c.Album.Id == id)
					.OrderByDescending(c => c.Created).Take(3).ToArray()
					.Select(c => new CommentContract(c)).ToArray();

				return contract;

			});

		}

		public AlbumForEditContract GetAlbumForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), GetAllLabels(session), PermissionContext.LanguagePreference));

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

		public ArtistContract[] GetArtists(int albumId) {

			return HandleQuery(session => session.Load<Album>(albumId).Artists
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

				var artists = session.Query<ArtistForAlbum>().Where(a => a.Album.Id == albumId).Where(a => !a.Artist.Deleted).Select(a => a.Artist).ToArray();
				var song = session.Load<Song>(songId);
				//var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				return new TrackPropertiesContract(song, 
					artists, PermissionContext.LanguagePreference);

			});

		}

		public ArchivedAlbumVersionDetailsContract GetVersionDetails(int id) {

			return HandleQuery(session =>
				new ArchivedAlbumVersionDetailsContract(session.Load<ArchivedAlbumVersion>(id), PermissionContext.LanguagePreference));

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionFlags.MergeEntries);

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
					var link = target.CreateWebLink(w.Description, w.Url);
					session.Save(link);
				}

				var artists = source.Artists.Where(a => !target.HasArtist(a.Artist)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					session.Update(a);
				}

				var songs = source.Songs.Where(s => !target.HasSong(s.Song)).ToArray();
				foreach (var s in songs) {
					s.Move(target);
					session.Update(s);
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

				//Archive(session, source, "Merged to '" + target.DefaultName + "'");
				Archive(session, target, AlbumArchiveReason.Merged);

				session.Update(source);
				session.Update(target);

			});

		}

		[Obsolete]
		public SongInAlbumContract[] MoveSongDown(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("moving down " + songInAlbum, session);

				songInAlbum.Album.MoveSongDown(songInAlbum);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.OrderBy(s => s.TrackNumber).Select(s => new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		[Obsolete]
		public SongInAlbumContract[] MoveSongUp(int songInAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				AuditLog("moving up " + songInAlbum, session);

				songInAlbum.Album.MoveSongUp(songInAlbum);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.OrderBy(s => s.TrackNumber).Select(s => new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});
			
		}

		[Obsolete("Integrated to saving properties")]
		public SongInAlbumContract[] ReorderTrack(int songInAlbumId, int? prevSongId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);
				var prevTrack = (prevSongId != null ? session.Load<SongInAlbum>(prevSongId.Value) : null);

				AuditLog("reordering " + songInAlbum, session);

				songInAlbum.Album.ReorderTrack(songInAlbum, prevTrack);
				session.Update(songInAlbum.Album);

				return songInAlbum.Album.Songs.OrderBy(s => s.TrackNumber).OrderBy(s => s.DiscNumber).Select(s => 
					new SongInAlbumContract(s, PermissionContext.LanguagePreference)).ToArray();

			});

		}

		public void Restore(int albumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var album = session.Load<Album>(albumId);

				album.Deleted = false;

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(album), session);

			});

		}

		public EntryRevertedContract RevertToVersion(int archivedAlbumVersionId) {

			PermissionContext.VerifyPermission(PermissionFlags.RestoreEntries);

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
					album.CoverPicture = versionWithPic.CoverPicture;

				// Original release
				album.OriginalRelease = (fullProperties.OriginalRelease != null ? new AlbumRelease(fullProperties.OriginalRelease) : null);

				// Artists
				SessionHelper.RestoreObjectRefs<ArtistForAlbum, Artist>(
					session, warnings, album.AllArtists, fullProperties.Artists, (a1, a2) => (a1.Artist.Id == a2.Id),
					artist => (!artist.HasAlbum(album) ? artist.AddAlbum(album) : null),
					albumForArtist => albumForArtist.Delete());

				// Songs
				SessionHelper.RestoreObjectRefs<SongInAlbum, Song, SongInAlbumRefContract>(
					session, warnings, album.AllSongs, fullProperties.Songs, (a1, a2) => (a1.Song.Id == a2.Id),
					(song, songRef) => (!album.HasSong(song) ? album.AddSong(song, songRef.TrackNumber, songRef.DiscNumber) : null),
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
						session.Save(album.CreatePV(pv.Service, pv.PVId, pv.PVType, pv.Name));
					}

					foreach (var pv in pvDiff.Removed) {
						pv.OnDelete();
						session.Delete(pv);
					}

				}

				Archive(session, album, AlbumArchiveReason.Reverted);
				AuditLog("reverted " + EntryLinkFactory.CreateEntryLink(album) + " to revision " + archivedVersion.Version, session);

				return new EntryRevertedContract(album, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int albumId, string[] tags) {

			ParamIs.NotNull(() => tags);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				tags = tags.Distinct(new CaseInsensitiveStringComparer()).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("tagging {0} with {1}", 
					EntryLinkFactory.CreateEntryLink(album), string.Join(", ", tags)), session, user);

				var existingTags = session.Query<Tag>().ToDictionary(t => t.Name, new CaseInsensitiveStringComparer());

				album.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session), new AlbumTagUsageFactory(session, album));

				return album.Tags.Usages.OrderByDescending(u => u.Count).Take(3).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, PictureDataContract pictureData) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(properties.Id);
				var diff = new AlbumDiff(DoSnapshot(album.ArchivedVersionsManager.GetLatestVersion()));

				AuditLog(string.Format("updating properties for {0}", album));

				if (album.DiscType != properties.DiscType) {
					album.DiscType = properties.DiscType;
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

				var validNames = properties.Names.Where(n => !string.IsNullOrWhiteSpace(n.Value)).ToArray();
				var nameDiff = album.Names.Sync(validNames, album);
				SessionHelper.Sync(session, nameDiff);

				album.Names.UpdateSortNames();

				if (nameDiff.Changed)
					diff.Names = true;

				var webLinkDiff = WebLink.Sync(album.WebLinks, properties.WebLinks, album);
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
					album.CoverPicture = new PictureData(pictureData);
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

						var song = new Song(contract.SongName);
						session.Save(song);

						Services.Songs.Archive(session, song, SongArchiveReason.Created, 
							string.Format("Created for album '{0}'", album.DefaultName));

						AuditLog(string.Format("created {0} for {1}", 
							EntryLinkFactory.CreateEntryLink(song), EntryLinkFactory.CreateEntryLink(album)), session);

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

				var pvDiff = album.SyncPVs(properties.PVs);
				SessionHelper.Sync(session, pvDiff);

				if (pvDiff.Changed)
					diff.PVs = true;

				AuditLog(string.Format("updated properties for {0} ({1})", EntryLinkFactory.CreateEntryLink(album), diff.ChangedFieldsString), session);

				Archive(session, album, diff, AlbumArchiveReason.PropertiesUpdated);
				session.Update(album);
				return new AlbumForEditContract(album, GetAllLabels(session), PermissionContext.LanguagePreference);

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

}
