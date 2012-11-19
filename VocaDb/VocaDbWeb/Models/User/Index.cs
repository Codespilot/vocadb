using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.User {

	public class Index {

		public Index() { }

		public Index(PagingData<UserContract> users, UserGroupId groupId) {
			Users = users;
			GroupId = groupId;
		}

		public UserGroupId GroupId { get; set; }

		public PagingData<UserContract> Users { get; set; }

	}

}