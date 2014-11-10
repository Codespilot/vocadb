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
		UserWithPermissionsContract LoggedUser { get; }

		/// <summary>
		/// Id of the logged in user. If not logged in, 0 (InvalidId) will be returned.
		/// </summary>
		int LoggedUserId { get; }

		/// <summary>
		/// Name of the currently acting agent. Null if not logged in.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Group of the currently logged in user. Cannot be null. If no user is logged in, this will be <see cref="UserGroup.Nothing"/>
		/// </summary>
		//UserGroup UserGroup { get; }

		UserGroupId UserGroupId { get; }

		bool HasPermission(PermissionToken flag);

		/// <summary>
		/// Overrides user's language preference for this request.
		/// </summary>
		/// <param name="languagePreference">Language preference.</param>
		void OverrideLanguage(ContentLanguagePreference languagePreference);

		void VerifyLogin();

		void VerifyPermission(PermissionToken flag);

	}

}
