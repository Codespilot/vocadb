namespace VocaDb.Model.Domain.Security {

	public interface IUserPermissionContext {

		bool HasPermission(PermissionFlags flag);

		void VerifyPermission(PermissionFlags flag);

	}

}
