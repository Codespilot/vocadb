using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	public interface IUserPermissionContext {

		ContentLanguagePreference LanguagePreference { get; }

		bool IsLoggedIn { get; }

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		UserContract LoggedUser { get; }

		/// <summary>
		/// Id of the logged in user. If not logged in, 0 will be returned.
		/// </summary>
		int LoggedUserId { get; }

		/// <summary>
		/// Name of the currently acting agent. Cannot be null or empty.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Group of the currently logged in user. Cannot be null. If no user is logged in, this will be <see cref="UserGroup.Nothing"/>
		/// </summary>
		//UserGroup UserGroup { get; }

		UserGroupId UserGroupId { get; }

		bool HasPermission(PermissionToken flag);

		void VerifyLogin();

		void VerifyPermission(PermissionToken flag);

	}

}
