using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/*public class UserWithMessagesContract : UserContract {

		public UserWithMessagesContract() {
			ReceiverName = string.Empty;
		}

		public UserWithMessagesContract(User user, bool unread, IUserIconFactory iconFactory)
			: base(user) {

			ReceiverName = string.Empty;
			Messages = new UserMessagesContract(user, 200, unread, iconFactory);

		}

		public UserMessagesContract Messages { get; set; }

		public string ReceiverName { get; set; }

	}*/

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserMessagesContract {

		public UserMessagesContract() {}

		public UserMessagesContract(User user, int maxCount, bool unread, IUserIconFactory iconFactory) {

			//Notifications = user.ReceivedMessages.Where(m => m.Sender == null).Select(m => new UserMessageContract(m, iconFactory)).ToArray();
			//ReceivedMessages = user.ReceivedMessages.Where(m => m.Sender != null).Select(m => new UserMessageContract(m, iconFactory)).ToArray();
			ReceivedMessages = user.ReceivedMessages.Where(m => !unread || !m.Read).Take(maxCount).Select(m => new UserMessageContract(m, iconFactory)).ToArray();

			if (!unread)
				SentMessages = user.SentMessages.Take(maxCount).Select(m => new UserMessageContract(m, iconFactory)).ToArray();		

		}

		[DataMember]
		public UserMessageContract[] ReceivedMessages { get; set; }

		[DataMember]
		public UserMessageContract[] SentMessages { get; set; }

	}
}
