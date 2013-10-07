using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserWithMessagesContract : UserContract {

		public UserWithMessagesContract() {
			ReceiverName = string.Empty;
		}

		public UserWithMessagesContract(User user, IUserIconFactory iconFactory)
			: base(user) {

			ReceiverName = string.Empty;
			Messages = new UserMessagesContract(user, iconFactory);

		}

		public UserMessagesContract Messages { get; set; }

		public string ReceiverName { get; set; }

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserMessagesContract {

		public UserMessagesContract() {}

		public UserMessagesContract(User user, IUserIconFactory iconFactory) {
			Notifications = user.ReceivedMessages.Where(m => m.Sender == null).Select(m => new UserMessageContract(m, iconFactory)).ToArray();
			ReceivedMessages = user.ReceivedMessages.Where(m => m.Sender != null).Select(m => new UserMessageContract(m, iconFactory)).ToArray();
			SentMessages = user.SentMessages.Select(m => new UserMessageContract(m, iconFactory)).ToArray();		
		}

		[DataMember]
		public UserMessageContract[] Notifications { get; set; }

		[DataMember]
		public UserMessageContract[] ReceivedMessages { get; set; }

		[DataMember]
		public UserMessageContract[] SentMessages { get; set; }

	}
}
