using System;
using System.Linq;
using System.Web;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Album"/>.
	/// </summary>
	public class AlbumQueries : QueriesBase<IAlbumRepository> {

		private readonly IEntryLinkFactory entryLinkFactory;

		public AlbumQueries(IAlbumRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;

		}

		public void Archive(IRepositoryContext<Album> ctx, Album album, AlbumDiff diff, AlbumArchiveReason reason, string notes = "") {

			var agentLoginData = ctx.CreateAgentLoginData(PermissionContext);
			var archived = ArchivedAlbumVersion.Create(album, diff, agentLoginData, reason, notes);
			ctx.OfType<ArchivedAlbumVersion>().Save(archived);

		}

		public void Archive(IRepositoryContext<Album> ctx, Album album, AlbumArchiveReason reason, string notes = "") {

			Archive(ctx, album, new AlbumDiff(), reason, notes);

		}
		public AlbumContract Create(CreateAlbumContract contract) {

			ParamIs.NotNull(() => contract);

			if (contract.Names == null || !contract.Names.Any())
				throw new ArgumentException("Album needs at least one name", "contract");

			VerifyManageDatabase();

			return repository.HandleTransaction(ctx => {

				ctx.AuditLogger.SysLog(string.Format("creating a new album with name '{0}'", contract.Names.First().Value));

				var album = new Album { DiscType = contract.DiscType };

				album.Names.Init(contract.Names, album);

				ctx.Save(album);

				foreach (var artistContract in contract.Artists) {
					var artist = ctx.OfType<Artist>().Load(artistContract.Id);
					if (!album.HasArtist(artist))
						ctx.OfType<ArtistForAlbum>().Save(ctx.OfType<Artist>().Load(artist.Id).AddAlbum(album));
				}

				album.UpdateArtistString();
				Archive(ctx, album, AlbumArchiveReason.Created);
				ctx.Update(album);

				ctx.AuditLogger.AuditLog(string.Format("created album {0} ({1})", entryLinkFactory.CreateEntryLink(album), album.DiscType));
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), album, EntryEditEvent.Created);

				return new AlbumContract(album, PermissionContext.LanguagePreference);

			});

		}

		public CommentContract CreateComment(int albumId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var album = ctx.Load(albumId);
				var agent = ctx.CreateAgentLoginData(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					entryLinkFactory.CreateEntryLink(album),
					HttpUtility.HtmlEncode(message)), agent.User);

				var comment = album.CreateComment(message, agent);
				ctx.OfType<AlbumComment>().Save(comment);

				new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx.OfType<User>());

				return new CommentContract(comment);

			});

		}

		public AlbumContract[] GetRelatedAlbums(int albumId) {

			return repository.HandleQuery(ctx => {

				var album = ctx.Load(albumId);
				var q = new RelatedAlbumsQuery(ctx);
				var albums = q.GetRelatedAlbums(album);

				return albums.ArtistMatches
					.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
					.OrderBy(a => a.Name)
					.Concat(albums.TagMatches
						.Select(a => new AlbumContract(a, permissionContext.LanguagePreference))
						.OrderBy(a => a.Name))
					.ToArray();

			});

		}

	}
}