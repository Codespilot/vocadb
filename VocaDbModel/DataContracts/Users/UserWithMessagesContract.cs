using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserWithMessagesContract : UserContract {

		public UserWithMessagesContract() {
			ReceiverName = string.Empty;
		}

		public UserWithMessagesContract(User user)
			: base(user) {

			ReceiverName = string.Empty;
			ReceivedMessages = user.ReceivedMessages.Select(m => new UserMessageContract(m)).ToArray();
			SentMessages = user.SentMessages.Select(m => new UserMessageContract(m)).ToArray();

		}

		public string ReceiverName { get; set; }

		public UserMessageContract[] ReceivedMessages { get; set; }

		public UserMessageContract[] SentMessages { get; set; }

	}
}
