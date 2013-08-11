using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport {

	public class FakePermissionContext : IUserPermissionContext {

		public FakePermissionContext() {}

		public FakePermissionContext(UserContract loggedUser) {
			LoggedUser = loggedUser;
		}

		public bool HasPermission(PermissionToken flag) {
			return true;
		}

		public bool IsLoggedIn { get; private set; }
		public ContentLanguagePreference LanguagePreference { get; private set; }

		public UserContract LoggedUser { get; set; }

		public int LoggedUserId {			
			get { return (LoggedUser != null ? LoggedUser.Id : 0); }
		}

		public string Name { get; private set; }
		public UserGroupId UserGroupId { get; private set; }

		public void VerifyLogin() {
			
		}

		public void VerifyPermission(PermissionToken flag) {
			
		}

	}
}
