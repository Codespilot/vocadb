using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Models.User {

	public class Index {

		public Index() { }

		public Index(UserContract[] users) {
			Users = users;
		}

		public UserContract[] Users { get; set; }

	}

}