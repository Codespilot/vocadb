using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	/// <summary>
	/// Database queries related to <see cref="Song"/>.
	/// </summary>
	public class SongQueries : QueriesBase<ISongRepository> {

		private readonly IEntryLinkFactory entryLinkFactory;

		public SongQueries(ISongRepository repository, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory)
			: base(repository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;

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