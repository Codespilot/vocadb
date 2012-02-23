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
		/// Name of the currently acting agent. Cannot be null or empty.
		/// </summary>
		string Name { get; }

		UserGroupId UserGroupId { get; }

		bool HasPermission(PermissionToken flag);

		void VerifyLogin();

		void VerifyPermission(PermissionToken flag);

	}

}
