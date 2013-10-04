using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class UserMessageContract {

		public UserMessageContract() { }

		public UserMessageContract(UserMessage message) {

			ParamIs.NotNull(() => message);

			Body = message.Message;
			Created = message.Created;
			HighPriority = message.HighPriority;
			Id = message.Id;
			Read = message.Read;
			Receiver = new UserContract(message.Receiver);
			Sender = (message.Sender != null ? new UserContract(message.Sender) : null);
			Subject = message.Subject;

		}

		public string Body { get; set; }

		public DateTime Created { get; set; }

		public bool HighPriority { get; set; }

		public int Id { get; set; }

		public bool Read { get; set; }

		public UserContract Receiver { get; set; }

		public UserContract Sender { get; set; }

		public string Subject { get; set; }

	}

}
