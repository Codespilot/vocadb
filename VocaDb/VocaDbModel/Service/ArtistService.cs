using System;
using System.Data;
using System.Linq;
using System.Web;
using NLog;
using VocaDb.Model.Domain.Globalization;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;
using System.Drawing;
using VocaDb.Model.Helpers;
using System.Collections.Generic;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Service {

	public class ArtistService : ServiceBase {

// ReSharper disable UnusedMember.Local
		private static readonly Logger log = LogManager.GetCurrentClassLogger();
// ReSharper restore UnusedMember.Local

		public PartialFindResult<Artist> Find(ISession session, ArtistQueryParams queryParams) {

			return new ArtistSearch(LanguagePreference).Find(session, queryParams);

		}

		public ArtistService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public ArtistForAlbumContract AddAlbum(int artistId, int albumId) {

			VerifyManageDatabase();

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var album = session.Load<Album>(albumId);

				var hasAlbum = session.Query<ArtistForAlbum>().Any(a => a.Artist.Id == artistId && a.Album.Id == albumId);

				if (hasAlbum)
					throw new LinkAlreadyExistsException(string.Format("{0} already has {1}", artist, album));

				AuditLog(string.Format("adding {0} for {1}", 
					EntryLinkFactory.CreateEntryLink(album), EntryLinkFactory.CreateEntryLink(artist)), session);

				var artistForAlbum = artist.AddAlbum(album);
				session.Save(artistForAlbum);

				album.UpdateArtistString();
				session.Update(album);

				return new ArtistForAlbumContract(artistForAlbum, PermissionContext.LanguagePreference);

			});

		}

		public void Archive(ISession session, Artist artist, ArtistDiff diff, ArtistArchiveReason reason, string notes = "") {

			SysLog("Archiving " + artist);

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = ArchivedArtistVersion.Create(artist, diff, agentLoginData, reason, notes);
			session.Save(archived);

		}

		public void Archive(ISession session, Artist artist, ArtistArchiveReason reason, string notes = "") {

			Archive(session, artist, new ArtistDiff(), reason, notes);

		}

		public ArtistContract Create(CreateArtistContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Artist needs at least one name", "contract");

			VerifyManageDatabase();

			return HandleTransaction(session => {

				SysLog(string.Format("creating a new artist with name '{0}'", contract.Names.First().Value));

				var artist = new Artist { 
					ArtistType = contract.ArtistType, 
					Description = contract.Description.Trim(), 
					Status = contract.Draft ? EntryStatus.Draft : EntryStatus.Finished 
				};

				artist.Names.Init(contract.Names, artist);

				if (contract.PictureData != null) {
					artist.Picture = new PictureData(contract.PictureData);
				}

				if (contract.WebLink != null) {
					artist.CreateWebLink(contract.WebLink.Description, contract.WebLink.Url, contract.WebLink.Category);
				}

				session.Save(artist);

				Archive(session, artist, ArtistArchiveReason.Created);
				session.Update(artist);

				AuditLog(string.Format("created {0}", EntryLinkFactory.CreateEntryLink(artist)), session);
				AddEntryEditedEntry(session, artist, EntryEditEvent.Created);

				return new ArtistContract(artist, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int artistId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);
				var author = GetLoggedUser(session);

				AuditLog(string.Format("creating comment for {0}: '{1}'", 
					EntryLinkFactory.CreateEntryLink(artist),
					HttpUtility.HtmlEncode(message.Truncate(60))), session, author);

				var comment = artist.CreateComment(message, author);
				session.Save(comment);

				return new CommentContract(comment);

			});

		}

		public bool CreateReport(int artistId, ArtistReportType reportType, string hostname, string notes) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(session => {

				var loggedUserId = PermissionContext.LoggedUserId;
				var existing = session.Query<ArtistReport>()
					.FirstOrDefault(r => r.Artist.Id == artistId
						&& ((loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname));

				if (existing != null)
					return false;

				var artist = session.Load<Artist>(artistId);
				var report = new ArtistReport(artist, reportType, GetLoggedUserOrDefault(session), hostname, notes.Truncate(200));

				var msg = string.Format("reported {0} as {1} ({2})", EntryLinkFactory.CreateEntryLink(artist), reportType, HttpUtility.HtmlEncode(notes));
				AuditLog(msg.Truncate(200), session, new AgentLoginData(GetLoggedUserOrDefault(session), hostname));

				session.Save(report);
				return true;

			}, IsolationLevel.ReadUncommitted);

		}

		public void Delete(int id) {

			UpdateEntity<Artist>(id, (session, a) => {

				AuditLog(string.Format("deleting {0}", EntryLinkFactory.CreateEntryLink(a)), session);

				//ArchiveArtist(session, permissionContext, a);
				a.Delete();
			                         
			}, PermissionToken.DeleteEntries, skipLog: true);

		}

		public void DeleteComment(int commentId) {

			HandleTransaction(session => {

				var comment = session.Load<ArtistComment>(commentId);
				var user = GetLoggedUser(session);

				AuditLog("deleting " + comment, session, user);

				if (!user.Equals(comment.Author))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				comment.Artist.Comments.Remove(comment);
				session.Delete(comment);

			});

		}

		public PartialFindResult<ArtistWithAdditionalNamesContract> FindArtists(ArtistQueryParams queryParams) {

			return FindArtists(a => new ArtistWithAdditionalNamesContract(a, PermissionContext.LanguagePreference), queryParams);

		}

		public PartialFindResult<T> FindArtists<T>(Func<Artist, T> fac, ArtistQueryParams queryParams) {

			return HandleQuery(session => {

				var result = Find(session, queryParams);

				return new PartialFindResult<T>(result.Items.Select(fac).ToArray(),
					result.TotalCount, result.Term, result.FoundExactMatch);

			});

		}

		public EntryRefWithCommonPropertiesContract[] FindDuplicates(string[] anyName, string url) {

			var names = anyName.Select(n => n.Trim()).Where(n => n != string.Empty).ToArray();
			var urlTrimmed = url != null ? url.Trim() : null;

			if (!names.Any() && string.IsNullOrEmpty(url))
				return new EntryRefWithCommonPropertiesContract[] { };

			return HandleQuery(session => {

				var nameMatches = (names.Any() ? session.Query<ArtistName>()
					.Where(n => names.Contains(n.Value))
					.Select(n => n.Artist)
					.Where(n => !n.Deleted)
					.Distinct()
					.Take(10)
					.ToArray() : new Artist[] {});

				var linkMatches = !string.IsNullOrEmpty(urlTrimmed) ?
					session.Query<ArtistWebLink>()
					.Where(w => w.Url == urlTrimmed)
					.Select(w => w.Artist)
					.Distinct()
					.Take(10)
					.ToArray() : new Artist[] {};

				return nameMatches.Union(linkMatches)
					.Select(n => new EntryRefWithCommonPropertiesContract(n, PermissionContext.LanguagePreference))
					.ToArray();

			});

		}

		public string[] FindNames(string query, int maxResults) {

			if (string.IsNullOrWhiteSpace(query))
				return new string[] {};

			query = query.Trim();

			return HandleQuery(session => {

				var names = session.Query<ArtistName>()
					.Where(a => !a.Artist.Deleted)
					.FilterByArtistName(query)
					.Select(n => n.Value)
					.OrderBy(n => n)
					.Distinct()
					.Take(maxResults)
					.ToArray();

				return NameHelper.MoveExactNamesToTop(names, query);

			});

		}

		public EntryForPictureDisplayContract GetArchivedArtistPicture(int archivedVersionId) {

			return HandleQuery(session =>
				EntryForPictureDisplayContract.Create(
				session.Load<ArchivedArtistVersion>(archivedVersionId), LanguagePreference));

		}

		public ArtistContract GetArtist(int id) {

			return HandleQuery(session => new ArtistContract(session.Load<Artist>(id), LanguagePreference));

		}

		public ArtistDetailsContract GetArtistDetails(int id) {

			return HandleQuery(session => {
			                   	
				var contract = new ArtistDetailsContract(session.Load<Artist>(id), PermissionContext.LanguagePreference);

				if (PermissionContext.LoggedUser != null) {
					contract.IsAdded = session.Query<ArtistForUser>()
						.Any(s => s.Artist.Id == id && s.User.Id == PermissionContext.LoggedUser.Id);
				}

				contract.CommentCount = session.Query<ArtistComment>().Where(c => c.Artist.Id == id).Count();

				contract.LatestAlbums = session.Query<ArtistForAlbum>()
					.Where(s => !s.Album.Deleted && s.Artist.Id == id && !s.IsSupport)
					.Select(s => s.Album)
					.OrderByDescending(s => s.CreateDate)
					.Take(6).ToArray()
					.Select(s => new AlbumContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				contract.LatestSongs = session.Query<ArtistForSong>()
					.Where(s => !s.Song.Deleted && s.Artist.Id == id && !s.IsSupport)
					.Select(s => s.Song)
					.OrderByDescending(s => s.CreateDate)
					.Take(8).ToArray()
					.Select(s => new SongContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				contract.TopAlbums = session.Query<ArtistForAlbum>()
					.Where(s => !s.Album.Deleted && s.Artist.Id == id && !s.IsSupport && s.Album.RatingAverageInt > 0)
					.Select(s => s.Album)
					.OrderByDescending(s => s.RatingAverageInt)
					.ThenByDescending(s => s.RatingCount)
					.Take(6).ToArray()
					.Where(a => contract.LatestAlbums.All(a2 => a.Id != a2.Id))
					.Select(s => new AlbumContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				contract.TopSongs = session.Query<ArtistForSong>()
					.Where(s => !s.Song.Deleted && s.Artist.Id == id && !s.IsSupport && s.Song.RatingScore > 0)
					.Select(s => s.Song)
					.OrderByDescending(s => s.RatingScore)
					.Take(8).ToArray()
					.Where(a => contract.LatestSongs.All(a2 => a.Id != a2.Id))
					.Select(s => new SongContract(s, PermissionContext.LanguagePreference))
					.ToArray();

				contract.LatestComments = session.Query<ArtistComment>()
					.Where(c => c.Artist.Id == id).OrderByDescending(c => c.Created).Take(3)
					.ToArray()
					.Select(c => new CommentContract(c)).ToArray();

				return contract;

			});

		}

		public ArtistForEditContract GetArtistForEdit(int id) {

			return
				HandleQuery(session =>
					new ArtistForEditContract(session.Load<Artist>(id), PermissionContext.LanguagePreference));

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
		public EntryForPictureDisplayContract GetArtistPicture(int id, Size requestedSize) {

			return HandleQuery(session => 
				EntryForPictureDisplayContract.Create(session.Load<Artist>(id), PermissionContext.LanguagePreference, requestedSize));

		}

		public ArtistWithArchivedVersionsContract GetArtistWithArchivedVersions(int artistId) {

			return HandleQuery(session => new ArtistWithArchivedVersionsContract(
				session.Load<Artist>(artistId), PermissionContext.LanguagePreference));

		}

		public ArtistForApiContract[] GetArtistsWithYoutubeChannels(ContentLanguagePreference languagePreference) {

			return HandleQuery(session => session.Query<ArtistWebLink>()
				.Where(l => !l.Artist.Deleted && (l.Artist.ArtistType == ArtistType.Producer || l.Artist.ArtistType == ArtistType.Circle || l.Artist.ArtistType == ArtistType.Animator) && l.Url.Contains("youtube.com/user/"))
				.Select(l => l.Artist)
				.Distinct()
				.ToArray()
				.Select(a => new ArtistForApiContract(a, languagePreference, ArtistEditableFields.WebLinks))
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

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int artistId) {

			return HandleQuery(session => {

				var artist = session.Load<Artist>(artistId);
				return new EntryWithTagUsagesContract(artist, artist.Tags.Usages);

			});

		}

		public TagSelectionContract[] GetTagSelections(int artistId, int userId) {

			return HandleQuery(session => {

				var tagsInUse = session.Query<ArtistTagUsage>().Where(a => a.Artist.Id == artistId).ToArray();
				var tagVotes = session.Query<ArtistTagVote>().Where(a => a.User.Id == userId && a.Usage.Artist.Id == artistId).ToArray();

				var tagSelections = tagsInUse.Select(t =>
					new TagSelectionContract(t.Tag.Name, t.Votes.Any(v => tagVotes.Any(v.Equals))));

				return tagSelections.ToArray();

			});

		}

		public ArchivedArtistVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedArtistVersionDetailsContract(session.Load<ArchivedArtistVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedArtistVersion>(comparedVersionId) : null, PermissionContext.LanguagePreference));

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target artists can't be the same", "targetId");

			HandleTransaction(session => {

				var source = session.Load<Artist>(sourceId);
				var target = session.Load<Artist>(targetId);

				AuditLog(string.Format("Merging {0} to {1}", 
					EntryLinkFactory.CreateEntryLink(source), EntryLinkFactory.CreateEntryLink(target)), session);

				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					session.Save(name);
				}

				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
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

				var ownerUsers = source.OwnerUsers.Where(s => !target.HasOwnerUser(s.User)).ToArray();
				foreach (var u in ownerUsers) {
					u.Move(target);
					session.Update(u);
				}

				var pictures = source.Pictures.ToArray();
				foreach (var p in pictures) {
					p.Move(target);
					session.Update(p);
				}

				var users = source.Users.ToArray();
				foreach (var u in users) {
					u.Move(target);
					session.Update(u);
				}

				if (target.Description == string.Empty)
					target.Description = source.Description;

				source.Deleted = true;

				Archive(session, target, ArtistArchiveReason.Merged, string.Format("Merged from '{0}'", source.DefaultName));

				session.Update(source);
				session.Update(target);

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return RemoveTagUsage<ArtistTagUsage>(tagUsageId);

		}

		public void Restore(int artistId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(session => {

				var artist = session.Load<Artist>(artistId);

				artist.Deleted = false;

				AuditLog("restored " + EntryLinkFactory.CreateEntryLink(artist), session);

			});

		}

		/// <summary>
		/// Reverts an album to an earlier archived version.
		/// </summary>
		/// <param name="archivedArtistVersionId">Id of the archived version to be restored.</param>
		/// <returns>Result of the revert operation, with possible warnings if any. Cannot be null.</returns>
		/// <remarks>Requires the RestoreRevisions permission.</remarks>
		public EntryRevertedContract RevertToVersion(int archivedArtistVersionId) {

			PermissionContext.VerifyPermission(PermissionToken.RestoreRevisions);

			return HandleTransaction(session => {

				var archivedVersion = session.Load<ArchivedArtistVersion>(archivedArtistVersionId);
				var artist = archivedVersion.Artist;

				SysLog("reverting " + artist + " to version " + archivedVersion.Version);

				var fullProperties = ArchivedArtistContract.GetAllProperties(archivedVersion);
				var warnings = new List<string>();

				artist.ArtistType = fullProperties.ArtistType;
				artist.Description = fullProperties.Description;
				artist.TranslatedName.DefaultLanguage = fullProperties.TranslatedName.DefaultLanguage;

				// Picture
				var versionWithPic = archivedVersion.GetLatestVersionWithField(ArtistEditableFields.Picture);

				if (versionWithPic != null)
					artist.Picture = versionWithPic.Picture;

				/*
				// Albums
				SessionHelper.RestoreObjectRefs<ArtistForAlbum, Album>(
					session, warnings, artist.AllAlbums, fullProperties.Albums, (a1, a2) => (a1.Album.Id == a2.Id),
					album => (!artist.HasAlbum(album) ? artist.AddAlbum(album) : null),
					albumForArtist => albumForArtist.Delete());
				 */

				// Groups
				SessionHelper.RestoreObjectRefs<GroupForArtist, Artist>(
					session, warnings, artist.AllGroups, fullProperties.Groups, (a1, a2) => (a1.Group.Id == a2.Id),
					grp => (!artist.HasGroup(grp) ? artist.AddGroup(grp) : null),
					groupForArtist => groupForArtist.Delete());

				/*
				// Members
				SessionHelper.RestoreObjectRefs<GroupForArtist, Artist>(
					session, warnings, artist.AllMembers, fullProperties.Members, (a1, a2) => (a1.Member.Id == a2.Id),
					member => (!artist.HasMember(member) ? artist.AddMember(member) : null),
					groupForArtist => groupForArtist.Delete());
				 */

				// Names
				if (fullProperties.Names != null) {
					var nameDiff = artist.Names.SyncByContent(fullProperties.Names, artist);
					SessionHelper.Sync(session, nameDiff);
				}

				// Weblinks
				if (fullProperties.WebLinks != null) {
					var webLinkDiff = WebLink.SyncByValue(artist.WebLinks, fullProperties.WebLinks, artist);
					SessionHelper.Sync(session, webLinkDiff);
				}

				Archive(session, artist, ArtistArchiveReason.Reverted);
				AuditLog(string.Format("reverted {0} to revision {1}", EntryLinkFactory.CreateEntryLink(artist), archivedVersion.Version), session);

				return new EntryRevertedContract(artist, warnings);

			});

		}

		public TagUsageContract[] SaveTags(int artistId, string[] tags) {

			ParamIs.NotNull(() => tags);

			VerifyManageDatabase();

			return HandleTransaction(session => {

				tags = tags.Distinct(new CaseInsensitiveStringComparer()).ToArray();

				var user = session.Load<User>(PermissionContext.LoggedUser.Id);
				var artist = session.Load<Artist>(artistId);

				AuditLog(string.Format("tagging {0} with {1}", 
					EntryLinkFactory.CreateEntryLink(artist), string.Join(", ", tags)), session, user);

				var existingTags = TagHelpers.GetTags(session, tags);

				artist.Tags.SyncVotes(user, tags, existingTags, new TagFactory(session, new AgentLoginData(user)), new ArtistTagUsageFactory(session, artist));

				return artist.Tags.Usages.OrderByDescending(u => u.Count).Select(t => new TagUsageContract(t)).ToArray();

			});

		}

		public void UpdateBasicProperties(ArtistForEditContract properties, PictureDataContract pictureData, IUserPermissionContext permissionContext) {
			
			ParamIs.NotNull(() => properties);
			ParamIs.NotNull(() => permissionContext);

			HandleTransaction(session => {

				var artist = session.Load<Artist>(properties.Id);

				VerifyEntryEdit(artist);

				var diff = new ArtistDiff(DoSnapshot(artist.GetLatestVersion(), GetLoggedUser(session)));

				SysLog(string.Format("updating properties for {0}", artist));

				if (artist.ArtistType != properties.ArtistType) {
					artist.ArtistType = properties.ArtistType;
					diff.ArtistType = true;
				}

				if (artist.Description != properties.Description) {
					artist.Description = properties.Description;
					diff.Description = true;
				}

				if (artist.TranslatedName.DefaultLanguage != properties.TranslatedName.DefaultLanguage) {
					artist.TranslatedName.DefaultLanguage = properties.TranslatedName.DefaultLanguage;
					diff.OriginalName = true;
				}

				if (pictureData != null) {
					artist.Picture = new PictureData(pictureData);
					diff.Picture = true;
				}

				if (artist.Status != properties.Status) {
					artist.Status = properties.Status;
					diff.Status = true;
				}

				var nameDiff = artist.Names.Sync(properties.Names.AllNames, artist);
				SessionHelper.Sync(session, nameDiff);

				if (nameDiff.Changed)
					diff.Names = true;

				var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(artist.WebLinks, validWebLinks, artist);
				SessionHelper.Sync(session, webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks = true;

				if (diff.ArtistType || diff.Names) {

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

				if (groupsDiff.Changed)
					diff.Groups = true;

				var picsDiff = artist.Pictures.SyncPictures(properties.Pictures, GetLoggedUser(session), artist.CreatePicture);
				SessionHelper.Sync(session, picsDiff);
				ImageHelper.GenerateThumbsAndMoveImages(picsDiff.Added);

				if (picsDiff.Changed)
					diff.Pictures = true;

				/*
				var albumGetter = new Func<AlbumForArtistEditContract, Album>(contract => {

					Album album;

					if (contract.AlbumId != 0) {

						album = session.Load<Album>(contract.AlbumId);

					} else {

						AuditLog(string.Format("creating a new album '{0}' to {1}", contract.AlbumName, artist));

						album = new Album(contract.AlbumName);
						session.Save(album);

						Services.Albums.Archive(session, album, AlbumArchiveReason.Created,
							string.Format("Created for artist '{0}'", artist.DefaultName));

						AuditLog(string.Format("created {0} for {1}", 
							EntryLinkFactory.CreateEntryLink(album), EntryLinkFactory.CreateEntryLink(artist)), session);
						AddEntryEditedEntry(session, album, EntryEditEvent.Created);

					}

					return album;

				});

				if (properties.AlbumLinks != null 
					&& !properties.TooManyAlbums
					&& (properties.AlbumLinks.Any() || artist.Albums.Count() < ArtistForEditContract.MaxAlbums / 2)  
					&& artist.Albums.Count() <= ArtistForEditContract.MaxAlbums) {

					var albumDiff = artist.SyncAlbums(properties.AlbumLinks, albumGetter);

					SessionHelper.Sync(session, albumDiff);

					if (albumDiff.Changed) {

						diff.Albums = true;

						var add = string.Join(", ", albumDiff.Added.Select(i => i.Album.ToString()));
						var rem = string.Join(", ", albumDiff.Removed.Select(i => i.Album.ToString()));

						var str = string.Format("edited albums (added: {0}, removed: {1})", add, rem)
							.Truncate(300);

						AuditLog(str, session);

					}

				}*/

				var logStr = string.Format("updated properties for {0} ({1})", EntryLinkFactory.CreateEntryLink(artist), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				AuditLog(logStr, session);
				AddEntryEditedEntry(session, artist, EntryEditEvent.Updated);

				Archive(session, artist, diff, ArtistArchiveReason.PropertiesUpdated, properties.UpdateNotes);
				session.Update(artist);

				return true;

			});

		}

	}

	public class ArtistTagUsageFactory : ITagUsageFactory<ArtistTagUsage> {

		private readonly Artist artist;
		private readonly ISession session;

		public ArtistTagUsageFactory(ISession session, Artist artist) {
			this.session = session;
			this.artist = artist;
		}

		public ArtistTagUsage CreateTagUsage(Tag tag) {

			var usage = new ArtistTagUsage(artist, tag);
			session.Save(usage);

			return usage;

		}

	}

	public enum ArtistSortRule {

		None,

		Name,

		AdditionDate

	}

}
