using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
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
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Artists;
using System.Drawing;
using System;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service {

	public class AlbumService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(AlbumService));

		private PartialFindResult<AlbumWithAdditionalNamesContract> Find(
			ISession session, string query, int start, int maxResults, bool getTotalCount = false, NameMatchMode nameMatchMode = NameMatchMode.Auto) {

			if (string.IsNullOrWhiteSpace(query)) {

				var albums = session.Query<Album>()
					.Where(s => !s.Deleted)
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Skip(start)
					.Take(maxResults)
					.ToArray();

				var contracts = albums.Select(s => new AlbumWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetAlbumCount(session, query) : 0);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(contracts, count);

			} else {

				var directQ = session.Query<Album>()
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
							|| s.ArtistString.Contains(query));

				}

				var direct = directQ
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Take(maxResults)
					.ToArray();

				var additionalNamesQ = session.Query<AlbumName>()
					.Where(m => !m.Album.Deleted);

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

				var contracts = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.Select(a => new AlbumWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetAlbumCount(session, query) : 0);

				return new PartialFindResult<AlbumWithAdditionalNamesContract>(contracts, count);

			}

		}

		private int GetAlbumCount(ISession session, string query) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Album>()
					.Where(s => !s.Deleted)
					.Count();

			}

			var directQ = session.Query<Album>()
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
						|| s.ArtistString.Contains(query));

			}

			var direct = directQ.ToArray();

			var additionalNamesQ = session.Query<AlbumName>()
				.Where(m => !m.Album.Deleted);

			if (query.Length < 3) {

				additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

			} else {

				additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

			}

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

		public AlbumService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext) 
			: base(sessionFactory, permissionContext) {}

		public ArtistForAlbumContract AddArtist(int albumId, string newArtistName) {

			ParamIs.NotNullOrEmpty(() => newArtistName);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var artist = new Artist(newArtistName);

				AuditLog(string.Format("creating a new artist '{0}' to {1}", newArtistName, album), session);

				var artistForAlbum = artist.AddAlbum(album);
				Services.Artists.Archive(session, artist, "Created for album '" + album.DefaultName + "'");
				session.Save(artist);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

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

		public void Archive(ISession session, Album album, AlbumDiff diff, string notes = "") {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Album album, string notes = "") {

			Archive(session, album, new AlbumDiff(), notes);

		}

		public AlbumContract Create(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				AuditLog(string.Format("creating a new artist with name '{0}'", name), session);

				var album = new Album(name);

				session.Save(album);

				Archive(session, album);
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

				AuditLog(string.Format("creating a new album with name '{0}'", contract.Names.First().Value), session);

				var album = new Album { DiscType = contract.DiscType };

				album.Names.Init(contract.Names, album);

				session.Save(album);

				foreach (var artist in contract.Artists) {
					session.Save(session.Load<Artist>(artist.Id).AddAlbum(album));
				}

				album.UpdateArtistString();
				Archive(session, album, "Created");
				session.Update(album);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public ArtistForAlbumContract CreateForArtist(int artistId, string newAlbumName) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				AuditLog("creating a new album '" + newAlbumName + "' for " + artist, session);

				var album = new Album(newAlbumName);

				session.Save(album);
				var artistForAlbum = artist.AddAlbum(album);
				session.Update(artist);
				session.Save(artistForAlbum);

				album.UpdateArtistString();
				Archive(session, album, "Created for artist '" + artist.DefaultName + "'");
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int albumId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			message = message.Trim();

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				var agent = SessionHelper.CreateAgentLoginData(session, PermissionContext);

				AuditLog("creating comment for " + album + ": '" + message.Substring(0, Math.Min(message.Length, 30)) + "'", session, agent.User);

				var comment = album.CreateComment(message.Trim(), agent);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		public PVContract CreatePV(int albumId, string pvUrl, PVType pvType) {

			ParamIs.NotNullOrEmpty(() => pvUrl);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			var result = VideoServiceHelper.ParseByUrl(pvUrl);

			return HandleTransaction(session => {

				var album = session.Load<Album>(albumId);
				AuditLog("creating a PV for " + album, session);

				var pv = album.CreatePV(result.Service, result.Id, pvType);
				session.Save(pv);

				return new PVContract(pv);

			});
			
		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.DeleteEntries);

			UpdateEntity<Album>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", a), session);

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

				if (PermissionContext.LoggedUser == null)
					throw new NotAllowedException("Must be logged in");

				var comment = session.Load<AlbumComment>(commentId);

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);

				AuditLog("deleting comment " + comment, session, user);

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

		public void DeletePv(int pvForAlbumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			HandleTransaction(session => {

				var pvForAlbum = session.Load<PVForAlbum>(pvForAlbumId);

				AuditLog("deleting " + pvForAlbum, session);

				pvForAlbum.OnDelete();

				session.Delete(pvForAlbum);

			});

		}

		public PartialFindResult<AlbumWithAdditionalNamesContract> Find(string query, int start, int maxResults, bool getTotalCount = false) {

			return HandleQuery(session => Find(session, query, start, maxResults, getTotalCount));

		}

		public AlbumWithAdditionalNamesContract FindByNames(string[] query) {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = Find(session, q, 0, 1, false, NameMatchMode.Exact);

					if (result.Items.Any())
						return result.Items.First();

				}

				return null;

			});

		}

		public AlbumContract GetAlbum(int id) {

			return HandleQuery(session => {

				return new AlbumContract(session.Load<Album>(id), PermissionContext.LanguagePreference);

			});

		}

		public AlbumDetailsContract GetAlbumDetails(int id) {

			return HandleQuery(session => {

				var contract = new AlbumDetailsContract(session.Load<Album>(id), PermissionContext.LanguagePreference);
				if (PermissionContext.LoggedUser != null)
					contract.UserHasAlbum = session.Query<AlbumForUser>()
						.Any(a => a.Album.Id == id && a.User.Id == PermissionContext.LoggedUser.Id);

				return contract;

			});

		}

		public AlbumForEditContract GetAlbumForEdit(int id) {

			return
				HandleQuery(session =>
					new AlbumForEditContract(session.Load<Album>(id), GetAllLabels(session), PermissionContext.LanguagePreference));

		}

		public AlbumWithArchivedVersionsContract GetAlbumWithArchivedVersions(int albumId) {

			return HandleQuery(session => new AlbumWithArchivedVersionsContract(session.Load<Album>(albumId), PermissionContext.LanguagePreference));

		}

		public AlbumContract[] GetAlbums() {

			return HandleQuery(session => session.Query<Album>()
				.Where(a => !a.Deleted)
				.ToArray()
				.OrderBy(a => a.DefaultName)
				.Select(a => new AlbumContract(a, PermissionContext.LanguagePreference))
				.ToArray());

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
		public AlbumWithCoverPictureContract GetCoverPicture(int id, Size requestedSize) {

			return HandleQuery(session => 
				new AlbumWithCoverPictureContract(
					session.Load<Album>(id), PermissionContext.LanguagePreference, requestedSize));

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

		public TrackPropertiesContract GetTrackProperties(int songInAlbumId) {

			return HandleQuery(session => {

				var songInAlbum = session.Load<SongInAlbum>(songInAlbumId);

				return new TrackPropertiesContract(songInAlbum.Song, 
					songInAlbum.Album.Artists.Select(a => a.Artist), PermissionContext.LanguagePreference);

			});

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionFlags.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target albums can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Album>(sourceId);
				var target = session.Load<Album>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", source, target), session);

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

				Archive(session, source, "Merged to '" + target.DefaultName + "'");
				Archive(session, target, "Merged from '" + source.DefaultName + "'");

				session.Update(source);
				session.Update(target);

			});

		}

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

		public TagUsageContract[] SaveTags(int albumId, string[] tags) {

			ParamIs.NotNull(() => tags);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				tags = tags.Distinct(new CaseInsensitiveStringComparer()).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				//var userId = PermissionContext.LoggedUser.Id;
				var album = session.Load<Album>(albumId);

				AuditLog("tagging " + album);

				var existingTags = session.Query<Tag>().ToDictionary(t => t.Name, new CaseInsensitiveStringComparer());

				album.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session), new AlbumTagUsageFactory(session, album));

				return album.Tags.Usages.Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public AlbumForEditContract UpdateBasicProperties(AlbumForEditContract properties, PictureDataContract pictureData) {

			ParamIs.NotNull(() => properties);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				var album = session.Load<Album>(properties.Id);

				AuditLog(string.Format("updating properties for {0}", album), session);

				album.DiscType = properties.DiscType;
				album.Description = properties.Description;
				album.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;

				var nameDiff = album.Names.Sync(properties.Names, album);
				SessionHelper.Sync(session, nameDiff);

				album.Names.UpdateSortNames();

				var webLinkDiff = WebLink.Sync(album.WebLinks, properties.WebLinks, album);
				SessionHelper.Sync(session, webLinkDiff);

				if (properties.OriginalRelease != null) {
					album.OriginalRelease = new AlbumRelease(properties.OriginalRelease);
				} else {
					album.OriginalRelease = null;
				}

				if (pictureData != null) {
					album.CoverPicture = new PictureData(pictureData);
				}

				Archive(session, album, new AlbumDiff { IncludeCover = (pictureData != null) });
				session.Update(album);
				return new AlbumForEditContract(album, GetAllLabels(session), PermissionContext.LanguagePreference);

			});

		}

	}

	public class AlbumTagUsageFactory : ITagUsageFactory<AlbumTagUsage> {

		private Album album;
		private ISession session;

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
