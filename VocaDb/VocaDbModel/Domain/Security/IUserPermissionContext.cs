using VocaDb.Model.DataContracts.Security;

namespace VocaDb.Model.Domain.Security {

	public interface IUserPermissionContext {

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		UserContract LoggedUser { get; }

		/// <summary>
		/// Name of the currently acting agent. Cannot be null or empty.
		/// </summary>
		string Name { get; }

		bool HasPermission(PermissionFlags flag);

		void VerifyPermission(PermissionFlags flag);

	}

}
