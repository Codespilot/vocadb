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

	}

}