using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.User {

	public class Index {

		public Index() { }

		public Index(UserContract[] users, UserGroupId groupId) {
			Users = users;
			GroupId = groupId;
		}

		public UserGroupId GroupId { get; set; }

		public UserContract[] Users { get; set; }

	}

}