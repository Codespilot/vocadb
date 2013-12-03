using System;
using System.Linq;
using System.Web;
using VocaDb.Model.Domain;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Song"/>.
	/// </summary>
	public class SongQueries : QueriesBase<ISongRepository, Song> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IPVParser pvParser;

		private VideoUrlParseResult ParsePV(IRepositoryContext<PVForSong> ctx, string url) {

			if (string.IsNullOrEmpty(url))
				return null;

			var pvResult = pvParser.ParseByUrl(url, true);

			if (!pvResult.IsOk)
				throw pvResult.Exception;

			var existing = ctx.Query().FirstOrDefault(
				s => s.Service == pvResult.Service && s.PVId == pvResult.Id && !s.Song.Deleted);

			if (existing != null) {
				throw new VideoParseException(string.Format("Song '{0}' already contains this PV",
					existing.Song.TranslatedName[PermissionContext.LanguagePreference]));
			}

			return pvResult;

		}

		public SongQueries(ISongRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IPVParser pvParser)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.pvParser = pvParser;

		}

		public void Archive(IRepositoryContext<Song> ctx, Song song, SongDiff diff, SongArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedSongVersion.Create(song, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedSongVersion>().Save(archived);

		}

		public void Archive(IRepositoryContext<Song> ctx, Song song, SongArchiveReason reason, string notes = "") {

			Archive(ctx, song, new SongDiff(), reason, notes);

		}

		public SongContract Create(CreateSongContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Song needs at least one name", "contract");

			VerifyManageDatabase();

			return repository.HandleTransaction(ctx => {

				var pvResult = ParsePV(ctx.OfType<PVForSong>(), contract.PVUrl);
				var reprintPvResult = ParsePV(ctx.OfType<PVForSong>(), contract.ReprintPVUrl);

				ctx.AuditLogger.SysLog(string.Format("creating a new song with name '{0}'", contract.Names.First().Value));

				var song = new Song {
					SongType = contract.SongType,
					Status = contract.Draft ? EntryStatus.Draft : EntryStatus.Finished
				};

				song.Names.Init(contract.Names, song);

				ctx.Save(song);

				foreach (var artistContract in contract.Artists) {
					var artist = ctx.OfType<Artist>().Load(artistContract.Id);
					if (!song.HasArtist(artist))
						ctx.OfType<ArtistForSong>().Save(song.AddArtist(artist));
				}

				if (pvResult != null) {
					ctx.OfType<PVForSong>().Save(song.CreatePV(new PVContract(pvResult, PVType.Original)));
				}

				if (reprintPvResult != null) {
					ctx.OfType<PVForSong>().Save(song.CreatePV(new PVContract(reprintPvResult, PVType.Reprint)));
				}

				song.UpdateArtistString();
				Archive(ctx, song, SongArchiveReason.Created);
				ctx.Update(song);

				ctx.AuditLogger.AuditLog(string.Format("created song {0} ({1})", entryLinkFactory.CreateEntryLink(song), song.SongType));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Created);

				new FollowedArtistNotifier().SendNotifications(ctx.OfType<UserMessage>(), song, song.ArtistList, PermissionContext.LoggedUser, entryLinkFactory);

				return new SongContract(song, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int songId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var song = ctx.Load(songId);
				var agent = ctx.OfType<User>().CreateAgentLoginData(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					entryLinkFactory.CreateEntryLink(song),
					HttpUtility.HtmlEncode(message.Truncate(60))), agent.User);

				var comment = song.CreateComment(message, agent);
				ctx.OfType<SongComment>().Save(comment);

				new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx.OfType<User>());

				return new CommentContract(comment);

			});

		}

		public void Merge(int sourceId, int targetId) {

			PermissionContext.VerifyPermission(PermissionToken.MergeEntries);

			if (sourceId == targetId)
				throw new ArgumentException("Source and target songs can't be the same", "targetId");

			repository.HandleTransaction(ctx => {

				var source = ctx.Load(sourceId);
				var target = ctx.Load(targetId);

				ctx.AuditLogger.AuditLog(string.Format("Merging {0} to {1}",
					entryLinkFactory.CreateEntryLink(source), entryLinkFactory.CreateEntryLink(target)));

				// Names
				foreach (var n in source.Names.Names.Where(n => !target.HasName(n))) {
					var name = target.CreateName(n.Value, n.Language);
					ctx.Save(name);
				}

				// Weblinks
				foreach (var w in source.WebLinks.Where(w => !target.HasWebLink(w.Url))) {
					var link = target.CreateWebLink(w.Description, w.Url, w.Category);
					ctx.Save(link);
				}

				// PVs
				var pvs = source.PVs.Where(a => !target.HasPV(a.Service, a.PVId));
				foreach (var p in pvs) {
					var pv = target.CreatePV(new PVContract(p));
					ctx.Save(pv);
				}

				// Artist links
				var artists = source.Artists.Where(a => !target.HasArtistLink(a)).ToArray();
				foreach (var a in artists) {
					a.Move(target);
					ctx.Update(a);
				}

				// Album links
				var albums = source.Albums.Where(s => !target.IsOnAlbum(s.Album)).ToArray();
				foreach (var s in albums) {
					s.Move(target);
					ctx.Update(s);
				}

				// Favorites
				var userFavorites = source.UserFavorites.Where(a => !target.IsFavoritedBy(a.User)).ToArray();
				foreach (var u in userFavorites) {
					u.Move(target);
					ctx.Update(u);
				}

				// Custom lists
				var songLists = source.ListLinks.ToArray();
				foreach (var s in songLists) {
					s.ChangeSong(target);
					ctx.Update(s);
				}

				// Other properties
				if (target.OriginalVersion == null)
					target.OriginalVersion = source.OriginalVersion;

				var alternateVersions = source.AlternateVersions.ToArray();
				foreach (var alternate in alternateVersions) {
					alternate.OriginalVersion = target;
					ctx.Update(alternate);
				}

				if (target.LengthSeconds == 0) {
					target.LengthSeconds = source.LengthSeconds;
				}

				// Create merge record
				var mergeEntry = new SongMergeRecord(source, target);
				ctx.Save(mergeEntry);

				source.Deleted = true;

				target.UpdateArtistString();
				target.UpdateNicoId();

				Archive(ctx, target, SongArchiveReason.Merged, string.Format("Merged from {0}", source));

				ctx.Update(source);
				ctx.Update(target);

			});

		}

		public SongForEditContract UpdateBasicProperties(SongForEditContract properties) {

			ParamIs.NotNull(() => properties);

			VerifyManageDatabase();

			return repository.HandleTransaction(ctx => {

				var song = ctx.Load(properties.Song.Id);

				VerifyEntryEdit(song);

				var diff = new SongDiff(DoSnapshot(song.GetLatestVersion(), ctx.OfType<User>().GetLoggedUser(PermissionContext)));

				ctx.AuditLogger.SysLog(string.Format("updating properties for {0}", song));

				if (song.Notes != properties.Notes) {
					diff.Notes = true;
					song.Notes = properties.Notes;
				}

				var newOriginalVersion = (properties.OriginalVersion != null && properties.OriginalVersion.Id != 0 ? ctx.Load(properties.OriginalVersion.Id) : null);

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
				ctx.OfType<SongName>().Sync(nameDiff);

				if (nameDiff.Changed)
					diff.Names = true;

				var validWebLinks = properties.WebLinks.Where(w => !string.IsNullOrEmpty(w.Url));
				var webLinkDiff = WebLink.Sync(song.WebLinks, validWebLinks, song);
				ctx.OfType<SongWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks = true;

				if (song.Status != properties.Song.Status) {
					song.Status = properties.Song.Status;
					diff.Status = true;
				}

				var artistGetter = new Func<ArtistForSongContract, Artist>(artistForSong => 
					ctx.OfType<Artist>().Load(artistForSong.Artist.Id));

				var artistsDiff = song.SyncArtists(properties.Artists, artistGetter);
				ctx.OfType<ArtistForSong>().Sync(artistsDiff);

				if (artistsDiff.Changed)
					diff.Artists = true;

				var pvDiff = song.SyncPVs(properties.PVs);
				ctx.OfType<PVForSong>().Sync(pvDiff);

				if (pvDiff.Changed)
					diff.PVs = true;

				var lyricsDiff = song.SyncLyrics(properties.Lyrics);
				ctx.OfType<LyricsForSong>().Sync(lyricsDiff);

				if (lyricsDiff.Changed)
					diff.Lyrics = true;

				var logStr = string.Format("updated properties for song {0} ({1})", entryLinkFactory.CreateEntryLink(song), diff.ChangedFieldsString)
					+ (properties.UpdateNotes != string.Empty ? " " + properties.UpdateNotes : string.Empty)
					.Truncate(400);

				ctx.AuditLogger.AuditLog(logStr);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), song, EntryEditEvent.Updated);

				Archive(ctx, song, diff, SongArchiveReason.PropertiesUpdated, properties.UpdateNotes);

				ctx.Update(song);

				var newSongCutoff = TimeSpan.FromMinutes(30);
				if (artistsDiff.Added.Any() && song.CreateDate >= DateTime.Now - newSongCutoff) {

					var addedArtists = artistsDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

					if (addedArtists.Any()) {
						new FollowedArtistNotifier().SendNotifications(ctx.OfType<UserMessage>(), song, addedArtists, PermissionContext.LoggedUser, entryLinkFactory);											
					}

				}

				return new SongForEditContract(song, PermissionContext.LanguagePreference);

			});

		}

	}

}