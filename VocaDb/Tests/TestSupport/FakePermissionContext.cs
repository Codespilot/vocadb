using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport {

	public class FakePermissionContext : IUserPermissionContext {

		public bool HasPermission(PermissionToken flag) {
			throw new NotImplementedException();
		}

		public bool IsLoggedIn { get; private set; }
		public ContentLanguagePreference LanguagePreference { get; private set; }
		public UserContract LoggedUser { get; private set; }
		public int LoggedUserId { get; private set; }
		public string Name { get; private set; }
		public UserGroupId UserGroupId { get; private set; }
		public void VerifyLogin() {
			throw new NotImplementedException();
		}

		public void VerifyPermission(PermissionToken flag) {
			throw new NotImplementedException();
		}

	}
}
