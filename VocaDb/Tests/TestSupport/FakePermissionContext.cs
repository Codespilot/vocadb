﻿using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakePermissionContext : IUserPermissionContext {

		public FakePermissionContext() {}

		public FakePermissionContext(UserWithPermissionsContract loggedUser) {
			LoggedUser = loggedUser;
		}

		public FakePermissionContext(User user)
			: this(new UserWithPermissionsContract(user, ContentLanguagePreference.Default)) {}

		public bool HasPermission(PermissionToken token) {

			if (token == PermissionToken.Nothing)
				return true;

			if (!IsLoggedIn || !LoggedUser.Active)
				return false;

			return (LoggedUser.EffectivePermissions.Contains(token));

		}

		public bool IsLoggedIn {
			get { return LoggedUser != null; }
		}

		public ContentLanguagePreference LanguagePreference { get; private set; }

		public UserWithPermissionsContract LoggedUser { get; set; }

		public int LoggedUserId {			
			get { return (LoggedUser != null ? LoggedUser.Id : 0); }
		}

		public string Name { get; private set; }
		public UserGroupId UserGroupId { get; private set; }

		public void OverrideLanguage(ContentLanguagePreference languagePreference) {
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Updates status, including permissions, of the currently logged in user.
		/// </summary>
		/// <typeparam name="T">Repository type.</typeparam>
		/// <param name="repository">Repository. Cannot be null.</param>
		public void RefreshLoggedUser<T>(IRepository<T> repository) {
			LoggedUser = repository.HandleQuery(ctx => new UserWithPermissionsContract(ctx.OfType<User>().Load(LoggedUserId), ContentLanguagePreference.Default));
		}

		public void VerifyLogin() {

			if (!IsLoggedIn)
				throw new NotAllowedException("Must be logged in.");

		}

		public void VerifyPermission(PermissionToken flag) {

			if (!HasPermission(flag)) {
				throw new NotAllowedException();
			}

		}

	}
}
