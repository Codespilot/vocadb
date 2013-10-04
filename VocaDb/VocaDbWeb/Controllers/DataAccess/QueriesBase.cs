using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Controllers.DataAccess {

	public abstract class QueriesBase<TRepo> where TRepo : class {

		protected readonly IUserPermissionContext permissionContext;
		protected readonly TRepo repository;

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
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