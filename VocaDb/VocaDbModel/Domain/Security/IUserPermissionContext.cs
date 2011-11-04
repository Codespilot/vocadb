using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;

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

		bool HasPermission(PermissionFlags flag);

		void VerifyPermission(PermissionFlags flag);

	}

}
