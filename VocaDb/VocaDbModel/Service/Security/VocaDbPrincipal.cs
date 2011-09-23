using System.Security.Principal;
using VocaDb.Model.DataContracts.Security;

namespace VocaDb.Model.Service.Security {

	public class VocaDbPrincipal : GenericPrincipal {

		private readonly UserContract user;

		public VocaDbPrincipal(IIdentity identity, UserContract user)
			: base(identity, new string[] {}) {

			this.user = user;

		}

		public UserContract User {
			get { return user; }
		}

	}

}