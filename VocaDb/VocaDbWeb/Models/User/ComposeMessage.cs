﻿using System.ComponentModel.DataAnnotations;
using ViewRes.User;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Models.User {

	public class ComposeMessage {

		public ComposeMessage() { }

		public ComposeMessage(string receiverName) {
			ReceiverName = receiverName;
		}

		[Display(ResourceType = typeof(MessagesStrings), Name = "Body")]
		[Required]
		[StringLength(10000)]
		public string Body { get; set; }

		[Display(ResourceType = typeof(MessagesStrings), Name = "HighPriority")]
		public bool HighPriority { get; set; }

		[Display(ResourceType = typeof(MessagesStrings), Name = "To")]
		[Required]
		[StringLength(100)]
		public string ReceiverName { get; set; }

		[Display(ResourceType = typeof(MessagesStrings), Name = "Subject")]
		[Required]
		[StringLength(200)]
		public string Subject { get; set; }

		public UserMessageContract ToContract(int senderId) {

			return new UserMessageContract {
				Body = this.Body,
				HighPriority = this.HighPriority,
				Receiver = new UserWithIconContract { Name = ReceiverName },
				Sender = new UserWithIconContract { Id = senderId },
				Subject = this.Subject
			};

		}

	}

}