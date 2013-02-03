using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserWithMessagesContract : UserContract {

		public UserWithMessagesContract() { }

		public UserWithMessagesContract(User user)
			: base(user) {

			ReceivedMessages = user.ReceivedMessages.Select(m => new UserMessageContract(m)).ToArray();
			SentMessages = user.SentMessages.Select(m => new UserMessageContract(m)).ToArray();

		}

		public UserMessageContract[] ReceivedMessages { get; set; }

		public UserMessageContract[] SentMessages { get; set; }

	}
}
