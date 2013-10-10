using System.Web;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Artist"/>.
	/// </summary>
	public class ArtistQueries : QueriesBase<IArtistRepository, Artist> {

		private readonly IEntryLinkFactory entryLinkFactory;

		public ArtistQueries(IArtistRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;

		}

		public CommentContract CreateComment(int artistId, string message) {

			ParamIs.NotNullOrEmpty(() => message);

			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			message = message.Trim();

			return repository.HandleTransaction(ctx => {

				var artist = ctx.Load(artistId);
				var author = ctx.OfType<User>().GetLoggedUser(PermissionContext);

				ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'",
					entryLinkFactory.CreateEntryLink(artist),
					HttpUtility.HtmlEncode(message.Truncate(60))), author);

				var comment = artist.CreateComment(message, author);
				ctx.OfType<ArtistComment>().Save(comment);

				new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx.OfType<User>());

				return new CommentContract(comment);

			});

		}

	}
}