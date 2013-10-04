using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserWithMessagesContract : UserContract {

		public UserWithMessagesContract() {
			ReceiverName = string.Empty;
		}

		public UserWithMessagesContract(User user)
			: base(user) {

			Notifications = user.ReceivedMessages.Where(m => m.Sender == null).Select(m => new UserMessageContract(m)).ToArray();
			ReceiverName = string.Empty;
			ReceivedMessages = user.ReceivedMessages.Where(m => m.Sender != null).Select(m => new UserMessageContract(m)).ToArray();
			SentMessages = user.SentMessages.Select(m => new UserMessageContract(m)).ToArray();

		}

		public UserMessageContract[] Notifications { get; set; }

		public string ReceiverName { get; set; }

		public UserMessageContract[] ReceivedMessages { get; set; }

		public UserMessageContract[] SentMessages { get; set; }

	}
}
