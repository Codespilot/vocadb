using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public abstract class QueriesBase<TRepo> where TRepo : class {

		protected readonly IUserPermissionContext permissionContext;
		protected readonly TRepo repository;

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		protected void AddActivityfeedEntry(IRepositoryContext<ActivityEntry> ctx, ActivityEntry entry) {

			var latestEntries = ctx.Query()
				.OrderByDescending(a => a.CreateDate)
				.Take(10)	// time cutoff would be better instead of an arbitrary number of activity entries
				.ToArray();

			if (latestEntries.Any(e => e.IsDuplicate(entry)))
				return;

			ctx.Save(entry);

		}

		protected void AddEntryEditedEntry(IRepositoryContext<ActivityEntry> ctx, Album entry, EntryEditEvent editEvent) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new AlbumActivityEntry(entry, editEvent, user);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void VerifyManageDatabase() {

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

		}

		protected void VerifyResourceAccess(params IUser[] owners) {

			VerifyResourceAccess(owners.Select(o => o.Id));

		}

		private void VerifyResourceAccess(IEnumerable<int> ownerIds) {

			PermissionContext.VerifyLogin();

			if (!ownerIds.Contains(PermissionContext.LoggedUser.Id))
				throw new NotAllowedException("You do not have access to this resource.");

		}

		protected QueriesBase(TRepo repository, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);

			this.repository = repository;
			this.permissionContext = permissionContext;

		}

	}

}