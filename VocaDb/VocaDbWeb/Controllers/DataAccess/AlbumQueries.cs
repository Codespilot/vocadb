using System.Linq;
using System.Web;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
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