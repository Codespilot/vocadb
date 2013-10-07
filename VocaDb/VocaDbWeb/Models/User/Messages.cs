using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Models.User {

	public class Messages {

		public Messages() {
			ReceiverName = string.Empty;
		}

		public Messages(UserBaseContract user, string receiverName) {

			User = user;
			ReceiverName = receiverName ?? string.Empty;

		}

		public string ReceiverName { get; set; }

		public UserBaseContract User { get; set; }

	}

}