using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;
using System.Drawing;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

		private static readonly ILog log = LogManager.GetLogger(typeof(ArtistService));

		private PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(
			ISession session, string query, ArtistType[] artistTypes, int start, int maxResults, bool getTotalCount, NameMatchMode nameMatchMode = NameMatchMode.Auto) {

			if (string.IsNullOrWhiteSpace(query)) {

				bool filterByArtistType = artistTypes.Any();
				Artist art = null;
				//IList<ArtistName> names = null;

				var q = session.QueryOver(() => art)
					//.Left.JoinAlias(a => a.Names, () => names)
					.Where(s => !s.Deleted);

				if (filterByArtistType)
					q = q.WhereRestrictionOn(s => s.ArtistType).IsIn(artistTypes);

				var artists = q
					.OrderBy(s => s.Names.SortNames.Romaji).Asc
					.TransformUsing(new DistinctRootEntityResultTransformer())
					.Skip(start)
					.Take(maxResults)
					.List();

				var contracts = artists.Select(s => new ArtistWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetArtistCount(session, query, artistTypes) : 0);

				return new PartialFindResult<ArtistWithAdditionalNamesContract>(contracts, count);

			} else {

				var directQ = session.Query<Artist>()
					.Where(s => !s.Deleted);

				if (artistTypes.Any())
					directQ = directQ.Where(s => artistTypes.Contains(s.ArtistType));

				if (nameMatchMode == NameMatchMode.Exact || (nameMatchMode == NameMatchMode.Auto && query.Length < 3)) {
					
					directQ = directQ.Where(s =>
						s.Names.SortNames.English == query
							|| s.Names.SortNames.Romaji == query
							|| s.Names.SortNames.Japanese == query);

				} else {

					directQ = directQ.Where(s =>
						s.Names.SortNames.English.Contains(query)
							|| s.Names.SortNames.Romaji.Contains(query)
							|| s.Names.SortNames.Japanese.Contains(query));

				}
					
				var direct = directQ
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Take(maxResults)
					//.FetchMany(s => s.Names)
					.ToArray();

				var additionalNamesQ = session.Query<ArtistName>()
					.Where(m => !m.Artist.Deleted);

				if (nameMatchMode == NameMatchMode.Exact || (nameMatchMode == NameMatchMode.Auto && query.Length < 3)) {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

				} else {

					additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

				}

				if (artistTypes.Any())
					additionalNamesQ = additionalNamesQ.Where(m => artistTypes.Contains(m.Artist.ArtistType));

				var additionalNames = additionalNamesQ
					.Select(m => m.Artist)
					.OrderBy(s => s.Names.SortNames.Romaji)
					.Distinct()
					.Take(maxResults)
					//.FetchMany(s => s.Names)
					.ToArray()
					.Where(a => !direct.Contains(a));

				var contracts = direct.Concat(additionalNames)
					.Skip(start)
					.Take(maxResults)
					.Select(a => new ArtistWithAdditionalNamesContract(a, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (getTotalCount ? GetArtistCount(session, query, artistTypes) : 0);

				return new PartialFindResult<ArtistWithAdditionalNamesContract>(contracts, count);

			}

		}

		private int GetArtistCount(ISession session, string query, ArtistType[] artistTypes) {

			if (string.IsNullOrWhiteSpace(query)) {

				return session.Query<Artist>()
					.Where(s => 
						!s.Deleted
						&& (!artistTypes.Any() || artistTypes.Contains(s.ArtistType)))
					.Count();

			}

			var directQ = session.Query<Artist>()
				.Where(s => !s.Deleted);

			if (artistTypes.Any())
				directQ = directQ.Where(s => artistTypes.Contains(s.ArtistType));

			if (query.Length < 3) {

				directQ = directQ.Where(s =>
					s.Names.SortNames.English == query
						|| s.Names.SortNames.Romaji == query
						|| s.Names.SortNames.Japanese == query);

			} else {

				directQ = directQ.Where(s =>
					s.Names.SortNames.English.Contains(query)
						|| s.Names.SortNames.Romaji.Contains(query)
						|| s.Names.SortNames.Japanese.Contains(query));

			}

			var direct = directQ.ToArray();

			var additionalNamesQ = session.Query<ArtistName>()
				.Where(m => !m.Artist.Deleted);

			if (query.Length < 3) {

				additionalNamesQ = additionalNamesQ.Where(m => m.Value == query);

			} else {

				additionalNamesQ = additionalNamesQ.Where(m => m.Value.Contains(query));

			}

			if (artistTypes.Any())
				additionalNamesQ = additionalNamesQ.Where(m => artistTypes.Contains(m.Artist.ArtistType));

			var additionalNames = additionalNamesQ
				.Select(m => m.Artist)
				.Distinct()
				.ToArray()
				.Where(a => !direct.Contains(a))
				.ToArray();

			return direct.Count() + additionalNames.Count();

		}

		[Obsolete]
		private T[] GetArtists<T>(Func<Artist, T> func) {

			return HandleQuery(session => session
				.Query<Artist>()
				.Where(a => !a.Deleted)
				.ToArray()
				.OrderBy(a => a.DefaultName)
				.Select(func)
				.ToArray());

		}

		public ArtistService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext)
			: base(sessionFactory, permissionContext) {}

		public ArtistForAlbumContract AddAlbum(int artistId, int albumId) {

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {
				
				var artist = session.Load<Artist>(artistId);
				var album = session.Load<Album>(albumId);

				AuditLog(string.Format("adding {0} for {1}", album, artist), session);

				var artistForAlbum = artist.AddAlbum(album);
				session.Save(artistForAlbum);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public void Archive(ISession session, Artist artist, ArtistDiff diff, string notes = "") {

			log.Info("Archiving " + artist);

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Artist artist, string notes = "") {

			Archive(session, artist, new ArtistDiff(), notes);

		}

		public ArtistContract Create(CreateArtistContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Artist needs at least one name", "contract");

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				AuditLog(string.Format("creating a new artist with name '{0}'", contract.Names.First().Value), session);

				var artist = new Artist { ArtistType = contract.ArtistType, Description = contract.Description };

				artist.Names.Init(contract.Names, artist);

				session.Save(artist);

				Archive(session, artist, "Created");
				session.Update(artist);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public ArtistContract Create(string name, IUserPermissionContext permissionContext) {

			ParamIs.NotNullOrEmpty(() => name);
			ParamIs.NotNull(() => permissionContext);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			return HandleTransaction(session => {

				AuditLog(string.Format("creating a new artist with name '{0}'", name), session);

				var artist = new Artist(name);

				session.Save(artist);

				Archive(session, artist, "Created");
				session.Update(artist);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int artistId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			message = message.Trim();

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var author = GetLoggedUser(session);

				AuditLog("creating comment for " + artist + ": '" + message.Substring(0, Math.Min(message.Length, 40)) + "'", session, author);

				var comment = artist.CreateComment(message, author);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		public void Delete(int id) {

			PermissionContext.VerifyPermission(PermissionFlags.DeleteEntries);

			UpdateEntity<Artist>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", a), session);

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();
			                         
			}, skipLog: true);

		}

		public void DeleteComment(int commentId) {

			HandleTransaction(session => {

				var comment = session.Load<ArtistComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author))
					PermissionContext.VerifyPermission(PermissionFlags.ManageUserBlocks);

				comment.Artist.Comments.Remove(comment);
				session.Delete(comment);

			});

		}

		public PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(string query, ArtistType[] artistTypes, int start, int maxResults, bool getTotalCount = false) {

			return HandleQuery(session => FindArtists(session, query, artistTypes, start, maxResults, getTotalCount));

		}

		public ArtistWithAdditionalNamesContract FindByNames(string[] query) {

			return HandleQuery(session => {

				foreach (var q in query.Where(q => !string.IsNullOrWhiteSpace(q))) {

					var result = FindArtists(session, q, new ArtistType[] {}, 0, 1, false, NameMatchMode.Exact);

					if (result.Items.Any())
						return result.Items.First();

				}

				return null;

			});

		}

		public AlbumWithAdditionalNamesContract[] GetAlbums(int artistId) {

			// TODO: sorting could be done in DB

			return HandleQuery(session =>
				session.Load<Artist>(artistId).Albums.Select(a => 
					new AlbumWithAdditionalNamesContract(a.Album, PermissionContext.LanguagePreference)).OrderBy(a => a.Name).ToArray());

		}

		public ArtistContract GetArtist(int id) {

			return HandleQuery(session => new ArtistContract(session.Load<Artist>(id), PermissionContext.LanguagePreference));

		}

		public ArtistDetailsContract GetArtistDetails(int id) {

			return HandleQuery(session => {
			                   	
				var contract = new ArtistDetailsContract(session.Load<Artist>(id), PermissionContext.LanguagePreference);

				contract.LatestAlbums = session.Query<ArtistForAlbum>()
					.Where(s => !s.Album.Deleted && s.Artist.Id == id)
					.Select(s => s.Album)
					.OrderByDescending(s => s.CreateDate)
					.Take(20).ToArray()
					.Select(s => new AlbumWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				contract.LatestSongs = session.Query<ArtistForSong>()
					.Where(s => !s.Song.Deleted && s.Artist.Id == id)
					.Select(s => s.Song)
					.OrderByDescending(s => s.CreateDate)
					.Take(20).ToArray()
					.Select(s => new SongWithAdditionalNamesContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				return contract;

			});

		}

		public ArtistForEditContract GetArtistForEdit(int id) {

			return
				HandleQuery(session =>
					new ArtistForEditContract(session.Load<Artist>(id), PermissionContext.LanguagePreference,
						session.Query<Artist>().Where(a => !a.Deleted 
							&& (a.ArtistType == ArtistType.Circle 
								|| a.ArtistType == ArtistType.Label
								|| a.ArtistType == ArtistType.OtherGroup))));

		}

		public ArtistWithAdditionalNamesContract GetArtistWithAdditionalNames(int id) {

			return HandleQuery(session => new ArtistWithAdditionalNamesContract(session.Load<Artist>(id), PermissionContext.LanguagePreference));

		}

		/// <summary>
		/// Gets the picture for a <see cref="Artist"/>.
		/// </summary>
		/// <param name="id">Artist Id.</param>
		/// <param name="requestedSize">Requested size. If Empty, original size will be returned.</param>
		/// <returns>Data contract for the picture. Can be null if there is no picture.</returns>
		public ArtistWithPictureContract GetArtistPicture(int id, Size requestedSize) {

			return HandleQuery(session => new ArtistWithPictureContract(
				session.Load<Artist>(id), PermissionContext.LanguagePreference, requestedSize));

		}

		public ArtistWithArchivedVersionsContract GetArtistWithArchivedVersions(int artistId) {

			return HandleQuery(session => new ArtistWithArchivedVersionsContract(
				session.Load<Artist>(artistId), PermissionContext.LanguagePreference));

		}

		[Obsolete]
		public ArtistContract[] GetArtists() {

			return GetArtists(a => new ArtistContract(a, PermissionContext.LanguagePreference));

		}

		[Obsolete]
		public ArtistWithAdditionalNamesContract[] GetArtistsWithAdditionalNames() {

			return GetArtists(a => new ArtistWithAdditionalNamesContract(a, PermissionContext.LanguagePreference));

		}

		public ArtistContract[] GetCircles() {

			return HandleQuery(session => session.Query<Artist>()
				.Where(a => !a.Deleted && (a.ArtistType == ArtistType.Circle || a.ArtistType == ArtistType.Label))
				.ToArray()
				.Select(a => new ArtistContract(a, PermissionContext.LanguagePreference))
				.ToArray());

		}

		public CommentContract[] GetComments(int artistId) {

			return HandleQuery(session => {

				return session.Query<ArtistComment>()
					.Where(c => c.Artist.Id == artistId)
					.OrderByDescending(c => c.Created)
					.Select(c => new CommentContract(c)).ToArray();

			});

		}

		public SongWithAdditionalNamesContract[] GetSongs(int artistId) {

			// TODO: sorting could be done in DB

			return HandleQuery(session =>
				session.Load<Artist>(artistId).Songs.Select(a => 
					new SongWithAdditionalNamesContract(a.Song, PermissionContext.LanguagePreference)).OrderBy(s => s.Name).ToArray());

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionFlags.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target artists can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Artist>(sourceId);
				var target = session.Load<Artist>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", source, target), session);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url);
					session.Save(link);
				}

				var groups = source.Groups.Where(g => !target.HasGroup(g.Group)).ToArray();
				foreach (var g in groups) {
					g.MoveToMember(target);
					session.Update(g);
				}

				var members = source.Members.Where(m => !m.Member.HasGroup(target)).ToArray();
				foreach (var m in members) {
					m.MoveToGroup(target);
					session.Update(m);
				}

				var albums = source.Albums.Where(a => !target.HasAlbum(a.Album)).ToArray();
				foreach (var a in albums) {
					a.Move(target);
					session.Update(a);
				}

				var songs = source.Songs.Where(s => !target.HasSong(s.Song)).ToArray();
				foreach (var s in songs) {
					s.Move(target);
					session.Update(s);
				}

				if (target.Description == string.Empty)
					target.Description = source.Description;

				source.Deleted = true;

				//Archive(session, source, "Merged to '" + target.DefaultName + "'");
				Archive(session, target, "Merged from '" + source.DefaultName + "'");

				session.Update(source);
				session.Update(target);

			});

		}

		public void UpdateBasicProperties(ArtistForEditContract properties, PictureDataContract pictureData, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNull(() => properties);
			ParamIs.NotNull(() => permissionContext);

			permissionContext.VerifyPermission(PermissionFlags.ManageDatabase);

			UpdateEntity<Artist>(properties.Id, (session, artist) => {

				AuditLog(string.Format("updating properties for {0}", artist));

				var artistTypeChanged = artist.ArtistType != properties.ArtistType;

				artist.ArtistType = properties.ArtistType;
				artist.Description = properties.Description;
				artist.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;

				if (pictureData != null) {
					artist.Picture = new PictureData(pictureData);
				}

				var nameDiff = artist.Names.Sync(properties.Names, artist);
				SessionHelper.Sync(session, nameDiff);

				var webLinkDiff = WebLink.Sync(artist.WebLinks, properties.WebLinks, artist);
				SessionHelper.Sync(session, webLinkDiff);

				if (artistTypeChanged || nameDiff.Edited.Any()) {

					foreach (var song in artist.Songs) {
						song.Song.UpdateArtistString();
						session.Update(song);
					}

				}

				var groupsDiff = CollectionHelper.Diff(artist.Groups, properties.Groups, (i, i2) => (i.Id == i2.Id));

				foreach (var grp in groupsDiff.Removed) {
					grp.Delete();
					session.Delete(grp);
				}

				foreach (var grp in groupsDiff.Added) {
					var link = artist.AddGroup(session.Load<Artist>(grp.Group.Id));
					session.Save(link);
				}

				Archive(session, artist, new ArtistDiff { IncludePicture = (pictureData != null) }, "Updated properties");
				session.Update(artist);

			});

		}

	}

}
