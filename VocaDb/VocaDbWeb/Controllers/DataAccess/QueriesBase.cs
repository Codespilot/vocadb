using VocaDb.Model;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Controllers.DataAccess {

	public abstract class QueriesBase<TRepo> where TRepo : class {

		protected readonly IUserPermissionContext permissionContext;
		protected readonly TRepo repository;

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		protected QueriesBase(TRepo repository, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);

			this.repository = repository;
			this.permissionContext = permissionContext;

		}

	}

}